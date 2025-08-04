namespace HackerNews.Model
{
    public class NewsStories
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string By { get; set; }
        public int Descendants { get; set; }
        public int Score { get; set; }
        public int Time { get; set; }
    }

    public class APIResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }

        public APIResponse(int status = 1, string message = "", object result = null)
        {
            Status = status;
            Message = string.IsNullOrEmpty(message) ? (status == 1 ? "Success." : "Fail.") : message;
            Result = result == null ? new object { } : result;
        }
    }
}