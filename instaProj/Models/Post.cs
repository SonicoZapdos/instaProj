﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace instaProj.Models
{
    public class Post
    {
        public int Id { get; set; } /* auto */

        public string? Description { get; set; } /* . */
        public int ContLike { get; set; } /* after */
        public DateTime DatePub { get; set; } /* . */
        public bool Private { get; set; } /* - false */

        public int User_Id { get; set; } /* . */
        [NotMapped]
        [JsonIgnore]
        public User? User { get; set; } /* auto */

        [NotMapped]
        [JsonIgnore]
        public List<Archive>? Archives { get; set; }
        [NotMapped]
        [JsonIgnore]
        public bool Rating { get; set; }
        [NotMapped]
        [JsonIgnore]
        public List<Comment>? Comment { get; set; }

    }
}
