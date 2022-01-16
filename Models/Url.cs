using System.ComponentModel.DataAnnotations;

namespace Short.Models
{
    public class Url
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string LongUrl { get; set; }

        [Required]
        public string ShortUrl { get; set; }
    }
}