using Foundation;
using UIKit;

[assembly: Dependency(typeof(material_inout_desktop.iOS.PrintService))]

namespace material_inout_desktop.iOS;

public class PrintService : IPrintService
{
    public void Print(Stream inputStream, string fileName)
    {
        var printInfo = UIPrintInfo.PrintInfo;
        printInfo.OutputType = UIPrintInfoOutputType.General;
        printInfo.JobName = "Print PDF Sample";

        //Get the path of the MyDocuments folder
        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //Get the path of the Library folder within the MyDocuments folder
        var library = Path.Combine(documents, "..", "Library");
        //Create a new file with the input file name in the Library folder
        var filepath = Path.Combine(library, fileName);

        //Write the contents of the input file to the newly created file
        using (MemoryStream tempStream = new MemoryStream())
        {
            inputStream.Position = 0;
            inputStream.CopyTo(tempStream);
            File.WriteAllBytes(filepath, tempStream.ToArray());
        }

        var printer = UIPrintInteractionController.SharedPrintController;
        printInfo.OutputType = UIPrintInfoOutputType.General;

        printer.PrintingItem = NSUrl.FromFilename(filepath);
        printer.PrintInfo = printInfo;


        printer.ShowsPageRange = true;

        printer.Present(true, (handler, completed, err) => {
            if (!completed && err != null)
            {
                Console.WriteLine("error");
            }
        });
    }
}