using System.ComponentModel.DataAnnotations.Schema;
namespace sktodo.Models
{
    public class Light
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Response { get; set; }
        [NotMapped]
        public bool? IsOn { get; set; }
    }
}