namespace material_inout_desktop.MaterialStore;

public interface IArticleRepository
{
    void EnsureArticle(Article article);
    List<Article> GetAllArticles();
    Article? GetByEAN(string ean);
    Voucher CreateVoucher(string name);

    List<Voucher> GetAllVouchers();

    List<Voucher> GetAllNonReturnedVouchers();

    List<Voucher> GetReturnedVouchers();

    VoucherLine AddVoucherLine(VoucherLine voucherLine);

    VoucherLine ReturnVoucherLine(int id, string returnText);

    Voucher GetVoucherById(int id);

    Voucher UpdateVoucher(Voucher voucher);

    List<VoucherLine> GetVoucherLinesByVoucherId(int voucherId);
}