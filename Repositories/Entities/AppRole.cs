using Microsoft.AspNetCore.Identity;
namespace  AssetManagementWebApi.Repositories.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        public string? Description { get; set; }
    }
}