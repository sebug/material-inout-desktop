using SQLite;

namespace material_inout_desktop.MaterialStore;

[Table("voucherlines")]
public class VoucherLine
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int VoucherId { get; set; }

    // Implemented without explicit link to article line so that
    // we can rename, move articles without reference constraints

    public string EAN { get; set; }

    public string Label { get; set; }
}