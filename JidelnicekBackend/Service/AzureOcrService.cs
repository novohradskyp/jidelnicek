using Jidelnicek.Backend.Model.AzureOcr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jidelnicek.Backend.Service
{
    class AzureOcrService : IOcrService
    {
        public async Task<string> GetTextFromImageAsync(string imageUrl)
        {
            string resultLocation;
            using (HttpClient ocrClient = new HttpClient())
            {
                Uri azureOcrEndpoint = new Uri(ConfigurationManager.AppSettings["AzureVisionEndpoint"]);
                UriBuilder azureOcrUrl = new UriBuilder(azureOcrEndpoint);
                azureOcrUrl.Path = "/vision/v2.0/read/core/asyncBatchAnalyze";
                ocrClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["AzureVisionApiKey"]);
                string imageUrlInJson = JsonConvert.SerializeObject(new { url = imageUrl });
                var ocrPostContent = new StringContent(imageUrlInJson, Encoding.Default, "application/json");
                var ocrResponse = await ocrClient.PostAsync(azureOcrUrl.Uri, ocrPostContent);
                if (ocrResponse.StatusCode != HttpStatusCode.Accepted)
                    return string.Empty;
                resultLocation = ocrResponse.Headers.GetValues("Operation-Location").FirstOrDefault();
            }
            OcrResult ocrResult;
            int progressiveWaitTimeMs = 100;
            do
            {
                ocrResult = await ReadResult(resultLocation);
                if (ocrResult != null && ocrResult.status.Equals("Running", StringComparison.InvariantCultureIgnoreCase))
                {
                    await Task.Delay(progressiveWaitTimeMs);
                    progressiveWaitTimeMs = Math.Min(progressiveWaitTimeMs * 2, 60000);
                }
            }
            while (ocrResult != null && ocrResult.status.Equals("Running", StringComparison.InvariantCultureIgnoreCase));
            if (ocrResult == null || !"Succeeded".Equals(ocrResult.status, StringComparison.InvariantCultureIgnoreCase))
                return string.Empty;

            var builder = new StringBuilder();
            foreach (var line in ocrResult.recognitionResults.First().lines)
            {
                builder.AppendLine(line.text);
            }

            return builder.ToString();
        }

        private async Task<OcrResult> ReadResult(string resultLocation)
        {
            string jsonResult;
            using (HttpClient ocrClient = new HttpClient())
            {
                ocrClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["AzureVisionApiKey"]);
                var ocrResponse = await ocrClient.GetAsync(resultLocation);
                if (!ocrResponse.IsSuccessStatusCode)
                    return null;
                jsonResult = await ocrResponse.Content.ReadAsStringAsync();
            }
            return JsonConvert.DeserializeObject<OcrResult>(jsonResult);
        }
    }
}
