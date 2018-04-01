using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProjectUtilsExtension.Core
{
    internal sealed class Launcher
    {
        private readonly string _projectUtilsPath = Settings.Default.ProjectUtilsPath;
        private readonly string _destFolderPath = Settings.Default.DestFolderPath;
        private readonly string _excludedNamespace = Settings.Default.ExcludedNamespace;
        private readonly string _addedNamespace = Settings.Default.AddedNamespace;
        private readonly bool _isUseExcludedNamespace = Settings.Default.IsUseExcludedNamespace;
        private readonly bool _isUseAddedNamespace = Settings.Default.IsUseAddedNamespace;
        private string _selectedFolderPath;
        private string _outputNamespace = Constants.FirstPartOfDefaultNamespase;
        private bool _isSelectedFolderPathAndName;

        public string ResponseOutput { get; private set; }
        public string ResponseErorr { get; private set; }

        public Launcher() {
            SetSelectedFolderPathAndNamespace();
        }

        private void SetSelectedFolderPathAndNamespace() {
            IsSingleProjectItemSelected(out var hierarchy, out var itemId);    
            var vsProject = hierarchy as IVsProject;
            if (vsProject == null) return;

            vsProject.GetMkDocument(itemId, out var itemFullPath);
            var transformFileInfo = new FileInfo(itemFullPath);
            _selectedFolderPath = transformFileInfo.DirectoryName;
            if (transformFileInfo.Directory != null) {
                _outputNamespace += transformFileInfo.Directory.Name;
            }
            _isSelectedFolderPathAndName = 
                !string.IsNullOrEmpty(_selectedFolderPath) 
                || _outputNamespace != Constants.FirstPartOfDefaultNamespase;
        }

        public static bool IsSingleProjectItemSelected(out IVsHierarchy hierarchy, out uint itemId) {
            hierarchy = null;
            itemId = VSConstants.VSITEMID_NIL;

            var monitorSelection = Package.GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
            var solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            if (monitorSelection == null || solution == null) {
                return false;
            }

            var hierarchyPtr = IntPtr.Zero;
            var selectionContainerPtr = IntPtr.Zero;
            try {
                var hr = monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemId, out var multiItemSelect, out selectionContainerPtr);
                hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;

                if (ErrorHandler.Failed(hr)
                    || hierarchyPtr == IntPtr.Zero
                    || itemId == VSConstants.VSITEMID_NIL
                    || multiItemSelect != null
                    || itemId == VSConstants.VSITEMID_ROOT
                    || hierarchy == null) {
                    return false;
                }

                var result = solution.GetGuidOfProject(hierarchy, out var _);
                return !ErrorHandler.Failed(result);
            } finally {
                if (selectionContainerPtr != IntPtr.Zero) {
                    Marshal.Release(selectionContainerPtr);
                }

                if (hierarchyPtr != IntPtr.Zero) {
                    Marshal.Release(hierarchyPtr);
                }
            }
        }

        public void Merge() {
            if (!_isSelectedFolderPathAndName) {
                ResponseErorr = Constants.FailToSetSelectedFolderPathAndNamespaceMessage;
                return;
            }
            TryToMerge();
        }

        private void TryToMerge() {
            var myProcess = new Process {
                StartInfo = new ProcessStartInfo(_projectUtilsPath, CreateArguments()) {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            try {
                myProcess.Start();
                if (!myProcess.WaitForExit(10000)) {
                    myProcess.Kill();
                }
                ResponseOutput = myProcess.StandardOutput.ReadToEnd();
                ResponseErorr = myProcess.StandardError.ReadToEnd();
            }
            catch (Exception ex) {
                ResponseOutput = Constants.CantStartProcessMessage + Environment.NewLine;
                ResponseErorr = ex.Message;
            }
        }

        private string CreateArguments() {
            var arguments = $"MergeFile -ip=\"{_selectedFolderPath}\" -op=\"{_destFolderPath}\" -n=\"{_outputNamespace}\"";
            if (_isUseExcludedNamespace && !string.IsNullOrEmpty(_excludedNamespace)) {
                arguments += $" -en=\"{_excludedNamespace}\"";
            }
            if (_isUseAddedNamespace && !string.IsNullOrEmpty(_addedNamespace)) {
                arguments += $" -an=\"{_addedNamespace}\"";
            }
            return arguments;
        }
    }
}
