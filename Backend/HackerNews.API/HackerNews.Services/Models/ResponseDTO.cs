namespace HackerNews.Services.Models
{
    public class ResponseDTO<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "";
        public ResponseDTO()
        {

        }
        public ResponseDTO(T result)
        {
            Data = result;
        }
    }
}
