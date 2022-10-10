namespace material_inout_desktop;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		Task.Run(() =>
		{
			Dispatcher.Dispatch(() =>
			{
				barCodeInput.Focus();
			});
		});
	}

	void OnBarCodeInputCompleted(object sender, EventArgs e)
	{
   		string text = ((Entry)sender).Text;
	}
}

