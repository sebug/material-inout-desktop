namespace material_inout_desktop;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		Routing.RegisterRoute("voucherdetail", typeof(VoucherDetailPage));

		MainPage = new AppShell();
	}
}
