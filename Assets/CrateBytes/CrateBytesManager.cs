using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrateBytes {
    public class CrateBytesManager : MonoBehaviour {
        /// <summary>
        /// Singleton instance of the CrateBytesManager.
        /// </summary>
        public static CrateBytesManager instance { get; private set; }

        /// <summary>
        /// API URL for CrateBytes.
        /// </summary>
        public string ApiUrl = "https://cratebytes.com";

        /// <summary>
        /// Unique project key for authentication.
        /// </summary>
        public string ProjectKey = "";

        private string token;

        private void Awake() {
            // Ensure only one instance exists.
            if (instance != null) {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start() {
            // Validate the project key.
            if (string.IsNullOrEmpty(ProjectKey)) {
                Debug.LogError("CrateBytes: Settings is not set.");
                Destroy(this);
                return;
            }
        }

        #region Authentication

        /// <summary>
        /// Authenticates a guest user with an optional player ID.
        /// </summary>
        /// <param name="playerId">Optional player ID for guest login.</param>
        /// <param name="callback">Callback for the guest login response.</param>
        public void GuestLogin(string playerId = "", System.Action<GuestLoginResponse> callback = null) {
            GuestLoginRequest request = new GuestLoginRequest {
                projectKey = ProjectKey,
            };

            if (!string.IsNullOrEmpty(playerId)) request.playerId = playerId;

            StartCoroutine(PostRequest(ApiUrl + "/api/guestLogin", request.ToString(), (response) => {
                try {
                    GuestLoginResponse guestLoginResponse = JsonUtility.FromJson<GuestLoginResponse>(response);
                    token = guestLoginResponse.token;
                    callback?.Invoke(guestLoginResponse);
                } catch (System.Exception) {
                    Debug.LogError("CrateBytes: Guest login failed.");
                    throw new System.Exception("CrateBytes: Guest login failed.");
                }
            }));
        }

        /// <summary>
        /// Authenticates a user via Steam using a Steam Auth Ticket.
        /// </summary>
        /// <param name="steamAuthTicket">The Steam authentication ticket.</param>
        /// <param name="callback">Callback for the Steam login response.</param>
        public void SteamLogin(string steamAuthTicket, System.Action<SteamLoginResponse> callback = null) {
            SteamLoginRequest request = new SteamLoginRequest {
                projectKey = ProjectKey,
                steamAuthTicket = steamAuthTicket,
            };

            StartCoroutine(PostRequest(ApiUrl + "/api/steamLogin", request.ToString(), (response) => {
                try {
                    SteamLoginResponse steamLoginResponse = JsonUtility.FromJson<SteamLoginResponse>(response);
                    token = steamLoginResponse.token;
                    callback?.Invoke(steamLoginResponse);
                } catch (System.Exception) {
                    Debug.LogError("CrateBytes: Steam login failed.");
                    throw new System.Exception("CrateBytes: Steam login failed.");
                }
            }));
        }
        #endregion

        #region Sessions

        /// <summary>
        /// Initiates a game session.
        /// </summary>
        /// <param name="callback">Callback for the session start response.</param>
        public void StartSession(System.Action<string> callback = null) {
            if (string.IsNullOrEmpty(token)) {
                Debug.LogError("CrateBytes: Token is not set. Please login first.");
                throw new System.Exception("CrateBytes: Token is not set. Please login first.");
            }

            StartCoroutine(PostRequest(ApiUrl + "/api/game/session/start", "", (response) => {
                callback?.Invoke(response);
            }, token));
        }

        /// <summary>
        /// Sends a heartbeat to keep the session alive.
        /// </summary>
        /// <param name="callback">Callback for the session heartbeat response.</param>
        public void HeartbeatSession(System.Action<string> callback = null) {
            if (string.IsNullOrEmpty(token)) {
                Debug.LogError("CrateBytes: Token is not set. Please login first.");
                throw new System.Exception("CrateBytes: Token is not set. Please login first.");
            }

            StartCoroutine(PostRequest(ApiUrl + "/api/game/session/heartbeat", "", (response) => {
                callback?.Invoke(response);
            }, token));
        }

        /// <summary>
        /// Ends the current game session.
        /// </summary>
        /// <param name="callback">Callback for the session end response.</param>
        public void EndSession(System.Action<string> callback = null) {
            if (string.IsNullOrEmpty(token)) {
                Debug.LogError("CrateBytes: Token is not set. Please login first.");
                throw new System.Exception("CrateBytes: Token is not set. Please login first.");
            }

            StartCoroutine(PostRequest(ApiUrl + "/api/game/session/end", "", (response) => {
                callback?.Invoke(response);
            }, token));
        }
        #endregion

        #region Leaderboards

        /// <summary>
        /// Submits a score to a specified leaderboard.
        /// </summary>
        /// <param name="leaderboardId">ID of the leaderboard.</param>
        /// <param name="score">Score to submit.</param>
        /// <param name="callback">Callback for the score submission response.</param>
        public void SubmitScoreToLeaderboard(string leaderboardId, int score, System.Action<string> callback = null) {
            if (string.IsNullOrEmpty(token)) {
                Debug.LogError("CrateBytes: Token is not set. Please login first.");
                throw new System.Exception("CrateBytes: Token is not set. Please login first.");
            }

            LeaderboardSubmitRequest request = new LeaderboardSubmitRequest {
                score = score
            };

            StartCoroutine(PostRequest(ApiUrl + $"/api/game/leaderboards/{leaderboardId}/submit", request.ToString(), (response) => {
                callback?.Invoke(response);
            }, token));
        }

        /// <summary>
        /// Retrieves leaderboard data.
        /// </summary>
        /// <param name="leaderboardId">ID of the leaderboard.</param>
        /// <param name="page">Leaderboard page to retrieve.</param>
        /// <param name="callback">Callback for the leaderboard data response.</param>
        public void GetLeaderboard(string leaderboardId, int page = 0, System.Action<LeaderboardResponse> callback = null) {
            if (string.IsNullOrEmpty(token)) {
                Debug.LogError("CrateBytes: Token is not set. Please login first.");
                throw new System.Exception("CrateBytes: Token is not set. Please login first.");
            }

            StartCoroutine(GetRequest(ApiUrl + $"/api/leaderboards/{leaderboardId}?page={page}", (response) => {
                try {
                    LeaderboardResponse leaderboardResponse = JsonUtility.FromJson<LeaderboardResponse>(response);
                    callback?.Invoke(leaderboardResponse);
                } catch (System.Exception) {
                    Debug.LogError("CrateBytes: Get leaderboard failed.");
                    throw new System.Exception("CrateBytes: Get leaderboard failed.");
                }
            }, token));
        }
        #endregion

        #region Metadata

        /// <summary>
        /// Retrieves game metadata.
        /// </summary>
        /// <param name="callback">Callback for the metadata retrieval response.</param>
        public void GetMetadata(System.Action<GetMetadataResponse> callback = null) {
            if (string.IsNullOrEmpty(token)) {
                Debug.LogError("CrateBytes: Token is not set. Please login first.");
                throw new System.Exception("CrateBytes: Token is not set. Please login first.");
            }

            StartCoroutine(GetRequest(ApiUrl + "/api/game/metadata/get", (response) => {
                try {
                    GetMetadataResponse getMetadataResponse = JsonUtility.FromJson<GetMetadataResponse>(response);
                    callback?.Invoke(getMetadataResponse);
                } catch (System.Exception) {
                    Debug.LogError("CrateBytes: Get metadata failed.");
                    throw new System.Exception("CrateBytes: Get metadata failed.");
                }
            }, token));
        }

        /// <summary>
        /// Adds or updates game metadata.
        /// </summary>
        /// <param name="data">Metadata to add or update.</param>
        /// <param name="callback">Callback for the add/update metadata response.</param>
        public void AddUpdateMetadata(string data, System.Action<AddUpdateMetadataResponse> callback = null) {
            if (string.IsNullOrEmpty(token)) {
                Debug.LogError("CrateBytes: Token is not set. Please login first.");
                throw new System.Exception("CrateBytes: Token is not set. Please login first.");
            }

            AddUpdateMetadataRequest request = new AddUpdateMetadataRequest {
                data = data
            };
            
            StartCoroutine(PutRequest(ApiUrl + "/api/game/metadata/add", request.ToString(), (response) => {
                try {
                    AddUpdateMetadataResponse addUpdateMetadataResponse = JsonUtility.FromJson<AddUpdateMetadataResponse>(response);
                    callback?.Invoke(addUpdateMetadataResponse);
                } catch (System.Exception) {
                    Debug.LogError("CrateBytes: Add/Update metadata failed.");
                    throw new System.Exception("CrateBytes: Add/Update metadata failed.");
                }
            }, token));
        }

        /// <summary>
        /// Deletes game metadata.
        /// </summary>
        /// <param name="callback">Callback for the metadata deletion response.</param>
        public void DeleteMetadata(System.Action<string> callback = null) {
            if (string.IsNullOrEmpty(token)) {
                Debug.LogError("CrateBytes: Token is not set. Please login first.");
                throw new System.Exception("CrateBytes: Token is not set. Please login first.");
            }

            StartCoroutine(DeleteRequest(ApiUrl + "/api/game/metadata/delete", (response) => {
                callback?.Invoke(response);
            }, token));
        }
        #endregion

        #region Utils

        /// <summary>
        /// Sends a GET request.
        /// </summary>
        private IEnumerator GetRequest(string uri, System.Action<string> callback, string bearerToken = null) {
            Debug.Log(uri);

            using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Get(uri)) {
                if (!string.IsNullOrEmpty(bearerToken)) webRequest.SetRequestHeader("Authorization", "Bearer " + bearerToken);
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError || webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError) {
                    Debug.LogError(webRequest.error);
                } else {
                    callback?.Invoke(webRequest.downloadHandler.text);
                }
            }
        }

        /// <summary>
        /// Sends a POST request with JSON data.
        /// </summary>
        private IEnumerator PostRequest(string uri, string jsonData, System.Action<string> callback, string bearerToken = null) {
            using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Post(uri, jsonData)) {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                webRequest.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                if (!string.IsNullOrEmpty(bearerToken)) webRequest.SetRequestHeader("Authorization", "Bearer " + bearerToken);

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError || webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError) {
                    Debug.LogError(webRequest.error);
                } else {
                    callback?.Invoke(webRequest.downloadHandler.text);
                }
            }
        }

        /// <summary>
        /// Sends a PUT request with JSON data.
        /// </summary>
        private IEnumerator PutRequest(string uri, string jsonData, System.Action<string> callback, string bearerToken = null) {
            using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Put(uri, jsonData)) {
                webRequest.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                if (!string.IsNullOrEmpty(bearerToken)) webRequest.SetRequestHeader("Authorization", "Bearer " + bearerToken);
                
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError || webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError) {
                    Debug.LogError(webRequest.error);
                } else {
                    callback?.Invoke(webRequest.downloadHandler.text);
                }
            }
        }

        /// <summary>
        /// Sends a DELETE request.
        /// </summary>
        private IEnumerator DeleteRequest(string uri, System.Action<string> callback, string bearerToken = null) {
            using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Delete(uri)) {
                if (!string.IsNullOrEmpty(bearerToken)) webRequest.SetRequestHeader("Authorization", "Bearer " + bearerToken);
                
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError || webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError) {
                    Debug.LogError(webRequest.error);
                } else {
                    callback?.Invoke("Deleted");
                }
            }
        }
        #endregion
    }
}
