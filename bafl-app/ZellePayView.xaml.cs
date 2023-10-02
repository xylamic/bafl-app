namespace bafl_app;

using System;
using bafl_app.library;

/// <summary>
/// The Zelle view.
/// </summary>
public partial class ZellePayView : ContentPage
{
    /// <summary>
    /// Construct the view.
    /// </summary>
	public ZellePayView()
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
            await Browser.Default.OpenAsync(BaflUtilities.ZelleUrl, BrowserLaunchMode.External);
#else
            await Browser.Default.OpenAsync(BaflUtilities.ZelleUrl, BrowserLaunchMode.SystemPreferred);
#endif
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Could not open the app.", "OK");
        }
    }
}


