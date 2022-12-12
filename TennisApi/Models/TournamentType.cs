namespace TennisApi.Models
{
    public class TournamentType
    {
        public string Type { get; set; }
        public League League { get; set; }
        public int NumberOfRounds { get; set; }
        public List<int> PointsForEachRound { get; set; }
    }
}
