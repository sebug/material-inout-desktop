using System.Windows.Input;
using material_inout_desktop.MaterialStore;

namespace material_inout_desktop;

public partial class ReturnedVoucherListPage : ContentPage
{
	private readonly IArticleRepository ArticleRepository;
	public ReturnedVoucherListPage(IArticleRepository articleRepository)
	{
		ArticleRepository = articleRepository;
		InitializeComponent();
	}


	protected override void OnAppearing()
	{
		Task.Run(() =>
		{
			Dispatcher.Dispatch(() =>
			{
                try
                {
                    var nonReturnedVouchers = ArticleRepository.GetReturnedVouchers();
                    vouchersListView.ItemsSource = nonReturnedVouchers;
                }
                catch (Exception ex)
                {
                    DisplayAlert("Error", "Error fetching returned vouchers: " + ex.ToString(), "OK");
                }
			});
		});
	}

    void ShowVoucherDetailsButtonClicked(object sender, EventArgs args)
    {
		var voucher = ((Button)sender).CommandParameter as Voucher;
        OpenDetailsPage(voucher);
    }

    async Task OpenDetailsPage(Voucher voucher)
    {
        try
        {
            await Shell.Current.GoToAsync("/voucherdetail", ((IDictionary<string, object>)new Dictionary<string, object>
            {
                { "VoucherId", voucher.Id.ToString() }
            }));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error going to voucher detail: " + ex.ToString(), "OK");
        }
    }
}

