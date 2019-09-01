using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jidelnicek.Backend.Service
{
    interface IOcrService
    {
        Task<string> GetTextFromImageAsync(string imageUrl);
    }
}
