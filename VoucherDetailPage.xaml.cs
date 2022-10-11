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
    <p><button class=""print"">Print</button></p>
    <h1>Bon de Sortie {{voucherId}}</h1>
    <p>Pour: {{name}}</p>
    <p>Date de création: {{createdDate}}</p>
    <script>
    let printButton = document.querySelector('.print');
    printButton.addEventListener('click', function () {
        try {
            window.print();
        } catch (e) {
            alert('Erreur impression');
        }
        alert('Document imprimé');
    });
    </script>
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
                    int voucherId = int.Parse(VoucherId);
                    var voucher = ArticleRepository.GetVoucherById(voucherId);
                    string html = TemplateHtml.Replace("{{voucherId}}", VoucherId)
                        .Replace("{{name}}", voucher.Name)
                        .Replace("{{createdDate}}", voucher.CreatedDate.ToString("dd.MM.yyyy"));
                    voucherDetailView.Source = new HtmlWebViewSource
                    {
                        Html =  html
                    };
                }
                catch (Exception ex)
                {
                    DisplayAlert("Error", "Error fetching vouchers: " + ex.ToString(), "OK");
                }
			});
		});
	}
}

