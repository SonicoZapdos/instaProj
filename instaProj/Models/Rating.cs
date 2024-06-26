using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace instaProj.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int User_Id { get; set; } //NOT NULL
        [NotMapped]
        [JsonIgnore]
        public User? User { get; set; }

        public int? Post_Id { get; set; } //POSSIBLE NULL - id post
        [NotMapped]
        [JsonIgnore]
        public Post? Post { get; set; }

        public int? Comment_Id { get; set; } //POSSIBLE NULL - id comment
        [NotMapped]
        [JsonIgnore]
        public Comment? Comment { get; set; }

        public int? SubComment_Id { get; set; } //POSSIBLE NULL - id comment
        [NotMapped]
        [JsonIgnore]
        public SubComment? SubComment { get; set; }
    }
}
