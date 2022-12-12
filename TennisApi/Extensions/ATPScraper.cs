using TennisApi.Models;
using HtmlAgilityPack;
using System.Net;

namespace TennisApi.Extensions
{
    public static class ATPScraper
    {
        public static string ScrapePlayer(string nameOfPlayer)
        {
            var splitName = nameOfPlayer.Split(" ");
            HtmlWeb web = new();
            // Set timeout to 5 seconds
            web.PreRequest = delegate (HttpWebRequest webRequest)
            {
                webRequest.Timeout = 5000;
                return true;
            };
            try
            {

                // Load rankings list first to locate a player
                var doc = web.Load("https://www.atptour.com/en/rankings/singles/live?rankRange=All%20Rankings&countryCode=all");
                // Select node where a div with class "player-name" is located and partly matches the name of the player
                var node = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("player-name"));
                // Get the href attribute of the node
                var href = node.First().SelectSingleNode("//a").Attributes["href"].Value;
                // Load the player's page
                var playerDoc = web.Load("https://www.atptour.com" + href);
                // Select the node where the player's ranking is located
            } catch (Exception e)
            {
                throw new Exception("Unable to find player with name " + nameOfPlayer, e);
            }



            return "";
        }
    }
}
