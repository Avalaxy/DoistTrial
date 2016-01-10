using System;
using System.Collections.Generic;

namespace Filters.Models
{
    public class Section
    {
        public string Title { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public DateTime? DueDate { get; set; }
        public Priority? Priority { get; set; }
    }
}
