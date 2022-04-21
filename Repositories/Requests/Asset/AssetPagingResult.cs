using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetManagementWebApi.Repositories.Entities.Enum;

namespace AssetManagementWebApi.Repositories.Requests
{
    public class AssetPagingResult<T> : PagingResult<T>
    {
        public string? Search { get; set; } = "";
        public string? Sort { get; set; } = "asc";
        public string? SortField { get; set; } = "";
        public string[]? CategoryPrefix { get; set; } = new string[0];
        public int[]? State { get; set; } = new int[0];
    }
}