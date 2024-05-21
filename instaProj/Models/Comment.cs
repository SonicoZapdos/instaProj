namespace instaProj.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public User? User { get; set; }
        public string? Comment { get; set; }
        public int ContLike { get; set; }
        public int Arc_Id { get; set; }
        public bool Ocult { get; set; }
    }
}
