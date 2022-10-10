namespace material_inout_desktop;

public partial class ImportArticlesPage : ContentPage
{
	public ImportArticlesPage()
	{
		InitializeComponent();
	}

    async void ImportButtonClicked(object sender, EventArgs args)
    {
        var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.macOS, new List<string> { "xlsx" } }
        });
        var options = new PickOptions
        {
            PickerTitle = "Importer fichier d'articles",
            FileTypes = customFileType
        };
        FilePicker.Default.PickAsync(options);
    }
}

