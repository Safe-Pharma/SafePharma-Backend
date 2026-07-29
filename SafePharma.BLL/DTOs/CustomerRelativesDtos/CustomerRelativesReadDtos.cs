using System;
using System.Collections.Generic;
using System.Text;

namespace SafePharma.BLL
{
     public record CustomerRelativeReadDto
    {
        public Guid RelativeId { get; init; }
        public string RelativeName { get; init; }
        public string RelativePhone { get; init; }
 

    }
}
