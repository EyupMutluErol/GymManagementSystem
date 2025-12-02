using GymManagementSystem.Business.Abstract;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace GymManagementSystem.Business.Concrete
{
    public class GeminiApiService : IAIService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public GeminiApiService(IConfiguration configuration)
        {
            _apiKey = configuration["GeminiApiKey"];
            _httpClient = new HttpClient();
        }

        public async Task<string> GetGymWorkoutPlanAsync(string promptText)
        {
            // 1. API Anahtarı Kontrolü
            if (string.IsNullOrEmpty(_apiKey))
            {
                return "HATA: API Anahtarı (GeminiApiKey) bulunamadı veya boş. Lütfen appsettings.json dosyasını kontrol edin.";
            }

            // 2. Model ve URL (En güncel ve kararlı model)
            var modelId = "gemini-2.0-flash";
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelId}:generateContent?key={_apiKey}";

            // 3. İstek Gövdesi (JSON)
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = promptText }
                        }
                    }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                // 4. İsteği Gönder
                var response = await _httpClient.PostAsync(url, jsonContent);

                // 5. Cevabı Oku
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var resultNode = JsonNode.Parse(responseString);
                    var textResponse = resultNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
                    return textResponse ?? "AI boş cevap döndürdü.";
                }
                else
                {
                    
                    return $"GOOGLE HATA VERDİ ({response.StatusCode}): {responseString}";
                }
            }
            catch (Exception ex)
            {
                return $"SİSTEM HATASI: {ex.Message}";
            }
        }
        public async Task<string> CheckAvailableModelsAsync()
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models?key={_apiKey}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();
                return responseString; // Tüm model listesini JSON olarak döndürür
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }
    }
}