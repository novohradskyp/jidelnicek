using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Jidelnicek.Backend.Util;

namespace Jidelnicek.Backend.Model.Zomato
{
    [JsonObject]
    public class ZomatoResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("daily_menus", ItemConverterType = typeof(SkipLevelConverter))]
        public List<ZomatoDailyMenu> Menus { get; set; }
    }
}
