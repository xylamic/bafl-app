namespace bafl_app;

using bafl.library;

public partial class CashAppPayView : ContentPage
{
	public CashAppPayView()
	{
        InitializeComponent();
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Browser.Default.OpenAsync(BaflUtilities.CashAppUrl, BrowserLaunchMode.External);
    }
}


