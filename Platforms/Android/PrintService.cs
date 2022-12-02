using Android.Content;
using Android.Print;

[assembly: Dependency(typeof(material_inout_desktop.Droid.PrintService))]
namespace material_inout_desktop.Droid;

class PrintService : IPrintService
{
    public void Print(Stream inputStream, string fileName)
        {
            if (inputStream.CanSeek)
                //Reset the position of PDF document stream to be printed
                inputStream.Position = 0;            
            //Create a new file in the Personal folder with the given name
            string createdFilePath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
            //Save the stream to the created file
            using (var dest = System.IO.File.OpenWrite(createdFilePath))
                inputStream.CopyTo(dest);
            string filePath = createdFilePath;
  var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            PrintManager printManager = (PrintManager)activity.GetSystemService(Context.PrintService);       
            PrintDocumentAdapter pda = new CustomPrintDocumentAdapter(filePath);
            //Print with null PrintAttributes
            printManager.Print(fileName, pda, null);
        }
}