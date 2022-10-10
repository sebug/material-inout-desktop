namespace material_inout_desktop;

public partial class ImportArticlesPage : ContentPage
{
	public ImportArticlesPage()
	{
		InitializeComponent();
	}

    void ImportButtonClicked(object sender, EventArgs args)
    {
        LoadExcelArticlesFile();
    }

    async Task LoadExcelArticlesFile()
    {
        try {
        var options = new PickOptions
        {
            PickerTitle = "Importer fichier d'articles"
        };
            await FilePicker.PickAsync(options);
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }
}

