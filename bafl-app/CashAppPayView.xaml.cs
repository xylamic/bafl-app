namespace bafl_app;

public partial class CashAppPayView : ContentPage
{
	public CashAppPayView()
	{
        InitializeComponent();
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Browser.Default.OpenAsync("https://cash.app/$payBAFL?qr=1");
    }
}


