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
        /// Handles the uploaded source pdf file
        /// </summary>
        /// <param name="pdfSource">the source PDF file that would be processed</param>
        /// <returns></returns>
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

            // Handle failed trims
            if (!DocInfoResponse.IsSuccessful)
            {
                ViewBag.ErrorMessage = DocInfoResponse.TrimmerException.Message;
                return View();
            }

            // If trim is successful, write the file to the server, so it can be opened in the view.
            System.IO.File.WriteAllBytes(HostingEnvironment.MapPath("/Data/sample-" + tempFileName), DocInfoResponse.SamplePages);

            return View(DocInfoResponse);
        }

        /// <summary>
        /// Receive the margins from the user and apply to the pdf
        /// </summary>
        /// <param name="llx">lower left x</param>
        /// <param name="lly">lower left y</param>
        /// <param name="urx">upper right x</param>
        /// <param name="ury">upper right y</param>
        /// <returns>Modified pdf file</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Process(int llx, int lly, int urx, int ury)
        {
            // Handling invalid margins
            if (llx < 0 || lly < 0 || urx < 0 || ury < 0)
            {
                ViewBag.ErrorMessage = "Invalid margin values. Please try again.";
                return View("Index");
            }

            var response = _trimmerService.Trim(new TrimmerRequest()
            {
                SourceFilePath = HostingEnvironment.MapPath("/Data/" + HttpContext.Items["pdfSource"]),
                llx = llx,
                lly = lly,
                urx = urx,
                ury = ury
            });

            if (response.IsSuccessful)
            {
                HttpContext.Items["pdfSource"] = null;
                System.IO.File.Delete(HostingEnvironment.MapPath("/Data/" + HttpContext.Items["pdfSource"].ToString()));
                System.IO.File.Delete(HostingEnvironment.MapPath("/Data/sample-" + HttpContext.Items["pdfSource"].ToString()));

                return null;
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}
