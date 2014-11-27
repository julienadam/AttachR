using System;
using System.IO;
using System.Threading;
using System.Windows;
using AttachR.Commands;
using AttachR.Components.Recent;
using AttachR.Engine;
using AttachR.Models;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace AttachR.ViewModels
{
    public class MainViewModel : PropertyChangedBase, IHandle<RunAllCommand>, IHandle<StopAllCommand>
    {
        private readonly IRecentFileListPersister persister;
        private readonly IWindowManager windowManager;
        private const string FILE_DIALOG_FILTERS = "Debug profiles|*.dpf|All Files|*.*";
        private readonly Maestro maestro = new Maestro();
        private readonly FileManager fileManager = new FileManager();
        private readonly object debuggingProfileLock = new object();

        // ReSharper disable once NotAccessedField.Local
        private Timer timer;

        public MainViewModel(
            [NotNull] IRecentFileListPersister persister, 
            [NotNull] IEventAggregator aggregator,
            [NotNull] IWindowManager windowManager)
        {
            if (persister == null) throw new ArgumentNullException("persister");
            if (aggregator == null) throw new ArgumentNullException("aggregator");
            if (windowManager == null) throw new ArgumentNullException("windowManager");

            this.persister = persister;
            this.windowManager = windowManager;

            aggregator.Subscribe(this);

            DebuggingProfile = new DebuggingProfileViewModel();
            timer = new Timer(state => CheckExitedProcesses(), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        private string error;
        private DebuggingProfileViewModel debuggingProfile;
        private string fileName;
        private bool isDirty;

        public DebuggingProfileViewModel DebuggingProfile
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

        private void CheckExitedProcesses()
        {
            lock (debuggingProfileLock)
            {
                foreach (var debuggingTarget in DebuggingProfile.Targets)
                {
                    var process = debuggingTarget.CurrentProcess;
                    if (process != null && process.HasExited)
                    {
                        debuggingTarget.CurrentProcess = null;
                    }
                }
            }
        }

        public IRecentFileListPersister Persister
        {
            get { return persister; }
        }

        private void Clear()
        {
            Load(new DebuggingProfileViewModel(), "");
        }

        private void Load(DebuggingProfileViewModel profile, string filePath)
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
            if (string.IsNullOrEmpty(FileName))
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
                FileName = newFileName;
                persister.InsertFile(newFileName);
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

        public void RunAll()
        {
            try
            {
                RunResult result = maestro.Run(DebuggingProfile.VisualStudioSolutionPath, DebuggingProfile.Targets);
                Error = result.Message;
            }
            catch (Exception ex)
            {
                Error = string.Format("Could not run all executables : {0}", ex.Message);
            }
        }

        public void StopAll()
        {
            try
            {
                var result = maestro.Stop(DebuggingProfile);
                Error = result.Message;
            }
            catch (Exception ex)
            {
                Error = string.Format("Could not stop all executables : {0}", ex.Message);
            }
        }

        public void AddExecutable()
        {
            lock (debuggingProfileLock)
            {
                //TODO: show window, add after.
                DebuggingProfile.Targets.Add(new DebuggingTargetViewModel());
            }
        }
        
        public void RemoveExecutable(DebuggingTargetViewModel target)
        {
            lock (debuggingProfileLock)
            {
                DebuggingProfile.Targets.Remove(target);
            }
        }

        public void BrowseForSolution()
        {
            var solutionFile = ShowOpenDialog("Solution files", "*.sln");
            if (solutionFile != null)
            {
                DebuggingProfile.VisualStudioSolutionPath = solutionFile;
            }
        }

        public void Start(DebuggingTargetViewModel target)
        {
            DebuggingProfileViewModel profile = DebuggingProfile;
            var result = maestro.Run(profile.VisualStudioSolutionPath, profile.Targets, target);
            Error = result.Message;
        }

        public void Stop(DebuggingTargetViewModel target)
        {
            var result = maestro.Stop(target);
            Error = result.Message;
        }

        public void OpenSolution()
        {
            maestro.OpenSolution(DebuggingProfile);
        }

        public void Closing()
        {
            ValidateDataLoss();
        }

        public void Handle(RunAllCommand message)
        {
            RunAll();
        }

        public void Handle(StopAllCommand message)
        {
            StopAll();
        }

        public void EditExecutable(DebuggingTargetViewModel target)
        {
            var clone = (DebuggingTargetViewModel) target.Clone();
            if (windowManager.ShowDialog(clone) == true)
            {
                var index = DebuggingProfile.Targets.IndexOf(target);
                DebuggingProfile.Targets.RemoveAt(index);
                DebuggingProfile.Targets.Insert(index, clone);
            }
        }
    }
}