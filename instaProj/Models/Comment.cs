using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace instaProj.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public bool Ocult { get; set; }
        [NotMapped]
        [JsonIgnore]
        public bool rating { get; set; }

        public int User_Id { get; set; }
        public User? User { get; set; }
        public int Post_Id {  get; set; }
        public Post? Post {  get; set; }
        
    }
}
