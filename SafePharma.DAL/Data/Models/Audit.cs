using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SafePharma.DAL
{
    public class Audit
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime Date { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;


    }
}
