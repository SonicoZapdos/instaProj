﻿namespace instaProj.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public bool Ocult { get; set; }

        public int User_Id { get; set; }
        public User? User { get; set; }
        public int Post_Id {  get; set; }
        public Post? Post {  get; set; }
    }
}
