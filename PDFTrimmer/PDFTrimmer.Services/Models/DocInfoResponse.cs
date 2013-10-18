using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTrimmer.Services
{
    public class DocInfoResponse : ResponseBase
    {
        public byte[] SamplePages { get; set; }
    }
}
