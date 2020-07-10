using IronWebScraper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Configuration;
using Microsoft.IdentityModel.Protocols;

namespace RunWebScraper.RunningScrapers
{
    class RunningScraper:WebScraper
    {


    public override void Init()
        {
            License.LicenseKey = "LicenseKey"; // Write License Key
            this.LoggingLevel = WebScraper.LogLevel.All; // All Events Are Logged
            this.Request("https://www.greatrun.org/events", Parse);
        }

    

    /// <summary>
    /// Override this method to create the default Response handler for your web scraper.
    /// If you have multiple page types, you can add additional similar methods.
    /// </summary>

    public override void Parse(Response response)
        {
            // set working directory for the project
            this.WorkingDirectory = @"C:\Users\jenir\source\repos\WebScraper\Output";
            // Loop on all Links
            foreach (var links in response.Css("div.event-box-2018-well >a[href^=\"https\"]"))
            {
                if (links.Attributes["class"] != "clearfix")
                {

                   var link= links.GetAttribute("href");
 
    


                    // Save Result to File
                    Scrape(new ScrapedData() { { "Link", link } }, "Links2.Jsonl");
                }
                
      
            }
            // Loop On All Links
            if (response.CssExists("div.prev-post > a[href]"))
            {
                // Get Link URL
                var next_page = response.Css("div.prev-post > a[href]")[0].Attributes["next"];
                // Scrpae Next URL
                this.Request(next_page, Parse);
            }
        }
    }

}

