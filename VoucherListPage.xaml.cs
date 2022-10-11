using System.Windows.Input;
using material_inout_desktop.MaterialStore;

namespace material_inout_desktop;

public partial class VoucherListPage : ContentPage
{
	private readonly IArticleRepository ArticleRepository;
	public VoucherListPage(IArticleRepository articleRepository)
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
                    var allVouchers = ArticleRepository.GetAllVouchers();
                    vouchersListView.ItemsSource = allVouchers;
                }
                catch (Exception ex)
                {
                    DisplayAlert("Error", "Error fetching vouchers: " + ex.ToString(), "OK");
                }
			});
		});
	}
}

