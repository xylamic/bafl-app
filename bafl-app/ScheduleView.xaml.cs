namespace bafl_app;

using bafl.library;

public partial class ScheduleView : ContentPage
{

    public ScheduleView()
    {
        InitializeComponent();

        BindingContext = this;
    }

    public List<BaflScheduleItem> ScheduleList
    {
        get { return App.ScheduleList; }
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Browser.Default.OpenAsync(BaflUtilities.ZelleUrl, BrowserLaunchMode.External);
    }
}


