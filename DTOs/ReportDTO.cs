using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssetManagementWebApi.DTOs
{
    public class ReportDTO
    {
        public string CategoryName { get; set; }
        public int TotalCount  { get; set; }
        public int AssignedCount { get; set; }
        public int AvailableCount { get; set; }
        public int NotAvailableCount { get; set; }
        public int WaitingForRecyclingCount { get; set; }
        public int RecycledCount { get; set; }
    }
}