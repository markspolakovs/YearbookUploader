using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YearbookUploader.Helpers
{
    class UploadAcceptedResult
    {
        public IEnumerable<string> nextExpectedRanges { get; set; }
        public string expirationDateTime { get; set; }
    }
}
