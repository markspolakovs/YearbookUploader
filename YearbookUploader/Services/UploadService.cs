using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YearbookUploader.Models;

namespace YearbookUploader.Services
{
    internal class UploadService
    {
        public static CancellationTokenSource CancellationToken { get; private set; }
        private static readonly int MAX_LOCAL_COPY_ATTEMPTS = 3;

        public static async Task Upload(UploadTask task, IProgress<double> totalProgress, IProgress<double> fileProgress, TextBoxWriter logs)
        {
            totalProgress.Report(0);
            CancellationToken = new CancellationTokenSource();
            var files = Directory.EnumerateFiles(task.Folder).OrderBy(x => x).ToList();
            var indexOfFirst = files.IndexOf(task.FirstFileName);
            var indexOfLast = files.IndexOf(task.LastFileName);
            IEnumerable<string> filesToUpload = files.Where((_, index) => index >= indexOfFirst && index <= indexOfLast);
            var fileIndex = 0;
            var filesForUpload = filesToUpload.ToList();
            foreach (var file in filesForUpload)
            {
                var index = fileIndex;
                await UploadSingleFile(task, file, new Progress<double>(x =>
                {
                    totalProgress.Report((index + x) / filesForUpload.Count);
                    fileProgress.Report(x);
                }), logs);
                fileIndex += 1;
            }
        }

        public static async Task UploadSingleFile(UploadTask task, string path, IProgress<double> progress, TextBoxWriter logs)
        {
            progress.Report(0);
            await logs.WriteLineAsync($"Starting upload of {path}");
            var localCopyAttempts = 0;
            var checkedOverwrite = false;
            while (true)
            {
                var targetPath = await LocalCopyService.LocalCopy(task, path, checkedOverwrite);
                if (ChecksumService.VerifyChecksum(path, targetPath))
                {
                    await logs.WriteLineAsync("Local copy successful");
                    break;
                } else
                {
                    await logs.WriteLineAsync($"Local copy attempt {localCopyAttempts} failed");
                    localCopyAttempts += 1;
                    checkedOverwrite = true;
                    if (localCopyAttempts == MAX_LOCAL_COPY_ATTEMPTS)
                    {
                        throw new Exception($"Tried {MAX_LOCAL_COPY_ATTEMPTS} times to copy file {path}");
                    }
                    CancellationToken.Token.ThrowIfCancellationRequested();
                }
            }
            CancellationToken.Token.ThrowIfCancellationRequested();
            await logs.WriteLineAsync($"Starting upload to OneDrive for {path}");
            await OneDriveUploadService.UploadToOneDrive(task, path, CancellationToken.Token, progress, logs);
            logs.WriteLine("Job's done!");
        }
    }
}
