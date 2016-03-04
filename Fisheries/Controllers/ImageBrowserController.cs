using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;

namespace Fisheries.Controllers
{
    public class ImageBrowserController : EditorImageBrowserController
    {
        private string contentFolderRoot = "~/InformationFiles/";
        private const string prettyName = "Images/";
        private static readonly string[] foldersToCopy = new[] { "~/Content/shared/" };


        /// <summary>
        /// Gets the base paths from which content will be served.
        /// </summary>
        public override string ContentPath
        {
            get
            {
                return CreateUserFolder();
            }
        }

        private int id;
        //public override ActionResult Create(string path, FileBrowserEntry entry)
        //{
        //    Content
        //    return base.Create(path, entry);
        //}

        private string CreateUserFolder()
        {
            var virtualPath = Path.Combine(contentFolderRoot, id.ToString());

            var path = Server.MapPath(virtualPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                foreach (var sourceFolder in foldersToCopy)
                {
                    //CopyFolder(Server.MapPath(sourceFolder), path);
                }
            }
            return virtualPath;
        }

        public ActionResult MyThumbnail(string path, int id)
        {
            this.id = id;
            return base.Thumbnail(path);
        }

        public ActionResult MyCreate(string path, FileBrowserEntry entry, int id)
        {
            this.id = id;
            return base.Create(path, entry);    
        }

        public ActionResult MyUpload(string path, HttpPostedFileBase file, int id)
        {
            this.id = id;
            return base.Upload(path, file);
        }

        public ActionResult MyDestroy(string path, FileBrowserEntry entry, int id)
        {
            this.id = id;
            return base.Destroy(path, entry);
        }

        public JsonResult MyRead(string path, int id)
        {
            this.id = id;
            return base.Read(path);
        }

        private void CopyFolder(string source, string destination)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            foreach (var file in Directory.EnumerateFiles(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(file));
                System.IO.File.Copy(file, dest);
            }

            foreach (var folder in Directory.EnumerateDirectories(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(folder));
                CopyFolder(folder, dest);
            }
        }
    }
}