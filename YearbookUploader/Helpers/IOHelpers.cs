using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YearbookUploader.Helpers
{
    internal static class IOHelpers
    {
        internal static async Task CopyFileAsync(string sourcePath, string destinationPath, bool overwrite = false)
        {
            Stream source = File.Open(sourcePath, FileMode.Open);
            Stream dest;
            if (File.Exists(destinationPath))
            {
                if (!overwrite) throw new IOException("cannot overwrite");
                dest = File.Open(destinationPath, FileMode.Truncate);
            }
            else
            {
                dest = File.Create(destinationPath);
            }
            await source.CopyToAsync(dest);
            dest.Close();
            source.Close();
        }
    }
}
