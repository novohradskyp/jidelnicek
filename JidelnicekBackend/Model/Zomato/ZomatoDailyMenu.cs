using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jidelnicek.Backend.Util;

namespace Jidelnicek.Backend.Model.Zomato
{
    [JsonObject]
    internal class ZomatoDailyMenu
    {
        [JsonProperty("daily_menu_id")]
        public int Id { get; set; }

        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }

        [JsonProperty("end_date")]
        public DateTime EndDate { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("dishes", ItemConverterType = typeof(SkipLevelConverter))]
        public List<ZomatoDish> Dishes { get; set; }
    }
}
