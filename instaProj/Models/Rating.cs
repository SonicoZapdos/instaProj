namespace instaProj.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int User_Id { get; set; } //NOT NULL
        public User? User { get; set; }

        public int? Post_Id { get; set; } //POSSIBLE NULL - id post
        public Post? Post { get; set; }

        public int? Comment_Id { get; set; } //POSSIBLE NULL - id comment
        public Comment? Comment { get; set; }

        public int? SubComment_Id { get; set; } //POSSIBLE NULL - id comment
        public SubComment? SubComment { get; set; }
    }
}
