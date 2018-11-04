using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OneDrive.Sdk;
using YearbookUploader.Helpers;
using YearbookUploader.Models;

namespace YearbookUploader.Services
{
    internal class OneDriveUploadService
    {

        private static readonly long MSGRAPH_MAX_LENGTH = 327_680;

        public static async Task<DriveItem> UploadToOneDrive(UploadTask task, string localFileName, CancellationToken token, IProgress<double> progress, TextBoxWriter logs)
        {
            if (MainWindow.Client == null)
            {
                throw new ApplicationException("OneDrive client null!");
            }
            var name = Path.GetFileName(localFileName);
            var targetPath = "YEARBOOK 2019/_Uploads/" + task.TaskName + "/" + name;

            var sesh = await MainWindow.Client.Drives["b!ciVzyDuG1kGQJn8UlMdOl6fArqsytz1JhhwdkjwpvaP7ZUv4lIZKSL4QhaM-19bz"]
                .Root
                .ItemWithPath(targetPath)
                .CreateUploadSession(new DriveItemUploadableProperties() {  })
                .Request().PostAsync();

            logs.WriteLine($"Started new sesh; timeout {sesh.ExpirationDateTime}");

            using (var stream = new FileStream(localFileName, FileMode.Open))
            {
                var maxChunkSize = 320 * 1024 * 2;
                var provider = new ChunkedUploadProvider(sesh, MainWindow.Client, stream, maxChunkSize);
                var readBuffer = new byte[maxChunkSize];
                var trackedExceptions = new List<Exception>();
                DriveItem item = null;
                int index = 0;
                var uploadChunkRequests = provider.GetUploadChunkRequests().ToList();
                foreach (var request in uploadChunkRequests)
                {
                    logs.WriteLine($"Uploading chunk {index} of {uploadChunkRequests.Count} (timeout {provider.Session.ExpirationDateTime})");
                    var result = await provider.GetChunkRequestResponseAsync(request, readBuffer, trackedExceptions);
                    if (result.UploadSucceeded)
                    {
                        if (result.ItemResponse != null)
                        {
                            item = result.ItemResponse;
                        }
                    }
                    index += 1;
                    progress.Report((double)index / uploadChunkRequests.Count);
                }

                logs.WriteLine("OneDrive upload completed; new item ID " + item.Id);
                return item;
            }

        }
    }
}
