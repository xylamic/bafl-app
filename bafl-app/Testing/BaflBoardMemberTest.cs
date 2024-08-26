#if IS_RI_NET80

namespace bafl_app.testing;

using bafl_app.library;
using System.Diagnostics;
using System.Text.Json;

public class BaflBoardMemberTest
{

    public BaflBoardMemberTest()
    {
    }

    public void BoardGeneration()
    {
        List<BaflBoardMember> memberList = new List<BaflBoardMember>();

        BaflBoardMember member = new BaflBoardMember(
            "President",
            "Gloria Sanchez",
            "eeeagles@yahoo.com");
        memberList.Add(member);

        member = new BaflBoardMember(
            "1st Vice President",
            "Chris Sweat",
            "chris_sweat@ymail.com");
        memberList.Add(member);

        member = new BaflBoardMember(
            "Secretary",
            "Jennifer Sorsby",
            "baflsecretary@gmail.com");
        memberList.Add(member);

        member = new BaflBoardMember(
            "Treasurer",
            "Kari",
            "bafltreasurer21@gmail.com");
        memberList.Add(member);

        member = new BaflBoardMember(
            "Drill Director",
            "Bel Wranich",
            "bafldrilldirector@gmail.com");
        memberList.Add(member);

        member = new BaflBoardMember(
            "Athletic Director",
            "Denny Wranich",
            "dwranich82@gmail.com");
        memberList.Add(member);

        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(memberList, options);
        // output.WriteLine(jsonString);

        var memberList2 =
            JsonSerializer.Deserialize<List<BaflBoardMember>>(jsonString);

        Debug.Assert(6 == memberList2?.Count);
    }
}

#endif