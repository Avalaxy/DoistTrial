using System;

namespace Filters.Models
{
    public class Item
    {
        public string Content { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
    }
}
