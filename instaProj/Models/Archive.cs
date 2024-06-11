namespace instaProj.Models
{
    public class Archive
    {
        public int Id { get; set; }
        public string? Link { get; set; }
        public string? NameLocal { get; set; }  
        public string? Type {  get; set; }

        public int Post_Id { get; set; }
        public Post? Post { get; set; }
    }
}
