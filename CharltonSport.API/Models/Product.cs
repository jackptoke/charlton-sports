using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CharltonSport.API.Models
{
    public class Product
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Sku { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 0;
        public bool IsAvailable { get; set; } = false;
        [Required]
        public int CategoryId { get; set; }
        [JsonIgnore]
        public virtual Category? Category { get; set; }

    }
}
