using System.ComponentModel.DataAnnotations.Schema;
namespace sktodo.Models
{
    public class ToDo
    {
        public int Id { get; set; }
        public string? UserQuestion { get; set; }
        public string? AssistantResponse { get; set; }
        [NotMapped]
        public DateTime Timestamp { get; set; }

    }
}





//Error
// namespace sktodo.Models
// {
//     public class ToDo
//     {
//         public int Id { get; set; }
//         public string UserQuestion { get; set; } = string.Empty;
//         public string AssistantResponse { get; set; } = string.Empty;
//         public DateTime Timestamp { get; set; }
//     }
// }
