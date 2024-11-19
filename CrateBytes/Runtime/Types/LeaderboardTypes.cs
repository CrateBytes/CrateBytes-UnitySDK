namespace CrateBytes
{
    public class CrateBytesLeaderboard
    {
        public string name { get; set; }
        public string description { get; set; }
    }

    public class CrateBytesLeaderboardEntry
    {
        public CrateBytesLeaderboardEntryPlayer player { get; set; }
        public int score { get; set; }
    }

    public class CrateBytesLeaderboardEntryPlayer
    {
        public bool guest { get; set; }

        public string playerId { get; set; }

        public string entryData { get; set; }
    }

    #region Responses

    public class CrateBytesLeaderboardResponse : BaseResponse
    {
        public CrateBytesLeaderboard leaderboard { get; set; }
        public CrateBytesLeaderboardEntry[] entries { get; set; }
        public int totalEntries { get; set; }
        public int pages { get; set; }
    }

    public class ScoreSubmitResponse : BaseResponse
    {
        public string message { get; set; }
    }

    #endregion

    #region Requests

    public class ScoreSubmitRequest
    {
        public int score { get; set; }
    }
    #endregion
}
