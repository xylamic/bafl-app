namespace bafl_unit_test;

using bafl_app.library;
using System.Text.Json;
using Xunit.Abstractions;

public class BaflClubTest
{
    private readonly ITestOutputHelper output;

    public BaflClubTest(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact()]
    public void ClubGeneration()
    {
        Dictionary<int, BaflClub> clubList = new Dictionary<int, BaflClub>();

        int id = 1;
        BaflClub club = new BaflClub(
            "Bay Area",
            "Buccaneers",
            "Legacy",
            "Mermaids",
            "https://www.bayareabucs.org");
        clubList.Add(id++, club);

        club = new BaflClub(
            "La Porte",
            "Texans",
            "Darlings",
            "Stars",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Southbelt",
            "Dolphins",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Brazosport",
            "Longhorns",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Alvin",
            "Raiders",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Angleton",
            "Wildcats",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Barbers Hill",
            "Eagles",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "East End",
            "Eagles",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Hitchcock",
            "Red Raiders",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "League City",
            "49ers",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Magnolia Park",
            "Sharks",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Manvel",
            "Texans",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "North Shore",
            "Mustangs",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Pasadena",
            "Panthers",
            "",
            "",
            "");
        clubList.Add(id++, club);


        club = new BaflClub(
            "Pearland",
            "Patriots",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Pearland",
            "Texans",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Sagemont",
            "Cowboys",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Southeast Houston",
            "Wildcats",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Santa Fe",
            "Braves",
            "",
            "",
            "");
        clubList.Add(id++, club);

        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(clubList, options);
        output.WriteLine(jsonString);

        var clubList2 =
            JsonSerializer.Deserialize<Dictionary<int, BaflClub>>(jsonString);

        Assert.Equal(19, clubList2?.Count);
    }
}
