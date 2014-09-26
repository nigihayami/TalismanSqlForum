using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace TalismanSqlForum.Controllers
{
    public class UploadController : Controller
    {
        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            var vImagePath = String.Empty;
            var vMessage = String.Empty;
            var vFilePath = String.Empty;
            var vOutput = String.Empty;
            try
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    var vFileName = Guid.NewGuid() +
                                    Path.GetExtension(upload.FileName).ToLower();
                    var vFolderPath = Server.MapPath("~/Content/Images/");
                    if (!Directory.Exists(vFolderPath))
                    {
                        Directory.CreateDirectory(vFolderPath);
                    }
                    vFilePath = Path.Combine(vFolderPath, vFileName);
                    upload.SaveAs(vFilePath);
                    var val = Url.RequestContext.HttpContext.Request.Url.Scheme;
                    vImagePath = Url.Action("Images", "Content", new { id = vFileName }, val);
                    vMessage = "Image was saved correctly";
                }
            }
            catch
            {
                vMessage = "There was an issue uploading";
            }
            vOutput = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + vImagePath + "\", \"" + vMessage + "\");</script></body></html>";
            return Content(vOutput);
        }
    }
}