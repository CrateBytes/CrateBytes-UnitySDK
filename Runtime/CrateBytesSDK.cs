using UnityEngine;
using System;

namespace CrateBytes
{
    /// <summary>
    /// Main SDK class for CrateBytes integration
    /// </summary>
    public class CrateBytesSDK : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private string baseUrl = "https://api.cratebytes.com/api/game";
        [SerializeField] private string publicKey = "";
        [SerializeField] private float heartbeatInterval = 60f; // 1 minute
        [SerializeField] private float sessionTimeout = 300f; // 5 minutes
        [SerializeField] public bool enableLogging = false;

        private static CrateBytesSDK _instance;
        private CrateBytesAuthService _authService;
        private CrateBytesSessionService _sessionService;
        private CrateBytesLeaderboardService _leaderboardService;
        private CrateBytesMetadataService _metadataService;
        private Coroutine _heartbeatCoroutine;
        private string _authToken;

        public static CrateBytesSDK Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CrateBytesSDK>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("CrateBytesSDK");
                        _instance = go.AddComponent<CrateBytesSDK>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        public CrateBytesAuthService Auth => _authService;
        public CrateBytesSessionService Session => _sessionService;
        public CrateBytesLeaderboardService Leaderboard => _leaderboardService;
        public CrateBytesMetadataService Metadata => _metadataService;

        public string BaseUrl => baseUrl;
        public string PublicKey => publicKey;
        public float HeartbeatInterval => heartbeatInterval;
        public float SessionTimeout => sessionTimeout;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeServices();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializeServices()
        {
            _authService = new CrateBytesAuthService(this);
            _sessionService = new CrateBytesSessionService(this);
            _leaderboardService = new CrateBytesLeaderboardService(this);
            _metadataService = new CrateBytesMetadataService(this);
            
            // Try auto-login if there's saved auth data
            if (_authService.HasSavedAuthData())
            {
                _authService.TryAutoLogin();
                CrateBytesLogger.Log("[CrateBytes] Auto-login attempted with saved credentials");
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                StopHeartbeat();
                _instance = null;
            }
        }

        public void StartHeartbeat()
        {
            StopHeartbeat();
            _heartbeatCoroutine = StartCoroutine(_sessionService.HeartbeatCoroutine());
        }

        public void StopHeartbeat()
        {
            if (_heartbeatCoroutine != null)
            {
                StopCoroutine(_heartbeatCoroutine);
                _heartbeatCoroutine = null;
            }
        }

        /// <summary>
        /// Initialize the SDK with custom configuration
        /// </summary>
        public void Initialize(string baseUrl, string publicKey)
        {
            this.baseUrl = baseUrl;
            this.publicKey = publicKey;
        }

        /// <summary>
        /// Check if the SDK is properly configured
        /// </summary>
        public bool IsConfigured()
        {
            return !string.IsNullOrEmpty(baseUrl) && !string.IsNullOrEmpty(publicKey);
        }

        /// <summary>
        /// Set the authentication token for all services
        /// </summary>
        public void SetAuthToken(string token)
        {
            _authToken = token;
            _authService?.SetAuthToken(token);
            _sessionService?.SetAuthToken(token);
            _leaderboardService?.SetAuthToken(token);
            _metadataService?.SetAuthToken(token);
            CrateBytesLogger.Log($"[CrateBytes] Auth token set for all services: {(string.IsNullOrEmpty(token) ? "null" : "valid")}");
        }

        /// <summary>
        /// Clear the authentication token from all services
        /// </summary>
        public void ClearAuthToken()
        {
            _authToken = null;
            _authService?.ClearAuthToken();
            _sessionService?.ClearAuthToken();
            _leaderboardService?.ClearAuthToken();
            _metadataService?.ClearAuthToken();
            CrateBytesLogger.Log("[CrateBytes] Auth token cleared from all services");
        }

        /// <summary>
        /// Get the current authentication token
        /// </summary>
        public string GetAuthToken()
        {
            return _authToken;
        }

        /// <summary>
        /// Check if user is currently authenticated
        /// </summary>
        public bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(_authToken);
        }
    }

    /// <summary>
    /// SDK-wide logger for CrateBytes
    /// </summary>
    public static class CrateBytesLogger
    {
        public static bool Enabled => CrateBytesSDK.Instance?.enableLogging ?? false;

        public static void Log(object message)
        {
            if (Enabled)
                UnityEngine.Debug.Log(message);
        }

        public static void LogWarning(object message)
        {
            if (Enabled)
                UnityEngine.Debug.LogWarning(message);
        }

        public static void LogError(object message)
        {
            if (Enabled)
                UnityEngine.Debug.LogError(message);
        }
    }
} 