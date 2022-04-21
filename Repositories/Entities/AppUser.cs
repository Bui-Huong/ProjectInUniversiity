using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AssetManagementWebApi.Repositories.Entities.Enum;
using Microsoft.AspNetCore.Identity;

namespace AssetManagementWebApi.Repositories.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string? Code { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Location { get; set; }
    [DataType(DataType.Date)]
    public DateTime DoB { get; set; }
    [DataType(DataType.Date)]
    public DateTime JoinDate { get; set; }
    public Role Type { get; set; }
    public Gender? Gender { get; set; }
    public bool IsDisabled { get; set; }
    public bool IsFirstLogin { get; set; }
    public List<AssignmentEntity>? HistoricalAssignments{ get; set;}
}