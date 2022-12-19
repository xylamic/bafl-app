using bafl.library;

namespace bafl_app;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
		BindingContext = this;

		_ = InitPage();
	}

	public async Task InitPage()
	{
		await App.UpdateConfiguration();
		OnPropertyChanged(null);
	}

	public List<BAFLClub> BaflClubs
	{
		get { return App.ClubList.Values.ToList(); }
	}

	public string NavigateText
	{
		get {
			return "Navigate the menu to learn about the league, follow the " +
				"football season, cheer competition, and drill competition.";
		}
	}

	public string DescriptionText
    {
		get
		{
            /*return "Since 1977, the Bay Area Football League has been " +
			"committed to making a difference in the lives of youths all " +
			"throughout the	Houston area. As a leading youth organization, " +
			"we provide children an opportunity to reach their fullest " +
			"potential through football, drill, & cheer. " +
			"We offer the support they need in order to ensure they grow, " +
			"learn, and thrive within their community.";*/

            return "Teaching and support youth through football, " +
				"drill, & cheer since 1977. 19 teams across the greater " +
				"Houston area";
        }
    }
}
