using System;

namespace CrateBytes {

    public class APIResponse<T> {
        public int status;
        public string error;
        public T data;
    }

    [Serializable]
    public struct Player {
        public bool guest;
        public string playerId;
        public string entryData;
    }

    [Serializable]
    public struct Leaderboard {
        public string id;
        public string name;
        public string description;
    }

    #region Guest Authentication
    [Serializable]
    public struct GuestLoginRequest {
        public string projectKey;
        public string playerId;

        public override string ToString() {
            if (string.IsNullOrEmpty(playerId)) {
                return $"{{\"projectKey\":\"{projectKey}\"}}";
            } else {
                return $"{{\"projectKey\":\"{projectKey}\",\"playerId\":\"{playerId}\"}}";
            }
        }
    }

    [Serializable]
    public struct GuestLoginResponse {
        public string token;
        public string playerId;
    }
    #endregion

    #region Steam Authentication
    [Serializable]
    public struct SteamLoginRequest {
        public string projectKey;
        public string steamAuthTicket;

        public override string ToString() {
            return $"{{\"projectKey\":\"{projectKey}\",\"steamAuthTicket\":\"{steamAuthTicket}\"}}";
        }
    }

    [Serializable]
    public struct SteamLoginResponse {
        public string token;
        public string playerId;
    }
    #endregion

    #region Leaderboards
    [Serializable]
    public struct LeaderboardSubmitRequest {
        public int score;

        public override string ToString() {
            return $"{{\"score\":{score}}}";
        }
    }

    [Serializable]
    public struct LeaderboardEntry {
        public Player player;
        public int score;
    }

    [Serializable]
    public struct LeaderboardResponse {
        public Leaderboard leaderboard;
        public LeaderboardEntry[] entries;
        public int totalEntries;
        public int pages;
    }
    #endregion

    #region Metadata
    [Serializable]
    public struct GetMetadataResponse {
        public string data;
    }

    [Serializable]
    public struct AddUpdateMetadataRequest {
        public string data;

        public override string ToString() {
            return $"{{\"data\":\"{data}\"}}";
        }
    }

    [Serializable]
    public struct AddUpdateMetadataResponse {
        public string data;
    }
    #endregion
}