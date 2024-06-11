namespace instaProj.Models
{
    public class SubComment
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public bool Ocult { get; set; }

        public int User_Id { get; set; }
        public User? User { get; set; }
        public int Comment_Id { get; set; }
        public Comment? Comment { get; set; }
    }
}
