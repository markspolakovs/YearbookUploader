using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace YearbookUploader
{
    class ShittyAuthenticationProvider : IAuthenticationProvider
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
#pragma warning restore CS1998
        {
            request.Headers.Add("Authorization", "Bearer " + MainWindow.AccessToken);
        }
    }
}
