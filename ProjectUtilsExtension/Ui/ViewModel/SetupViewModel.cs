using System.Windows.Forms;
using ProjectUtilsExtension.Core;
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

        public DelegateCommand BrowseProjectUtilsCommand { get; private set; }
        public DelegateCommand BrowseDestFolderCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }

        public SetupViewModel() {
            InitializeCommands();
        }

        private void InitializeCommands() {
            BrowseProjectUtilsCommand = new DelegateCommand(BrowseProjectUtils);
            BrowseDestFolderCommand = new DelegateCommand(BrowseDestFolder);
            SaveCommand = new DelegateCommand(Save, CanSave);
        }

        private void BrowseProjectUtils(object obj) {
            using (
                var dialog = new OpenFileDialog {
                    Title = Constants.BrowseProjectUtilWindowTitle,
                    DefaultExt = ".exe",
                    Filter = @"*.exe|*.exe|All files (*.*)|*.*",
                    Multiselect = false
                }) { 
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK) {
                    ProjectUtilsPath = dialog.FileName;
                }
            }
        }

        private void BrowseDestFolder(object obj) {
            using (
                var dialog = new FolderBrowserDialog {
                    Description = Constants.BrowseOutputFolderWindowDescription
                }) {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK) {
                    DestFolderPath = dialog.SelectedPath;
                }
            }
        }

        private void Save(object obj) {
            Settings.Default.ProjectUtilsPath = _projectUtilPath;
            Settings.Default.DestFolderPath = _destFolderPath;
            Settings.Default.Save();
        }

        private bool CanSave(object obj) {
            return Settings.Default.DestFolderPath != _destFolderPath 
                   || Settings.Default.ProjectUtilsPath != _projectUtilPath; 
        }
    }
}
