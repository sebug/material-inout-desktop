using System.Windows.Input;
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
				if (!articleList.Any(art => art.EAN == article.EAN))
				{
					articleList.Add(article);
				}
				articlesListView.ItemsSource = articleList.ToList();
			}
			((Entry)sender).Text = String.Empty;
			((Entry)sender).Focus();
		}
	}

	void RemoveArticleButtonClicked(object sender, EventArgs args)
    {
		var article = ((Button)sender).CommandParameter as Article;
		if (article != null)
		{
			RemoveArticleFromList(article);
		}
		barCodeInput.Focus();
    }

	void CreateVoucher(object sender, EventArgs args)
	{
		if (String.IsNullOrEmpty(nameInput.Text))
		{
			DisplayAlert("Erreur", "Veuillez rentrer le nom du/de la responsable", "OK");
			return;
		}
	}

	public void RemoveArticleFromList(Article article)
	{
		if (article == null)
		{
			return;
		}
		var articleList = EnsureArticleList();
		_articles = articleList.Where(art => art.EAN != article.EAN).ToList();
		articlesListView.ItemsSource = _articles;
	}
}

