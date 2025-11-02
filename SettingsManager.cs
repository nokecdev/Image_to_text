using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;

namespace Image_to_text
{
    public class SettingsManager : INotifyPropertyChanged
    {
        private static readonly Lazy<SettingsManager> instance = new(() => new SettingsManager());
        public static SettingsManager Instance => instance.Value;

        private string _savePath = string.Empty;
        private OpenFileDialog _ofd;
        private string _fileSource = string.Empty;

        public string FileSource
        {
            get => _fileSource;
            set
            {
                if (_fileSource != value)
                {
                    _fileSource = value;
                    OnPropertyChanged(nameof(FileSource));
                }
            }
        }

        public string SavePath
        {
            get => _savePath;
            set
            {
                if (_savePath != value)
                {
                    _savePath = value;
                    OnPropertyChanged(nameof(SavePath));
                }
            }
        }


        public OpenFileDialog Ofd
        {
            get => _ofd;
            set
            {
                if (_ofd != value)
                {
                    _ofd = value;
                    //OnPropertyChanged(nameof(_ofd));
                }
            }
        }

        private SettingsManager()
        {
            SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), ""); // Alapértelmezett útvonal
            _ofd = new OpenFileDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}