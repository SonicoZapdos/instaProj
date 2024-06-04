namespace instaProj.Models
{
    public class Archive
    {
        public int Id { get; set; }
        public string? Desc { get; set; }
        public string? Link { get; set; }
        public string? NameLocal { get; set; }
        public DateTime DatePub { get; set; }
        public bool Private {  get; set; }
        public int FK_Comments_User_Id { get; set; }
        public User? User { get; set; }
    }
}
