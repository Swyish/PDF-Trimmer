using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDFTrimmer.WebUI.Controllers;
using Moq;
using PDFTrimmer.Services;

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
    }
}
