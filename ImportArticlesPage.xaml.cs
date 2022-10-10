using material_inout_desktop.Excel;

namespace material_inout_desktop;

public partial class ImportArticlesPage : ContentPage
{
    private readonly IArticlesListReader ListReader;
	public ImportArticlesPage(IArticlesListReader listReader)
	{
        this.ListReader = listReader;
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
                    var lines = ListReader.ReadExcelFile(bytes);
                    DisplayAlert("Notification", "Number of lines read " + lines.Count, "OK");
                }
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }
}

