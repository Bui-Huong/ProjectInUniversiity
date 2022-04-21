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

namespace AssetManagementWebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AssignmentController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;
    private readonly IMapper _mapper;
    public AssignmentController(IAssignmentService assignmentService, IMapper mapper)
    {
        _assignmentService = assignmentService;
        _mapper = mapper;
    }
    [NonAction]
    public string GetUserLocation()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        return claimsIdentity.FindFirst(ClaimTypes.Locality).Value;
    }
    [NonAction]
    public string GetUserRole()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        return claimsIdentity.FindFirst(ClaimTypes.Role).Value;
    }
    [NonAction]
    public string GetUserCode()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var code = claimsIdentity.FindFirst(ClaimTypes.UserData).Value.Split(",")[0];
        return code;
    }
    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("list")]
    public async Task<IActionResult> Assignments(
        string? code,string? listOf,
        [FromQuery] SortRequest sortRequest,
        [FromQuery] FilterRequest filterRequest,
        [FromQuery(Name = "pageSize")] int pageSize,
        [FromQuery(Name = "pageIndex")] int pageIndex = 1

    )
    {
        var role = GetUserRole();
        var location = GetUserLocation();
        var userCode = GetUserCode();
        var request = new PagingRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        if (role == "Staff")
        {
            code = userCode;
            return Ok(await _assignmentService.GetAssignmentsAsync(role, location,"assignments", request, sortRequest, filterRequest, code));
        }
        return Ok(await _assignmentService.GetAssignmentsAsync(role, location,listOf,request, sortRequest, filterRequest, code));
    }
    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("detail")]
    public async Task<IActionResult> Assignment(Guid id)
    {   
        var code = GetUserCode();
        var role = GetUserRole();
        var result = await _assignmentService.GetAssignmentAsync(id,role,code);
        if (result != null)
        {
            return Ok(result);
        }
        return BadRequest("Not Found");
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateAssignment(AssignmentCreateModel model,[FromServices] SerialQueue serialQueue)
    {   
        string result = null;
        model.location = GetUserLocation();
        model.AssignerCode = GetUserCode();
        await serialQueue.Enqueue(() => result = _assignmentService.CreateAssignment(model).Result);
        bool isGuid = Guid.TryParse(result, out Guid output);
        if(isGuid) return Ok(result);
        return BadRequest(result);
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("edit/{id}")]
    public async Task<IActionResult> EditAssignment(AssignmentModel model, Guid id)
    {
        model.AssignerCode = GetUserCode();
        var result = await _assignmentService.EditAssignment(model, id);
        bool isGuid = Guid.TryParse(result, out Guid output);
        if(isGuid) return Ok(result);
        return BadRequest(result);
    }
    [Authorize(Roles = "Admin,Staff")]
    [HttpPut("respond")]
    public async Task<IActionResult> RespondASM(Guid id,string respond)
    {   
        var code = GetUserCode();
        var result = await _assignmentService.ResponseAssignment(id,respond,code);
        if(result == "Response assignment successfully!") return Ok(result);
        return BadRequest(result);
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteAssignmentAsync(Guid id)
    {
        var result = await _assignmentService.DeleteAssignmentAsync(id);
        if(result == "Assignment Has Been Delete Success") return Ok(result);
        return BadRequest(result);
    }
    [Authorize(Roles = "Admin,Staff")]
    [HttpPut("return")]
     public async Task<IActionResult> ReturnAsset(ReturningRequestModel model)
    {
        var result = await _assignmentService.ReturnAsset(model);
        if(result == "Return request created") return Ok(result);
        return BadRequest(result);
        
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("accept")]
     public async Task<IActionResult> AcceptReturnRequest(ReturningRequestModel model)
     {
         var result = await _assignmentService.AcceptReturnAsset(model);
        if(result == "Quest has been Accepted") return Ok(result);
        return BadRequest(result);
     }
    [Authorize(Roles = "Admin")]
    [HttpPut("reject")]
      public async Task<IActionResult> RejectReturnRequest(ReturningRequestModel model)
     {
        var result = await _assignmentService.RejectReturnAsset(model);
        if(result == "Quest has been Rejected") return Ok(result);
        return BadRequest(result);
     }
}
