using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using AttachR.Components;
using AttachR.Engine;
using AttachR.Models;
using Microsoft.Win32;

namespace AttachR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string FILE_DIALOG_FILTERS = "Debug profiles|*.dpf|All Files|*.*";
        private readonly Maestro maestro = new Maestro();
        private readonly FileManager fileManager = new FileManager();

        private Model Model
        {
            get { return ((Model)DataContext); }
            set { DataContext = value; }
        }

        public MainWindow() : this(null)
        { }

        public MainWindow(string path)
        {
            InitializeComponent();
            Model = new Model();

            if (!string.IsNullOrEmpty(path))
            {
                FileOpenCore(path);
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
                Model.Error = string.Format("Could not load data from {0}. File does not exist", filePath);
                RecentFileList.RemoveFile(filePath);
                return;
            }

            try
            {
                // Load profile
                Model.DebuggingProfile = fileManager.Open(filePath);
                Model.IsDirty = false;
                Model.FileName = filePath;
                Model.Error = "";
                RecentFileList.InsertFile(filePath);
            }
            catch(Exception ex)
            {
                Model.Error = string.Format("Could not load data from {0}. It might be corrupted. Error was : {1}", filePath, ex.Message);
            }
        }

        private void ButtonRun_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                maestro.Run(Model.DebuggingProfile);
            }
            catch(Exception ex)
            {
                Model.Error = string.Format("Could not run this configuration : {0}", ex.Message);
            }
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            Model.DebuggingProfile.Targets.Add(new DebuggingTarget());
        }
        
        private void MenuItem_Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_Open_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = FILE_DIALOG_FILTERS,
                CheckFileExists = true,
                CheckPathExists = true,
                FilterIndex = 0,
            };

            if (dialog.ShowDialog() == true)
            {
                FileOpenCore(dialog.FileName);
            }
        }

        private void RecentFileList_OnMenuClick(object sender, RecentFileList.MenuClickEventArgs e)
        {
            FileOpenCore(e.Filepath);
        }

        private void MenuItem_Save_OnClick(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Save()
        {
            if (Model.FileName != null)
            {
                SaveAs();
            }
            else
            {
                Save(Model.FileName);
            }
        }

        private void MenuItem_SaveAs_OnClick(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }

        private void SaveAs()
        {
            SaveFileDialog d = new SaveFileDialog
            {
                AddExtension = true,
                ValidateNames = true,
                CheckPathExists = true,
                Filter = FILE_DIALOG_FILTERS,
                FilterIndex = 0,
            };

            if (d.ShowDialog() == true)
            {
                Save(d.FileName);
            }
        }

        private void Save(string fileName)
        {
            try
            {
                fileManager.Save(fileName, Model.DebuggingProfile);
            }
            catch (Exception ex)
            {
                Model.Error = string.Format("Could not save file to {0}. Error was : {1}", fileName, ex.Message);
            }
        }

        private void MenuItem_New_OnClick(object sender, RoutedEventArgs e)
        {
            ValidateDataLoss();
            Model.Clear();
        }

        private void ValidateDataLoss()
        {
            if (!Model.IsDirty) return;

            if (MessageBox.Show(
                "Do you want to save your changes?\r\nYes to save and continue\r\nNo to continue without saving",
                "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Save();
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            ValidateDataLoss();
        }
    }
}
