using UnityEngine;
using System;
using System.Collections;

namespace CrateBytes
{
    /// <summary>
    /// Session management service for CrateBytes API
    /// </summary>
    public class CrateBytesSessionService : CrateBytesHttpService
    {
        private bool _isSessionActive = false;
        private SessionData _currentSession;

        public CrateBytesSessionService(CrateBytesSDK sdk) : base(sdk) { }

        /// <summary>
        /// Start a new session
        /// </summary>
        public IEnumerator StartSession(Action<CrateBytesResponse<SessionData>> callback = null)
        {
            yield return PostRequest("/session/start", new { }, (response) =>
            {
                if (response.Success && response.Data != null)
                {
                    _currentSession = response.Data;
                    _isSessionActive = true;
                }
                callback?.Invoke(response);
            });
        }

        /// <summary>
        /// Send heartbeat to keep session alive
        /// </summary>
        public IEnumerator Heartbeat(Action<CrateBytesResponse<SessionData>> callback = null)
        {
            yield return PostRequest("/session/heartbeat", new { }, (response) =>
            {
                if (response.Success && response.Data != null)
                {
                    _currentSession = response.Data;
                }
                callback?.Invoke(response);
            });
        }

        /// <summary>
        /// Stop the current session
        /// </summary>
        public IEnumerator StopSession(Action<CrateBytesResponse<SessionData>> callback = null)
        {
            yield return PostRequest("/session/stop", new { }, (response) =>
            {
                if (response.Success)
                {
                    _isSessionActive = false;
                    _currentSession = null;
                }
                callback?.Invoke(response);
            });
        }

        /// <summary>
        /// Coroutine for automatic heartbeat
        /// </summary>
        public IEnumerator HeartbeatCoroutine()
        {
            while (_isSessionActive)
            {
                yield return new WaitForSeconds(_sdk.HeartbeatInterval);
                
                if (_isSessionActive)
                {
                    yield return Heartbeat((response) =>
                    {
                        if (!response.Success)
                        {
                            CrateBytesLogger.LogWarning($"Heartbeat failed: {response.Error?.Message}");
                            _isSessionActive = false;
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Check if session is currently active
        /// </summary>
        public bool IsSessionActive()
        {
            return _isSessionActive;
        }

        /// <summary>
        /// Get current session data
        /// </summary>
        public SessionData GetCurrentSession()
        {
            return _currentSession;
        }

        /// <summary>
        /// Force stop session (local only, doesn't call API)
        /// </summary>
        public void ForceStopSession()
        {
            _isSessionActive = false;
            _currentSession = null;
        }
    }


} 