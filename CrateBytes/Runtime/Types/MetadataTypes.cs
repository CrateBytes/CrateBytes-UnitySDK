namespace CrateBytes
{
    #region Responses

    public class MetadataResponse : BaseResponse
    {
        public string data { get; set; }
    }

    #endregion

    #region Requests

    public class SubmitMetadataRequest
    {
        public string data { get; set; }
    }

    #endregion

}
