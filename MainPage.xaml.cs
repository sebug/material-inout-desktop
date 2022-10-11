using material_inout_desktop.MaterialStore;

namespace material_inout_desktop;

public partial class MainPage : ContentPage
{
	private readonly IArticleRepository ArticleRepository;
	public MainPage(IArticleRepository articleRepository)
	{
		ArticleRepository = articleRepository;
		InitializeComponent();
	}

	private List<Article> _articles;
	private List<Article> EnsureArticleList()
	{
		if (_articles == null)
		{
			_articles = new List<Article>();
		}
		return _articles;
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
		if (!String.IsNullOrEmpty(text))
		{
			var article = ArticleRepository.GetByEAN(text);
			if (article != null)
			{
				var articleList = EnsureArticleList();
				articleList.Add(article);
				articlesListView.ItemsSource = articleList.ToList();
			}
			((Entry)sender).Text = String.Empty;
			((Entry)sender).Focus();
		}
	}
}

