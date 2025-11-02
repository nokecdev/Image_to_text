using System.ComponentModel;

namespace Image_to_text
{
    public class UserDataContext : INotifyPropertyChanged
    {
        private ItemType _itemType;
        private SettingsManager _settingsManager;

        public ItemType ItemType
        {
            get => _itemType;
            set
            {
                if (_itemType != value)
                {
                    _itemType = value;
                    OnPropertyChanged(nameof(ItemType));
                }
            }
        }

        public SettingsManager SettingsManager
        {
            get => _settingsManager;
            set
            {
                if (_settingsManager != value)
                {
                    _settingsManager = value;
                    OnPropertyChanged(nameof(SettingsManager));
                }
            }
        }

        public UserDataContext()
        {
            ItemType = new ItemType();
            SettingsManager = SettingsManager.Instance;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}