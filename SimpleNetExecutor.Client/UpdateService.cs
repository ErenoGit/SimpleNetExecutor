using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SimpleNetExecutor.Client
{
    internal class UpdateService
    {
        private readonly string ServerAddress = "https://localhost:7041";
        private readonly HttpClient _http = new HttpClient();
        public async Task CheckForUpdateAsync()
        {
            string endpointId;
            string pathToEndpointId = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimpleNetExecutor", "clientId.txt");

            if (!File.Exists(pathToEndpointId))
            {
                endpointId = Guid.NewGuid().ToString();
                File.WriteAllText(pathToEndpointId, endpointId);
            }
            else
                endpointId = File.ReadAllText(pathToEndpointId);

            var latestMd5 = await _http.GetStringAsync($"{ServerAddress}/api/moduleMd5?endpointId={endpointId}");

            if (string.IsNullOrEmpty(latestMd5))
                return;

            string pathToModuleDll = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimpleNetExecutor", "module.dll");

            if (!File.Exists(pathToModuleDll) || CalculateMD5(pathToModuleDll) != latestMd5)
                await DownloadDll(latestMd5, pathToModuleDll);
        }

        private async Task DownloadDll(string md5, string pathToModuleDll)
        {
            var bytes = await _http.GetByteArrayAsync($"{ServerAddress}/api/moduleDll?moduleMd5={md5}");
            File.WriteAllBytes(pathToModuleDll, bytes);
        }

        private string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
