using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Fisheries.Models
{
    public class ImageUploaderController : Controller
    {
        static private  int AvatarStoredWidth = 140;  // ToDo - Change the size of the stored avatar image
        static private  int AvatarStoredHeight = 140; // ToDo - Change the size of the stored avatar image
        static private  int AvatarScreenWidth = 400;  // ToDo - Change the value of the width of the image on the screen

        private const int EventWidth = 360;  // ToDo - Change the size of the stored avatar image
        private const int EventHeight = 360; // ToDo - Change the size of the stored avatar image
        private const int EventScreenWidth = 400;  // ToDo - Change the value of the width of the image on the screen

        private const int InfoWidth = 360;  // ToDo - Change the size of the stored avatar image
        private const int InfoHeight = 210; // ToDo - Change the size of the stored avatar image
        private const int InfoScreenWidth = 400;  // ToDo - Change the value of the width of the image on the screen

        private const int CeleWidth = 360;  // ToDo - Change the size of the stored avatar image
        private const int CeleHeight = 360; // ToDo - Change the size of the stored avatar image
        private const int CeleScreenWidth = 400;  // ToDo - Change the value of the width of the image on the screen

        private const int AdWidth = 1024;  // ToDo - Change the size of the stored avatar image
        private const int AdHeight = 160; // ToDo - Change the size of the stored avatar image
        private const int AdScreenWidth = 1024;  // ToDo - Change the value of the width of the image on the screen

        private const int HomeAdWidth = 960;  // ToDo - Change the size of the stored avatar image
        private const int HomeAdHeight = 465; // ToDo - Change the size of the stored avatar image
        private const int HomeAdScreenWidth = 1024;  // ToDo - Change the value of the width of the image on the screen

        private const string TempFolder = "/Temp";
        private const string MapTempFolder = "~" + TempFolder;
        private const string EventAvatarPath = "/Avatars/Events";
        private const string InfoAvatarPath = "/Avatars/Information";
        private const string CelebrityAvatarPath = "/Avatars/Celebrities";
        private const string AdsAvatarPath = "/Avatars/Ads";

        private readonly string[] _imageFileExtensions = { ".jpg", ".png", ".gif", ".jpeg" };

        public void setSize(int w, int h, int sw)
        {
            AvatarStoredWidth = w;
            AvatarStoredHeight = h;
            AvatarScreenWidth = sw;
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpGet]
        public ActionResult _Upload()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult UploadImage(IEnumerable<HttpPostedFileBase> files)
        {
            return _Upload(files);
        }

        [ValidateAntiForgeryToken]
        public ActionResult _Upload(IEnumerable<HttpPostedFileBase> files)
        {
            if (files == null || !files.Any()) return Json(new { success = false, errorMessage = "No file uploaded." });
            var file = files.FirstOrDefault();  // get ONE only
            if (file == null || !IsImage(file)) return Json(new { success = false, errorMessage = "File is of wrong format." });
            if (file.ContentLength <= 0) return Json(new { success = false, errorMessage = "File cannot be zero length." });
            var webPath = GetTempSavedFilePath(file);
            return Json(new { success = true, fileName = webPath.Replace("/", "\\") }); // success
        }

        [HttpPost]
        public ActionResult EventSave(string t, string l, string h, string w, string fileName)
        {
            return Save(t, l, h, w, fileName, EventAvatarPath);
        }

        [HttpPost]
        public ActionResult InfoSave(string t, string l, string h, string w, string fileName)
        {
            return Save(t, l, h, w, fileName, InfoAvatarPath);
        }

        [HttpPost]
        public ActionResult CelebritySave(string t, string l, string h, string w, string fileName)
        {
            return Save(t, l, h, w, fileName, CelebrityAvatarPath);
        }

        [HttpPost]
        public ActionResult AdSave(string t, string l, string h, string w, string fileName)
        {
            return Save(t, l, h, w, fileName, AdsAvatarPath);
        }

        [HttpPost]
        public ActionResult Save(string t, string l, string h, string w, string fileName, int type)
        {
           switch(type)
            {
                case 1:
                    setSize(EventWidth, EventHeight, EventScreenWidth);
                    return Save(t, l, h, w, fileName, EventAvatarPath);
                case 2:
                    setSize(InfoWidth, InfoHeight, InfoScreenWidth);
                    return Save(t, l, h, w, fileName, InfoAvatarPath);
                case 3:
                    setSize(CeleWidth, CeleHeight, CeleScreenWidth);
                    return Save(t, l, h, w, fileName, CelebrityAvatarPath);
                case 4:
                    setSize(AdWidth, AdHeight, AdScreenWidth);
                    return Save(t, l, h, w, fileName, AdsAvatarPath);
                case 5:
                    setSize(HomeAdWidth, HomeAdHeight, HomeAdScreenWidth);
                    return Save(t, l, h, w, fileName, AdsAvatarPath);
            }
            return Json(new { success = false, errorMessage = "Unable to upload file.\nERRORINFO: " });
        }

   
        public ActionResult Save(string t, string l, string h, string w, string fileName, string AvatarPath)
        {
            try
            {
                // Calculate dimensions
                var top = Convert.ToInt32(t.Replace("-", "").Replace("px", ""));
                var left = Convert.ToInt32(l.Replace("-", "").Replace("px", ""));
                var height = Convert.ToInt32(h.Replace("-", "").Replace("px", ""));
                var width = Convert.ToInt32(w.Replace("-", "").Replace("px", ""));

                // Get file from temporary folder
                var fn = Path.Combine(Server.MapPath(MapTempFolder), Path.GetFileName(fileName));
                // ...get image and resize it, ...
                var img = new WebImage(fn);
                img.Resize(width, height);
                // ... crop the part the user selected, ...
                img.Crop(top, left, img.Height - top - AvatarStoredHeight, img.Width - left - AvatarStoredWidth);
                // ... delete the temporary file,...
                System.IO.File.Delete(fn);
                // ... and save the new one.

                var filename = Helper.GuidEncoder.Encode(Guid.NewGuid());
                var ext = Path.GetExtension(fn);
                filename = filename + ext;
                var newFileName = Path.Combine(AvatarPath, Path.GetFileName(filename));

                //var newFileName = Path.Combine(AvatarPath, Path.GetFileName(fn));
                var newFileLocation = HttpContext.Server.MapPath(newFileName);
                if (Directory.Exists(Path.GetDirectoryName(newFileLocation)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newFileLocation));
                }

                img.Save(newFileLocation);
                return Json(new { success = true, avatarFileLocation = newFileName });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Unable to upload file.\nERRORINFO: " + ex.Message });
            }
        }

        

        private bool IsImage(HttpPostedFileBase file)
        {
            if (file == null) return false;
            var f1= file.ContentType.Contains("image");
            var f2 = _imageFileExtensions.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
            return f1 || f2;
            return file.ContentType.Contains("image") ||
                _imageFileExtensions.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        private string GetTempSavedFilePath(HttpPostedFileBase file)
        {
            // Define destination
            var serverPath = HttpContext.Server.MapPath(TempFolder);
            if (Directory.Exists(serverPath) == false)
            {
                Directory.CreateDirectory(serverPath);
            }

            // Generate unique file name
            var fileName = Path.GetFileName(file.FileName);
            fileName = SaveTemporaryAvatarFileImage(file, serverPath, fileName);

            // Clean up old files after every save
            CleanUpTempFolder(1);
            return Path.Combine(TempFolder, fileName);
        }

        private static string SaveTemporaryAvatarFileImage(HttpPostedFileBase file, string serverPath, string fileName)
        {
            var img = new WebImage(file.InputStream);
            var ratio = img.Height / (double)img.Width;
            img.Resize(AvatarScreenWidth, (int)(AvatarScreenWidth * ratio));

            var fullFileName = Path.Combine(serverPath, fileName);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }

            img.Save(fullFileName);
            return Path.GetFileName(img.FileName);
        }

        private void CleanUpTempFolder(int hoursOld)
        {
            try
            {
                var currentUtcNow = DateTime.UtcNow;
                var serverPath = HttpContext.Server.MapPath("/Temp");
                if (!Directory.Exists(serverPath)) return;
                var fileEntries = Directory.GetFiles(serverPath);
                foreach (var fileEntry in fileEntries)
                {
                    var fileCreationTime = System.IO.File.GetCreationTimeUtc(fileEntry);
                    var res = currentUtcNow - fileCreationTime;
                    if (res.TotalHours > hoursOld)
                    {
                        System.IO.File.Delete(fileEntry);
                    }
                }
            }
            catch
            {
                // Deliberately empty.
            }
        }
    }
}
