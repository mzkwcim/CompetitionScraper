namespace CompetitionScraper
{
    public class CustomStructure
    {
        public string AthleteName { get; set; }
        public Dictionary<string, List<string>> ListOfStartsOnCompetition { get; set; }
        public int MaximumNumberOfStartsOnCompetition { get; set; }
        public Dictionary<string, Dictionary<string, string>> CompetitionNameCityAndName { get; set; }
        public string AthleteCategory { get; set; }

        public CustomStructure(string athleteName, Dictionary<string, List<string>> listOfStartsOnCompetition, int maximumNumberOfStartsOnCompetition, Dictionary<string, Dictionary<string, string>> competitionNameCityAndName, string athleteCategory)
        {
            AthleteName = athleteName;
            ListOfStartsOnCompetition = listOfStartsOnCompetition;
            MaximumNumberOfStartsOnCompetition = maximumNumberOfStartsOnCompetition;
            CompetitionNameCityAndName = competitionNameCityAndName;
            AthleteCategory = athleteCategory;
        }
    }
}
