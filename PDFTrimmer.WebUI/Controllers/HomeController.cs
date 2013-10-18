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

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Handles the uploaded source pdf file
        /// </summary>
        /// <param name="pdfSource">the source PDF file that would be processed</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(HttpPostedFileBase pdfSource)
        {
            // Validate the uploaded file
            if (pdfSource == null || pdfSource.ContentType != "application/pdf")
            {
                ViewBag.ErrorMessage = "Oops. Something went wrong. Please upload a pdf file again.";
                return View();
            }

            // Create a unique name for the uploaded file
            var tempFileName = Guid.NewGuid() + ".pdf";
            // Set a temp file path for the uploaded file
            var tempFilePath = HostingEnvironment.MapPath("/Data/" + tempFileName);

            // Save the uploaded file to a temp location
            pdfSource.SaveAs(tempFilePath);
            // Save the file name to the session for the future reference
            
            HttpContext.Items["pdfSource"] = tempFileName;
           
            var DocInfoResponse = _trimmerService.GetDocInfo(new DocInfoRequest()
            {
                SourceFilePath = tempFilePath
            });

            if (!DocInfoResponse.IsSuccessful)
            {
                ViewBag.ErrorMessage = DocInfoResponse.TrimmerException.Message;
                return View();
            }

            System.IO.File.WriteAllBytes(HostingEnvironment.MapPath("/Data/sample-" + tempFileName), DocInfoResponse.SamplePages);

            return View(DocInfoResponse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Process(int llx, int lly, int urx, int ury)
        {
            var response = _trimmerService.Trim(new TrimmerRequest()
            {
                SourceFilePath = HostingEnvironment.MapPath("/Data/" + Session.Contents["pdfSource"]),
                llx = llx,
                lly = lly,
                urx = urx,
                ury = ury
            });

            if (response.IsSuccessful)
            {
                Session.Contents["pdfSource"] = null;
                System.IO.File.Delete(HostingEnvironment.MapPath("/Data/" + Session.Contents["pdfSource"].ToString()));
                System.IO.File.Delete(HostingEnvironment.MapPath("/Data/sample-" + Session.Contents["pdfSource"].ToString()));

                return null;
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}
