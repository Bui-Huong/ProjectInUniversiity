using AssetManagementWebApi.Repositories.Entities.Enum;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementWebApi.Repositories.Requests
{
    public class FilterRequest
    {
        [FromQuery(Name = "search")]
        public string? SearchValue { get; set; }
        [FromQuery(Name = "role[]")]        
        public List<Role>? Role { get; set; }
        [FromQuery(Name = "date")]
        public DateTime? FilterDate { get; set; }
        [FromQuery(Name = "asmState[]")]
        public List<AsmState>? AsmState { get; set; }
        [FromQuery(Name = "returnState[]")]
        public List<ReturnState>? ReturnState { get; set; }
    }
}