using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDFTrimmer.Services;

namespace PDF_Trimmer.Services.Tests
{
    [TestClass]
    public class PDFTrimmerServiceTests
    {
        private ITrimmerService _trimmerService;

        [TestInitialize]
        public void SetUp()
        {
            _trimmerService = new PDFTrimmerService();
        }

        [TestMethod]
        public void TrimmerService_Exists()
        {
            Assert.IsNotNull(_trimmerService);
        }
    }
}
