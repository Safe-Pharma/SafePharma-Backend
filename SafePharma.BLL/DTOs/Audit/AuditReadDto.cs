using System;
using System.Collections.Generic;
using System.Text;

namespace SafePharma.BLL
{
    public class AuditReadDto
    {
        public DateTime Date { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
    }
}
