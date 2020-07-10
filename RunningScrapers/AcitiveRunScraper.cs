using IronWebScraper;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Configuration;
using Microsoft.IdentityModel.Protocols;
using API_dash.Models;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace RunWebScraper.RunningScrapers
{
    class ActiveRunScraper : WebScraper
    {


        public override void Init()
        {
            License.LicenseKey = "LicenseKey"; // Write License Key
            this.LoggingLevel = WebScraper.LogLevel.All; // All Events Are Logged
            this.Request("https://www.active.com/search?keywords=&include_virtual_events=1&location=Scotland&category=Activities&daterange=All+future+dates", Parse);
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
            foreach (var links in response.Css("article.activity-feed a"))
            {

                var runningEvent = new RunningEvent();
                // set working directory for the project

                runningEvent.URL = links.GetAttribute("href");

 
                this.Request(runningEvent.URL, ParseDetails, new MetaData() { { "link", runningEvent } });
            }
            // Loop On All Links
            if (response.CssExists("a.next-pagebtn-small-yellow" ))
            {
                // Get Link URL
                var next_page = response.Css("a.next-page.btn-small-yellow")[0].Attributes["href"];
                // Scrpae Next URL
                this.Request(next_page, Parse);
            }
        }

        public void ParseDetails(Response response)
        {
            var runningEvent = response.MetaData.Get<RunningEvent>("link");

            //Set event name to json values from spript tag
            runningEvent.Name = response.Css("h1.event-title")[0].TextContent;


            var longitude = response.QuerySelector("meta[property=\"og:longitude\"]").GetAttribute("content");
            
            //Set longitude and latitude values to json values from spript tag
         
            var latitude = response.Css("meta[property=\"og:latitude\"]")[0].GetAttribute("content");

            runningEvent.longitude = float.Parse(longitude, CultureInfo.InvariantCulture.NumberFormat);       //Convert String values to float
            runningEvent.latitude = float.Parse(latitude, CultureInfo.InvariantCulture.NumberFormat);


            //Set loaction country always last array element
            runningEvent.Country = response.Css("meta[property^=\"og:country-name\"]")[0].GetAttribute("content");


            runningEvent.Postcode =  response.Css("meta[property^=\"og:postal-code\"]")[0].GetAttribute("content");
            runningEvent.City =  response.Css("meta[property^=\"og:locality\"]")[0].GetAttribute("content");

            runningEvent.Location = response.Css("meta[property^=\"og:street-address\"]")[0].GetAttribute("content"); 
        


            //Scrape and save to txt file
            Scrape(runningEvent, "ActiveRunDetails.Jsonl");
        }






    }

}

