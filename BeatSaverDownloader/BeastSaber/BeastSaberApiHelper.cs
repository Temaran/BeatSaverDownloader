﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
namespace BeatSaverDownloader.BeastSaber
{
    public static class BeastSaberApiHelper
    {
        private static HttpClient BeastSaberRequestClient;

        internal static void InitializeBeastSaberHttpClient(IPA.Loader.PluginMetadata metadata)
        {
            BeastSaberRequestClient = new HttpClient() { BaseAddress = new Uri("https://bsaber.com/wp-json/bsaber-api/") };
            BeastSaberRequestClient.DefaultRequestHeaders.Add("User-Agent", $"BeatSaverDownloader/{metadata.Version}");
        }

        public static async Task<BeastSaberApiResult> GetPage(Misc.Filters.BeastSaberFilterOptions filter, uint page, uint itemsPerPage, CancellationToken cancellationToken)
        {
            string apiUrl = "";
            try
            {
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                switch (filter)
                {
                    case Misc.Filters.BeastSaberFilterOptions.CuratorRecommended:
                        apiUrl = $"songs?bookmarked_by=curatorrecommended&page={page + 1}&count={itemsPerPage}";
                        break;
                }
                HttpResponseMessage response = await BeastSaberRequestClient.GetAsync(apiUrl, cancellationToken);
                response.EnsureSuccessStatusCode();
                Stream result = await response.Content.ReadAsStreamAsync();
                StreamReader reader = new StreamReader(result);
                Newtonsoft.Json.JsonReader jsonReader = new Newtonsoft.Json.JsonTextReader(reader);
                return serializer.Deserialize<BeastSaber.BeastSaberApiResult>(jsonReader);
            }
            catch (Exception ex)
            {
                Plugin.log.Error($"Failed to get BeastSaber api page: {ex}");
                return new BeastSaberApiResult { songs = new List<BeastSaberSong>(), next_page = -1 };
            }


        }
    }
}
