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

        #region GetDocInfo Tests

        [TestMethod]
        public void TrimmerService_GetDocInfo_HandlesNullRequestObject()
        {
            var expected = false;
            var actual = _trimmerService.Prepare(null).IsSuccessful;

            var actualException = _trimmerService.Prepare(null).TrimmerException;

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(actualException.GetType(), typeof(TrimmerRequestException));
        }

        #endregion
    }
}
