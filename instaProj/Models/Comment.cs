﻿namespace instaProj.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public User? User { get; set; }
        public string? Commeting { get; set; }
        public int ContLike { get; set; }
        public Archive? Archive { get; set; }
        public bool Ocult { get; set; }
    }
}
