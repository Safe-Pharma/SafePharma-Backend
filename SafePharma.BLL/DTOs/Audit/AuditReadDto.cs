using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SafePharma.BLL
{
    public class AuditReadDto
    {
        public DateTime Date { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public JsonElement? newValues { get; set; }
        public JsonElement? oldValues { get; set; }
    }
}
