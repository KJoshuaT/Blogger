using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChitTalk.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public string? ImagePath { get; set; }

        public DateTime PublishedDate { get; set; } 
    }
}