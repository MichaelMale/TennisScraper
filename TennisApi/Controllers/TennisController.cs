using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TennisApi.Extensions;
using TennisApi.Models;

namespace TennisApi.Controllers
{
    public class TennisController : Controller
    {
        // Create a get method to return a player by ID
        [HttpGet]
        [Route("api/tennis/{id}")]
        public IActionResult GetPlayerById(int id)
        {
            return Json(new TennisPlayer { Id = id, Name = "Roger Federer",
                Age = 40,
                Country = "Switzerland",
                Points = 0,
                Rank = 0 });
        }

        // Create a post method to create a player by the name of the player
        [HttpPost]
        [Route("api/tennis/{name}")]
        public IActionResult CreatePlayer(string name)
        {
            return Json(ATPScraper.ScrapePlayer(name));
        }

    }
}
