using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDFTrimmer.WebUI.Controllers;
using Moq;
using PDFTrimmer.Services;
using System.Web.Mvc;
using System.Web;
using PDFTrimmer.TestHelpers;

namespace PDFTrimmer.WebUI.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        private Mock<ITrimmerService> _trimmerServiceMock;
        private HomeController _homeController;

        [TestInitialize]
        public void SetUp()
        {
            _trimmerServiceMock = new Mock<ITrimmerService>();
            _homeController = new HomeController(_trimmerServiceMock.Object);
        }

        [TestMethod]
        public void HomeController_Exists()
        {
            Assert.IsNotNull(_homeController);
        }

        #region Index Post Method

        [TestMethod]
        public void HomeController_IndexPost_HandlesInvalidFilePost()
        {
            var expected = "Oops. Something went wrong. Please upload a pdf file again.";
            var actual = ((ViewResult)_homeController.Index(null)).ViewBag.ErrorMessage;

            Assert.AreEqual(expected, actual);

            var httpPostedFileBase = new FakeHttpPostedFileBase("WrongType");
            var actual2 = ((ViewResult)_homeController.Index(httpPostedFileBase)).ViewBag.ErrorMessage;

            Assert.AreEqual(expected, actual2);
        }

        [TestMethod]
        public void HomeController_IndexPost_HandlesExceptionFromTrimmerService()
        {
            var testResponse = new PrepareResponse()
            {
                IsSuccessful = false,
                TrimmerException = new InvalidPDFException()
            };
            _trimmerServiceMock.Setup(p => p.Prepare(It.IsAny<PrepareRequest>())).Returns(testResponse);

            _homeController = new HomeController(_trimmerServiceMock.Object);
            _homeController.ControllerContext = new ControllerContext();
            _homeController.ControllerContext.HttpContext = new HttpContextWrapper(
                ControllerHelper.SetFakeHttpContext("Test"));

            var httpPostedFileBase = new FakeHttpPostedFileBase("application/pdf");

            var expected = "Cannot read the PDF file. Please make sure the PDF file is not corrupted.";
            var actual = ((ViewResult)_homeController.Index(httpPostedFileBase)).ViewBag.ErrorMessage;

            Assert.AreEqual(expected, actual);
        }

        #endregion

        //[TestMethod]
        //public void HomeController_Process_HandlesInvalidArguments()
        //{
        //    var expected = "Invalid margin values. Please try again.";
        //    var actual = ((ViewResult)_homeController.Process(-1, -1, -1, -1)).ViewBag.ErrorMessage;

        //    Assert.AreEqual(expected, actual);
        //}

        #region Process Method Tests



        #endregion
    }
}
