using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
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
        private readonly Maestro maestro = new Maestro();
        private readonly FileManager fileManager = new FileManager();

        private Model Model
        {
            get { return ((Model)DataContext); }
            set { DataContext = value; }
        }

        public MainWindow(string path)
        {
            InitializeComponent();
            Model = new Model();

            if (!string.IsNullOrEmpty(path))
            {
                FileOpenCore(path);
            }
        }

        private void FileOpenCore(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                return;
            }

            if (!File.Exists(filepath))
            {
                Model.Error = string.Format("Could not load data from {0}. File does not exist", filepath);
                return;
            }

            try
            {
                // Load profile
                Model.DebuggingProfile = fileManager.Open(filepath);
                Model.IsDirty = false;
                Model.FileName = filepath;
                Model.Error = "";
            }
            catch(Exception ex)
            {
                Model.Error = string.Format("Could not load data from {0}. It might be corrupted. Error was : {1}", filepath, ex.Message);
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
                Filter = "*.dpf|Debug profiles|*.*|All Files",
                CheckFileExists = true,
                CheckPathExists = true
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
            if (Model.FileName != null)
            {
                SaveAs();
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
                Filter = "*.dpf|Debug profiles|*.*|All Files",
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
    }
}
