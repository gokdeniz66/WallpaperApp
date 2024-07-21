using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace WallpaperApp
{
    public partial class MainWindow : Window
    {
        private const string WallpaperFolderName = "Wallpapers";
        private static readonly string picturesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        private readonly string wallpaperFolderPath = Path.Combine(picturesFolder, WallpaperFolderName);
        private readonly List<string> usedImages = new List<string>();
        private readonly MediaPlayer mediaPlayer = new();
        private static readonly Random rand = new();
        public MainWindow()
        {
            InitializeComponent();
            CreateWallpaperFolder();
        }
        private void CreateWallpaperFolder()
        {
            if (!Directory.Exists(wallpaperFolderPath))
                Directory.CreateDirectory(wallpaperFolderPath);
        }
        private void BtnRandomWallpaper_Click(object sender, RoutedEventArgs e)
        {
            string randomImagePath = GetRandomImagePath();
            SetWallpaper(randomImagePath);
        }
        private string GetRandomImagePath()
        {          
            string[] extensions = ["*.jpg", "*.jpeg", "*.png"];
            List<string> allFiles = new List<string>();
            foreach (string extension in extensions)
            {
                allFiles.AddRange(Directory.GetFiles(wallpaperFolderPath, extension));
            }
            if (allFiles.Count == 0)
            {
                MessageBox.Show("No images found in the wallpapers folder!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            return allFiles[rand.Next(allFiles.Count)];
        }
        private void BtnCustomWallpaper_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Choose wallpaper (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if(openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;
                SetWallpaper(selectedImagePath);
            }
        }
        private void BtnPlayMusic_Click(object sender, RoutedEventArgs e)
        {
            string musicFilePath = Path.Combine(wallpaperFolderPath, "music.mp3");
            if (File.Exists(musicFilePath))
            {
                mediaPlayer.Open(new Uri(musicFilePath));
                mediaPlayer.Play();
            }
            else
            {
                MessageBox.Show("Music file not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        private static void SetWallpaper(string path)
        {
            const int SPI_SETDESKWALLPAPER = 20;
            const int SPIF_UPDATEINIFILE = 0x01;
            const int SPIF_SENDWININICHANGE = 0x02;
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}