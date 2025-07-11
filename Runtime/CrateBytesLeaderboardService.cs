using UnityEngine;
using System;
using System.Collections;

namespace CrateBytes
{
    /// <summary>
    /// Leaderboard service for CrateBytes API
    /// </summary>
    public class CrateBytesLeaderboardService : CrateBytesHttpService
    {
        public CrateBytesLeaderboardService(CrateBytesSDK sdk) : base(sdk) { }

        /// <summary>
        /// Get leaderboard entries with pagination
        /// </summary>
        public IEnumerator GetLeaderboard(string leaderboardId, int page = 1, Action<CrateBytesResponse<LeaderboardResponse>> callback = null)
        {
            string endpoint = $"/leaderboard/{leaderboardId}?page={page}";
            
            yield return GetRequest(endpoint, callback);
        }

        /// <summary>
        /// Submit a score to the leaderboard
        /// </summary>
        public IEnumerator SubmitScore(string leaderboardId, string score, Action<CrateBytesResponse<ScoreSubmissionResponse>> callback = null)
        {
            var requestData = new ScoreSubmissionRequest
            {
                score = score
            };

            string endpoint = $"/leaderboard/{leaderboardId}";
            
            yield return PostRequest(endpoint, requestData, callback);
        }

        /// <summary>
        /// Get leaderboard entries for a specific player
        /// </summary>
        public IEnumerator GetPlayerLeaderboard(string leaderboardId, int page = 1, Action<CrateBytesResponse<LeaderboardResponse>> callback = null)
        {
            return GetLeaderboard(leaderboardId, page, callback);
        }
    }

    /// <summary>
    /// Score submission request data
    /// </summary>
    [Serializable]
    public class ScoreSubmissionRequest
    {
        public string score { get; set; }
    }
} 