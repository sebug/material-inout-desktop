namespace material_inout_desktop.Excel;

public interface IArticlesListReader
{
    List<ArticleLine> ReadExcelFile(byte[] bytes);
}