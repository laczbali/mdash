using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Dashboard.API.Data;
using Dashboard.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dashboard.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TorrentController : ControllerBase
    {
        private readonly ISettingsRepository _settingsRepo;
        private readonly IAuthRepository _authRepo;
        public TorrentController(ISettingsRepository settingsRepo, IAuthRepository authRepo)
        {
            _authRepo = authRepo;
            _settingsRepo = settingsRepo;

        }

        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetTorrentList()
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;

            string apiCookie;
            try
            {
                apiCookie = await LoginToAPI(clientUsername);
            }
            catch (NotSupportedException)
            {
                return BadRequest("User settings are missing");
            }

            var userSettings = await _settingsRepo.GetSettings(clientUsername);
            var apiUrl = userSettings.Single(x => x.Name == "Overview-Torrent").Fields.Single(x => x.Name == "url").Value;
            var uri = new Uri(apiUrl + "/api/v2/torrents/info");
            JArray torrentList;

            var cookieContainer = new CookieContainer();
            using (var httpClientHandler = new HttpClientHandler { CookieContainer = cookieContainer })
            {
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    cookieContainer.Add(uri, new Cookie("SID", apiCookie));
                    var responseString = await httpClient.GetStringAsync(uri);
                    torrentList = JsonConvert.DeserializeObject<JArray>(responseString);
                }
            }

            // Parse out:
            // Leeched torrents: names, percentages, etas
            // Seeded torrents: number of torrents
            // Other: total download and uploads speed, total number of torrents
            TorrentDataDTO torrentData = new TorrentDataDTO();
            torrentData.uiUrl = apiUrl;

            foreach (JObject torrent in torrentList)
            {
                // If downloading
                if (torrent["dlspeed"].Value<Int32>() > 0)
                {
                    TorrentSingleDTO download = new TorrentSingleDTO();
                    download.name = torrent["name"].Value<string>();
                    download.progress = torrent["progress"].Value<float>();
                    download.eta = torrent["eta"].Value<int>();
                    torrentData.downloads.Add(download);

                    torrentData.totalDlSpeed += torrent["dlspeed"].Value<int>();
                    if (download.eta > torrentData.maxEta) { torrentData.maxEta = download.eta; }
                }

                // If uploading
                if (torrent["upspeed"].Value<Int32>() > 0)
                {
                    torrentData.numOfUpload++;
                    torrentData.totalUpSpeed += torrent["upspeed"].Value<int>();
                }

                torrentData.totalTorrents++;
            }

            return Ok(torrentData);
        }

        private async Task<string> LoginToAPI(string clientUsername)
        {
            var userSettings = await _settingsRepo.GetSettings(clientUsername);
            string apiUrl, apiUsername, apiPassword;
            try
            {
                // TODO: ^ Store password encrypted
                apiUrl = userSettings.Single(x => x.Name == "Overview-Torrent").Fields.Single(x => x.Name == "url").Value;
                apiUsername = userSettings.Single(x => x.Name == "Overview-Torrent").Fields.Single(x => x.Name == "username").Value;
                apiPassword = userSettings.Single(x => x.Name == "Overview-Torrent").Fields.Single(x => x.Name == "password").Value;
            }
            catch (InvalidOperationException)
            {
                throw new NotSupportedException("User settings are missing");
            }
            string url = apiUrl + "/api/v2/auth/login?username=" + apiUsername + "&password=" + apiPassword;
            var uri = new Uri(url);

            string cookie = "";
            var cookieContainer = new CookieContainer();

            using (var httpClientHandler = new HttpClientHandler { CookieContainer = cookieContainer })
            {
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    await httpClient.GetAsync(uri);
                    cookie = cookieContainer.GetCookies(uri)[0].Value;
                }
            }

            if (cookie != "")
            {
                return cookie;
            }

            throw new Exception("Failed to log in to torrent webAPI");
        }
    }
}