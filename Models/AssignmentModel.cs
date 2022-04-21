using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AssetManagementWebApi.DTOs;
using AssetManagementWebApi.Repositories.Entities;
using AssetManagementWebApi.Repositories.Entities.Enum;

namespace AssetManagementWebApi.Models
{
    public class AssignmentModel
    {
        public string? AssetCode { get; set; }
        public string? AssigneeCode { get; set; }
        public string? AssignerCode { get; set; }
        [DataType(DataType.Date)]
        public DateTime AssignedDate { get; set; }
        public AsmState AssignmentState { get; set; }
        public string? Note { get; set; }
    }
    public class AssignmentCreateModel:AssignmentModel
    {
           public string? location{get;set;}
    }
    public class ReturningRequestModel
    {
        public Guid? assetId{get;set;}
        public string? reuesterCode{get;set;}
    }
}