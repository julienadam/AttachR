using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using AttachR.Commands;
using AttachR.Components.Recent;
using AttachR.Engine;
using AttachR.Serializers;
using AttachR.VisualStudio;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace AttachR.ViewModels
{
    public class MainViewModel : PropertyChangedBase, IHandle<RunAllCommand>, IHandle<StopAllCommand>
    {
        private readonly IWindowManager windowManager;
        private readonly IPreferencesSerializer preferencesSerializer;
        private const string FILE_DIALOG_FILTERS = "Debug profiles|*.dpf|All Files|*.*";
        private readonly Maestro maestro;

        private readonly DebuggingTargetFileSerializer debuggingTargetFileSerializer = new DebuggingTargetFileSerializer();
        private readonly object debuggingProfileLock = new object();

        private string error;
        private DebuggingProfileViewModel debuggingProfile;
        private string fileName;
        private bool isDirty;
        private readonly IEventAggregator aggregator;

        // ReSharper disable once NotAccessedField.Local
        private Timer timer;

        public MainViewModel(
            [NotNull] IRecentFileListPersister persister, 
            [NotNull] IEventAggregator aggregator,
            [NotNull] IWindowManager windowManager, 
            [NotNull] IPreferencesSerializer preferencesSerializer)
        {
            if (persister == null) throw new ArgumentNullException(nameof(persister));
            if (aggregator == null) throw new ArgumentNullException(nameof(aggregator));
            if (windowManager == null) throw new ArgumentNullException(nameof(windowManager));
            if (preferencesSerializer == null) throw new ArgumentNullException(nameof(preferencesSerializer));

            this.aggregator = aggregator;
            this.Persister = persister;
            this.windowManager = windowManager;
            this.preferencesSerializer = preferencesSerializer;

            aggregator.Subscribe(this);

            DebuggingProfile = new DebuggingProfileViewModel();
            maestro = new Maestro();
            timer = new Timer(state => CheckExitedProcesses(), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        public DebuggingProfileViewModel DebuggingProfile
        {
            get { return debuggingProfile; }
            set
            {
                if (debuggingProfile == value)
                {
                    return;
                }
                debuggingProfile = value;
                NotifyOfPropertyChange(() => DebuggingProfile);
            }
        }

        public string FileName
        {
            get { return fileName; }
            set
            {
                if (fileName == value)
                {
                    return;
                }
                fileName = value;
                NotifyOfPropertyChange(() => FileName);
            } 
        }

        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                if (isDirty == value)
                {
                    return;
                }
                isDirty = value;
                NotifyOfPropertyChange(() => IsDirty);
            }
        }

        public string Error
        {
            get { return error; }
            set
            {
                if (error == value)
                {
                    return;
                }
                error = value;
                NotifyOfPropertyChange(() => Error);
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

        public IRecentFileListPersister Persister { get; }

        private void Clear()
        {
            Load(new DebuggingProfileViewModel(), "");
        }

        private void Load(DebuggingProfileViewModel profile, string filePath)
        {
            DebuggingProfile = profile;
            foreach (var target in profile.Targets)
            {
                target.SetDebuggingEngineSelector(GetAvailableEnginesFromVisualStudio);
            }

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
                debuggingTargetFileSerializer.Save(newFileName, DebuggingProfile);
                FileName = newFileName;
                Persister.InsertFile(newFileName);
            }
            catch (Exception ex)
            {
                Error = $"Could not save file to {newFileName}. Error was : {ex.Message}";
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
                Error = $"Could not load data from {filePath}. File does not exist";

                Persister.RemoveFile(filePath);
                return;
            }

            try
            {
                Load(debuggingTargetFileSerializer.Open(filePath), filePath);
                Persister.InsertFile(filePath);
            }
            catch (Exception ex)
            {
                Error = $"Could not load data from {filePath}. It might be corrupted. Error was : {ex.Message}";
            }
        }

        public void Open(object sender, RoutedEventArgs e)
        {
            var profileFile = DialogHelper.ShowOpenDialog("Debug profiles", ".dpf", null);
            if (profileFile != null)
            {
                FileOpenCore(profileFile);
            }
        }

        public void OpenRecent(RecentFileList.MenuClickEventArgs recentItemClicked)
        {
            FileOpenCore(recentItemClicked.Filepath);
        }

        public void Preferences(object sender, RoutedEventArgs e)
        {
            windowManager.ShowDialog(new PreferencesViewModel(aggregator, preferencesSerializer));
        }

        public void RunAll()
        {
            RunOrDebugAll(false);
        }

        public void DebugAll()
        {
            RunOrDebugAll(true);
        }
        
        private void RunOrDebugAll(bool debug)
        {
            try
            {
                var result = maestro.Run(DebuggingProfile.VisualStudioSolutionPath, DebuggingProfile.Targets, debug, false);
                Error = result.Message;
            }
            catch (Exception ex)
            {
                Error = $"Could not run all executables : {ex.Message}";
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
                Error = $"Could not stop all executables : {ex.Message}";
            }
        }

        public void AddExecutable()
        {
            lock (debuggingProfileLock)
            {
                var newModel = new DebuggingTargetViewModel();
                if (windowManager.ShowDialog(newModel) == true)
                {
                    DebuggingProfile.Targets.Add(newModel);
                }
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
            var solutionFile = DialogHelper.ShowOpenDialog("Solution files", "*.sln", DebuggingProfile.VisualStudioSolutionPath);
            if (solutionFile != null)
            {
                DebuggingProfile.VisualStudioSolutionPath = solutionFile;
            }
        }

        public void Debug(DebuggingTargetViewModel target)
        {
            var result = maestro.Run(DebuggingProfile.VisualStudioSolutionPath, new List<DebuggingTargetViewModel> { target }, true, true);
            Error = result.Message;
        }

        public void Run(DebuggingTargetViewModel target)
        {
            var result = maestro.Run(DebuggingProfile.VisualStudioSolutionPath, new List<DebuggingTargetViewModel>{ target }, false, true);
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
        
        public void Handle(DebugAllCommand message)
        {
            DebugAll();
        }

        public void Handle(StopAllCommand message)
        {
            StopAll();
        }

        public void EditExecutable(DebuggingTargetViewModel target)
        {
            var clone = (DebuggingTargetViewModel) target.Clone();
            clone.SetDebuggingEngineSelector(GetAvailableEnginesFromVisualStudio);

            if (windowManager.ShowDialog(clone) != true)
            {
                return;
            }

            var index = DebuggingProfile.Targets.IndexOf(target);
            DebuggingProfile.Targets.RemoveAt(index);
            DebuggingProfile.Targets.Insert(index, clone);
        }

        public IEnumerable<string> GetAvailableEnginesFromVisualStudio()
        {
            var engines = DebuggingEngineDetector.GetAvailableEngines(
                DebuggingProfile.CurrentVisualStudioProcess,
                DebuggingProfile.VisualStudioSolutionPath);

            return engines.Select(e => e.Name).OrderBy(s => s);
        }
    }
}