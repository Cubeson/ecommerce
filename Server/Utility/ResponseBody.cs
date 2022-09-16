namespace Server.Utility
{
    public sealed class ResponseBody
    {
        public ResponseBody(string status, string message)
        {
            Status = status;
            Message = message;
        }

        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
