using System.Net.Http;

namespace CrateBytes
{
    public class CrateBytesEndPoint
    {
        public string endPoint { get; set; }

        public HttpMethod method { get; set; }

        public CrateBytesEndPoint(string url, HttpMethod _method)
        {
            endPoint = url;
            method = _method;
        }
    }

    public class CrateBytesEndPointList
    {

        #region Authentication

        public static CrateBytesEndPoint guestAuthentication = new CrateBytesEndPoint("platform/guest", HttpMethod.Post);
        public static CrateBytesEndPoint steamAuthentication = new CrateBytesEndPoint("platform/steam", HttpMethod.Post);

        #endregion

        #region Tracking

        public static CrateBytesEndPoint startTrackingSession = new CrateBytesEndPoint("gameplay/start", HttpMethod.Post);
        public static CrateBytesEndPoint trackHeartbeat = new CrateBytesEndPoint("gameplay/heartbeat", HttpMethod.Post);
        public static CrateBytesEndPoint endTrackingSession = new CrateBytesEndPoint("gameplay/end", HttpMethod.Post);

        #endregion

        #region Leaderboard

        public static CrateBytesEndPoint submitLeaderboardScore = new CrateBytesEndPoint("leaderboards/{0}/submit", HttpMethod.Post);
        public static CrateBytesEndPoint getLeaderboard = new CrateBytesEndPoint("leaderboards/{0}?page={1}", HttpMethod.Get);

        #endregion

        #region Metadata

        public static CrateBytesEndPoint getMetadata = new CrateBytesEndPoint("metadata/get", HttpMethod.Get);
        public static CrateBytesEndPoint updateOrAddMetadata = new CrateBytesEndPoint("metadata/add", HttpMethod.Put);
        public static CrateBytesEndPoint deleteMetadata = new CrateBytesEndPoint("/metadata/delete", HttpMethod.Delete);


        #endregion
    }
}