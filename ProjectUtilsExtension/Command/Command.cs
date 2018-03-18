using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ProjectUtilsExtension.Core;
using ProjectUtilsExtension.Ui;

namespace ProjectUtilsExtension.Command
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class Command
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("e5472ebf-534a-478c-986c-d95e1bfacca3");
        public static readonly Guid ToolsCommandSet = new Guid("B90441ED-3F82-493D-B68F-FC0FE11FB9F2");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private Command(Package package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));

            if (!(ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)) return;

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(_onFolderContextMenuClick, menuCommandId);
            menuItem.BeforeQueryStatus += _onBeforeFolderContextMenuClicked;
            commandService.AddCommand(menuItem);

            var toolsCommandId = new CommandID(ToolsCommandSet, CommandId);
            var toolsItem = new MenuCommand(_onToolsMenuSetupClick, toolsCommandId);
            commandService.AddCommand(toolsItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static Command Instance {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new Command(package);
        }

        private static void _onBeforeFolderContextMenuClicked(object sender, EventArgs e)
        {
            if (!(sender is OleMenuCommand myCommand)) return;
            myCommand.Enabled = Launcher.IsSingleProjectItemSelected(out _, out _);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void _onFolderContextMenuClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Settings.Default.ProjectUtilsPath)
                || string.IsNullOrEmpty(Settings.Default.DestFolderPath))
            {
                _onToolsMenuSetupClick(null, null);
                return;
            }

            var launcher = new Launcher();
            launcher.Merge();

            VsShellUtilities.ShowMessageBox(
                ServiceProvider,
                launcher.ResponseOutput + launcher.ResponseErorr,
                string.IsNullOrEmpty(launcher.ResponseErorr) ? "Success" : "Error",
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        private static void _onToolsMenuSetupClick(object sender, EventArgs e)
        {
            var setupFormContainer = new SetupFormContainer { StartPosition = FormStartPosition.CenterScreen };
            setupFormContainer.ShowDialog();
        }
    }
}