using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace YearbookUploader.Services
{
    internal class ChecksumService
    {
        public static bool VerifyChecksum(string file1, string file2)
        {
            using (var fs1 = File.OpenRead(file1))
            {
                using (var fs2 = File.OpenRead(file2))
                {
                    var hash1 = SHA256.Create().ComputeHash(fs1);
                    var hash2 = SHA256.Create().ComputeHash(fs2);
                    return hash1.SequenceEqual(hash2);
                }
            }
        }
    }
}
