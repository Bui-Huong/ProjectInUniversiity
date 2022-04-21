using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AssetManagementWebApi.Repositories.Entities.Enum;

namespace AssetManagementWebApi.Models
{
    public class AssetCreateModel
    {
        [Required]
        public string AssetName { get; set; }
        [Required]
        public string CategoryPrefix { get; set; }
        [Required]
        public string Specification { get; set; }
        [Required]
        public DateTime InstalledDate { get; set; }
        [Required]
        public AssetState State { get; set; }
    }
}