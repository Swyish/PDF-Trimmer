using PDFTrimmer.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace PDFTrimmer.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private ITrimmerService _trimmerService;

        public HomeController(ITrimmerService trimmerService)
        {
            _trimmerService = trimmerService;
        }

        /// <summary>
        /// Displays the main page where user can submit a pdf file
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Handles uploaded PDF file
        /// </summary>
        /// <param name="pdfSource"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Index(HttpPostedFileBase pdfSource)
        {
            // Validate the uploaded file
            if (pdfSource == null || pdfSource.ContentType != "application/pdf")
            {
                ViewBag.ErrorMessage = "Oops. Something went wrong. Please upload a pdf file again.";
                return Json(new
                   {
                       statusCode = 500,
                       status = "Error uploading image.",
                       file = string.Empty
                   }, "text/html");
            }

            var tempFileName = Guid.NewGuid() + ".pdf";
            var baseFilePath = HostingEnvironment.MapPath("/Data/");

            Session.Contents["originalName"] = pdfSource.FileName.Split('.')[0];
            Session.Contents["sourceFileName"] = tempFileName;

            pdfSource.SaveAs(baseFilePath + tempFileName);

            var prepareResponse = _trimmerService.Prepare(new PrepareRequest()
            {
                BaseFilePath = baseFilePath,
                SourceFileName = tempFileName
            });

            // Handle failed trims
            if (!prepareResponse.IsSuccessful)
            {
                ViewBag.ErrorMessage = prepareResponse.TrimmerException.Message;
                return Json(new
                {
                    statusCode = 500,
                    status = "Error uploading image.",
                    file = string.Empty
                }, "text/html");
            }

            return Json(new
            {
                statusCode = 200,
                status = "Image uploaded.",
                file = Session.Contents["originalName"].ToString(),
            }, "text/html");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Process()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Process(int marginLeft, int marginBottom, int marginRight, int marginTop)
        {
            if (Session.Contents["sourceFileName"] == null)
            {
                return RedirectToAction("Index");
            }

            // Handling invalid margins
            if (marginLeft < 0 || marginBottom < 0 || marginRight < 0 || marginTop < 0)
            {
                ViewBag.ErrorMessage = "Invalid margin values. Please try again.";
                return RedirectToAction("Index");
            }

            var response = _trimmerService.Trim(new TrimmerRequest()
            {
                BaseFilePath = HostingEnvironment.MapPath("/Data/"),
                SourceFileName = Session.Contents["sourceFileName"].ToString(),
                MarginLeft = marginLeft,
                MarginBottom = marginBottom,
                MarginRight = marginRight,
                MarginTop = marginTop
            });

            if (response.IsSuccessful)
            {
                System.IO.File.Delete(HostingEnvironment.MapPath("/Data/" + Session.Contents["sourceFileName"].ToString()));
                System.IO.File.Delete(HostingEnvironment.MapPath("/Data/prepared-" + Session.Contents["sourceFileName"].ToString()));
                System.IO.File.Delete(HostingEnvironment.MapPath("/Data/sample-" + Session.Contents["sourceFileName"].ToString()));
                Session.Contents["sourceFileName"] = null;

                MemoryStream ms = new MemoryStream(response.OutputFile);
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + Session.Contents["originalName"] + ".pdf");
                Session.Contents["originalName"] = null;
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                Response.End();

                return new FileStreamResult(ms, "application/pdf");
            }
            else
            {
                return RedirectToAction("Process");
            }
        }

        public ActionResult ThankYou()
        {
            return View();
        }
    }
}