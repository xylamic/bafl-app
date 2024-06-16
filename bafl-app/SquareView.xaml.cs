namespace bafl_app;

using bafl_app.library;

/// <summary>
/// The Square view.
/// </summary>
public partial class SquareView : ContentPage
{
    /// <summary>
    /// Construct the view.
    /// </summary>
	public SquareView()
	{
        InitializeComponent();
    }

    /// <summary>
    /// Image button clicked.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The args.</param>
    private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        try
        {
#if IOS    
            await Browser.Default.OpenAsync(BaflUtilities.SquareUrl, BrowserLaunchMode.External);
#else
            await Browser.Default.OpenAsync(BaflUtilities.SquareUrl, BrowserLaunchMode.SystemPreferred);
#endif
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Could not open the app.", "OK");
        }
    }
}


