using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jidelnicek.Backend.Model.AzureOcr
{
    class Word
    {
        public int[] boundingBox { get; set; }
        public string text { get; set; }
    }
}
