namespace material_inout_desktop;

public interface IPrintService
{
    void Print(Stream inputStream, string fileName);
}