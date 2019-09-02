using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jidelnicek.Backend.Util
{
    public static class TelemetrySetting
    {
        public static TelemetryClient TelemetryClientInstance { get; set; }
    }
}
