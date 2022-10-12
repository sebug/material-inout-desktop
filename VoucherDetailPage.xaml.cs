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
    <style>
    body {
        font-family: sans-serif;
    }
    th {
        text-align: left;
        padding-left: 1em;
    }
    td {
        padding-left: 1em;
    }
    .signature {
        padding-bottom: 5em;
        border-bottom: 1px solid black;
    }
    .address {
        float: right;
    }
    .logo {
        clear: left;
        float: left;
    }
    h1 {
        clear: both;
    }
    @media print {
        .print-options {
            display: none;
        }
    }
    </style>
    </head>
    <body>
    <p class=""print-options""><button class=""print"">Imprimer</button></p>
    <p class=""logo""><img width=""200"" src=""{{logo_url}}"" /></p>
    <p class=""address"">ORPC Valavran,<br />
Rue du Village 27<br />
1294 Genthod<br />
Tél. +41 22 774 08 06</p>
    <h1>Bon de Sortie {{voucherId}}</h1>
    <p>Responsable: {{name}}</p>
    <p>Date de création: {{createdDate}}</p>

    {{lines}}

    <p class=""signature"">Signature</p>

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
                        .Replace("{{createdDate}}", voucher.CreatedDate.ToString("dd.MM.yyyy"))
                        .Replace("{{logo_url}}", OrganizationLogo.DATA_URL);

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

