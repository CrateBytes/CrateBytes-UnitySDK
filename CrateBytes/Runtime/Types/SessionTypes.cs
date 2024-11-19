namespace CrateBytes
{
    #region Responses

    public class SessionStartResponse : BaseResponse
    {
        public string message { get; set; }
    }

    public class SessionHeartbeatResponse : BaseResponse
    {
        public string message { get; set; }
    }

    public class SessionEndResponse : BaseResponse
    {
        public string message { get; set; }
        public int sessionDuriation { get; set; }
    }
     
    #endregion
}
