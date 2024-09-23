namespace HackerNews.Services.Models
{
    public class PagedResponseDTO<T>: ResponseDTO<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
