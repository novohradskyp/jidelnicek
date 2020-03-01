using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Jidelnicek.Backend.Model;
using Jidelnicek.Backend.Service;
using Jidelnicek.Backend.Util;

namespace Jidelnicek.Backend.Provider
{
    internal class WebPageMenuProvider : IMenuProvider
    {
        private readonly string url;
        private readonly string nodeXpath;
        private readonly IUrlTranslator imageUrlTranslator;

        public IOcrService OcrService { get; set; } = new AzureOcrService();

        public WebPageMenuProvider(string url, string nodeXpath, IUrlTranslator imageUrlTranslator = null)
        {
            this.url = url;
            this.nodeXpath = nodeXpath;
            this.imageUrlTranslator = imageUrlTranslator;
        }

        public async Task<IEnumerable<IMenuItem>> ProvideMenuAsync()
        {
            var menus = new LinkedList<IMenuItem>();
            var menuText = await LoadTextFromHtmlNode();
            if (string.IsNullOrWhiteSpace(menuText))
                return menus;
            ParseMenuFromText(menuText, menus);
            return menus;
        }

        private void ParseMenuFromText(string menuText, LinkedList<IMenuItem> menus)
        {
            var today = DateTime.Now;
            var tomorow = today.AddDays(1);
            var menuParser = new Regex(@"\s*(\S.*?)\s*(\d{2,})?(?:,-)?\s?(?:Kč|kč|Kc|kc|KC)?\s*$", RegexOptions.Multiline);
            var menuMatches = menuParser.Matches(menuText);
            bool insideCurrentDay = false;
            foreach (Match match in menuMatches)
            {
                if (insideCurrentDay)
                {
                    if (IsDayMark(match.Value, tomorow.DayOfWeek))
                    {
                        TelemetrySetting.TelemetryClientInstance.TrackTrace("Found next day");
                        break;
                    }
                    TelemetrySetting.TelemetryClientInstance.TrackTrace("Found menu inside current day");
                    var menuItem = new MenuItem();
                    menuItem.Day = today;
                    menuItem.Name = match.Groups[1].Value;
                    menuItem.Price = match.Groups[2].Value;
                    if (!string.IsNullOrEmpty(menuItem.Price))
#pragma warning disable S1643 // Strings should not be concatenated using '+' in a loop
                        menuItem.Price += " Kč";
#pragma warning restore S1643 // Strings should not be concatenated using '+' in a loop
                    menus.AddLast(menuItem);
                }
                else
                {
                    if (IsDayMark(match.Value, today.DayOfWeek))
                    {
                        insideCurrentDay = true;
                        TelemetrySetting.TelemetryClientInstance.TrackTrace("Found current day");
                    }
                }
            }
        }

        private async Task<string> LoadTextFromHtmlNode()
        {
            var Client = new HttpClient();
            var webResponse = await Client.GetAsync(url);
            if (!webResponse.IsSuccessStatusCode)
                return null;
            var responseStream = await webResponse.Content.ReadAsStreamAsync();
            var document = new HtmlDocument();
            document.Load(responseStream, true);
            var menuNodes = document.DocumentNode.SelectNodes(nodeXpath);
            if (menuNodes == null)
                return null;
            StringBuilder resultBuilder = new StringBuilder();
            TelemetrySetting.TelemetryClientInstance.TrackTrace($"Found {menuNodes.Count} nodes.");
            foreach (var node in menuNodes)
            {
                var nodeText = await HtmlNodeToText(node);
                resultBuilder.AppendLine(nodeText);
            }
            return resultBuilder.ToString();
        }

        private async Task<string> HtmlNodeToText(HtmlNode node)
        {
            if ("img".Equals(node.Name, StringComparison.InvariantCultureIgnoreCase))
            {//Nalezený node je obrázek
                return await GetTextFromImage(node.GetAttributeValue("src", string.Empty));
            }
            else if (string.Equals(node.Name, "table", StringComparison.InvariantCultureIgnoreCase))
            {//Nalezený node je tabulka
                return GetTextFromHtmlTable(node);
            }
            else //Nalezený node je normální HTML
                return WebUtility.HtmlDecode(node.InnerText);
            
        }

        private string GetTextFromHtmlTable(HtmlNode node)
        {
            var result = new StringBuilder();
            var rowNodes = node.SelectNodes(".//tr");
            foreach (var rowNode in rowNodes)
            {
                foreach (var column in rowNode.SelectNodes(".//td"))
                {
                    result.Append(WebUtility.HtmlDecode(column.InnerText.Trim()));
                    result.Append('\t');
                }
                result.AppendLine();
            }
            return result.ToString();
        }

        private async Task<string> GetTextFromImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return string.Empty;
            if (imageUrlTranslator != null)
                imageUrl = imageUrlTranslator.TranslateUrl(imageUrl);
            return await OcrService.GetTextFromImageAsync(imageUrl);
        }

        private bool IsDayMark(string line, DayOfWeek day)
        {
            var formatInfo = new CultureInfo("cs-cz").DateTimeFormat;
            return StringContainsIgnoreCaseAndDiacritics(line, formatInfo.DayNames[(int)day]);
        }

        private bool StringContainsIgnoreCaseAndDiacritics(string text, string contained)
        {
            string textWoDiacritics = RemoveDiacritics(text);
            string containedWoDiacritics = RemoveDiacritics(contained);
            return textWoDiacritics.IndexOf(containedWoDiacritics, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
