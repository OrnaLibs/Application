using System.Diagnostics;
using System;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace OrnaLibs.Application
{
    public static class Updater
    {
        private static Thread thread = null;
        private static string _execArgs = null;
        private static string _account = null;
        private static string _repo = null;
        private static int _interval;
        private static Version _version = null;

        public static void Init(string account, string repo, Version version, int interval = 3600, string execArgs = "")
        {
            _execArgs = execArgs;
            _account = account;
            _repo = repo;
            _version = version;
            _interval = interval;
            Task.Run(Checker);
        }

        public static void Init(Application appInfo, int interval = 3600, string execArgs = "") =>
            Init(appInfo.CompanyName, appInfo.Name, appInfo.Version, interval, execArgs);

        public static event Action OnUpdated;

        public static void Dispose() => thread?.Interrupt();

        private static async Task Checker()
        {
            while (true)
            {
                try
                {
                    var info = await IsInstalledLastVersion();
                    if (info.Item1)
                    {
                        var path = await DownloadFile(info.Item2, info.Item3);
                        Process.Start(path, _execArgs);
                        OnUpdated.Invoke();
                    }
                    Thread.Sleep(_interval * 1000);
                }
                catch (ThreadInterruptedException)
                {
                    break;
                }
            }
        }

        private static async Task<(bool, string, string)> IsInstalledLastVersion()
        {
            using (var http = new HttpClient())
            {
                var resp = await http.GetAsync("https://gist.githubusercontent.com/OrnarasUS/90a15c9999d28459c44ae2e9425fa05b/raw");
                var body = await resp.Content.ReadAsStringAsync();
                var json = JsonNode.Parse(body);
                var projInfo = json?[_repo];
                if (projInfo is null) return (false, "", "");
                if (!Version.TryParse(projInfo["version"]?.GetValue<string>(), out var lastVersion) ||
                    lastVersion.CompareTo(_version) <= 0)
                    return (false, "", "");
                var id = projInfo["id"]?.GetValue<string>();
                var token = projInfo["token"]?.GetValue<string>();
                return (true, id, token);
            }
        }

        internal static async Task<string> DownloadFile(string id, string token)
        {
            var path = $"{Path.GetTempPath()}{_repo}.exe";
            var url = new StringBuilder("https://api.github.com/repos/%org%/%repo%/releases/assets/%id%");
            url.Replace("%org%", _account);
            url.Replace("%repo%", _repo);
            url.Replace("%id%", id);

            var req = new HttpRequestMessage(HttpMethod.Get, url.ToString());
            if (!string.IsNullOrWhiteSpace(token))
                req.Headers.Add("Authorization", $"token {token}");
            req.Headers.Add("Accept", "application/octet-stream");
            req.Headers.Add("User-Agent", "curl/7.64.1");

            using (var http = new HttpClient())
            {
                var resp = await http.SendAsync(req);
                using (var fs = new FileStream(path, FileMode.Create))
                    await resp.Content.CopyToAsync(fs);
            }
            return path;
        }
    }
}