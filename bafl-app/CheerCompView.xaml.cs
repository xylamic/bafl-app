using System.Globalization;
using System.Text.Json;
using bafl_app.library;

namespace bafl_app;

/// <summary>
/// The Cheer Competition view.
/// </summary>
public partial class CheerCompView : ContentPage
{
    private bool _isLoading = true;
    private BaflEvent _event = new BaflEvent();

    /// <summary>
    /// Construct the view.
    /// </summary>
    public CheerCompView()
	{
		InitializeComponent();

        VisualStateManager.GoToState(CheerButtonOn, "On");
        VisualStateManager.GoToState(CheerButtonOff, "Off");
        VisualStateManager.GoToState(MascotButtonOn, "On");
        VisualStateManager.GoToState(MascotButtonOff, "Off");
        CheerShown = true;

        // set binding to itself
        BindingContext = this;

        // begin the data load
        Task.Run(async () => { await LoadView(); });
	}

    /// <summary>
    /// Load the view data.
    /// </summary>
    /// <returns>The async task.</returns>
    private async Task LoadView()
    {
        try
        {
            // construct the API key
            string code = String.Format("?code={0}", App.GetApiKey("cheercomp"));

            // try to read the latest configuration information.
            HttpClient client = new HttpClient();
            string cheerContent = await client.GetStringAsync(BaflUtilities.CHEERCOMP_URL + code);
            _event = JsonSerializer.Deserialize<BaflEvent>(cheerContent);

            // set the text for the page header
            LastUpdated = String.Format("V  Updated {0}, pull to refresh  V", DateTime.Now.ToShortTimeString());
        }
        catch (Exception)
        {
            LastUpdated = String.Format("V  Failed load {0}, try again  V", DateTime.Now.ToShortTimeString());
        }

        _isLoading = false;
        OnPropertyChanged(null);

        (stackLay as IView).InvalidateMeasure();
    }

    /// <summary>
    /// Name of the event.
    /// </summary>
    public string Name
    {
        get => _event.Name;
    }

    /// <summary>
    /// The date string for the event.
    /// </summary>
    public string EventDate
    {
        get => _event.DateString;
    }

    /// <summary>
    /// Whether these is an important message to show.
    /// </summary>
    public bool HasMessage
    {
        get => String.IsNullOrEmpty(_event.Message) ? false : true;
    }

    /// <summary>
    /// The important message, if there is one.
    /// </summary>
    public string Message
    {
        get => _event.Message;
    }

    /// <summary>
    /// Whether the page is currently loading.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
    }

    /// <summary>
    /// Whether the page is currently NOT loading.
    /// </summary>
    public bool IsNotLoading
    {
        get => !_isLoading;
    }

    /// <summary>
    /// The last updated header string.
    /// </summary>
    public string LastUpdated
    {
        get; private set;
    }

    /// <summary>
    /// The set of items to be visible on the screen.
    /// </summary>
    public IEnumerable<BaflEventLineItem> Items
    {
        get
        {
            if (CheerShown)
                return CheerItems;
            else
                return MascotItems;
        }
    }

    /// <summary>
    /// Get the set of items grouped by Cheer.
    /// </summary>
    public IEnumerable<BaflEventLineItem> CheerItems
    {
        get
        {
            return from item in _event.Schedule where item.Group == "Cheer" select item;
        }
    }

    /// <summary>
    /// Get the set of items grouped by Mascot.
    /// </summary>
    public IEnumerable<BaflEventLineItem> MascotItems
    {
        get
        {
            return from item in _event.Schedule where item.Group == "Mascot" select item;
        }
    }

    /// <summary>
    /// Get whether cheer is shown.
    /// </summary>
    public bool CheerShown
    {
        get; protected set;
    }

    /// <summary>
    /// Get whether mascot is shown.
    /// </summary>
    public bool MascotShown
    {
        get { return !CheerShown; }
    }

    /// <summary>
    /// The refresh was triggered by the user.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The args.</param>
    protected void RefreshView_Refreshing(System.Object sender, System.EventArgs e)
    {
        Task.Run(async () =>
        {
            _isLoading = true;
            OnPropertyChanged(nameof(IsLoading));

            await LoadView();
        });
    }

    protected void CheerSection_Clicked(System.Object sender, System.EventArgs e)
    {
        CheerShown = true;

        OnPropertyChanged(nameof(Items));
        OnPropertyChanged(nameof(CheerShown));
        OnPropertyChanged(nameof(MascotShown));
    }

    protected void MascotSection_Clicked(System.Object sender, System.EventArgs e)
    {
        CheerShown = false;

        OnPropertyChanged(nameof(Items));
        OnPropertyChanged(nameof(CheerShown));
        OnPropertyChanged(nameof(MascotShown));
    }
}


