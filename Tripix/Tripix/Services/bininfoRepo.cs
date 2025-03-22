using Newtonsoft.Json;

namespace Tripix.Services
{
    public class bininfoRepo
    {
        private readonly HttpClient _httpClient;

        public bininfoRepo ( HttpClient httpClient )
        {
            _httpClient = httpClient;
        }
        public async Task<BinInfo> GetBinInfo ( string bin )
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://lookup.binlist.net/{bin}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"BIN Lookup API Error: {response.StatusCode}");
                    return null;
                }

                string json = await response.Content.ReadAsStringAsync();

                // ✅ طباعة الاستجابة للتحقق منها
                Console.WriteLine($"API Response: {json}");

                if (string.IsNullOrEmpty(json) || !json.Trim().StartsWith("{"))
                {
                    Console.WriteLine("Invalid JSON response received.");
                    return null;
                }

                return JsonConvert.DeserializeObject<BinInfo>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetBinInfo: {ex.Message}");
                return null;
            }
        }

    }
}
