using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Threading;

namespace Gandalf
{
    public enum cellstate
    {
        failed = -1,
        ready = 0,
        waiting = 1,
        uploading = 2,
        success = 3
    }

    public struct jsonin
    {
        public string video_id { get; set; }
        public string video_unique { get; set; }
        public string upload_url { get; set; }
        public string progress_url { get; set; }
        public string upload_size { get; set; }
        public string token { get; set; }
        public string uploadtype { get; set; }
        public string isdrm { get; set; }
    }

    public struct jsonout
    {
        public string code { get; set; }
        public string message { get; set; }
        public string total { get; set; }
        public Object data;
    }

    public class SliceFile
    {
        private string filePath;
        private FileStream fsFile;

        public int sliceSize = 10485760; //10M
        public int sliceCount = 0;  //slice count of file 
        public int maxFailedCount = 20;
        public int failedCount = 0; //upload failed count

        public List<cellstate> sliceState = new List<cellstate>();
        public List<int> slicePost = new List<int>();

        public int lastCount = 100;
        public List<int> lastSize = new List<int>();
        public List<double> lastSecond = new List<double>();

        public SliceFile(string filePath, int sliceSize)
        {
            this.filePath = filePath;
            this.fsFile = new FileStream(filePath, FileMode.Open);
            if (sliceSize > 1024)
                this.sliceSize = sliceSize;

            this.sliceCount = (int)(this.fsFile.Length + this.sliceSize - 1) / this.sliceSize;
            for (int i = 0; i < this.sliceCount; ++i)
            {
                this.sliceState.Add(cellstate.ready);
                this.slicePost.Add(0);
            }
        }

        ~SliceFile()
        {
            fsFile.Close();
        }

        public void close()
        {
            fsFile.Close();
        }

        public static SliceFile createSliceFile(string filePath)
        {
            return createSliceFile(filePath, 10485760);
        }

        public static SliceFile createSliceFile(string filePath, int sliceSize)
        {
            if (!System.IO.File.Exists(filePath))
                return null;

            return new SliceFile(filePath, sliceSize);
        }

        public long Length()
        {
            return this.fsFile.Length;
        }

        public string Name()
        {
            return System.IO.Path.GetFileName(this.fsFile.Name);
        }

        public byte[] fileSlice(int start)
        {
            int count = this.sliceSize;
            if (start + this.sliceSize > this.fsFile.Length)
                count = (int)this.fsFile.Length - start;

            byte[] data = new byte[count];
            lock (this)
            {
                this.fsFile.Seek(start, SeekOrigin.Begin);
                this.fsFile.Read(data, 0, count);
            }

            return data;
        }

        public void setState(int index, cellstate state)
        {
            if (index >= this.sliceState.Count)
                return;

            if (state == cellstate.failed)
            {
                failedCount++;
                resetSlice(index);
            }

            this.sliceState[index] = state;
        }

        public void progress(int index, int size, double mseconds)
        {
            if (index >= this.slicePost.Count)
                return;

            this.slicePost[index] += size;

            this.lastSize.Add(size);
            this.lastSecond.Add(mseconds);

            while(this.lastSize.Count > this.lastCount)
            {
                this.lastSize.RemoveAt(0);
                this.lastSecond.RemoveAt(0);
            }
        }

        public bool failed()
        {
            if (failedCount > maxFailedCount)
                return true;

            return false;
        }

        public void resetSlice(int index)
        {
            if (index >= this.sliceState.Count || index >= this.slicePost.Count)
                return;

            this.sliceState[index] = cellstate.ready;
            this.slicePost[index] = 0;
        }
    }

    public class SliceInfo
    {
        public string uploadUrl;
        public string filename;
        public long totalSize;
        public int sliceIndex;
        public int sliceSize;
        public byte[] sliceData;

        public SliceInfo(string url, string name, long totalSize, int index, int sliceSize, byte[] data)
        {
            this.uploadUrl = url;
            this.filename = name;
            this.totalSize = totalSize;
            this.sliceIndex = index;
            this.sliceSize = sliceSize;
            sliceData = new byte[data.Length];
            Array.Copy(data, this.sliceData, data.Length);
        }
    }

    public abstract class BlockCallback
    {
        public abstract void handleInit(string token);
        public abstract void handleComplete(int code, String message);
        public abstract void handleProgress(double progress, double rate);
    }

    //Software Define Storage
    public class LCUploader
    {
        static int MajorVersion = 1;
        static int MinorVersion = 0;

        //public delegate void delegateCallback(int code, string message);

        delegate void delegateProgress(int index, int size, double mseconds);
        delegate void delegateState(int index, cellstate state);

        BlockingCollection<SliceInfo> taskQueue = new BlockingCollection<SliceInfo>(3);

        BlockCallback cbResult = null;

        private string userUnique = "";
        private string secretKey = "";

        private string apiUrl = "http://api.letvcloud.com/open.php";
        private string apiInit = "video.upload.init";
        private string apiResume = "video.upload.resume";
        private int resumePosition = 0;

        public string format = "json";
        public string apiVersion = "2.0";

        public int errorCode = 0;
        public string message = String.Empty;

        public string token;

        private int concurrentCount = 1;

        SliceFile sliceFile;
        string uploadUrl = "";

        CancellationTokenSource cts = new CancellationTokenSource();

        public LCUploader(string userUnique, string secretKey)
        {
            this.userUnique = userUnique;
            this.secretKey = secretKey;

            System.Net.ServicePointManager.DefaultConnectionLimit = 200;
        }

        private bool checkFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return false;

            //check extension

            return true;
        }

        public jsonout jsonGetResult(String src)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            jsonout json = serializer.Deserialize<jsonout>(src);
            return json;
        }

        public jsonin jsonGetData(String src)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            jsonin json = serializer.Deserialize<jsonin>(src);
            return json;
        }

        private string initUpload(string filePath, string clientIp)
        {
            if (!checkFile(filePath))
                return String.Empty;

            this.sliceFile = SliceFile.createSliceFile(filePath);
            if (this.sliceFile == null)
                return String.Empty;

            string videoName = this.sliceFile.Name();
            long fileSize = this.sliceFile.Length();
            if (fileSize == 0)
                return String.Empty;

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("video_name", videoName);
            args.Add("file_size", fileSize.ToString());
            args.Add("client_ip", clientIp);
            args.Add("api", apiInit);
            args.Add("uploadtype", "1");
            args.Add("uc1", "0");
            args.Add("uc2", "0");

            string retUrl = handleParam(args);

            string strResult = doRequest(retUrl);

//            System.Console.WriteLine(strResult);

            jsonout jsonRes = jsonGetResult(strResult);
            this.errorCode = int.Parse(jsonRes.code);
            this.message = jsonRes.message;
            //string total = json.total;
            //string data = json.data.token;
            if (!jsonRes.code.Equals("0"))
            {
                cbResult.handleComplete(this.errorCode, this.message);

                return String.Empty;
            }

            //jsonin json = jsonGetData(jsonRes.data.ToString());
            Dictionary<string, object> data = (Dictionary<string, object>)jsonRes.data;

            return data["upload_url"].ToString();
        }

        private string resumeUpload(string filePath, string clientIp, string token)
        {
            if (!checkFile(filePath))
                return String.Empty;

            this.sliceFile = SliceFile.createSliceFile(filePath);
            if (this.sliceFile == null)
                return String.Empty;

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("token", token);
            args.Add("client_ip", clientIp);
            args.Add("api", apiResume);
            args.Add("uploadtype", "1");

            string retUrl = handleParam(args);

            string strResult = doRequest(retUrl);

            jsonout jsonRes = jsonGetResult(strResult);
            errorCode = int.Parse(jsonRes.code);
            message = jsonRes.message;
            //string total = json.total;
            //string data = json.data.token;
            if (!jsonRes.code.Equals("0"))
            {
                //callback();
                cbResult.handleComplete(this.errorCode, this.message);

                return String.Empty;
            }

            Dictionary<string, object> data = (Dictionary<string, object>)jsonRes.data;

            int pos = 0;
            if (int.TryParse(data["upload_size"].ToString(), out pos))
                resumePosition = pos;

            return data["upload_url"].ToString();
        }

        private string sliceUpload(SliceInfo sliceInfo)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(sliceInfo.uploadUrl));
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Timeout = 300000;

            int start = sliceInfo.sliceIndex * sliceInfo.sliceSize;
            int contentLen = sliceInfo.sliceData.Length;

            string range = start == -1 ? "bytes *" : "bytes " + (start + 1) + "-" + (start + contentLen) + "/" + sliceInfo.totalSize;

            request.ContentType = "application/octet-stream";
            request.ContentLength = contentLen;
            request.Headers.Add("X_FILENAME", sliceInfo.filename);
            request.Headers.Add("Content-Range", range);
            request.UserAgent = String.Format("letv_sdk; sds_upload/csharp/{0}.{1}", MajorVersion, MinorVersion);

            //post file
            //try
            {
                updateState(sliceInfo.sliceIndex, cellstate.uploading);

                Stream streamPost = request.GetRequestStream();

                //post header 
                byte[] buffer = sliceInfo.sliceData;
                int sendSize = 128 * 1024; //128k
                //position of last upload
                //int startPos = this.sliceFile.slicePost[this.sliceIndex];
                int sendCount = (buffer.Length + sendSize - 1) / sendSize;
                byte[] buf = new byte[sendSize];
                for (int index = 0; index < sendCount; ++index)
                {
                    if (cts.IsCancellationRequested)
                    {
                        streamPost.Close();
                        return String.Empty;
                    }

                    int pos = index * sendSize;
                    int len = buffer.Length - pos > sendSize ? sendSize : buffer.Length - pos;
                    Array.Copy(buffer, pos, buf, 0, len);

                    DateTime dtStart = DateTime.UtcNow;

                    streamPost.Write(buf, 0, len);

                    //DateTime dtEnd = DateTime.Now;

                    double mseconds = (dtStart - new System.DateTime(1970, 1, 1)).TotalMilliseconds;
                    updateProgress(sliceInfo.sliceIndex, len, mseconds);
                }
                streamPost.Close();

                WebResponse response = request.GetResponse();
                Stream streamResp = response.GetResponseStream();
                StreamReader sr = new StreamReader(streamResp);
                String strRet = sr.ReadToEnd();
                streamResp.Close();
                sr.Close();

                updateState(sliceInfo.sliceIndex, cellstate.success);

                return strRet;
            }
            //catch (Exception e)
            //{
            //    updateState(sliceInfo.sliceIndex, cellstate.failed);

            //    return e.Message;
            //}
        }

        public int tryUpload(string filePath, string clientIp, BlockCallback cb)
        {
            return tryUpload(filePath, clientIp, cb, String.Empty);
        }

        public int tryUpload(string filePath, string clientIp, BlockCallback cb, string token)
        {
            if (cb == null)
                return -1;

            string strUploadUrl = String.Empty;

            try
            {
                if (String.IsNullOrEmpty(token))
                {
                    strUploadUrl = initUpload(filePath, clientIp);
                }
                else
                {
                    strUploadUrl = resumeUpload(filePath, clientIp, token);
                }
            }
            catch (Exception e)
            {
                this.message = e.Message;
                
                cbResult.handleComplete(this.errorCode, this.message);

                return -1;
            }

            if (String.IsNullOrEmpty(strUploadUrl))
                return -1;

            cbResult = cb;

            //remove &fmt=cjson
            this.uploadUrl = strUploadUrl.Substring(0, strUploadUrl.Length - 10);

            int slicePos = this.resumePosition / this.sliceFile.sliceSize; //start upload position

            //resume upload
            for (int index = 0; index < slicePos; index++)
            {
                this.sliceFile.sliceState[index] = cellstate.success;
                this.sliceFile.slicePost[index] = this.sliceFile.sliceSize;
            }

            int runCount = Math.Min(this.concurrentCount, this.sliceFile.sliceCount - 1);
            runCount = Math.Max(runCount, 1);

            tryProducer();
            tryConsumer(runCount);

            string tokenUrl = this.uploadUrl.Split(new string[]{"token="}, StringSplitOptions.None)[1];
            string videoToken = tokenUrl.Split('&')[0];

            this.token = videoToken;

            cbResult.handleInit(this.token);

            return 0;
            //return videoToken;
        }

        private void tryProducer()
        {
            Task.Factory.StartNew(() =>
            {
                bool bComplete = false;
                while (!bComplete)
                {
                    if (cts.IsCancellationRequested)
                        return;

                    bool bLast = true;
                    for (int i = 0; i < this.sliceFile.sliceCount - 1; i++)
                    {
                        //not all slice upload success
                        if (this.sliceFile.sliceState[i] != cellstate.success && bLast)
                        {
                            bLast = false;
                        }

                        if (this.sliceFile.sliceState[i] == cellstate.ready )
                        {
                            byte[] data = this.sliceFile.fileSlice(this.sliceFile.sliceSize * i);
                            SliceInfo sliceInfo = new SliceInfo(this.uploadUrl, this.sliceFile.Name(), this.sliceFile.Length(), i, this.sliceFile.sliceSize, data);
                            this.taskQueue.Add(sliceInfo);

                            this.sliceFile.sliceState[i] = cellstate.waiting;
                        }
                    }

                    //all upload success except last slice
                    if (bLast)
                    {
                        int last = this.sliceFile.sliceCount - 1;
                        if (this.sliceFile.sliceState[last] == cellstate.ready)
                        {
                            byte[] data = this.sliceFile.fileSlice(this.sliceFile.sliceSize * last);
                            SliceInfo sliceInfo = new SliceInfo(this.uploadUrl, this.sliceFile.Name(), this.sliceFile.Length(), last, this.sliceFile.sliceSize, data);
                            this.taskQueue.Add(sliceInfo);

                            this.sliceFile.sliceState[this.sliceFile.sliceCount - 1] = cellstate.waiting;
                        }
                    }

                    bComplete = true;
                    for (int i = 0; i < this.sliceFile.sliceCount; i++)
                    {
                        if (this.sliceFile.sliceState[i] != cellstate.success)
                        {
                            bComplete = false;
                            break;
                        }
                    }
                    System.Threading.Thread.Sleep(500);
                }

                if (bComplete)
                    this.taskQueue.CompleteAdding();
            }, cts.Token);
        }

        private void tryConsumer(int nNumber)
        {
            for (int i = 0; i < nNumber; ++i)
            {
                Task.Factory.StartNew(() =>
                {
                    while (!this.taskQueue.IsCompleted)
                    {
                        if (cts.IsCancellationRequested)
                            return;

                        SliceInfo sliceInfo = null;

                        try
                        {
                            sliceInfo = this.taskQueue.Take();
                        }
                        catch (InvalidOperationException)
                        {
                            //queue is empty
                            return;
                        }

                        bool bSend = false;
                        while(!bSend)
                        {
                            try
                            {
                                sliceUpload(sliceInfo);
                                bSend = true;
                            }
                            catch (Exception e)
                            {
                                updateState(sliceInfo.sliceIndex, cellstate.failed);
                                this.message = e.Message;

                                if (this.sliceFile.failed())
                                {
                                    cbResult.handleComplete(this.errorCode, this.message);

                                    return;
                                }
                            }
                        }
                    }
                }, cts.Token);
            }
        }

        public bool isFinish()
        {
            bool isFinished = true;
            for (int i = 0; i < this.sliceFile.sliceCount; i++)
            {
                if (this.sliceFile.sliceState[i] != cellstate.success)
                {
                    isFinished = false;
                }
            }

            return isFinished;
        }

        public double getProgress()
        {
            int sendedSize = 0;
            for (int i = 0; i < this.sliceFile.slicePost.Count; ++i)
            {
                sendedSize += this.sliceFile.slicePost[i];
            }

            return sendedSize * 100.0 / this.sliceFile.Length();
        }

        public double getRate()
        {
            //average rate in 10s
            int step = 10;

            //get data now - 3s
            double tsEnd = (DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalMilliseconds - 3000;
            double tsStart = tsEnd - step * 1000;

            int posStart = -1;
            int posEnd = -1;

            for (int index = this.sliceFile.lastSecond.Count - 1; index >= 0; --index)
            {
                if (this.sliceFile.lastSecond[index] <= tsEnd && posEnd < 0)
                {
                    posEnd = index + 1;
                    break;
                }
            }

            for (int index = 0; index < this.sliceFile.lastSecond.Count; ++index)
            {
                if (this.sliceFile.lastSecond[index] >= tsStart && posStart < 0)
                {
                    posStart = index;
                    break;
                }
            }

            if (posEnd >= this.sliceFile.lastSecond.Count)
                posEnd = this.sliceFile.lastSecond.Count - 1;

            if (posStart >= posEnd)
                return 0;

            if (posStart < 0)
                return 0;

            int nSize = 0;
            for (int index = posStart; index <= posEnd; ++index)
            {
                nSize += this.sliceFile.lastSize[index];
            }

            return nSize / step;
        }

        public void stop()
        {
            cts.Cancel();
            this.sliceFile.close();
        }

        //make request url
        public string handleParam(Dictionary<string, string> args)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);  //timestamp
            string timestamp = Convert.ToInt64(ts.TotalSeconds).ToString();
            args.Add("timestamp", timestamp);
            args.Add("user_unique", userUnique);
            args.Add("ver", apiVersion);
            args.Add("format", format);

            List<string> keyList = new List<string>(args.Keys);
            keyList.Sort();

            string urlParam = "";
            string keyStr = "";
            for (int i = 0; i < keyList.Count; i++)
            {
                string key = keyList[i];
                urlParam += (String.IsNullOrEmpty(urlParam) ? "?" : "&") + key + "=" + HttpUtility.UrlEncode(args[key]);
                keyStr += key + args[key];
            }
            keyStr += secretKey;

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(keyStr));
            string signStr = BitConverter.ToString(output).Replace("-", String.Empty).ToLower();

            urlParam += "&sign=" + signStr;
            String resUrl = apiUrl + urlParam;

            return resUrl;
        }

        public string doRequest(String url)
        {
            Uri uri = new Uri(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var mstream = new MemoryStream())
            {
                responseStream.CopyTo(mstream);
                return System.Text.Encoding.UTF8.GetString(mstream.ToArray());
            }
        }

        public void sliceProgress(int index, int size, double mseconds)
        {
            this.sliceFile.progress(index, size, mseconds);

            cbResult.handleProgress(getProgress(), getRate());
        }

        public void sliceState(int index, cellstate state)
        {
            this.sliceFile.setState(index, state);

            if (isFinish())
            {
                this.errorCode = 0;
                this.message = "success";

                cbResult.handleComplete(this.errorCode, this.message);
            }
        }

        public void updateProgress(int index, int size, double mseconds)
        {
            delegateProgress del = new delegateProgress(sliceProgress);
            del.BeginInvoke(index, size, mseconds, null, null);
        }

        public void updateState(int index, cellstate state)
        {
            delegateState del = new delegateState(sliceState);
            del.BeginInvoke(index, state, null, null);
        }

//        private void callback()
//        {
            //if (cbResult != null)
            //    cbResult.BeginInvoke(this.errorCode, this.message, null, null);
//        }

    }
}
