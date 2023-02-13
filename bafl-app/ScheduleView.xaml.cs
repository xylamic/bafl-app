namespace bafl_app;

using bafl.library;

public partial class ZellePayView : ContentPage
{
	public ZellePayView()
	{
        InitializeComponent();
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Browser.Default.OpenAsync(BaflUtilities.ZelleUrl, BrowserLaunchMode.External);
    }
}


