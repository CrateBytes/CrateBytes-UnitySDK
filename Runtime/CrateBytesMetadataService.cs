using UnityEngine;
using System;
using System.Collections;

namespace CrateBytes
{
    /// <summary>
    /// Metadata service for CrateBytes API
    /// </summary>
    public class CrateBytesMetadataService : CrateBytesHttpService
    {
        public CrateBytesMetadataService(CrateBytesSDK sdk) : base(sdk) { }

        /// <summary>
        /// Get current player's metadata as a string
        /// </summary>
        public IEnumerator GetPlayerData(Action<CrateBytesResponse<string>> callback = null)
        {
            yield return GetRequest("/metadata", callback);
        }

        /// <summary>
        /// Get player data by sequential ID as a string
        /// </summary>
        public IEnumerator GetPlayerDataBySequentialId(int sequentialId, Action<CrateBytesResponse<string>> callback = null)
        {
            string endpoint = $"/metadata/{sequentialId}";
            
            yield return GetRequest(endpoint, callback);
        }

        /// <summary>
        /// Set current player's metadata
        /// </summary>
        public IEnumerator SetPlayerData(string data, Action<CrateBytesResponse<string>> callback = null)
        {
            var requestData = new PlayerDataRequest
            {
                data = data
            };

            yield return PostRequest("/metadata", requestData, (response) =>
            {
                if (response.Success && response.Data != null)
                {
                    // Extract the data string from the nested response
                    var stringResponse = new CrateBytesResponse<string>
                    {
                        Success = response.Success,
                        StatusCode = response.StatusCode,
                        Error = response.Error,
                        Data = response.Data.data
                    };
                    callback?.Invoke(stringResponse);
                }
                else
                {
                    var errorResponse = new CrateBytesResponse<string>
                    {
                        Success = response.Success,
                        StatusCode = response.StatusCode,
                        Error = response.Error
                    };
                    callback?.Invoke(errorResponse);
                }
            });
        }

        /// <summary>
        /// Set current player's metadata using a JSON object
        /// </summary>
        public IEnumerator SetPlayerDataObject(object data, Action<CrateBytesResponse<string>> callback = null)
        {
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            return SetPlayerData(jsonData, callback);
        }

        /// <summary>
        /// Delete current player's metadata
        /// </summary>
        public IEnumerator DeletePlayerData(Action<CrateBytesResponse<string>> callback = null)
        {
            yield return DeleteRequest("/metadata", callback);
        }

        /// <summary>
        /// Get player data as a specific type
        /// </summary>
        public IEnumerator GetPlayerData<T>(Action<CrateBytesResponse<T>> callback = null)
        {
            yield return GetRequest("/metadata", (response) =>
            {
                if (response.Success && response.Data != null)
                {
                    try
                    {
                        var typedResponse = new CrateBytesResponse<T>
                        {
                            Success = response.Success,
                            StatusCode = response.StatusCode,
                            Error = response.Error
                        };

                        // The API returns data as a direct JSON string
                        string rawData = response.Data;
                        CrateBytesLogger.Log($"[CrateBytes] Raw metadata string: {rawData}");

                        // Parse the JSON string directly to the target type
                        typedResponse.Data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(rawData);

                        callback?.Invoke(typedResponse);
                    }
                    catch (Exception ex)
                    {
                        CrateBytesLogger.LogError($"[CrateBytes] Deserialization error: {ex.Message}");
                        var errorResponse = new CrateBytesResponse<T>
                        {
                            Success = false,
                            Error = new CrateBytesError { Message = $"Failed to deserialize data: {ex.Message}" }
                        };
                        callback?.Invoke(errorResponse);
                    }
                }
                else
                {
                    var errorResponse = new CrateBytesResponse<T>
                    {
                        Success = response.Success,
                        StatusCode = response.StatusCode,
                        Error = response.Error
                    };
                    callback?.Invoke(errorResponse);
                }
            });
        }
    }

    /// <summary>
    /// Player data request
    /// </summary>
    [Serializable]
    public class PlayerDataRequest
    {
        public string data { get; set; }
    }
} 