namespace instaProj.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int FK_Comments_User_Id { get; set; } //NOT NULL
        public int Archive_Id { get; set;} //POSSIBLE NULL - id post
        public int Comment_Id { get; set;} //POSSIBLE NULL - id comment
    }
}
