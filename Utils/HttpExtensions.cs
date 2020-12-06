using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CV.Ads_Client.Utils
{
    public static class HttpExtensions
    {
        public static HttpRequestMessage AddJsonContent<T>(this HttpRequestMessage request, T content)
        {
            request.Content = new StringContent(
                JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
            return request;
        }

        public static HttpRequestMessage AddFormFileContent(this HttpRequestMessage request, FileStream file)
        {
            request.Content = new MultipartFormDataContent
            {
                { new StreamContent(file), "formFile", file.Name }
            };
            return request;
        }

        public static HttpRequestMessage SetAuthorizationHeader(this HttpRequestMessage request, string bearerToken)
        {
            request.Headers.Add("Authorization", "Bearer " + bearerToken);
            return request;
        }

        public static Task<T> ReadResponseAsync<T>(this HttpResponseMessage response)
        {
            var defaultJsonNamingPolicy = JsonNamingPolicy.CamelCase;
            return response.ReadResponseAsync<T>(defaultJsonNamingPolicy);
        }

        public static async Task<T> ReadResponseAsync<T>(this HttpResponseMessage response, JsonNamingPolicy jsonNamingPolicy)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<T>(
                responseContent, new JsonSerializerOptions() { PropertyNamingPolicy = jsonNamingPolicy });
            return responseData;
        }

        public static async Task<string> SaveResponseFileAsync(this HttpResponseMessage response, string pathToSaveFile)
        {
            var fileBytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync(pathToSaveFile, fileBytes);

            return pathToSaveFile;
        }
    }
}
