namespace material_inout_desktop.MaterialStore;

public interface IArticleRepository
{
    void EnsureArticle(Article article);
    List<Article> GetAllArticles();
    Article? GetByEAN(string ean);
    Voucher CreateVoucher(string name);

    List<Voucher> GetAllVouchers();

    VoucherLine AddVoucherLine(VoucherLine voucherLine);

    Voucher GetVoucherById(int id);

    List<VoucherLine> GetVoucherLinesByVoucherId(int voucherId);
}