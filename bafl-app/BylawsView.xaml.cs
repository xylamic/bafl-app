namespace bafl_app;

using bafl_app.library;

/// <summary>
/// The Zelle view.
/// </summary>
public partial class BylawsView : ContentPage
{
    /// <summary>
    /// Construct the view.
    /// </summary>
	public BylawsView()
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
        await Browser.Default.OpenAsync(BaflUtilities.ZelleUrl, BrowserLaunchMode.External);
    }
}


