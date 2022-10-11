using material_inout_desktop.Excel;
using material_inout_desktop.MaterialStore;

namespace material_inout_desktop;

public partial class ImportArticlesPage : ContentPage
{
    private readonly IArticlesListReader ListReader;
    private readonly IArticleRepository ArticleRepository;
	public ImportArticlesPage(IArticlesListReader listReader,
        IArticleRepository articleRepository)
	{
        this.ListReader = listReader;
        this.ArticleRepository = articleRepository;
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
                    try
                    {
                        var linesWithEAN = lines.Where(line => !String.IsNullOrEmpty(line.EAN))
                        .ToList();
                        foreach (var line in linesWithEAN)
                        {
                            ArticleRepository.EnsureArticle(new Article
                            {
                                Label = line.Label,
                                Mnemonic = line.Mnemonic,
                                EAN = line.EAN
                            });
                        }
                        var linesInRepository = ArticleRepository.GetAllArticles();
                        DisplayAlert("Notification", "Number of lines in repository: " + linesInRepository.Count, "OK");
                    }
                    catch (Exception ex)
                    {
                        DisplayAlert("Error", $"An error occurred while storing articles: {ex.Message} - {ex.StackTrace}", "OK");
                        if (ex.InnerException != null)
                        {
                            DisplayAlert("Inner Error", ex.ToString(), "OK");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }
}

