#if IS_RI_NET80

namespace bafl_app.testing;

using bafl_app.library;
using System.Text.Json;
using System.Diagnostics;

public class BaflClubTest
{
    public BaflClubTest()
    {
    }

    public void ClubGeneration()
    {
        Dictionary<int, BaflClub> clubList = new Dictionary<int, BaflClub>();

        int id = 1;
        BaflClub club = new BaflClub(
            "Bay Area",
            "Buccaneers",
            "Legacy",
            "Mermaids",
            "https://www.bayareabucs.org",
            "LSA",
            "coord1");
        clubList.Add(id++, club);

        club = new BaflClub(
            "La Porte",
            "Texans",
            "Darlings",
            "Stars",
            "",
            "",
            "");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Southbelt",
            "Dolphins",
            "",
            "",
            "",
            "Loc1",
            "coord2");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Brazosport",
            "Longhorns",
            "",
            "",
            "",
            "Loc2",
            "coord3");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Alvin",
            "Raiders",
            "",
            "",
            "",
            "Loc3",
            "coord4");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Angleton",
            "Wildcats",
            "",
            "",
            "",
            "Loc4",
            "coord5");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Barbers Hill",
            "Eagles",
            "",
            "",
            "",
            "Loc5",
            "coord6");
        clubList.Add(id++, club);

        club = new BaflClub(
            "East End",
            "Eagles",
            "",
            "",
            "",
            "Loc6",
            "coord7");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Hitchcock",
            "Red Raiders",
            "",
            "",
            "",
            "Loc7",
            "coord8");
        clubList.Add(id++, club);

        club = new BaflClub(
            "League City",
            "49ers",
            "",
            "",
            "",
            "Loc8",
            "coord9");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Magnolia Park",
            "Sharks",
            "",
            "",
            "",
            "Loc9",
            "coord10");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Manvel",
            "Texans",
            "",
            "",
            "",
            "Loc10",
            "coord11");
        clubList.Add(id++, club);

        club = new BaflClub(
            "North Shore",
            "Mustangs",
            "",
            "",
            "",
            "Loc11",
            "coord12");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Pasadena",
            "Panthers",
            "",
            "",
            "",
            "Loc12",
            "coord13");
        clubList.Add(id++, club);


        club = new BaflClub(
            "Pearland",
            "Patriots",
            "",
            "",
            "",
            "Loc13",
            "coord14");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Pearland",
            "Texans",
            "",
            "",
            "",
            "Loc14",
            "coord15");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Sagemont",
            "Cowboys",
            "",
            "",
            "",
            "Loc15",
            "coord16");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Southeast Houston",
            "Wildcats",
            "",
            "",
            "",
            "Loc16",
            "coord17");
        clubList.Add(id++, club);

        club = new BaflClub(
            "Santa Fe",
            "Braves",
            "",
            "",
            "",
            "Loc17",
            "coord18");
        clubList.Add(id++, club);

        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(clubList, options);
        //output.WriteLine(jsonString);

        var clubList2 =
            JsonSerializer.Deserialize<Dictionary<int, BaflClub>>(jsonString);

        Debug.Assert(19 == clubList2?.Count);
    }
}

#endif