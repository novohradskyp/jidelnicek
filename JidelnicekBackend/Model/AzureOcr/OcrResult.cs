using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jidelnicek.Backend.Model.AzureOcr
{
    class OcrResult
    {
        public string status { get; set; }
        public RecognitionResult[] recognitionResults { get; set; }
    }
}
