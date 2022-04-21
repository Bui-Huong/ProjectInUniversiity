using AssetManagementWebApi.Repositories.Entities.Enum;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementWebApi.Repositories.Requests
{
    public class SortRequest
    {
        [FromQuery(Name = "field")]
        public string? SortField { get; set; }
        [FromQuery(Name = "order")]        
        public string? OrderBy { get; set; }
        
    }
}