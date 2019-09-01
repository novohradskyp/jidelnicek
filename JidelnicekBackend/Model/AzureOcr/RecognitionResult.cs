using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jidelnicek.Backend.Model.AzureOcr
{
    class RecognitionResult
    {
        public int page { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string unit { get; set; }
        public TextLine[] lines { get; set; }
    }
}
