using AssetManagementWebApi.Repositories.Entities.Enum;

namespace AssetManagementWebApi.Models;
public class AppUserModel{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Gender Gender { get; set; }
    public Role Type { get; set; }
    public DateTime DoB { get; set; }
    public DateTime JoinDate { get; set; }
}   