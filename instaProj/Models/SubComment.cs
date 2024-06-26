using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace instaProj.Models
{
    public class SubComment
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public bool Ocult { get; set; }

        public int User_Id { get; set; }
        [NotMapped]
        [JsonIgnore]
        public User? User { get; set; }
        public int Comment_Id { get; set; }
        [NotMapped]
        [JsonIgnore]
        public Comment? Comment { get; set; }
    }
}
