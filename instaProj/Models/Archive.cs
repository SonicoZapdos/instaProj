namespace instaProj.Models
{
    public class Archive
    {
        public int Id { get; set; }
        public string? Link { get; set; } /* List - Link - opcional ( String ) */
        public string? NameLocal { get; set; }  /* List - Not Link - opcional ( Archive ) */
        public string? Type {  get; set; } /* List - Not Link - opcional - ex: .jpeg ( Archive )*/

        public int Post_Id { get; set; } /* Post.Id */
        public Post? Post { get; set; }
    }
}
