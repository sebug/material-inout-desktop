using SQLite;

namespace material_inout_desktop.MaterialStore;

public class ArticleRepository : IArticleRepository
{
    private string _dbPath;

    private SQLiteConnection _conn;

    public ArticleRepository(string dbPath)
    {
        _dbPath = dbPath ?? throw new ArgumentNullException(nameof(dbPath));
    }

    private void Init()
    {
        if (_conn != null)
        {
            return;
        }
        _conn = new SQLiteConnection(_dbPath);
        _conn.CreateTable<Article>();
        _conn.CreateTable<Voucher>();
        _conn.CreateTable<VoucherLine>();
    }

    public void EnsureArticle(Article article)
    {
        Init();
        int result = 0;
        var toInsert = _conn.Table<Article>().FirstOrDefault(
            art => art.EAN == article.EAN
        );
        if (toInsert != null)
        {
            toInsert.Mnemonic = article.Mnemonic;
            toInsert.Label = article.Label;
            _conn.Update(toInsert);
        }
        else
        {
            result = _conn.Insert(article);
        }
    }

    public List<Article> GetAllArticles()
    {
        Init();
        return _conn.Table<Article>().ToList();
    }

    public Article GetByEAN(string ean)
    {
        Init();
        return _conn.Table<Article>().FirstOrDefault(art => art.EAN == ean);
    }

    public Voucher CreateVoucher(string name)
    {
        var voucher = new Voucher
        {
            Name = name,
            CreatedDate = DateTimeOffset.Now
        };
        _conn.Insert(voucher);
        return voucher;
    }

    public List<Voucher> GetAllVouchers()
    {
        Init();
        var result = _conn.Table<Voucher>().OrderByDescending(voucher => voucher.CreatedDate).ToList();
        foreach (var voucher in result)
        {
            voucher.VoucherLineCount =
                _conn.Table<VoucherLine>().Count(line => line.VoucherId == voucher.Id);
        }
        return result;
    }

    public VoucherLine AddVoucherLine(VoucherLine voucherLine)
    {
        Init();
        _conn.Insert(voucherLine);
        return voucherLine;
    }

    public Voucher GetVoucherById(int id)
    {
        Init();
        return _conn.Table<Voucher>().FirstOrDefault(v => v.Id == id);
    }

    public List<VoucherLine> GetVoucherLinesByVoucherId(int voucherId)
    {
        Init();
        var voucherLines = _conn.Table<VoucherLine>().Where(vl => vl.VoucherId == voucherId).ToList();
        return voucherLines;
    }

    public VoucherLine ReturnVoucherLine(int id, string returnText)
    {
        Init();
        var voucherLine = _conn.Table<VoucherLine>().FirstOrDefault(vl => vl.Id == id);
        if (voucherLine == null)
        {
            throw new Exception("Could not find voucher line " + id);
        }
        voucherLine.ReturnStatus = returnText;
        _conn.Update(voucherLine);
        return voucherLine;
    }

    public Voucher UpdateVoucher(Voucher voucher)
    {
        _conn.Update(voucher);
        return voucher;
    }
}