using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Jidelnicek.Backend.Model.Zomato;
using Newtonsoft.Json.Serialization;
using Jidelnicek.Backend.Misc;
using System.Configuration;

namespace Jidelnicek.Backend.Service
{
    public class ZomatoReaderService
    {
        private const string ZomatoApiUrl = "https://developers.zomato.com/api/v2.1/";

        public async Task<IEnumerable<ZomatoDailyMenu>> ReadMenuAsync(int restaurantId)
        {
            UriBuilder ApiUri = new UriBuilder(ZomatoApiUrl + "dailymenu")
            {
                Query = "res_id=" + restaurantId
            };
            var Settings = new JsonSerializerSettings()
            {
                DateFormatString = "yyyy-MM-dd hh:mm:ss",
                MissingMemberHandling = MissingMemberHandling.Error,
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };
            var Client = GetClient();
            var Response = await Client.GetAsync(ApiUri.Uri);
            if (Response.StatusCode == HttpStatusCode.OK)
            {
                var ResultContent = await Response.Content.ReadAsStringAsync();
                var Result = JsonConvert.DeserializeObject<ZomatoResponse>(ResultContent, Settings);
                return Result.Menus;
            }
            else
            {
                var Error = new ZomatoReadException("Zomato call returned " + Response.StatusCode);
                Error.Data.Add("Zomato restaurant id", restaurantId);
                Error.Data.Add("Response status code", (int)Response.StatusCode);
                try
                {
                    var Result = JsonConvert.DeserializeObject<ZomatoResponse>(await Response.Content.ReadAsStringAsync());
                    if (Result != null && !string.IsNullOrWhiteSpace(Result.Status))
                        Error.Data.Add("Zomato status text", Result.Status);
                }
                catch (Exception)
                {
                    //Výjimka při zpracování chyby se ignoruje
                }
                throw Error;
            }
        }

        private static HttpClient GetClient()
        {
            HttpClient Client = new HttpClient();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("user_key", ConfigurationManager.AppSettings["ZomatoApiKey"]);
            return Client;
        }
    }
}
