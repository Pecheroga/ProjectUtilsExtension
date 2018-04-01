namespace ProjectUtilsExtension.Core
{
    public static class Constants
    {
        public const string FirstPartOfDefaultNamespase = "Terrasoft.Configuration.";
        public const string FailToSetSelectedFolderPathAndNamespaceMessage = "Fail to set selected folder path or name";
        public const string CantStartProcessMessage = "Can't start ProjectUtils.exe process";
        public const string SettingsWindowTitle = "Setup ProjectUtils Extension";
        public const string SettingsSetProjectUtilPathTip = "ProjectUtils full folder path with name:";
        public const string SettingsSetDestinationPathTip = "Destination full folder path:";
        public const string BrowseProjectUtilWindowTitle = "Select ProjectUtils executing file";
        public const string BrowseOutputFolderWindowDescription = "Select output folder to store merged code:";
        public const string ExcludeNamespaceTip = "Exclude namespace by Regex (e.g.: \"using System.Text\\S*\\r?\\n)\":";
        public const string AddNamespaceTip = "Add namespace (comma separator):";
    }
}
