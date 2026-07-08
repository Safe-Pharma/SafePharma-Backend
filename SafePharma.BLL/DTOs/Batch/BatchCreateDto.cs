using System;
using System.Collections.Generic;
using System.Text;

namespace SafePharma.BLL
{
    public class BatchCreateDto
    {
        public Guid MedicineId { get; set; }
        public int BatchNumber { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int QuantityReceived { get; set; }
    
    }
}
