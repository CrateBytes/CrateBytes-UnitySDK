using UnityEngine;
using System;
using System.Collections;

namespace CrateBytes
{
    /// <summary>
    /// Authentication service for CrateBytes API
    /// </summary>
    public class CrateBytesAuthService : CrateBytesHttpService
    {
        private const string PLAYER_ID_KEY = "CrateBytes_PlayerId";
        private const string AUTH_TOKEN_KEY = "CrateBytes_AuthToken";
        private const string SEQUENTIAL_ID_KEY = "CrateBytes_SequentialId";

        public CrateBytesAuthService(CrateBytesSDK sdk) : base(sdk) 
        {
            // Load saved authentication data on initialization
            LoadSavedAuthData();
        }

        /// <summary>
        /// Load saved authentication data from PlayerPrefs
        /// </summary>
        private void LoadSavedAuthData()
        {
            string savedPlayerId = PlayerPrefs.GetString(PLAYER_ID_KEY, "");
            string savedAuthToken = PlayerPrefs.GetString(AUTH_TOKEN_KEY, "");
            
            if (!string.IsNullOrEmpty(savedPlayerId) && !string.IsNullOrEmpty(savedAuthToken))
            {
                _sdk.SetAuthToken(savedAuthToken);
                CrateBytesLogger.Log($"[CrateBytes] Loaded saved auth data for player: {savedPlayerId}");
            }
        }

        /// <summary>
        /// Save authentication data to PlayerPrefs
        /// </summary>
        private void SaveAuthData(string playerId, string authToken, int sequentialId)
        {
            PlayerPrefs.SetString(PLAYER_ID_KEY, playerId);
            PlayerPrefs.SetString(AUTH_TOKEN_KEY, authToken);
            PlayerPrefs.SetInt(SEQUENTIAL_ID_KEY, sequentialId);
            PlayerPrefs.Save();
            
            CrateBytesLogger.Log($"[CrateBytes] Saved auth data for player: {playerId}");
        }

        /// <summary>
        /// Clear saved authentication data from PlayerPrefs
        /// </summary>
        private void ClearSavedAuthData()
        {
            PlayerPrefs.DeleteKey(PLAYER_ID_KEY);
            PlayerPrefs.DeleteKey(AUTH_TOKEN_KEY);
            PlayerPrefs.DeleteKey(SEQUENTIAL_ID_KEY);
            PlayerPrefs.Save();
            
            CrateBytesLogger.Log("[CrateBytes] Cleared saved auth data");
        }

        /// <summary>
        /// Get saved player ID from PlayerPrefs
        /// </summary>
        public string GetSavedPlayerId()
        {
            return PlayerPrefs.GetString(PLAYER_ID_KEY, "");
        }

        /// <summary>
        /// Get saved sequential ID from PlayerPrefs
        /// </summary>
        public int GetSavedSequentialId()
        {
            return PlayerPrefs.GetInt(SEQUENTIAL_ID_KEY, 0);
        }

        /// <summary>
        /// Check if there's saved authentication data
        /// </summary>
        public bool HasSavedAuthData()
        {
            string savedPlayerId = PlayerPrefs.GetString(PLAYER_ID_KEY, "");
            string savedAuthToken = PlayerPrefs.GetString(AUTH_TOKEN_KEY, "");
            return !string.IsNullOrEmpty(savedPlayerId) && !string.IsNullOrEmpty(savedAuthToken);
        }

        /// <summary>
        /// Auto-login using saved authentication data
        /// </summary>
        public bool TryAutoLogin()
        {
            if (HasSavedAuthData())
            {
                LoadSavedAuthData();
                return IsAuthenticated();
            }
            return false;
        }

        /// <summary>
        /// Authenticate as a guest user
        /// </summary>
        public IEnumerator GuestLogin(string playerId = null, Action<CrateBytesResponse<AuthResponse>> callback = null)
        {
            // Use saved player ID if none provided
            if (string.IsNullOrEmpty(playerId))
            {
                playerId = GetSavedPlayerId();
            }

            var requestData = new GuestLoginRequest
            {
                publicKey = _sdk.PublicKey,
                playerId = playerId
            };

            yield return PostRequest("/auth/guest", requestData, (response) =>
            {
                if (response.Success && response.Data != null)
                {
                    _sdk.SetAuthToken(response.Data.token);
                    // Save authentication data
                    SaveAuthData(response.Data.playerId, response.Data.token, response.Data.sequentialId);
                }
                callback?.Invoke(response);
            });
        }

        /// <summary>
        /// Authenticate using Steam
        /// </summary>
        public IEnumerator SteamLogin(string steamAuthTicket, Action<CrateBytesResponse<AuthResponse>> callback = null)
        {
            var requestData = new SteamLoginRequest
            {
                publicKey = _sdk.PublicKey,
                steamAuthTicket = steamAuthTicket
            };

            yield return PostRequest("/auth/steam", requestData, (response) =>
            {
                if (response.Success && response.Data != null)
                {
                    _sdk.SetAuthToken(response.Data.token);
                    // Save authentication data
                    SaveAuthData(response.Data.playerId, response.Data.token, response.Data.sequentialId);
                }
                callback?.Invoke(response);
            });
        }

        /// <summary>
        /// Check if user is currently authenticated
        /// </summary>
        public bool IsAuthenticated()
        {
            return _sdk.IsAuthenticated();
        }

        /// <summary>
        /// Get the current authentication token
        /// </summary>
        public string GetAuthToken()
        {
            return _sdk.GetAuthToken();
        }

        /// <summary>
        /// Debug method to check auth token status
        /// </summary>
        public void DebugAuthStatus()
        {
            CrateBytesLogger.Log($"[CrateBytes] Auth Status:");
            CrateBytesLogger.Log($"[CrateBytes] - IsAuthenticated: {IsAuthenticated()}");
            CrateBytesLogger.Log($"[CrateBytes] - HasSavedAuthData: {HasSavedAuthData()}");
            CrateBytesLogger.Log($"[CrateBytes] - Current Token: {(string.IsNullOrEmpty(_sdk.GetAuthToken()) ? "null" : "valid")}");
            CrateBytesLogger.Log($"[CrateBytes] - Saved Player ID: {GetSavedPlayerId()}");
        }

        /// <summary>
        /// Logout and clear authentication
        /// </summary>
        public void Logout()
        {
            _sdk.ClearAuthToken();
            ClearSavedAuthData();
        }
    }

    /// <summary>
    /// Guest login request data
    /// </summary>
    [Serializable]
    public class GuestLoginRequest
    {
        public string publicKey { get; set; }
        public string playerId { get; set; }
    }

    /// <summary>
    /// Steam login request data
    /// </summary>
    [Serializable]
    public class SteamLoginRequest
    {
        public string publicKey { get; set; }
        public string steamAuthTicket { get; set; }
    }
} 