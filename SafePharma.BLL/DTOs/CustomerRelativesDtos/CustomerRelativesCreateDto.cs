using SafePharma.Common.Enums;
using SafePharma.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SafePharma.BLL
{
    public class CustomerRelativeCreateDto
    {
        public Guid CustomerId { get; set; }

        [ForeignKey("Relative")]
        public Guid RelativeId { get; set; }
        public bool HasAccessToRelative { get; set; } = false;

    }
}
