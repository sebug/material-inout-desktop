using System.Text;
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

    {{lines}}

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

                    var voucherLines = ArticleRepository.GetVoucherLinesByVoucherId(voucherId);
                    html = html.Replace("{{lines}}", GetVoucherLinesTable(voucherLines));

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

    private string GetVoucherLinesTable(List<VoucherLine> voucherLines)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<table>");
        sb.AppendLine("<thead>");
        sb.AppendLine("<tr>");
        sb.AppendLine("<th>EAN</th>");
        sb.AppendLine("<th>Libellé</th>");
        sb.AppendLine("</tr>");
        sb.AppendLine("</thead>");
        sb.AppendLine("<tbody>");
        foreach (var voucherLine in voucherLines)
        {
            sb.AppendLine("<tr>");
            sb.AppendLine("<td>" + voucherLine.EAN + "</td>");
            sb.AppendLine("<td>" + voucherLine.Label + "</td>");
            sb.AppendLine("</tr>");
        }
        sb.AppendLine("</tbody>");
        sb.AppendLine("</table>");
        return sb.ToString();
    }
}

