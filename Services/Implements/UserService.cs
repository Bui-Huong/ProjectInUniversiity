using AssetManagementWebApi.DTOs;
using AssetManagementWebApi.Repositories.EFContext;
using AssetManagementWebApi.Repositories.Entities;
using AssetManagementWebApi.Repositories.Entities.Enum;
using AssetManagementWebApi.Repositories.Requests;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AssetManagementWebApi.Services.Implements;
public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<UserService> _logger;
    private readonly AssetManagementDBContext _assetManagementDBContext;
    private readonly IMapper _mapper;
    public UserService(UserManager<AppUser> userManager,
    AssetManagementDBContext assetManagementDBContext,
    ILogger<UserService> logger,
    IMapper mapper)
    {
        _logger = logger;
        _userManager = userManager;
        _assetManagementDBContext = assetManagementDBContext;
        _mapper = mapper;
    }
    public AppUser GetByCode(string code) => _assetManagementDBContext.AppUser
    .Include(x => x.HistoricalAssignments.Where(x => x.AssigneeCode == code))
    .FirstOrDefault(x => x.Code == code && x.IsDisabled == false);
    public async Task<PagingResult<AppUserDTO>> GetUsersAsync(string location, PagingRequest pageRequest, SortRequest requestSort = null, FilterRequest filterRequest = null)
    {   
        List<AppUserDTO> data ;
        var users = _assetManagementDBContext.AppUser.Where(x => x.IsDisabled == false && x.Location == location);
        var result = AddFilterQuery(requestSort, filterRequest, users);
        int total = await result.CountAsync();
        if(pageRequest.PageIndex != 0 && pageRequest.PageIndex != 0)
        {
            data = await result.Skip((int)((pageRequest.PageIndex - 1) * pageRequest.PageSize)).Take((int)pageRequest.PageSize)
            .Select(x => _mapper.Map<AppUserDTO>(x)).ToListAsync();
        }
        else{
            data = await result.Select(x => _mapper.Map<AppUserDTO>(x)).ToListAsync();
        }
        var pageResult = new PagingResult<AppUserDTO>()
        {
            Items = data,
            TotalRecords = total,
            PageSize = pageRequest.PageSize,
            PageIndex = pageRequest.PageIndex,
        };
        return pageResult;
    }
    private IQueryable<AppUserDTO> AddFilterQuery(SortRequest? requestSort, FilterRequest? requestFilter, IQueryable<AppUser> userFilter)
    {
        if (!string.IsNullOrEmpty(requestSort.SortField))
        {
            if (requestSort.OrderBy == "ascend")
            {
                if (requestSort.SortField == "code")
                {
                    userFilter = userFilter.OrderBy(x => Convert.ToInt32(x.Code.Substring(x.Code.Length - 4)));
                }
                if (requestSort.SortField == "fullName")
                {
                    userFilter = userFilter.OrderBy(x => x.FirstName);
                }
                if (requestSort.SortField == "joinDate")
                {
                    userFilter = userFilter.OrderBy(x => x.JoinDate);
                }
                if (requestSort.SortField == "type")
                {
                    userFilter = userFilter.OrderBy(x => x.Type);
                }
            }
            if (requestSort.OrderBy == "descend")
            {
                if (requestSort.SortField == "code")
                {
                    userFilter = userFilter.OrderByDescending(x => Convert.ToInt32(x.Code.Substring(x.Code.Length - 4)));
                }
                if (requestSort.SortField == "fullName")
                {
                    userFilter = userFilter.OrderByDescending(x => x.FirstName);
                }
                if (requestSort.SortField == "joinDate")
                {
                    userFilter = userFilter.OrderByDescending(x => x.JoinDate);
                }
                if (requestSort.SortField == "type")
                {
                    userFilter = userFilter.OrderByDescending(x => x.Type);
                }
            }
        }
        if (!string.IsNullOrEmpty(requestFilter?.SearchValue))
        {
            requestFilter.SearchValue = requestFilter.SearchValue.ToLower().Trim();
            userFilter = userFilter.Where(x =>  (x.FirstName.ToLower().Trim() +" "+ x.LastName.ToLower().Trim()).Contains(requestFilter.SearchValue)
            || x.UserName.ToLower().Trim().Contains(requestFilter.SearchValue)
            || x.Code.ToLower().Trim().Contains(requestFilter.SearchValue));
        }

        if (requestFilter.Role != null)
        {
            userFilter = userFilter.Where(x => requestFilter.Role.Contains(x.Type));
        }
        return userFilter.Select(x => _mapper.Map<AppUserDTO>(x));
    }
    public async Task<AppUserDTO> GetUserAsync(string code)
    {
        var existUser = GetByCode(code);
        AppUserDTO result = null;
        if (existUser != null && existUser.IsDisabled == false)
        {
            result = new AppUserDTO()
            {
                Code = existUser.Code,
                FirstName = existUser.FirstName,
                LastName = existUser.LastName,
                HistoricalAssignments = existUser.HistoricalAssignments.Select(x => new AssignmentEntity()
                {
                    AssigneeCode = x.AssigneeCode,
                    AssignmentState = x.AssignmentState,
                    ReturnState = x.ReturnState
                }).ToList(),
                Type = existUser.Type,
                IsDisabled = existUser.IsDisabled,
                DoB = existUser.DoB,
                Gender = (Gender)existUser.Gender,
                JoinDate = existUser.JoinDate,
                Location = existUser.Location,
                UserName = existUser.UserName,
                IsFirstLogin = existUser.IsFirstLogin,
            };
            return result;
        }
        return null;
    }
    public async Task<string> DisableUserAsync(string code)
    {
        var existUser = GetByCode(code);
        if (existUser != null)
        {
            if (existUser.HistoricalAssignments == null)
            {
                existUser.IsDisabled = true;
                await _assetManagementDBContext.SaveChangesAsync();
                return "Disable user successful!";
            }
            else if (existUser.HistoricalAssignments != null)
            {
                foreach (var relatedAssignment in existUser.HistoricalAssignments)
                {
                    if (relatedAssignment.AssigneeCode == code &&
                    relatedAssignment.AssignmentState != AsmState.Declined
                    && relatedAssignment.ReturnState != ReturnState.Completed)
                    {
                        return "This user still have valid assignments!";
                    }
                }
                existUser.IsDisabled = true;
                await _assetManagementDBContext.SaveChangesAsync();
                return "Disable user successful!";
            }
        }
        return "This user is already disabled!";
    }
    public async Task<AppUserDTO> CreateUserAsync(string location, AppUserDTO user)
    {   
        var sea = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        AppUserDTO result = null;
        string LNfirst = null;
        string CountDuplicate = null;
        using var transaction = _assetManagementDBContext.Database.BeginTransaction();
        try
        {
            var newUser = new AppUser()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DoB = TimeZoneInfo.ConvertTimeFromUtc( user.DoB, sea),
                Gender = user.Gender,
                JoinDate = TimeZoneInfo.ConvertTimeFromUtc( user.JoinDate, sea),
                Location = location,
                Type = user.Type
            };
            var currentTotal = _assetManagementDBContext.AppUser.ToList().Count;
            newUser.Code = "SD" + (currentTotal + 1).ToString("D4");
            for (int i = 0; i < newUser.LastName.Split(' ').Length; i++)
            {
                LNfirst = LNfirst + newUser.LastName.Split(" ")[i][0];
            }
            CountDuplicate = _assetManagementDBContext.AppUser.Where(x => x.FirstName == user.FirstName && x.LastName == user.LastName).Count().ToString();
            if (CountDuplicate == "0")
            {
                CountDuplicate = "";
            }
            var tempFirstName = newUser.FirstName.ToLower().Split(" ");
            newUser.UserName = String.Join("", tempFirstName) + LNfirst.ToLower() + CountDuplicate;
            user.Password = newUser.UserName + "@" + newUser.DoB.ToString("ddMMyyyy");
            var existUserWithCode = _assetManagementDBContext.AppUser.FirstOrDefault(x => x.Code == newUser.Code);
            if(existUserWithCode != null) return null;
            await _userManager.CreateAsync(newUser, user.Password);
            await _assetManagementDBContext.AppUser.AddAsync(newUser);

            string role = user.Type == (Role)(0) ? "Admin" : "Staff";
            if (await _userManager.IsInRoleAsync(newUser, role) == false)
                await _userManager.AddToRoleAsync(newUser, role);

            await _assetManagementDBContext.SaveChangesAsync();
            await transaction.CommitAsync();
            result = new AppUserDTO()
            {
                Code = newUser.Code,
                UserName = newUser.UserName,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Location = newUser.Location,
                Type = newUser.Type,
                DoB = newUser.DoB,
                JoinDate = newUser.JoinDate,
            };
        }
        catch (Exception e)
        {
            _logger.LogError("Error");
        }
        return result;
    }

    public async Task<AppUserDTO> EditUserAsync(string code, AppUserDTO user)
    {   
        var sea = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        AppUserDTO result = null;
        var existUser = GetByCode(code);
        using var transaction = _assetManagementDBContext.Database.BeginTransaction();
        try
        {
            if (existUser == null) throw new IndexOutOfRangeException();
             if(existUser.IsFirstLogin == true)
            {
                existUser.Gender = user.Gender;
                existUser.JoinDate = TimeZoneInfo.ConvertTimeFromUtc(user.JoinDate, sea);
                existUser.Type = user.Type;
            }else{
                existUser.DoB = TimeZoneInfo.ConvertTimeFromUtc(user.DoB, sea);
                existUser.Gender = user.Gender;
                existUser.JoinDate = TimeZoneInfo.ConvertTimeFromUtc(user.JoinDate, sea);
                existUser.Type = user.Type;
            }
            var oldRole = _userManager.GetRolesAsync(existUser).Result[0].ToString();
            if (await _userManager.IsInRoleAsync(existUser, oldRole))
            {
                await _userManager.RemoveFromRoleAsync(existUser, oldRole);
            }
            string newRole = user.Type == (Role)(0) ? "Admin" : "Staff";

            if (!await _userManager.IsInRoleAsync(existUser, newRole))
            {
                await _userManager.AddToRoleAsync(existUser, newRole);
            }

            await _assetManagementDBContext.SaveChangesAsync();
            await transaction.CommitAsync();

            result = new AppUserDTO()
            {
                DoB = existUser.DoB,
                JoinDate = existUser.JoinDate,
                Type = existUser.Type,
            };

        }
        catch (Exception e)
        {
            _logger.LogError("Error");
        }

        return result;
    }
}