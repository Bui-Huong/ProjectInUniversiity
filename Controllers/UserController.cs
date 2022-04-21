using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AssetManagementWebApi.DTOs;
using AssetManagementWebApi.Models;
using AssetManagementWebApi.QueuingTasks;
using AssetManagementWebApi.Repositories.Requests;
using AssetManagementWebApi.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AssetManagementWebApi.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [NonAction]
    public string GetUserLocation()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        return claimsIdentity.FindFirst(ClaimTypes.Locality).Value;
    }
    [NonAction]
    public string GetUserCode()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var code = claimsIdentity.FindFirst(ClaimTypes.UserData).Value.Split(",")[0];
        return code;
    }
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    public UserController(IMapper mapper, IUserService userService)
    {
        _mapper = mapper;
        _userService = userService;
    }
    [HttpGet("list")]
    public async Task<IActionResult> Users(
        [FromQuery] SortRequest sortRequest,
        [FromQuery] FilterRequest filterRequest,
        [FromQuery(Name = "pageSize")] int pageSize,
        [FromQuery(Name = "pageIndex")] int pageIndex
    )
    {
        var location = GetUserLocation();
        var request = new PagingRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        return Ok(await _userService.GetUsersAsync(location, request, sortRequest, filterRequest));
    }
    [HttpGet("details")]
    public async Task<IActionResult> UserDetails(string code)
    {
        var result = await _userService.GetUserAsync(code);
        if (result != null)
        {
            return Ok(result);
        }
        return NotFound();
    }
    [HttpPut("disable")]
    public async Task<IActionResult> DisableUser(string code)
    {
        var result = await _userService.DisableUserAsync(code);
        if(result == "Disable user successful!")  return Ok(result);
        return BadRequest(result);
    }
    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync(AppUserModel userModel,[FromServices] SerialQueue serialQueue)
    {
        if (ModelState.IsValid)
        {
            var userDTO = new AppUserDTO()
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                DoB = userModel.DoB,
                JoinDate = userModel.JoinDate,
                Type = userModel.Type,
                Gender = userModel.Gender,
            };
            var location = GetUserLocation();
            AppUserDTO result = null;
            await serialQueue.Enqueue(() => result =_userService.CreateUserAsync(location, userDTO).Result);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        return BadRequest();
    }
    [HttpPut("edit")]
    public async Task<IActionResult> UpdateAsync(string code, AppUserModel userModel)
    {
        try
        {
            var currentUser = await _userService.GetUserAsync(code);
            if (currentUser == null) return NotFound();
            if(currentUser.IsFirstLogin == true)
            {
                currentUser.JoinDate = userModel.JoinDate;
                currentUser.Gender = userModel.Gender;
                currentUser.Type = userModel.Type;
            }else{
                 currentUser.DoB = userModel.DoB;
                currentUser.JoinDate = userModel.JoinDate;
                currentUser.Gender = userModel.Gender;
                currentUser.Type = userModel.Type;
            }

            var userUpdate = new AppUserDTO()
            {
                DoB = currentUser.DoB,
                JoinDate = currentUser.JoinDate,
                Gender = currentUser.Gender,
                Type = currentUser.Type,
            };
            await _userService.EditUserAsync(code, userUpdate);
            if (currentUser != null)
            {
                return Ok(currentUser);
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}