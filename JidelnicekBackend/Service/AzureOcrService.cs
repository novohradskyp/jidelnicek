﻿using Jidelnicek.Backend.Exceptions;
using Jidelnicek.Backend.Model.AzureOcr;
using Jidelnicek.Backend.Util;
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
                TelemetrySetting.TelemetryClientInstance.TrackTrace($"OCR - asyncBatchAnalyze - status code: {ocrResponse.StatusCode.ToString()}");
                if (ocrResponse.StatusCode != HttpStatusCode.Accepted)
                    return string.Empty;
                resultLocation = ocrResponse.Headers.GetValues("Operation-Location").FirstOrDefault();
            }
            OcrResult ocrResult;
            int progressiveWaitTimeMs = 200;
            do
            {
                TelemetrySetting.TelemetryClientInstance.TrackTrace($"OCR - ReadResult - waiting for {progressiveWaitTimeMs}ms");
                await Task.Delay(progressiveWaitTimeMs);
                progressiveWaitTimeMs = Math.Min(progressiveWaitTimeMs * 2, 60000);

                ocrResult = await ReadResult(resultLocation);
                TelemetrySetting.TelemetryClientInstance.TrackTrace($"OCR - ReadResult - response status: {ocrResult?.status}");
            }
            while (ShouldReadAgain(ocrResult));
            if (ocrResult == null || !"Succeeded".Equals(ocrResult.status, StringComparison.InvariantCultureIgnoreCase))
            {
                var exception = new MenuReadException("OCR reading error");
                exception.Data["OCR status"] = ocrResult?.status ?? "null";
                throw exception;
            }

            var builder = new StringBuilder();
            TelemetrySetting.TelemetryClientInstance.TrackTrace($"OCR - ReadResult - response have {ocrResult.recognitionResults.Count()} results and {ocrResult.recognitionResults.First().lines.Count()} lines in first one");
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
                TelemetrySetting.TelemetryClientInstance.TrackTrace($"OCR - ReadResult - HTTP status code: {ocrResponse.StatusCode.ToString()}");
                if (!ocrResponse.IsSuccessStatusCode)
                    return null;
                jsonResult = await ocrResponse.Content.ReadAsStringAsync();
            }
            return JsonConvert.DeserializeObject<OcrResult>(jsonResult);
        }

        private bool ShouldReadAgain(OcrResult ocrResult)
        {
            if (ocrResult == null)
                return false;
            if (ocrResult.status.Equals("Running", StringComparison.InvariantCultureIgnoreCase)
                || ocrResult.status.Equals("NotStarted", StringComparison.InvariantCultureIgnoreCase))
                return true;
            else
                return false;
        }
    }
}
