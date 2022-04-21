namespace AssetManagementWebApi.Repositories.Entities;

public class ChangePasswordRequest
{
    public string? UserName { get; set; }
    public string? OldPassword { get; set; }
    public string? Password { get; set; }
}