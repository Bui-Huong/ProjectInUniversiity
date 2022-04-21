using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AssetManagementWebApi.Repositories.EFContext;
using AssetManagementWebApi.Repositories.Entities;
using AssetManagementWebApi.Services.Implements;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AssetManagementWebApi.Services.Implements;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<AppUser>? _userManager;
    private readonly SignInManager<AppUser>? _signInManager;
    private readonly RoleManager<AppRole>? _roleManager;

    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly AssetManagementDBContext _assetMangementDBContext;

    public AuthenticationService(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 RoleManager<AppRole>? roleManager,
                                 ILogger<AuthenticationService> logger,
                                 AssetManagementDBContext assetMangementDBContext,
                                 IConfiguration configuration,
                                 IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _assetMangementDBContext = assetMangementDBContext;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<string> Authenticate(LoginRequest request)
    {
        var signin = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, request.RememberMe, true);
        if (!signin.Succeeded) return "Wrong username or pass";
        var user = await _userManager.FindByNameAsync(request.UserName);
            if(user == null) return "Wrong username or pass";
            if(user.IsDisabled) return "Account Disabled";
        if (signin.Succeeded)
        {
            string token = null;
            Console.WriteLine(user);
            var role = await _userManager.GetRolesAsync(user);
            if (request.RememberMe == true)
            {
                token = CreateToken(user, role[0], DateTime.Now.AddDays(7));
            }
            else
            {
                token = CreateToken(user, role[0], DateTime.Now.AddMinutes(30));
            }
            return token;
        }
        return "Wrong username or pass";

    }


    public async Task<string> FirstLogIn(ChangePasswordRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.Password);
        user.IsFirstLogin = false;
        await _userManager.UpdateAsync(user);
        if (result.Succeeded) return "Ok";
        return result.ToString();
    }

    private string CreateToken(AppUser user, string role, DateTime expiresTime)
    {
        List<Claim> claims = new List<Claim>();

        claims = new List<Claim>
            {   new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Locality, user.Location),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.UserData, string.Concat(user.Code+','+user.IsFirstLogin)),
                new Claim(ClaimTypes.DateOfBirth, user.DoB.ToString("ddMMyyyy")),
                new Claim("IsActive",user.IsDisabled.ToString())
            };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Tokens:Key").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(_configuration["Tokens:Issuer"],
         _configuration["Tokens:Issuer"],
         claims,
         expires: expiresTime,
         signingCredentials: creds);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt.ToString();
    }
}