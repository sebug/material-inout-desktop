namespace material_inout_desktop.MaterialStore;

public interface IArticleRepository
{
    void EnsureArticle(Article article);
    List<Article> GetAllArticles();
    Article? GetByEAN(string ean);
    Voucher CreateVoucher(string name);
}