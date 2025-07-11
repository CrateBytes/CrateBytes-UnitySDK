using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;

namespace CrateBytes
{
    /// <summary>
    /// Base HTTP service class for CrateBytes API calls
    /// </summary>
    public abstract class CrateBytesHttpService
    {
        protected readonly CrateBytesSDK _sdk;
        protected string _authToken;

        protected CrateBytesHttpService(CrateBytesSDK sdk)
        {
            _sdk = sdk;
        }

        /// <summary>
        /// Set the authentication token for API calls
        /// </summary>
        public void SetAuthToken(string token)
        {
            _authToken = token;
            Debug.Log($"[CrateBytes] Auth token set: {(string.IsNullOrEmpty(token) ? "null" : "valid")}");
        }

        /// <summary>
        /// Clear the authentication token
        /// </summary>
        public void ClearAuthToken()
        {
            _authToken = null;
        }

        /// <summary>
        /// Make a GET request to the API
        /// </summary>
        protected IEnumerator GetRequest<T>(string endpoint, Action<CrateBytesResponse<T>> callback)
        {
            string url = $"{_sdk.BaseUrl}{endpoint}";
            
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                if (!string.IsNullOrEmpty(_authToken))
                {
                    request.SetRequestHeader("Authorization", $"Bearer {_authToken}");
                }

                yield return request.SendWebRequest();

                HandleResponse(request, callback);
            }
        }

        /// <summary>
        /// Make a GET request to the API with explicit type
        /// </summary>
        protected IEnumerator GetRequest(string endpoint, Action<CrateBytesResponse<LeaderboardResponse>> callback)
        {
            return GetRequest<LeaderboardResponse>(endpoint, callback);
        }

        /// <summary>
        /// Make a GET request to the API that returns a string
        /// </summary>
        protected IEnumerator GetRequest(string endpoint, Action<CrateBytesResponse<string>> callback)
        {
            return GetRequest<string>(endpoint, callback);
        }

        /// <summary>
        /// Make a POST request to the API
        /// </summary>
        protected IEnumerator PostRequest<T>(string endpoint, object data, Action<CrateBytesResponse<T>> callback)
        {
            string url = $"{_sdk.BaseUrl}{endpoint}";
            string jsonData = JsonConvert.SerializeObject(data);
            
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                if (!string.IsNullOrEmpty(_authToken))
                {
                    request.SetRequestHeader("Authorization", $"Bearer {_authToken}");
                }

                yield return request.SendWebRequest();

                HandleResponse(request, callback);
            }
        }

        /// <summary>
        /// Make a POST request to the API with explicit type
        /// </summary>
        protected IEnumerator PostRequest(string endpoint, object data, Action<CrateBytesResponse<AuthResponse>> callback)
        {
            return PostRequest<AuthResponse>(endpoint, data, callback);
        }

        /// <summary>
        /// Make a POST request to the API with explicit type
        /// </summary>
        protected IEnumerator PostRequest(string endpoint, object data, Action<CrateBytesResponse<SessionData>> callback)
        {
            return PostRequest<SessionData>(endpoint, data, callback);
        }

        /// <summary>
        /// Make a POST request to the API with explicit type
        /// </summary>
        protected IEnumerator PostRequest(string endpoint, object data, Action<CrateBytesResponse<ScoreSubmissionResponse>> callback)
        {
            return PostRequest<ScoreSubmissionResponse>(endpoint, data, callback);
        }

        /// <summary>
        /// Make a POST request to the API with explicit type
        /// </summary>
        protected IEnumerator PostRequest(string endpoint, object data, Action<CrateBytesResponse<MetadataResponse>> callback)
        {
            return PostRequest<MetadataResponse>(endpoint, data, callback);
        }



        /// <summary>
        /// Make a DELETE request to the API
        /// </summary>
        protected IEnumerator DeleteRequest<T>(string endpoint, Action<CrateBytesResponse<T>> callback)
        {
            string url = $"{_sdk.BaseUrl}{endpoint}";
            
            using (UnityWebRequest request = UnityWebRequest.Delete(url))
            {
                if (!string.IsNullOrEmpty(_authToken))
                {
                    request.SetRequestHeader("Authorization", $"Bearer {_authToken}");
                }

                yield return request.SendWebRequest();

                HandleResponse(request, callback);
            }
        }

        private void HandleResponse<T>(UnityWebRequest request, Action<CrateBytesResponse<T>> callback)
        {
            CrateBytesResponse<T> response = new CrateBytesResponse<T>();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    response = JsonConvert.DeserializeObject<CrateBytesResponse<T>>(request.downloadHandler.text);
                    response.Success = true;
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Error = new CrateBytesError { Message = $"Failed to parse response: {ex.Message}" };
                }
            }
            else
            {
                response.Success = false;
                response.StatusCode = (int)request.responseCode;
                
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<CrateBytesErrorResponse>(request.downloadHandler.text);
                    response.Error = errorResponse?.Error ?? new CrateBytesError { Message = request.error };
                }
                catch
                {
                    response.Error = new CrateBytesError { Message = request.error };
                }
            }

            callback?.Invoke(response);
        }
    }

    /// <summary>
    /// Generic response wrapper for CrateBytes API calls
    /// </summary>
    [Serializable]
    public class CrateBytesResponse<T>
    {
        public int StatusCode { get; set; }
        public T Data { get; set; }
        public CrateBytesError Error { get; set; }
        public bool Success { get; set; }
    }

    /// <summary>
    /// Error response wrapper
    /// </summary>
    [Serializable]
    public class CrateBytesErrorResponse
    {
        public int StatusCode { get; set; }
        public CrateBytesError Error { get; set; }
    }

    /// <summary>
    /// Error information
    /// </summary>
    [Serializable]
    public class CrateBytesError
    {
        public string Message { get; set; }
    }

    /// <summary>
    /// Authentication response data
    /// </summary>
    [Serializable]
    public class AuthResponse
    {
        public string token { get; set; }
        public string playerId { get; set; }
        public int sequentialId { get; set; }
        public string steamId { get; set; }
    }

    /// <summary>
    /// Session data structure
    /// </summary>
    [Serializable]
    public class SessionData
    {
        public string id { get; set; }
        public string playerId { get; set; }
        public DateTime startTime { get; set; }
        public DateTime lastHeartbeat { get; set; }
        public DateTime? endTime { get; set; }
    }

    /// <summary>
    /// Score submission response data
    /// </summary>
    [Serializable]
    public class ScoreSubmissionResponse
    {
        public string message { get; set; }
    }



    /// <summary>
    /// Leaderboard response data
    /// </summary>
    [Serializable]
    public class LeaderboardResponse
    {
        public LeaderboardInfo leaderboard { get; set; }
        public LeaderboardEntry[] entries { get; set; }
        public int totalEntries { get; set; }
        public int pages { get; set; }
    }

    /// <summary>
    /// Leaderboard information
    /// </summary>
    [Serializable]
    public class LeaderboardInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    /// <summary>
    /// Leaderboard entry data
    /// </summary>
    [Serializable]
    public class LeaderboardEntry
    {
        public PlayerInfo player { get; set; }
        public string score { get; set; }
    }

    /// <summary>
    /// Player information
    /// </summary>
    [Serializable]
    public class PlayerInfo
    {
        public string playerId { get; set; }
        public bool guest { get; set; }
        public EntryDataInfo entryData { get; set; }
    }

    /// <summary>
    /// Entry data information
    /// </summary>
    [Serializable]
    public class EntryDataInfo
    {
        public string data { get; set; }
    }

    /// <summary>
    /// Metadata response data
    /// </summary>
    [Serializable]
    public class MetadataResponse
    {
        public string data { get; set; }
    }
} 