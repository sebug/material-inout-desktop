using Windows.Storage.Streams;
using Windows.Storage;
using Windows.Graphics.Printing;
using Microsoft.UI.Xaml.Printing;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Windows.UI.Popups;
using Windows.UI.Core;

[assembly: Dependency(typeof(material_inout_desktop.UWP.PrintService))]

namespace material_inout_desktop.UWP;

public  class PrintService : IPrintService
{
    internal int pageCount;
    internal global::Windows.Data.Pdf.PdfDocument pdfDocument;
    private PrintDocument printDocument;
    private IPrintDocumentSource printDocumentSource;
    string fileName;
    double marginWidth = 0;

    double marginHeight = 0;
    private Canvas pdfDocumentPanel = new Canvas();

    internal Dictionary<int, UIElement> printPreviewPages = new Dictionary<int, UIElement>();
    private List<string> imagePaths = new List<string>();
    IRandomAccessStream randomStream;
    public async void Print(Stream inputStream, string fileName)
    {
        inputStream.Position = 0;
        MemoryStream ms = new MemoryStream();
        //Copy the input stream to a new MemoryStream
        inputStream.CopyTo(ms);
        ms.Position = 0;
        //Convert the MemoryStream to a stream that allows random access
        randomStream = await ConvertToRandomAccessStream(ms);
        IAsyncOperation<global::Windows.Data.Pdf.PdfDocument> result = null;
        //Create the IAsyncOperation object from the given random stream
        result = global::Windows.Data.Pdf.PdfDocument.LoadFromStreamAsync(randomStream);

        result.AsTask().Wait();
        //Get the PdfDocument instance that represents the PDF document that is loaded
        pdfDocument = result.GetResults();
        result = null;
        pageCount = (int)pdfDocument.PageCount;
        fileName = fileName;
        await IncludeCanvas();
        try
        {
            UIDispatcher.Execute(async () =>
            {
                RegisterForPrint();
                //Show the UI window with printing options
                await PrintManager.ShowPrintUIAsync();
            });
        }
        catch
        {
            UIDispatcher.Execute(async () =>
            {
                RegisterForPrint();
                //Show the UI window with printing options
                PrintManager.ShowPrintUIAsync();
            });
        }
    }

    //Method to convert the given MemoryStream to a stream that allows random access.
    public async Task<IRandomAccessStream> ConvertToRandomAccessStream(MemoryStream memoryStream)
    {
        var randomAccessStream = new InMemoryRandomAccessStream();
        MemoryStream contentStream = new MemoryStream();
        memoryStream.CopyTo(contentStream);
        using (var outputStream = randomAccessStream.GetOutputStreamAt(0))
        {
            using (var dw = new DataWriter(outputStream))
            {
                var task = new Task(() => dw.WriteBytes(contentStream.ToArray()));
                task.Start();

                await task;
                await dw.StoreAsync();

                await outputStream.FlushAsync();
                await dw.FlushAsync();
                outputStream.Dispose();
                dw.DetachStream();
                dw.Dispose();
            }
        }
        return randomAccessStream;
    }
    PrintManager printMan;
    private void RegisterForPrint()
    {
        printDocument = new PrintDocument();

        // Save the DocumentSource.
        printDocumentSource = printDocument.DocumentSource;
        // Add an event handler which creates preview pages.
        printDocument.Paginate += CreatePrintPreviewPages;

        // Add an event handler which provides a specified preview page.
        printDocument.GetPreviewPage += GetPrintPreviewPage;

        // Add an event handler which provides all final print pages.
        printDocument.AddPages += AddPrintPages;

        // Create a PrintManager and add a handler for printing initialization.
        printMan = PrintManager.GetForCurrentView();
        printMan.PrintTaskRequested += PrintTaskRequested;
    }
    
    Microsoft.UI.Xaml.Controls.Image imageCtrl = new Microsoft.UI.Xaml.Controls.Image();
    private async void AddPrintPages(object sender, AddPagesEventArgs e)
    {
        try
        {
            await PrepareForPrint(0, pageCount);
            PrintDocument printDoc = (PrintDocument)sender;
            printDoc.AddPagesComplete();
        }
        catch
        {
            PrintDocument printDoc = (PrintDocument)sender;
            printDoc.InvalidatePreview();
        }
    }
    private async Task<int> PrepareForPrint(int startIndex, int count)
    {
        StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;
        int result = await PrepareForPrint(startIndex, count, tempFolder);
        tempFolder = null;
        return result;
    }
    private async Task<int> PrepareForPrint(int p, int count, StorageFolder tempfolder)
    {
        for (int i = p; i < count; i++)
        {
            float zoomfactor = 1;
            //Adjust the zoom factor according to the display properties
            double customZoomFactor = zoomfactor * 96 / DisplayProperties.LogicalDpi;                
            ApplicationLanguages.PrimaryLanguageOverride = CultureInfo.InvariantCulture.TwoLetterISOLanguageName;
            //Get the page from the PDF document with a given index
            var pdfPage = pdfDocument.GetPage(uint.Parse(i.ToString()));
            double pdfPagePreferredZoom = pdfPage.PreferredZoom /** m_pdfViewer.PrinterSettings.QualityFactor*/;
            IRandomAccessStream randomStream = new InMemoryRandomAccessStream();
            global::Windows.Data.Pdf.PdfPageRenderOptions pdfPageRenderOptions = new global::Windows.Data.Pdf.PdfPageRenderOptions();
            Size pdfPageSize = pdfPage.Size;
            //Set the height to which the page is to be printed
            pdfPageRenderOptions.DestinationHeight = (uint)(pdfPageSize.Height * pdfPagePreferredZoom);
            //Set the width to which the page is to be printed
            pdfPageRenderOptions.DestinationWidth = (uint)(pdfPageSize.Width * pdfPagePreferredZoom);
            await pdfPage.RenderToStreamAsync(randomStream, pdfPageRenderOptions);
            //Create a new Image to which the page will be rendered
            imageCtrl = new Image();
            BitmapImage src = new BitmapImage();
            randomStream.Seek(0);
            //Obtain image source from the randomstream
            src.SetSource(randomStream);
            //set the image source to the image
            imageCtrl.Source = src;
            var DisplayInformation = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();

            var dpi = DisplayInformation.LogicalDpi/96;
            imageCtrl.Height = src.PixelHeight/dpi;
            imageCtrl.Width = src.PixelWidth/dpi;
            randomStream.Dispose();
            pdfPage.Dispose();
            printDocument.AddPage(imageCtrl);
        }
        return 0;
    }
    private void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
    {
        PrintTaskOptions printingOptions = ((PrintTaskOptions)e.PrintTaskOptions);
        PrintPageDescription pageDescription = printingOptions.GetPageDescription((uint)e.CurrentPreviewPageNumber);
        marginWidth = pageDescription.PageSize.Width;
        marginHeight = pageDescription.PageSize.Height;
        AddOnePrintPreviewPage();

        PrintDocument printDoc = (PrintDocument)sender;
        printDoc.SetPreviewPageCount(pageCount, PreviewPageCountType.Final);
    }
    private void AddOnePrintPreviewPage()
    {
        for (int i = pdfDocumentPanel.Children.Count - 1; i >= 0; i--)
        {
            Canvas print = pdfDocumentPanel.Children[i] as Canvas;
            if (print != null)
            {
                print.Width = marginWidth;
                print.Height = marginHeight;
                printPreviewPages.Add(i, print);

            }
        }
    }

    private async void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
    {
        PrintDocument printDoc = (PrintDocument)sender;
        pdfDocumentPanel.Children.Remove(printPreviewPages[e.PageNumber - 1]);
        printDoc.SetPreviewPage(e.PageNumber, printPreviewPages[e.PageNumber - 1]);
    }

    private async Task<int> IncludeCanvas()
    {
        for (int i = 0; i < pageCount; i++)
        {
            int pageIndex = i;
            var pdfPage = pdfDocument.GetPage(uint.Parse(i.ToString()));
            double width = pdfPage.Size.Width;
            double height = pdfPage.Size.Height;
            Canvas page = new Canvas();
            page.Width = width;
            page.Height = height;
            page.VerticalAlignment = global::Windows.UI.Xaml.VerticalAlignment.Top;
            page.HorizontalAlignment = global::Windows.UI.Xaml.HorizontalAlignment.Center;
            page.Background = new Windows.UI.Xaml.Media.SolidColorBrush(global::Windows.UI.Color.FromArgb(255, 255, 255, 255));
            page.Margin = new Thickness(0, 0, 0, 0);

            double pdfPagePreferredZoom = pdfPage.PreferredZoom /** m_pdfViewer.PrinterSettings.QualityFactor*/;
            IRandomAccessStream randomStream = new InMemoryRandomAccessStream();
            global::Windows.Data.Pdf.PdfPageRenderOptions pdfPageRenderOptions = new global::Windows.Data.Pdf.PdfPageRenderOptions();
            Size pdfPageSize = pdfPage.Size;
            //Set the height to which the page is to be printed
            pdfPageRenderOptions.DestinationHeight = (uint)(pdfPageSize.Height * pdfPagePreferredZoom);
            //Set the width to which the page is to be printed
            pdfPageRenderOptions.DestinationWidth = (uint)(pdfPageSize.Width * pdfPagePreferredZoom);
            await pdfPage.RenderToStreamAsync(randomStream, pdfPageRenderOptions);
            //Create a new Image to which the page will be rendered
            imageCtrl = new Image();
            BitmapImage src = new BitmapImage();
            randomStream.Seek(0);
            //Obtain image source from the randomstream
            src.SetSource(randomStream);
            //set the image source to the image
            imageCtrl.Source = src;
            var DisplayInformation = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();

            var dpi = DisplayInformation.LogicalDpi / 96;
            imageCtrl.Height = src.PixelHeight / dpi;
            imageCtrl.Width = src.PixelWidth / dpi;
            randomStream.Dispose();
            pdfPage.Dispose();

            page.Children.Add(imageCtrl);
            pdfDocumentPanel.Children.Add(page);
        }
        return 0;
    }
    PrintTask printTask;
    MessageDialog msgDialog;

    private async void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
    {

        printTask = e.Request.CreatePrintTask(fileName, sourceRequested => sourceRequested.SetSource(printDocumentSource));

            printTask.Completed += printTask_Completed;
        
    }

    
    //Called when the printing operation completes
    private void printTask_Completed(PrintTask sender, PrintTaskCompletedEventArgs args)
    {
        printTask.Completed -= printTask_Completed;
        printMan.PrintTaskRequested -= PrintTaskRequested;


        UIDispatcher.Execute(async () =>
        {

            
            msgDialog = new MessageDialog("Printing operation has been completed");
            await msgDialog.ShowAsync();                

        });
    }  
}

//Helper class to run operations async
internal class UIDispatcher
{
    private static CoreDispatcher Dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
    /// <summary>
    /// Executes the action using UIElement.
    /// </summary>
    /// <param name="action">An Action.</param>
    internal static void Execute(Action action)
    {
        if (CoreApplication.MainView.CoreWindow == null
            || Dispatcher.HasThreadAccess)
            action();
        else
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action()).AsTask().Wait();
    }
}