using System.Windows.Input;
using material_inout_desktop.MaterialStore;

namespace material_inout_desktop;

[QueryProperty("VoucherId", "VoucherId")]
public partial class VoucherDetailPage : ContentPage
{
    public string VoucherId
    {
        get; set;
    }

	private readonly IArticleRepository ArticleRepository;
	public VoucherDetailPage(IArticleRepository articleRepository)
	{
		ArticleRepository = articleRepository;
		InitializeComponent();
	}

    public string TemplateHtml = @"
    <!DOCTYPE html>
    <html>
    <head>
    </head>
    <body>
    <h1>Voucher {{voucherId}}</h1>
    </body>
    </html>
    ";


	protected override void OnAppearing()
	{
		Task.Run(() =>
		{
			Dispatcher.Dispatch(() =>
			{
                try
                {
                    voucherDetailView.Source = new HtmlWebViewSource
                    {
                        Html =  TemplateHtml.Replace("{{voucherId}}", VoucherId)
                    };
                }
                catch (Exception ex)
                {
                    DisplayAlert("Error", "Error fetching vouchers: " + ex.ToString(), "OK");
                }
			});
		});
	}

    void PrintButtonClicked(object sender, EventArgs args)
    {
		PrintPage();
    }

    async Task PrintPage()
    {
        await DisplayAlert("Information", "Bon de sortie imprimé", "OK");
    }
}

