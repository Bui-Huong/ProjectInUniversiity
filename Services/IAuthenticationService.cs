using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetManagementWebApi.Repositories.Entities;

namespace AssetManagementWebApi.Services;
public interface IAuthenticationService
{
    Task<string> Authenticate(LoginRequest request);
    Task<string> FirstLogIn(ChangePasswordRequest request);
}