using System;
using System.Collections.Generic;
using System.Text;

namespace SafePharma.BLL
{
   
        public class BatchItemDto
        {
            public Guid Id { get; set; }
            public string BatchNumber { get; set; } = "";
            public DateTime ExpiryDate { get; set; }
            public int QuantityRemaining { get; set; }
            public int DaysLeft { get; set; }
        }
    
}
