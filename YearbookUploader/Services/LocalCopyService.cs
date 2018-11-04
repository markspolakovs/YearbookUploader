using System.IO;
using System.Threading.Tasks;
using System.Windows;
using YearbookUploader.Helpers;
using YearbookUploader.Models;

namespace YearbookUploader.Services
{
    internal class LocalCopyService
    {

        public static async Task<string> LocalCopy(UploadTask task, string sourceFileName, bool doNotCheckOverwrite = false)
        {
            var name = Path.GetFileName(sourceFileName);
            var targetPath = task.LocalBackupPath + Path.DirectorySeparatorChar + name;
            var overwrite = doNotCheckOverwrite || task.OverwriteAll;
            if (File.Exists(targetPath) && !overwrite)
            {
                if (MessageBox.Show($"File {name} exists in target directory {task.LocalBackupPath}. Are you sure you want to overwrite it?", "Overwrite?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    throw new ServiceFailureException();
                }
                else
                {
                    overwrite = true;
                }
            }

            await IOHelpers.CopyFileAsync(sourceFileName, targetPath, overwrite);
            return targetPath;
        }
    }
}
