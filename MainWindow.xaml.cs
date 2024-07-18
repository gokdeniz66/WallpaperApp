using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace WallpaperApp
{
    public partial class MainWindow : Window
    {
        private readonly string picturesFolder = SpecialDirectories.MyPictures;
        private readonly List<string> usedImages = new List<string>();
        private readonly MediaPlayer mediaPlayer = new MediaPlayer();
        private readonly string userName = Environment.UserName;

        public MainWindow()
        {
            InitializeComponent();
            mediaPlayer = new MediaPlayer();
        }
        private void BtnRandomWallpaper_Click(object sender, RoutedEventArgs e)
        {
            string randomImagePath = GetRandomImagePath();

            while (usedImages.Contains(randomImagePath))
            {
                randomImagePath = GetRandomImagePath();
            }
            usedImages.Add(randomImagePath);
            SetWallpaper(randomImagePath);

            if(usedImages.Count == GetTotalImageCount())
            {
                usedImages.Clear();
            }
        }
        private int GetTotalImageCount()
        {
            string[] extensions = ["*.jpg", "*.jpeg", "*.png"];
            int totalCount = 0;
            foreach (var extension in extensions)
            {
                totalCount += Directory.GetFiles(picturesFolder, extension).Length;
            }
            return totalCount;
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
        private string GetRandomImagePath()
        {
            string[] extensions = ["*.jpg", "*.jpeg", "*.png"];

            List<string> allFiles = new List<string>();
            foreach (string extension in extensions)
            {
                allFiles.AddRange(Directory.GetFiles(picturesFolder, extension));
            }

            if (allFiles.Count == 0)
            {
                MessageBox.Show("No images found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            Random rand = new Random();
            return allFiles[rand.Next(allFiles.Count)];
        }

        private void BtnPlayMusic_Click(object sender, RoutedEventArgs e)
        {
            string musicFilePath = "C:\\Users\\" + userName + "\\Desktop\\WallpaperApp\\assets\\music.mp3";
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
        private void SetWallpaper(string path)
        {
            const int SPI_SETDESKWALLPAPER = 20;
            const int SPIF_UPDATEINIFILE = 0x01;
            const int SPIF_SENDWININICHANGE = 0x02;
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}