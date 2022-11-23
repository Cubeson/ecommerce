namespace Shared.DTO
{
    public class ProductDTOUnity
    {
        public int Id;
        public string Description = string.Empty;
        public string Title = string.Empty;
        public decimal Price;
    }
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
