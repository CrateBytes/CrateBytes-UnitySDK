using CrateBytes.Net;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CrateBytes
{
    public class CrateBytesAPIManager
    {

        //TODO: Remove to a configurable object and reference it through
        private static string projectKey = "cm2w95j0f0000julzpuvfm8ou-Rb7Ughy4vxF5jDiE";

        #region Authentication

        /// <summary>
        /// Authenticate a Player as a Guest
        /// </summary>
        /// <returns></returns>
        public static async Task<GuestSessionResponse> AuthenticateGuest()
        {
            var response = await AuthenticateGuest(null);

            return response;
        }

        /// <summary>
        /// Authenticates Player as a Guest with a specific ID
        /// </summary>
        /// <param name="playerID">ID of the player to Authenticate</param>
        /// <returns></returns>
        public static async Task<GuestSessionResponse> AuthenticateGuest(string playerID)
        {
            var endPoint = CrateBytesEndPointList.guestAuthentication;

            var request = new GuestSessionRequest()
            {
                projectKey = projectKey,
            };

            if (PlayerPrefs.GetString("CrateBytes_PlayerID", "") != "" || !string.IsNullOrEmpty(playerID))
            {
                Debug.Log("Got stored player");
                request.playerId = playerID;
            }

            string stringRequest = JsonConvert.SerializeObject(request);

            var response = await CrateBytesAPI.CallAPI<DataResponse<GuestSessionResponse>>(endPoint.endPoint, endPoint.method, stringRequest);

            if (response.statusCode == 200)
            {
                PlayerPrefs.SetString("CrateBytes_PlayerID", response.data.playerId);
                PlayerPrefs.SetString("CrateBytes_AuthenticationToken", response.data.token);
            }

            if (response == null || response.data == null)
            {
                Debug.LogError("Failed to get a valid response from the API.");
                return null;
            }

            return response.data;

        }

        /// <summary>
        /// Authenticates a player through Steam
        /// </summary>
        /// <param name="authToken">Token gotten from using the Steam API</param>
        /// <returns></returns>
        public static async Task<SteamSessionResponse> AuthenticateSteam(string authToken)
        {
            var endPoint = CrateBytesEndPointList.steamAuthentication;

            var request = new SteamSessionRequest()
            {
                projectKey = projectKey,
                steamAuthTicket = authToken
            };

            string stringRequest = JsonConvert.SerializeObject(request);

            var response = await CrateBytesAPI.CallAPI<DataResponse<SteamSessionResponse>>(endPoint.endPoint, endPoint.method, stringRequest);


            if (response.statusCode == 200)
            {
                PlayerPrefs.SetString("CrateBytes_PlayerID", response.data.playerId);
                PlayerPrefs.SetString("CrateBytes_AuthenticationToken", response.data.token);
            }

            if (response == null || response.data == null)
            {
                Debug.LogError("Failed to get a valid response from the API.");
                return null;
            }

            return response.data;

        }

        #endregion

        #region Sessions

        /// <summary>
        /// Starts a tracking session
        /// </summary>
        /// <returns></returns>
        public static async Task<SessionStartResponse> StartSessionTracking()
        {
            var endPoint = CrateBytesEndPointList.startTrackingSession;

            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (PlayerPrefs.GetString("CrateBytes_AuthenticationToken") != string.Empty)
            {
                headers.Add("Authorization", $"Bearer {PlayerPrefs.GetString("CrateBytes_AuthenticationToken")}");
            } else
            {
                Debug.Log("Tried to send request without Bearer");
            }

            var response = await CrateBytesAPI.CallAPI<DataResponse<SessionStartResponse>>(endPoint.endPoint, endPoint.method, null, headers);

            if (response == null || response.data == null)
            {
                Debug.LogError("Failed to get a valid response from the API.");
                return null;
            }

            return response.data;

        }

        /// <summary>
        /// Checks a current tracking session and prolongs it if found
        /// </summary>
        /// <returns></returns>
        public static async Task<SessionHeartbeatResponse> SessionHeartbeat()
        {
            var endPoint = CrateBytesEndPointList.trackHeartbeat;
            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (PlayerPrefs.GetString("CrateBytes_AuthenticationToken") != string.Empty)
            {
                headers.Add("Authorization", $"Bearer {PlayerPrefs.GetString("CrateBytes_AuthenticationToken")}");
            }
            else
            {
                Debug.Log("Tried to send request without Bearer");
            }

            var response = await CrateBytesAPI.CallAPI<DataResponse<SessionHeartbeatResponse>>(endPoint.endPoint, endPoint.method, null, headers);

            if (response == null || response.data == null)
            {
                Debug.LogError("Failed to get a valid response from the API.");
                return null;
            }

            return response.data;
        }

        /// <summary>
        /// Ends the tracking session
        /// </summary>
        /// <returns></returns>
        public static async Task<SessionEndResponse> EndTrackingSession()
        {
            var endPoint = CrateBytesEndPointList.endTrackingSession;
            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (PlayerPrefs.GetString("CrateBytes_AuthenticationToken") != string.Empty)
            {
                headers.Add("Authorization", $"Bearer {PlayerPrefs.GetString("CrateBytes_AuthenticationToken")}");
            }
            else
            {
                Debug.Log("Tried to send request without Bearer");
            }

            var response = await CrateBytesAPI.CallAPI<DataResponse<SessionEndResponse>>(endPoint.endPoint, endPoint.method, null, headers);

            if (response == null || response.data == null)
            {
                Debug.LogError("Failed to get a valid response from the API.");
                return null;
            }

            return response.data;
        }

        #endregion

        #region Leaderboards

        /// <summary>
        /// Submit a score to a specific leaderboard
        /// </summary>
        /// <param name="leaderboardID">ID of the Leaderboard</param>
        /// <param name="score">Score to upload</param>
        /// <returns></returns>
        public static async Task<ScoreSubmitResponse> SubmitScore(string leaderboardID, int score)
        {
            var endPoint = CrateBytesEndPointList.submitLeaderboardScore;

            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (PlayerPrefs.GetString("CrateBytes_AuthenticationToken") != string.Empty)
            {
                headers.Add("Authorization", $"Bearer {PlayerPrefs.GetString("CrateBytes_AuthenticationToken")}");
            }
            else
            {
                Debug.Log("Tried to send request without Bearer");
            }

            var request = new ScoreSubmitRequest()
            {
                score = score,
            };

            var formattedEndPoint = string.Format(endPoint.endPoint, leaderboardID);
            Debug.Log(formattedEndPoint);
            string stringRequest = JsonConvert.SerializeObject(request);

            var response = await CrateBytesAPI.CallAPI<DataResponse<ScoreSubmitResponse>>(formattedEndPoint, endPoint.method, stringRequest, headers);
            
            if (response == null)
            {
                Debug.LogError("Failed to get a valid response from the API.");
                return null;
            }
            if (response.data == null)
            {
                Debug.LogError("Failed to get a valid data response from the API.");
                return null;
            }

            return response.data;
        }

        /// <summary>
        /// Gets a page of a specific Leaderboard
        /// </summary>
        /// <param name="leaderboardID">ID of the Leaderboard</param>
        /// <param name="pageNumber">integer of which page to get</param>
        /// <returns></returns>
        public static async Task<CrateBytesLeaderboardResponse> GetLeaderboard(string leaderboardID, int pageNumber = 0)
        {
            var endPoint = CrateBytesEndPointList.getLeaderboard;

            string formattedEndpoint = string.Format(endPoint.endPoint, leaderboardID, pageNumber);

            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (PlayerPrefs.GetString("CrateBytes_AuthenticationToken") != string.Empty)
            {
                headers.Add("Authorization", $"Bearer {PlayerPrefs.GetString("CrateBytes_AuthenticationToken")}");
            }
            else
            {
                Debug.Log("Tried to send request without Bearer");
            }

            var response = await CrateBytesAPI.CallAPI<DataResponse<CrateBytesLeaderboardResponse>>(formattedEndpoint, endPoint.method, null, headers);

            if (response == null || response.data == null)
            {
                Debug.LogError("Failed to get a valid response from the API.");
                return null;
            }

            return response.data;
        }

        #endregion

        #region Metadata

        /// <summary>
        /// Get the Players Metadata
        /// </summary>
        /// <returns>MetadataResponse which contains the metadata.</returns>
        public static async Task<MetadataResponse> GetPlayerMetadata()
        {
            var endPoint = CrateBytesEndPointList.getMetadata;
            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (PlayerPrefs.GetString("CrateBytes_AuthenticationToken") != string.Empty)
            {
                headers.Add("Authorization", $"Bearer {PlayerPrefs.GetString("CrateBytes_AuthenticationToken")}");
            }
            else
            {
                Debug.Log("Tried to send request without Bearer");
            }

            var response = await CrateBytesAPI.CallAPI<DataResponse<MetadataResponse>>(endPoint.endPoint, endPoint.method, null, headers);
            return response.data;
        }

        /// <summary>
        /// Add or Update a Players Metadata
        /// </summary>
        /// <param name="newData">The new data to be added or updated.</param>
        /// <returns></returns>
        public static async Task<MetadataResponse> AddOrUpdatePlayerMetadata(string newData)
        {
            var endPoint = CrateBytesEndPointList.updateOrAddMetadata;
            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (PlayerPrefs.GetString("CrateBytes_AuthenticationToken") != string.Empty)
            {
                headers.Add("Authorization", $"Bearer {PlayerPrefs.GetString("CrateBytes_AuthenticationToken")}");
            }
            else
            {
                Debug.Log("Tried to send request without Bearer");
            }

            var request = new SubmitMetadataRequest()
            {
                data = newData,
            };

            string stringRequest = JsonConvert.SerializeObject(request);

            var response = await CrateBytesAPI.CallAPI<DataResponse<MetadataResponse>>(endPoint.endPoint, endPoint.method, stringRequest, headers);

            if (response == null || response.data == null)
            {
                Debug.LogError("Failed to get a valid response from the API.");
                return null;
            }

            return response.data;
        }

        /// <summary>
        /// Deletes the players metadata entry
        /// </summary>
        /// <returns>MetadataResponse type with statusCode and response message.</returns>
        public static async Task<MetadataResponse> DeletePlayerMetadata()
        {
            var endPoint = CrateBytesEndPointList.deleteMetadata;
            Dictionary<string, string> headers = new Dictionary<string, string>();

            if (PlayerPrefs.GetString("CrateBytes_AuthenticationToken") != string.Empty)
            {
                headers.Add("Authorization", $"Bearer {PlayerPrefs.GetString("CrateBytes_AuthenticationToken")}");
            }
            else
            {
                Debug.Log("Tried to send request without Bearer");
            }

            var response = await CrateBytesAPI.CallAPI<DataResponse<MetadataResponse>>(endPoint.endPoint, endPoint.method, null, headers);

            if (response == null || response.data == null)
            {
                Debug.LogError("Failed to get a valid response from the API.");
                return null;
            }

            return response.data;
        }

        #endregion
    }

}
