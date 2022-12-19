namespace bafl_unit_test;

using bafl.library;
using System.Text.Json;
using Xunit.Abstractions;

public class BAFLClubTest
{
    private readonly ITestOutputHelper output;

    public BAFLClubTest(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact()]
    public void ClubGeneration()
    {
        Dictionary<int, BAFLClub> clubList = new Dictionary<int, BAFLClub>();

        int id = 1;
        BAFLClub club = new BAFLClub(
            "Bay Area",
            "Buccaneers",
            "Legacy",
            "Mermaids",
            "https://www.bayareabucs.org");
        clubList.Add(id++, club);

        club = new BAFLClub(
            "La Porte",
            "Texans",
            "Darlings",
            "Stars",
            "");
        clubList.Add(id++, club);

        club = new BAFLClub(
            "Southbelt",
            "Dolphins",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BAFLClub(
            "Brazosport",
            "Longhorns",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BAFLClub(
            "Alvin",
            "Raiders",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BAFLClub(
            "Angleton",
            "Wildcats",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BAFLClub(
            "Barbers Hill",
            "Eagles",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BAFLClub(
            "East End",
            "Eagles",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BAFLClub(
            "Hitchcock",
            "Red Raiders",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BAFLClub(
            "League City",
            "49ers",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BAFLClub(
            "Magnolia Park",
            "Sharks",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BAFLClub(
            "Manvel",
            "Texans",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BAFLClub(
            "North Shore",
            "Mustangs",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BAFLClub(
            "Pasadena",
            "Panthers",
            "",
            "",
            "");
        clubList.Add(id++, club);


        club = new BAFLClub(
            "Pearland",
            "Patriots",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BAFLClub(
            "Pearland",
            "Texans",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BAFLClub(
            "Sagemont",
            "Cowboys",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BAFLClub(
            "Southeast Houston",
            "Wildcats",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BAFLClub(
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
            JsonSerializer.Deserialize<Dictionary<int, BAFLClub>>(jsonString);

        Assert.Equal(19, clubList2.Count);
    }
}
