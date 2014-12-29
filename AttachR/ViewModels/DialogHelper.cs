using System;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace AttachR.ViewModels
{
    internal static class DialogHelper
    {
        public static string ShowOpenDialog(string fileTypeDescription, string fileTypeExtension, string existingValue)
        {
            var dialog = new OpenFileDialog
            {
                Filter = String.Format("{0}|{1}|All files|*.*", fileTypeDescription, fileTypeExtension),
                FilterIndex = 0,
                CheckFileExists = true,
                CheckPathExists = true,
                ValidateNames = true,
                FileName = existingValue
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public static string ShowFolderDialog(string title, string currentValue)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = title,
                SelectedPath = currentValue,
            };
            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : null;
        }
    }
}