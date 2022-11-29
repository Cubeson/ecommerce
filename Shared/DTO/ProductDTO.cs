namespace Shared.DTO
{
    //public class ProductDTOUnity
    //{
    //    public int Id = 0;
    //    public string Description = string.Empty;
    //    public string Title = string.Empty;
    //    public double Price;
    //}
    public class ProductDTO
    {
        public int Id { get; set; } = 0;
        public string? Description { get; set; } = string.Empty;
        public string? Title { get; set; } = string.Empty;
        //public double? Price { get; set; }
        public decimal? Price { get; set; }
    }
}
