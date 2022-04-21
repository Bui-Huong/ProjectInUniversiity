using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AssetManagementWebApi.Repositories.Requests
{
    public class CategoryRequest
    {
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public string CategoryPrefix { get; set; }
    }
}