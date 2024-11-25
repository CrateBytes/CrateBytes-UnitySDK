using System.Net.Http;
using System.Threading.Tasks;
using System;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;


namespace CrateBytes
{
    public static class CrateBytesAPI
    {
        public static HttpClient client = new HttpClient();

        private static string contentType = "application/json";
        private static Encoding encoding = Encoding.UTF8;

        public static async Task<T> CallAPI<T>(string endpoint, HttpMethod httpMethod, string jsonString = null, Dictionary<string, string> headers = null) where T : BaseResponse, new()
        {
            try
            {

                var settings = CrateBytesSettingsBootstrap.Instance;

                var request = new HttpRequestMessage();

                request.Method = httpMethod;

                if (httpMethod != HttpMethod.Get && !string.IsNullOrEmpty(jsonString))
                {
                    request.Content = new StringContent(jsonString, encoding, contentType);
                }

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                request.RequestUri = new Uri(settings.DomainURL + endpoint);

                HttpResponseMessage httpResponse = client.SendAsync(request).Result;

                if (httpResponse.IsSuccessStatusCode)
                {
                    string responseBody = await httpResponse.Content.ReadAsStringAsync();

                    JObject json = JObject.Parse(responseBody);

                    json["data"]["statusCode"] = json["statusCode"];
                    json["data"]["error"] = json["error"];

                    var response = JsonConvert.DeserializeObject<T>(json.ToString());

                    return response;
                }
                else
                {
                    string result = await request.Content.ReadAsStringAsync();
                    JObject response = JObject.Parse(result);
                    response["data"]["error"] = response["error"];

                    Debug.LogWarning($"Error: {response.ToString()}");

                    var returnResponse = JsonConvert.DeserializeObject<T>(response.ToString());

                    return returnResponse;
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.LogError($"HTTP Request failed: {ex.Message}");
                var exception = new CrateBytesError
                {
                    message = $"HTTP Request failed: {ex.Message}"
                };

                return new T { error = exception };
            }
            catch (JsonException ex)
            {
                Debug.LogError($"JSON Parsing failed: {ex.Message}");
                var exception = new CrateBytesError
                {
                    message = $"JSON Parsing failed: {ex.Message}"
                };

                return new T { error = exception };
            }
            catch (TaskCanceledException ex)
            {
                Debug.LogError($"Request timeout or task canceled: {ex.Message}");
                var exception = new CrateBytesError
                {
                    message = $"Request timeout or task canceled: {ex.Message}"
                };

                return new T { error = exception };
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error: {ex.Message}");
                var exception = new CrateBytesError
                {
                    message = $"Unexpected error: {ex.Message}"
                };

                return new T { error = exception };
            }
        }

    }
}