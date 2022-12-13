using TennisApi.Models;
using HtmlAgilityPack;
using System.Net;
using System.Globalization;
using Bia.Countries.Iso3166;

namespace TennisApi.Extensions
{
    public static class ATPScraper
    {
        private static int GetPlayerRank(this HtmlDocument playerDoc)
        {
            var rankingBlock = playerDoc.DocumentNode.Descendants(0).Where(n => n.HasClass("player-ranking-position")).FirstOrDefault();
            var rankingNode = rankingBlock?.ChildNodes;
            string? ranking = null;
            if (rankingNode != null)
            {
                foreach (var node in rankingNode)
                {
                    if (node.InnerHtml.Any(char.IsDigit))
                    {
                        ranking = node.InnerText;
                        break;
                    }
                }
            }


            return int.Parse(ranking ?? "0");
        }

        private static int GetAge(this DateTime birthDate)
        {
            DateTime n = DateTime.Now; // To avoid a race condition around midnight
            int age = n.Year - birthDate.Year;

            if (n.Month < birthDate.Month || (n.Month == birthDate.Month && n.Day < birthDate.Day))
                age--;

            return age;
        }

        private static int GetPlayerAge(this HtmlDocument playerDoc)
        {
            // get the birthday codeblock
            var birthdayBlock = playerDoc.DocumentNode.Descendants(0).FirstOrDefault(n => n.HasClass("table-birthday"))?.InnerText;
            birthdayBlock = birthdayBlock?.Trim()
                .Replace("(", string.Empty)
                .Replace(")", string.Empty);
            if (birthdayBlock == null)
            {
                throw new Exception("Player age not found");
            } else
            {
                var birthday = DateTime.ParseExact(birthdayBlock, "yyyy.MM.dd", CultureInfo.InvariantCulture);
                return birthday.GetAge();
            }
           
        }

        private static string GetPlayerCountry(this HtmlDocument playerDoc)
        {
            var countryBlock = playerDoc.DocumentNode.Descendants(0).FirstOrDefault(n => n.HasClass("player-flag-code"))?.InnerText;
            var country = Countries.GetCountryByAlpha3(countryBlock);
            return country != null ? country.ActiveDirectoryName : "Not Listed";
        }

        public static TennisPlayer ScrapePlayer(string nameOfPlayer)
        {
            var splitName = nameOfPlayer.Split(" ");
            HtmlWeb web = new();
            var playerDoc = new HtmlDocument();
            // Set timeout to 5 seconds
            web.PreRequest = delegate (HttpWebRequest webRequest)
            {
                webRequest.Timeout = 5000;
                return true;
            };
            try
            {
                // Loading the ATP player list, get a player by its name
                var doc = web.Load("https://www.atptour.com/en/rankings/singles/live?rankRange=All%20Rankings&countryCode=all");
                var node = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("player-name") && n.InnerHtml.Contains(splitName[0])
                && n.InnerHtml.Contains(splitName[1])).FirstOrDefault();
                var playerUrl = node?.InnerHtml;
                var document = new HtmlDocument();
                document.LoadHtml(playerUrl);
                var tempValue = document.DocumentNode.SelectSingleNode("//a");
                var href = tempValue.Attributes["href"].Value;
                playerDoc = web.Load("https://www.atptour.com" + href);
            }
            catch (Exception e)
            {
                throw new Exception("Unable to find player with name " + nameOfPlayer, e);
            }

            return new TennisPlayer
            {
                Name = nameOfPlayer,
                Rank = playerDoc.GetPlayerRank(),
                Country = playerDoc.GetPlayerCountry(),
                Age = playerDoc.GetPlayerAge(),
                Points = 0
            };
        }



    }

}
