namespace Shared.DTO
{

    public class ProductDTO
    {
        public int Id { get; set; } = 0;
        public string? Description { get; set; } = string.Empty;
        public string? Title { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public int InStock { get; set; }
    }
}
