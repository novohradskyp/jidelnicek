using Jidelnicek.Backend.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jidelnicek.Backend.Provider
{
    class RuzekUrlTranslator : IUrlTranslator
    {
        public string TranslateUrl(string url)
        {
            try
            {
                UriBuilder builder = new UriBuilder(url);
                builder.Path = builder.Path.Replace("/200/", "/");
                return builder.ToString();
            }
            catch(Exception e)
            {//Chyby ignorovat a vrátit původní url
                TelemetrySetting.TelemetryClientInstance.TrackException(e);
                return url;
            }
        }
    }
}
