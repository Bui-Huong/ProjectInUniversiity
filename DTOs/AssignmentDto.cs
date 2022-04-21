using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AssetManagementWebApi.Repositories.Entities;
using AssetManagementWebApi.Repositories.Entities.Enum;

namespace AssetManagementWebApi.DTOs
{
    public class AssignmentDto
    {
        public Guid Id { get; set; }
        // public int SerialNumber { get; set; }
        public string? AssetCode { get; set; }
        public string? AssetName { get; set; }
        public string? Specification { get; set; }
        public string? AssigneeUserName { get; set; }
        public string? AssigneeCode { get; set; }
        public string? AssignerUserName { get; set; }
        public string? AssignerCode { get; set; }
        [DataType(DataType.Date)]
        public DateTime AssignedDate { get; set; }
        public AsmState AssignmentState { get; set; }
        public string? RequesterUserName { get; set; }
        public string? RequesterCode { get; set; }
        public string? VerifierUserName { get; set; }
        public string? VerifierCode { get; set; }
        [DataType(DataType.Date)]
        public DateTime ReturnDate { get; set; }
        public ReturnState ReturnState { get; set; }
        public string? Note { get; set; }
    }
}