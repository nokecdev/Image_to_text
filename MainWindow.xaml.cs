using System.Windows;
using System.IO;
using System;

namespace Image_to_text
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private const string V = @"\";
        ImageProcessing ip;
        private readonly UserDataContext _context;


        public MainWindow()
        {
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            _context = new();

            DataContext = _context;

            ip = new ImageProcessing(_context);
            AppDomain.CurrentDomain.UnhandledException += new System.UnhandledExceptionEventHandler(AppDomain_UnhandledException);
            
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _context.SettingsManager.SavePath = Path.Combine(desktopPath + V);
        }

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            ip.LoadImage();
        }


        private void BtnConvert(object sender, RoutedEventArgs e)
        {
            ip.ConvertImage(_context.ItemType.SelectedItemType.Name);
        }


        /// <summary>
        /// Application domain exception handler
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        public static void AppDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            using (StreamWriter sr = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\error.txt"))
            {
                sr.WriteLine(e.ExceptionObject);
            }
            MessageBox.Show("Error: " + e.ExceptionObject);
        }
    }
}
