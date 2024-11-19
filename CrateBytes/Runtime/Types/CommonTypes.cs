
namespace CrateBytes
{


    public class BaseResponse
    {
        public int statusCode { get; set; }
        public CrateBytesError error { get; set; }

        public bool RequestSucceeded()
        {
            if(statusCode == 200 || statusCode == 204)
            {
                return true;
            }
            return false;
        }
    }

    public class DataResponse<T> : BaseResponse
    {
        public T data { get; set; }
    }

    public class CrateBytesError
    {
        public string message { get; set; }

        public override string ToString()
        {
            return $"[CrateBytesError]: {message}";
        }

    }

}


