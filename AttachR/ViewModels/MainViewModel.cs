using System;
using System.IO;
using System.Windows;
using AttachR.Components.Recent;
using AttachR.Engine;
using Caliburn.Micro;
using Microsoft.Win32;

namespace AttachR.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        private readonly IRecentFileListPersister persister;
        private const string FILE_DIALOG_FILTERS = "Debug profiles|*.dpf|All Files|*.*";
        private readonly Maestro maestro = new Maestro();
        private readonly FileManager fileManager = new FileManager();

        public MainViewModel(IRecentFileListPersister persister)
        {
            this.persister = persister;
            DebuggingProfile = new DebuggingProfile();
        }

        private string error;
        private DebuggingProfile debuggingProfile;
        private string fileName;
        private bool isDirty;

        public DebuggingProfile DebuggingProfile
        {
            get { return debuggingProfile; }
            set
            {
                if (debuggingProfile != value)
                {
                    debuggingProfile = value;
                    NotifyOfPropertyChange(() => DebuggingProfile);
                }
            }
        }

        public string FileName
        {
            get { return fileName; }
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    NotifyOfPropertyChange(() => FileName);
                }
            } 
        }

        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                if (isDirty != value)
                {
                    isDirty = value;
                    NotifyOfPropertyChange(() => IsDirty);
                }
            }
        }

        public string Error
        {
            get { return error; }
            set
            {
                if (error != value)
                {
                    error = value;
                    NotifyOfPropertyChange(() => Error);
                }
            }
        }

        public IRecentFileListPersister Persister
        {
            get { return persister; }
        }

        private void Clear()
        {
            Load(new DebuggingProfile(), "");
        }

        private void Load(DebuggingProfile profile, string filePath)
        {
            DebuggingProfile = profile;
            FileName = filePath;
            Error = "";
            IsDirty = false;
        }

        public void New(object sender, RoutedEventArgs e)
        {
            ValidateDataLoss();
            Clear();
        }

        private void ValidateDataLoss()
        {
            if (!IsDirty) return;

            if (MessageBox.Show(
                "Do you want to save your changes?\r\nYes to save and continue\r\nNo to continue without saving",
                "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Save();
            }
        }

       
        public void Save()
        {
            if (FileName != null)
            {
                SaveAs();
            }
            else
            {
                Save(FileName);
            }
        }

        public void SaveAs()
        {
            var dialog = new SaveFileDialog
            {
                AddExtension = true,
                ValidateNames = true,
                CheckPathExists = true,
                Filter = FILE_DIALOG_FILTERS,
                FilterIndex = 0,
            };

            if (dialog.ShowDialog() == true)
            {
                Save(dialog.FileName);
            }
        }

        private void Save(string newFileName)
        {
            try
            {
                fileManager.Save(newFileName, DebuggingProfile);
            }
            catch (Exception ex)
            {
                Error = string.Format("Could not save file to {0}. Error was : {1}", newFileName, ex.Message);
            }
        }

        private void FileOpenCore(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            if (!File.Exists(filePath))
            {
                Error = string.Format("Could not load data from {0}. File does not exist", filePath);

                persister.RemoveFile(filePath);
                return;
            }

            try
            {
                Load(fileManager.Open(filePath), filePath);
                persister.InsertFile(filePath);
            }
            catch (Exception ex)
            {
                Error = string.Format("Could not load data from {0}. It might be corrupted. Error was : {1}", filePath, ex.Message);
            }
        }

        public void Open(object sender, RoutedEventArgs e)
        {
            var profileFile = ShowOpenDialog("Debug profiles", ".dpf");
            if (profileFile != null)
            {
                FileOpenCore(profileFile);
            }
        }

        public void OpenRecent(RecentFileList.MenuClickEventArgs recentItemClicked)
        {
            FileOpenCore(recentItemClicked.Filepath);
        }

        private static string ShowOpenDialog(string fileTypeDescription, string fileTypeExtension)
        {
            var dialog = new OpenFileDialog
            {
                Filter = string.Format("{0}|{1}|All files|*.*", fileTypeDescription, fileTypeExtension),
                FilterIndex = 0,
                CheckFileExists = true,
                CheckPathExists = true,
                ValidateNames = true,
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public void BrowseForExe(DebuggingTarget target)
        {
            var executableFile = ShowOpenDialog("Executable files", "*.exe");
            if (executableFile != null)
            {
                target.Executable = executableFile;
            }
        }

        public void Run()
        {
            try
            {
                maestro.Run(DebuggingProfile);
            }
            catch (Exception ex)
            {
                Error = string.Format("Could not run this configuration : {0}", ex.Message);
            }
        }

        public void AddExecutable()
        {
            DebuggingProfile.Targets.Add(new DebuggingTarget());
        }

        public void BrowseForSolution()
        {
            var solutionFile = ShowOpenDialog("Solution files", "*.sln");
            if (solutionFile != null)
            {
                DebuggingProfile.VisualStudioSolutionPath = solutionFile;
            }
        }

        public void Start(DebuggingTarget target)
        {
            maestro.Run(DebuggingProfile, target);
        }

        public void Stop(DebuggingTarget target)
        { 
            maestro.Stop(target);
        }

        public void OpenSolution()
        {
            maestro.OpenSolution(DebuggingProfile);
        }

        public void Closing()
        {
            ValidateDataLoss();
        }
    }
}