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
        return _conn.Table<Voucher>().OrderByDescending(voucher => voucher.CreatedDate).ToList();
    }

    public VoucherLine AddVoucherLine(VoucherLine voucherLine)
    {
        Init();
        _conn.Insert(voucherLine);
        return voucherLine;
    }
}