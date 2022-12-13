namespace TennisApi.Models
{
    public class TennisPlayer
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public int Age { get; set; }
        public int Rank { get; set; }
        public int Points { get; set; }
    }
}
