using IronWebScraper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Configuration;
using Microsoft.IdentityModel.Protocols;

namespace RunWebScraper.RunningScrapers
{
    class LetsDoThisScraper2 : WebScraper
    {


        public override void Init()
        {
            License.LicenseKey = "LicenseKey"; // Write License Key
            this.LoggingLevel = WebScraper.LogLevel.All; // All Events Are Logged
            
            
            this.Request("https://www.letsdothis.com/gb/running-events", Parse);
        }



        /// <summary>
        /// Override this method to create the default Response handler for your web scraper.
        /// If you have multiple page types, you can add additional similar methods.
        /// </summary>
        /// 
        public List<string> eventLink { get; set; }

        public override void Parse(Response response)
        {
            // set working directory for the project
            this.WorkingDirectory = @"C:\Users\jenir\source\repos\WebScraper\Output";
            // Loop on all Links
            foreach (var links in response.Css("li.s8_u a[href]"))
            {


                var link = links.TextContentClean;

                eventLink.Add(link);

                // Save Result to File
                Scrape(new ScrapedData() { { "Link", link } }, "LetsDoThis.Jsonl");


            }


            // Loop On All Links
            if (response.CssExists("a[rel^=\"next\"]"))
            {
                // Get Link URL=

                var next_page = response.Css("a[rel^=\"next\"]")[0].Attributes["href"];
                // Scrpae Next URL
                this.Request(next_page, Parse);
            }
        }
    }

}