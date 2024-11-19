using System;

namespace CrateBytes
{
    #region Responses

    public class GuestSessionResponse : BaseResponse
    {
        public string token { get; set; }
        public string playerId { get; set; }
    }

    public class SteamSessionResponse : BaseResponse
    {
        public string token { get; set; }
        public string playerId { get; set; }
    }

    #endregion

    #region Requests

    public class GuestSessionRequest
    {
        public string projectKey { get; set; }
        public string playerId { get; set; }
    }

    public class SteamSessionRequest
    {
        public string projectKey { get; set; }
        public string steamAuthTicket { get; set; }
    }

    #endregion

}

