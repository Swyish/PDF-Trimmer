using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PDFTrimmer.TestHelpers
{
    /// <summary>
    /// Helps testing HomeController's Index method which takes in a HttppostedFileBase object
    /// </summary>
    public class FakeHttpPostedFileBase : HttpPostedFileBase
    {
        private string _contentType;

        public FakeHttpPostedFileBase(string contentType)
        {
            _contentType = contentType;
        }

        public override string ContentType
        {
            get
            {
                return _contentType;
            }
        }

        public override void SaveAs(string filename)
        {
            return;
        }
    }
}
