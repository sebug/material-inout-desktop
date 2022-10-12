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

                    voucherLinesListView.ItemsSource = _voucherLines.ToList();

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
        try
        {
            var matchingLine = _voucherLines.FirstOrDefault(vl => vl.EAN == ean);
            if (matchingLine != null)
            {
                _voucherLines = _voucherLines.Select(vl => {
                    if (vl.Id == matchingLine.Id)
                    {
                        return new VoucherLine
                        {
                            Id = matchingLine.Id,
                            VoucherId = matchingLine.VoucherId,
                            EAN = matchingLine.EAN,
                            Label = matchingLine.Label,
                            ReturnStatus = "Retourné"
                        };
                    }
                    else
                    {
                        return vl;
                    }
                }).ToList();
            }
            voucherLinesListView.ItemsSource = _voucherLines.ToList();
            barCodeInput.Text = String.Empty;
            barCodeInput.Focus();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Erreur de retour: " + ex.ToString(), "OK");
        }
    }

    void MarkAsReturned(object sender, EventArgs e)
    {
        try 
        {
            var button = (Button)sender;
            ReturnManually((VoucherLine)button.CommandParameter);
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", "Erreur de retour manuel: " + ex.ToString(), "OK");
        }
    }

    async Task ReturnManually(VoucherLine voucherLine)
    {
        try 
        {
            _voucherLines = _voucherLines.Select(vl => {
                if (vl.Id == voucherLine.Id)
                {
                    return new VoucherLine
                    {
                        Id = voucherLine.Id,
                        VoucherId = voucherLine.VoucherId,
                        EAN = voucherLine.EAN,
                        Label = voucherLine.Label,
                        ReturnStatus = "Retour Manuel"
                    };
                }
                else
                {
                    return vl;
                }
            }).ToList();
            voucherLinesListView.ItemsSource = _voucherLines.ToList();
            barCodeInput.Text = String.Empty;
            barCodeInput.Focus();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Erreur de retour manuel: " + ex.ToString(), "OK");
        }
    }

    void ConfirmButtonClicked(object sender, EventArgs args)
    {
		ConfirmMaterialReturn();
    }

    async Task ConfirmMaterialReturn()
    {
        try
        {
            string returningPersonName = nameInput.Text;
            if (String.IsNullOrEmpty(returningPersonName))
            {
                await DisplayAlert("Erreur", "Veuillez rentrer le nom de la personne qui confirme le retour", "OK");
                return;
            }
            if (_voucherLines.Any(vl => String.IsNullOrEmpty(vl.ReturnStatus)))
            {
                var notReturnedLines = _voucherLines.Where(vl => String.IsNullOrEmpty(vl.ReturnStatus))
                .ToList();
                await DisplayAlert("Erreur", "Pas retourné: " +
                String.Join(", ", notReturnedLines.Select(vl => vl.Label)), "OK");
                return;
            }
            foreach (var voucherLine in _voucherLines)
            {
                ArticleRepository.ReturnVoucherLine(voucherLine.Id, voucherLine.ReturnStatus);
            }
            int voucherId = int.Parse(VoucherId);
            var voucher = ArticleRepository.GetVoucherById(voucherId);
            voucher.ReturnedDate = DateTimeOffset.Now;
            voucher.ReturningPersonName = returningPersonName;
            ArticleRepository.UpdateVoucher(voucher);
            await Shell.Current.GoToAsync("/voucherdetail", ((IDictionary<string, object>)new Dictionary<string, object>
            {
                { "VoucherId", VoucherId }
            }));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Erreur de retour de matériel: " + ex.ToString(), "OK");
        }
    }
}

