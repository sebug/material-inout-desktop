using SQLite;

namespace material_inout_desktop.MaterialStore;

[Table("vouchers")]
public class Voucher
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? ReturnedDate { get; set; }

    [Ignore]
    public int VoucherLineCount { get; set; }
}