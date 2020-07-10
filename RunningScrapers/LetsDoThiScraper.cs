using IronWebScraper;
using System;
using API_dash.Models;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using System.Linq;

namespace RunWebScraper.RunningScrapers
{
    class LetsDoThisScraper : WebScraper
    {


        public override void Init()
        {
            License.LicenseKey = "License"; // Write License Key
            this.LoggingLevel = WebScraper.LogLevel.All; // All Events Are Logged
            this.Request("https://www.letsdothis.com/gb/running-events", Parse);
        }



        /// <summary>
        /// Override this method to create the default Response handler for your web scraper.
        /// If you have multiple page types, you can add additional similar methods.
        /// </summary>
        /// 


        public override void Parse(Response response)
        {



            // Loop on all Links
            foreach (var links in response.Css("li[data-testing-id=\"event-card\"] a"))
            {
                var runningEvent = new RunningEvent();
                // set working directory for the project
                
               runningEvent.URL = links.GetAttribute("href");
                this.WorkingDirectory = @"C:\Users\jenir\source\repos\WebScraper\Output";
                this.Request(runningEvent.URL, ParseDetails, new MetaData() { { "link", runningEvent } });

            }

            if (response.CssExists("a[rel^=\"next\"]"))
            {
                // Get Link URL=
 
                    var next_page = response.Css("a[rel^=\"next\"]")[0].Attributes["href"];
                // Scrpae Next URL
                this.Request(next_page, Parse);
            }


        }
        public void ParseDetails(Response response)
        {
            var runningEvent = response.MetaData.Get<RunningEvent>("link");




            var scriptData = response.QuerySelector("script[type=\"application/ld+json\"]").TextContent;
            
            //Convert script data contents to jason object
            JObject jsonScriptData = JObject.Parse(scriptData);

            //Set event name to json values from spript tag
            runningEvent.Name = jsonScriptData.GetValue("name").ToString();

            //Set longitude and latitude values to json values from spript tag
            var longitude = jsonScriptData.GetValue("longitude").ToString();
            var latitude = jsonScriptData.GetValue("latitude").ToString();

            runningEvent.longitude = float.Parse(longitude, CultureInfo.InvariantCulture.NumberFormat);       //Convert String values to float
            runningEvent.latitude = float.Parse(latitude, CultureInfo.InvariantCulture.NumberFormat);


            //Parse event date and time to json values from spript tag 
            var date = jsonScriptData.GetValue("startDate");
            runningEvent.date = DateTime.Parse(date.ToString());

            //Set event location to json values from spript tag
            var fullLocation = jsonScriptData.GetValue("address").ToString();

            //Get Location details
            string[] locationDetails = fullLocation.Split(',');
            int size = locationDetails.Length;

            //Set loaction country always last array element
            runningEvent.Country = locationDetails[(size - 1)];

            //City and postcode information is formatted together not split by commas
            var cityAndPostcode = locationDetails[(size - 2)]; ;
            string lastWord = cityAndPostcode.Substring(cityAndPostcode.LastIndexOf(' ') + 1);

            runningEvent.Postcode = lastWord;    //Last array element always post code
            runningEvent.City = cityAndPostcode.Remove(cityAndPostcode.LastIndexOf(' ') +1);

            //Get Location details like street address or building and combine
            string full_add = "";
            for (int i = 0; i < (size - 2); i++)
            {

                full_add += locationDetails[i];
            }
            runningEvent.Location = full_add;

            //Distance
            if (response.CssExists("article[data-testing-id=\"race-summary\"]"))
            {
                // Loop on all Links
                foreach (var distanceTitle in response.Css("article[data-testing-id=\"race-summary\"] h3"))
                {
                    var distanceLength = distanceTitle.TextContentClean;
                    runningEvent.Distance.Add(distanceLength);
                }
            }
            


            //Scrape and save to txt file
            Scrape(runningEvent, "LDTDetails.Jsonl");
        }




    }

}

