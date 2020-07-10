using RunWebScraper.RunningScrapers;

namespace RunWebScraper
{
    class Program
    {
        static void Main(string[] args)
        {


            //ActiveRunScraper scrape = new ActiveRunScraper();
            //scrape.Start();

            ActiveRunScraper scrape = new ActiveRunScraper();
            scrape.Start();

        }
    }

}
