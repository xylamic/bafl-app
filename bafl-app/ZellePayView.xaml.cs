namespace bafl_app;

public partial class ZellePayView : ContentPage
{
	public ZellePayView()
	{
        InitializeComponent();
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Browser.Default.OpenAsync("https://enroll.zellepay.com/qr-codes?data=ewogICJuYW1lIiA6ICJCQVkgQVJFQSBGT09UQkFMTCBMRUFHVUUgSU5DIiwKICAidG9rZW4iIDogImJhZmx0cmVhc3VyZXIyMUBnbWFpbC5jb20iLAogICJhY3Rpb24iIDogInBheW1lbnQiCn0=");
    }
}


