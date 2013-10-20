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

            var buffer = new byte[pdfSource.InputStream.Length];
            pdfSource.InputStream.Read(buffer, 0, (int)pdfSource.InputStream.Length);
            
            var tempFileName = Guid.NewGuid() + ".pdf";
            Session.Contents["pdfSource"] = tempFileName;
           
            var prepareResponse = _trimmerService.Prepare(new PrepareRequest()
            {
                SourceFile = buffer
            });

            // Handle failed trims
            if (!prepareResponse.IsSuccessful)
            {
                ViewBag.ErrorMessage = prepareResponse.TrimmerException.Message;
                return View();
            }

            // If trim is successful, write the file to the server, so it can be opened in the view.
            System.IO.File.WriteAllBytes(HostingEnvironment.MapPath("/Data/" + tempFileName), prepareResponse.PreparedDoc);

            return View(prepareResponse);
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
        public ActionResult Process(int marginLeft, int marginBottom, int marginRight, int marginTop)
        {
            // Handling invalid margins
            if (marginLeft < 0 || marginBottom < 0 || marginRight < 0 || marginTop < 0)
            {
                ViewBag.ErrorMessage = "Invalid margin values. Please try again.";
                return View("Index");
            }

            var pdfFile = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath("/Data/" + Session.Contents["pdfSource"]));

            var response = _trimmerService.Trim(new TrimmerRequest()
            {
                SourceFile = pdfFile,
                MarginLeft = marginLeft,
                MarginBottom = marginBottom,
                MarginRight = marginRight,
                MarginTop = marginTop 
            });

            if (response.IsSuccessful)
            {
                System.IO.File.Delete(HostingEnvironment.MapPath("/Data/" + Session.Contents["pdfSource"].ToString()));
                Session.Contents["pdfSource"] = null;

                MemoryStream ms = new MemoryStream(response.OutputFile);
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=NamedQuery.pdf");
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                Response.End();

                return new FileStreamResult(ms, "application/pdf");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult ThankYou()
        {
            return View();
        }
    }
}