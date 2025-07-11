using UnityEngine;
using System.Collections;

namespace CrateBytes
{
    /// <summary>
    /// Simple test script for CrateBytes SDK
    /// </summary>
    public class CrateBytesTest : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private bool runTestOnStart = true;
        [SerializeField] private string testLeaderboardId = "test-leaderboard";

        private void Start()
        {
            if (runTestOnStart)
            {
                StartCoroutine(RunTest());
            }
        }

        /// <summary>
        /// Run a complete test of the SDK functionality
        /// </summary>
        public IEnumerator RunTest()
        {
            CrateBytesLogger.Log("=== CrateBytes SDK Test Started ===");

            // Test 1: Check SDK configuration
            if (!CrateBytesSDK.Instance.IsConfigured())
            {
                CrateBytesLogger.LogWarning("SDK not configured! Please set baseUrl and publicKey.");
                yield break;
            }

            CrateBytesLogger.Log("✓ SDK configured successfully");

            // Test 2: Guest Authentication
            CrateBytesLogger.Log("Testing guest authentication...");
            yield return CrateBytesSDK.Instance.Auth.GuestLogin(null, (response) =>
            {
                if (response.Success)
                {
                    CrateBytesLogger.Log($"✓ Guest authentication successful! Player ID: {response.Data.playerId}");
                }
                else
                {
                    CrateBytesLogger.LogWarning($"✗ Guest authentication failed: {response.Error?.Message}");
                }
            });

            // Test 3: Session Management
            if (CrateBytesSDK.Instance.Auth.IsAuthenticated())
            {
                CrateBytesLogger.Log("Testing session management...");
                yield return CrateBytesSDK.Instance.Session.StartSession((response) =>
                {
                    if (response.Success)
                    {
                        CrateBytesLogger.Log("✓ Session started successfully");
                        CrateBytesSDK.Instance.StartHeartbeat();
                    }
                    else
                    {
                        CrateBytesLogger.LogWarning($"✗ Session start failed: {response.Error?.Message}");
                    }
                });
            }

            // Test 4: Leaderboard Operations
            if (CrateBytesSDK.Instance.Auth.IsAuthenticated())
            {
                CrateBytesLogger.Log("Testing leaderboard operations...");
                
                // Submit a test score
                yield return CrateBytesSDK.Instance.Leaderboard.SubmitScore(testLeaderboardId, "1000", (response) =>
                {
                    if (response.Success)
                    {
                        CrateBytesLogger.Log("✓ Score submitted successfully");
                    }
                    else
                    {
                        CrateBytesLogger.LogWarning($"✗ Score submission failed: {response.Error?.Message}");
                    }
                });

                // Get leaderboard entries
                yield return CrateBytesSDK.Instance.Leaderboard.GetLeaderboard(testLeaderboardId, 1, (response) =>
                {
                    if (response.Success)
                    {
                        CrateBytesLogger.Log($"✓ Leaderboard retrieved! Total entries: {response.Data.totalEntries}");
                    }
                    else
                    {
                        CrateBytesLogger.LogWarning($"✗ Failed to get leaderboard: {response.Error?.Message}");
                    }
                });
            }

            // Test 5: Metadata Operations
            if (CrateBytesSDK.Instance.Auth.IsAuthenticated())
            {
                CrateBytesLogger.Log("Testing metadata operations...");
                
                // Create test player data
                var testData = new TestPlayerData
                {
                    TestLevel = 1,
                    TestScore = 100,
                    TestTimestamp = System.DateTime.UtcNow
                };

                // Set player data
                yield return CrateBytesSDK.Instance.Metadata.SetPlayerDataObject(testData, (response) =>
                {
                    if (response.Success)
                    {
                        CrateBytesLogger.Log("✓ Player data saved successfully");
                    }
                    else
                    {
                        CrateBytesLogger.LogWarning($"✗ Failed to save player data: {response.Error?.Message}");
                    }
                });

                // Retrieve player data
                yield return CrateBytesSDK.Instance.Metadata.GetPlayerData<TestPlayerData>((response) =>
                {
                    if (response.Success && response.Data != null)
                    {
                        CrateBytesLogger.Log($"✓ Player data retrieved! Level: {response.Data.TestLevel}, Score: {response.Data.TestScore}");
                    }
                    else
                    {
                        CrateBytesLogger.LogWarning($"✗ Failed to retrieve player data: {response.Error?.Message}");
                    }
                });
            }

            // Test 6: Session Cleanup
            if (CrateBytesSDK.Instance.Session.IsSessionActive())
            {
                CrateBytesLogger.Log("Cleaning up session...");
                yield return CrateBytesSDK.Instance.Session.StopSession((response) =>
                {
                    if (response.Success)
                    {
                        CrateBytesLogger.Log("✓ Session stopped successfully");
                    }
                    else
                    {
                        CrateBytesLogger.LogWarning($"✗ Session stop failed: {response.Error?.Message}");
                    }
                });
            }

            CrateBytesLogger.Log("=== CrateBytes SDK Test Completed ===");
        }

        /// <summary>
        /// Test player data structure
        /// </summary>
        [System.Serializable]
        public class TestPlayerData
        {
            public int TestLevel { get; set; }
            public int TestScore { get; set; }
            public System.DateTime TestTimestamp { get; set; }
        }

        /// <summary>
        /// Manual test methods for individual functionality
        /// </summary>
        [ContextMenu("Test Authentication")]
        public void TestAuthentication()
        {
            StartCoroutine(CrateBytesSDK.Instance.Auth.GuestLogin(null, (response) =>
            {
                CrateBytesLogger.Log(response.Success ? "Authentication successful!" : $"Authentication failed: {response.Error?.Message}");
            }));
        }

        [ContextMenu("Test Session")]
        public void TestSession()
        {
            if (CrateBytesSDK.Instance.Auth.IsAuthenticated())
            {
                StartCoroutine(CrateBytesSDK.Instance.Session.StartSession((response) =>
                {
                    CrateBytesLogger.Log(response.Success ? "Session started!" : $"Session failed: {response.Error?.Message}");
                }));
            }
            else
            {
                CrateBytesLogger.LogWarning("Not authenticated. Please authenticate first.");
            }
        }

        [ContextMenu("Test Leaderboard")]
        public void TestLeaderboard()
        {
            if (CrateBytesSDK.Instance.Auth.IsAuthenticated())
            {
                StartCoroutine(CrateBytesSDK.Instance.Leaderboard.GetLeaderboard(testLeaderboardId, 1, (response) =>
                {
                    CrateBytesLogger.Log(response.Success ? "Leaderboard retrieved!" : $"Leaderboard failed: {response.Error?.Message}");
                }));
            }
            else
            {
                CrateBytesLogger.LogWarning("Not authenticated. Please authenticate first.");
            }
        }
    }
} 