﻿using Server.Models;

namespace Server.DTO
{
    public sealed class ProductDTO
    {
        public ProductDTO() { }
        public ProductDTO(int id, string name, string description, decimal price, string fileFormat)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            FileFormat = fileFormat;
        }
        public ProductDTO(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            Price = product.Price;
            FileFormat = product.FileFormat;
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string FileFormat { get; set; } = "fbx";
    }
}