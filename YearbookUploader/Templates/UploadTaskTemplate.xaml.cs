using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using YearbookUploader.Models;

namespace YearbookUploader.Templates
{
    public partial class UploadTaskTemplate
    {
        private string BrowseFolder()
        {
            using (var picker = new FolderBrowserDialog())
            {
                var result = picker.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(picker.SelectedPath))
                {
                    return picker.SelectedPath;
                }

                return null;
            }
        }

        private string BrowseFile(string startingFolder)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.InitialDirectory = startingFolder;
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    return dialog.FileName;
                }

                return null;
            }
        }

        private void BrowseFolderButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var task = (UploadTask) ((FrameworkElement) sender).DataContext;
            var folderMaybe = BrowseFolder();
            if (folderMaybe != null)
            {
                task.Folder = folderMaybe;
            }
        }

        private void BrowseFirstFileButton_Click(object sender, RoutedEventArgs e)
        {
            var task = (UploadTask)((FrameworkElement)sender).DataContext;
            var fileMaybe = BrowseFile(task.Folder);
            if (fileMaybe != null)
            {
                task.FirstFileName = fileMaybe;
            }
        }

        private void BrowseLastFileButton_Click(object sender, RoutedEventArgs e)
        {
            var task = (UploadTask)((FrameworkElement)sender).DataContext;
            var fileMaybe = BrowseFile(task.Folder);
            if (fileMaybe != null)
            {
                task.LastFileName = fileMaybe;
            }
        }

        private void BrowseLocalPath_Click(object sender, RoutedEventArgs e)
        {
            var task = (UploadTask)((FrameworkElement)sender).DataContext;
            var folderMaybe = BrowseFolder();
            if (folderMaybe != null)
            {
                Debug.Print(folderMaybe);
                task.LocalBackupPath = folderMaybe;
                Debug.Print(task.LocalBackupPath);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var task = (UploadTask)((FrameworkElement)sender).DataContext;
            task.State = UploadTaskState.Initial;
        }
    }
}
