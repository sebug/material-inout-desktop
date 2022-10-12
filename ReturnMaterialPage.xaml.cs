using System.Text;
using System.Windows.Input;
using material_inout_desktop.MaterialStore;

namespace material_inout_desktop;

[QueryProperty("VoucherId", "VoucherId")]
public partial class ReturnMaterialPage : ContentPage
{
    public string VoucherId
    {
        get; set;
    }

	private readonly IArticleRepository ArticleRepository;
	public ReturnMaterialPage(IArticleRepository articleRepository)
	{
		ArticleRepository = articleRepository;
		InitializeComponent();
	}

    private List<VoucherLine> _voucherLines { get; set; }

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

                    _voucherLines = ArticleRepository.GetVoucherLinesByVoucherId(voucherId);
                    voucherIdLabel.Text = "Numéro du Bon de Sortie: " + voucherId;
                    personResponsibleLabel.Text = "Responsable: " + voucher.Name;

                    barCodeInput.Focus();
                }
                catch (Exception ex)
                {
                    DisplayAlert("Error", "Error fetching vouchers: " + ex.ToString(), "OK");
                }
			});
		});
	}

    void OnBarCodeInputCompleted(object sender, EventArgs e)
	{
   		string text = ((Entry)sender).Text;
		if (!String.IsNullOrEmpty(text))
		{
            ReturnEAN(text);
		}
	}

    async Task ReturnEAN(string ean)
    {
        var matchingLine = _voucherLines.FirstOrDefault(vl => vl.EAN == ean);
        if (matchingLine != null)
        {
            await DisplayAlert("Information", "De retour: " + matchingLine.Label, "OK");
        }
        barCodeInput.Text = String.Empty;
        barCodeInput.Focus();
    }
}

