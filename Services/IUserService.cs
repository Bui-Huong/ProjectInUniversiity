using AssetManagementWebApi.DTOs;
using AssetManagementWebApi.Repositories.Entities;
using AssetManagementWebApi.Repositories.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementWebApi.Services;
public interface IUserService{
    Task<PagingResult<AppUserDTO>> GetUsersAsync(string location, PagingRequest pageRequest, SortRequest requestSort = null, FilterRequest filterRequest = null);
    Task<AppUserDTO> GetUserAsync(string code);
    Task<AppUserDTO> CreateUserAsync(string location, AppUserDTO user);
    Task<string> DisableUserAsync(string code);
    Task<AppUserDTO> EditUserAsync(string code, AppUserDTO user);    
}