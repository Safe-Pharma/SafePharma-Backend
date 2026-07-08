using SafePharma.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace SafePharma.BLL.DTOs.Batch
{
    public class BatchReadDto
    {
        public String MedeicineName=string.Empty;
        public String MedeicineCategory = string.Empty;
        public int BatchesCount;
        public decimal OnHand;
        public int MinStockLevel;
        public string i;



    }
}
