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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(HttpPostedFileBase pdfSource)
        {
            // Do validations
            if (pdfSource == null || pdfSource.ContentType != "application/pdf")
            {
                return View();
            }

            // Creating a unique name for the uploaded PDF file
            var tempFileName = Guid.NewGuid() + ".pdf";
            var tempFilePath = HostingEnvironment.MapPath("/Data/" + tempFileName);

            // Save to the temporary data folder, then store the file path to the session, so I can be loaded on the next step
            pdfSource.SaveAs(tempFilePath);
            Session.Contents["pdfSource"] = tempFileName;

            var DocInfoResponse = _trimmerService.GetDocInfo(new DocInfoRequest()
            {
                SourceFilePath = tempFilePath
            });

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
