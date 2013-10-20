using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTrimmer.Services
{
    public interface ITrimmerService
    {
        PrepareResponse Prepare(PrepareRequest request);
        TrimmerResponse Trim(TrimmerRequest request);
    }
}
