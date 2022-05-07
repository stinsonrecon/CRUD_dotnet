namespace CRUD.ViewModels
{
    public class Response
    {
        public string Message { get; set; }
        public object Data { get; set; }
        public string ErrorCode { get; set; }
        public bool Success { get; set; }
        public Response(string message, object data, string errorCode, bool success)
        {
            Message = message;  
            Data = data;
            ErrorCode = errorCode;
            Success = success;
        }
    }

    public class ResponsePostViewModel
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public ResponsePostViewModel()
        {

        }
        public ResponsePostViewModel(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }

    public class ResponseWithPaginationViewModel
    {
        public int StatusCode { get; set; }
        public int TotalRecord { get; set; }
        public ResponseWithPaginationViewModel(int statusCode, int totalRecord)
        {
            StatusCode = statusCode;
            TotalRecord = totalRecord;
        }
    }

    public class ResponsePaging
    {
        public string Message { get; set; }
        public object Data { get; set; }
        public string ErrorCode { get; set; }
        public bool Success { get; set; }
        public int TotalRecord { get; set; }
        public ResponsePaging(string message, object data, string errorCode, bool success, int totalRecord)
        {
            Message = message;
            Data = data;
            ErrorCode = errorCode;
            Success = success;
            TotalRecord = totalRecord;
        }
    }
}
