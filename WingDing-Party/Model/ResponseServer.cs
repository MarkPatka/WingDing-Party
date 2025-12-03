using System.Net;

namespace WingDing_Party.Model
{
    public class ResponseServer
    {
        public ResponseServer()
        {
            this.IsSuccess = true;
            this.ErrorMessage = new();
        }

        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> ErrorMessage { get; set; }
        public object? Result { get; set; }
    }
}
