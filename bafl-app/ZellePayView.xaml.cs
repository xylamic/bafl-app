namespace bafl_app;

public partial class ZellePayView : ContentPage
{
	public ZellePayView()
	{
        InitializeComponent();
    }

    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        await Browser.Default.OpenAsync("https://enroll.zellepay.com/qr-codes?data=eyJuYW1lIjoiQURBTSIsImFjdGlvbiI6InBheW1lbnQiLCJ0b2tlbiI6IjI4MTczMTE5MDQifQ==");
    }
}


