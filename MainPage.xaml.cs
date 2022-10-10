namespace material_inout_desktop;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	void OnBarCodeInputCompleted(object sender, EventArgs e)
	{
   		string text = ((Entry)sender).Text;
		DisplayAlert("Bar Code Scanned", text, "OK");
	}
}

