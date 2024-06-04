namespace instaProj.Models
{
    public class SubComment
    {
        public int Id { get; set; }
        public int FK_Comments_User_Id { get; set; }
        public User? User { get; set; }
        public string? Commeting { get; set; }
        public int Arc_Id { get; set; }
        public Archive? Archive { get; set; }
        public bool Ocult { get; set; }
        public int Comment_Id { get; set; }
        public Comment? Comment { get; set; }
    }
}
