using ProjectUtilsExtension.Helpers;

namespace ProjectUtilsExtension.Ui.ViewModel
{
    internal class SetupViewModel: Notifier
    {
        private string _projectUtilPath = Settings.Default.ProjectUtilsPath;
        public string ProjectUtilsPath {
            get => _projectUtilPath;
            set {
                _projectUtilPath = value;
                OnPropertyChanged();
            }
        }

        private string _destFolderPath = Settings.Default.DestFolderPath;
        public string DestFolderPath {
            get => _destFolderPath;
            set {
                _destFolderPath = value; 
                OnPropertyChanged();
            }
        }

        public DelegateCommand SaveProjectUtilsPathCommand { get; private set; }
        public DelegateCommand SaveDestPathCommand { get; private set; }

        public SetupViewModel() {
            InitializeCommands();
        }

        private void InitializeCommands() {
            SaveProjectUtilsPathCommand = new DelegateCommand(SaveProjectUtilsPath, CanSaveProjectUtilsPath);
            SaveDestPathCommand = new DelegateCommand(SaveDestPath, CanSaveDestPath);
        }

        private void SaveProjectUtilsPath(object obj) {
            Settings.Default.ProjectUtilsPath = _projectUtilPath;
            Settings.Default.Save();
        }

        private bool CanSaveProjectUtilsPath(object obj) {
            return Settings.Default.ProjectUtilsPath != _projectUtilPath;
        }

        private void SaveDestPath(object obj) {
            Settings.Default.DestFolderPath = _destFolderPath;
            Settings.Default.Save();
        }

        private bool CanSaveDestPath(object obj) {
            return Settings.Default.DestFolderPath != _destFolderPath;
        }
    }
}
