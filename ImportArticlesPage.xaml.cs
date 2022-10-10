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
            var result = await FilePicker.PickAsync(options);
            if (result != null)
            {
                if (!result.FileName.EndsWith(".xlsx"))
                {
                    throw new Exception("Veuillez choisir un fichier .xlsx");
                }
                using (var stream = await result.OpenReadAsync())
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    var bytes = ms.ToArray();
                    DisplayAlert("Notification", "Number of bytes read " + bytes.Length, "OK");
                }
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }
}

