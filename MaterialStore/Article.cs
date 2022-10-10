using SQLite;

namespace material_inout_desktop.MaterialStore;

[Table("articles")]
public class Article
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Unique]
    public string EAN { get; set; }
    public string Mnemonic { get; set; }
    public string Description { get; set; }
}