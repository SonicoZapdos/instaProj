namespace instaProj.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int ContLike { get; set; }
        public DateTime DatePub { get; set; }
        public bool Private { get; set; }

        public int User_Id { get; set; }
        public User? User { get; set; }
    }
}
