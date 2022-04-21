using AssetManagementWebApi.Repositories.Entities;
using AssetManagementWebApi.Repositories.Entities.Enum;

namespace AssetManagementWebApi.DTOs;
public class AppUserDTO{
    public string? Code { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Gender Gender { get; set; }
    public string? Location { get; set; }
    public string? UserName { get; set; }
    public string FullName { get{return $"{FirstName} {LastName}";} }
    public List<AssignmentEntity>? HistoricalAssignments{ get; set;}
    public Role Type { get; set; }
    public bool IsDisabled { get; set; }
    public DateTime DoB { get; set; }
    public DateTime JoinDate { get; set; }
    public string? Password { get; set; }
    public bool IsFirstLogin { get; set; }
}   