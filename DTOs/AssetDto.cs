using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetManagementWebApi.Repositories.Entities;
using AssetManagementWebApi.Repositories.Entities.Enum;

namespace AssetManagementWebApi.DTOs;
public class AssetDto
{
    // public Guid Id { get; set; }
    public string? AssetCode { get; set; }
    public string? AssetName { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryPrefix { get; set; }
    public CategoryEntity? Category { get; set; }
    public string? Specification { get; set; }
    public DateTime InstalledDate { get; set; }
    public AssetState State { get; set; }
    public List<AssignmentDto>? HistoricalAssignments { get; set; }
}