﻿using System;
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

namespace Jidelnicek.Backend.Provider
{
    internal class WebPageMenuProvider : IMenuProvider
    {
        private readonly string url;
        private readonly string nodeXpath;

        public WebPageMenuProvider(string url, string nodeXpath)
        {
            this.url = url;
            this.nodeXpath = nodeXpath;
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
            var menuParser = new Regex(@"^\s*(\w.*?)\s*(\d+)?(?:,-)?(?:Kč|kč|Kc|kc)?\s*$", RegexOptions.Multiline);
            var menuMatches = menuParser.Matches(menuText);
            bool insideCurrentDay = false;
            foreach (Match match in menuMatches)
            {
                if (insideCurrentDay)
                {
                    if (IsDayMark(match.Value, tomorow.DayOfWeek))
                        break;
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
                        insideCurrentDay = true;
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
            foreach (var node in menuNodes)
            {
                var nodeText = WebUtility.HtmlDecode(node.InnerText);
                resultBuilder.AppendLine(nodeText);
            }
            return resultBuilder.ToString();
        }

        private bool IsDayMark(string line, DayOfWeek day)
        {
            var formatInfo = new CultureInfo("cs-cz").DateTimeFormat;
            return StringContainsIgnoreCase(line, formatInfo.DayNames[(int)day]);
        }

        private bool StringContainsIgnoreCase(string text, string contained)
        {
            return text.ToUpper().Contains(contained.ToUpper());
        }
    }
}
