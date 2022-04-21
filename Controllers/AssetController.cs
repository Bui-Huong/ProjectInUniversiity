using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AssetManagementWebApi.DTOs;
using AssetManagementWebApi.Models;
using AssetManagementWebApi.QueuingTasks;
using AssetManagementWebApi.Repositories.Entities.Enum;
using AssetManagementWebApi.Repositories.Requests;
using AssetManagementWebApi.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementWebApi.Controllers;
[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/asset")]
public class AssetController : ControllerBase
{
    private readonly IAssetService _assetService;
    private readonly IMapper _mapper;
    public AssetController(IAssetService assetService, IMapper mapper)
    {
        _assetService = assetService;
        _mapper = mapper;
    }
    [NonAction]
    public string? GetUserLocation()
    {
        try{
            ClaimsIdentity? claimsIdentity = User?.Identity as ClaimsIdentity;
            // return claimsIdentity;
            while (claimsIdentity == null)
            {
                return null;
            } 
            var location = claimsIdentity.FindFirst(ClaimTypes.Locality);
            if(location == null) return null;
            return location.Value;
        }catch (NullReferenceException e){
            return null;
        }
    }

    [HttpGet("list")]
    public async Task<ActionResult<AssetPagingResult<AssetDto>>> GetAssets(
        [FromQuery(Name = "search")] string? search,
        [FromQuery(Name = "category[]")] string[]? categoryPrefix,
        [FromQuery(Name = "state[]")] int[]? state,
        [FromQuery(Name = "pageIndex")] int? pageIndex,
        [FromQuery(Name = "sortByTitle")] string? sortByTitle,
        [FromQuery(Name = "sortField")] string? sortField,
        [FromQuery(Name = "pageSize")] int? pageSize
    )
    {
        var request = new AssetPagingRequest()
        {
            PageIndex = (int)(pageIndex.HasValue && pageIndex > 0 ? pageIndex : 1),
            PageSize = (int)(pageSize.HasValue && pageSize > 0 ? pageSize : 0),
            Search = !String.IsNullOrEmpty(search) ? search.ToLower() : null,
            Sort = String.IsNullOrEmpty(sortByTitle) || sortByTitle.ToLower() == "asc" ? "asc" : "desc",
            SortField = !String.IsNullOrEmpty(sortField) ? sortField.ToLower() : null,
            // CategoryPrefix = !String.IsNullOrEmpty(categoryPrefix) ? categoryPrefix.ToUpper() : null,
            CategoryPrefix =( categoryPrefix != null && categoryPrefix.Length > 0 ?
                            (from cate in categoryPrefix
                            select cate.ToUpper()).ToArray() :
                            new string[0] ),
            // State = state.HasValue ? state : 5
            State = state != null && state.Length > 0 ? state : null
        };
        var location = GetUserLocation();
        var listAssetDto = await _assetService.getAssetList(location, request);
        if (listAssetDto == null) return BadRequest();
        return Ok(listAssetDto);
    }

    [HttpGet("state")]
    public ActionResult GetState()
    {
        return Ok(_assetService.getAssetState());
    }
    [HttpGet("create/state")]
    public ActionResult GetCreateState()
    {
        return Ok(_assetService.getAssetCreateState());
    }
    [HttpGet("detail/{assetCode}")]
    public async Task<IActionResult> GetAssetDetail(string assetCode)
    {
        var location = GetUserLocation();
        var detailDto = await _assetService.getAssetDetail(location, assetCode);
        if (detailDto == null) return BadRequest();

        var history = new List<AssignmentModel>();
        foreach (AssignmentDto item in detailDto.HistoricalAssignments)
        {
            history.Add(
                _mapper.Map<AssignmentModel>(item)
            );
        }

        var detailModel = new AssetModel()
        {
            AssetCode = detailDto.AssetCode,
            AssetName = detailDto.AssetName,
            CategoryName = detailDto.Category.CategoryName,
            CategoryPrefix = detailDto.Category.CategoryPrefix,
            Specification = detailDto.Specification,
            InstalledDate = detailDto.InstalledDate,
            State = detailDto.State,
            HistoricalAssignments = history
        };
        return Ok(detailModel);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync(AssetCreateModel assetModel,[FromServices] SerialQueue serialQueue)
    {
        if (ModelState.IsValid)
        {
            var assetDto = new AssetDto()
            {
                AssetName = assetModel.AssetName,
                Specification = assetModel.Specification,
                InstalledDate = assetModel.InstalledDate,
                State = assetModel.State,
                CategoryPrefix = assetModel.CategoryPrefix,

            };
            if (assetDto != null)
            {   
                AssetDto result = null;
                var location = GetUserLocation();
                await serialQueue.Enqueue(() => result = _assetService.postAsset(location, assetDto).Result);
                if (result == null) return BadRequest();
                return Ok(result);
            }
            return BadRequest();
        }
        return BadRequest();
    }
    [HttpDelete("delete/{assetCode}")]
    public async Task<IActionResult> DeleteAsset(string assetCode)
    {
        var result = await _assetService.DeleteAsset(assetCode);
        if (result == "Asset Has Been Deleted") return Ok(result);
        return BadRequest(result);
    }
    [HttpPut("update")]
    public async Task<IActionResult> UpdateAsync(string assetCode, AssetModel assetModel)
    {
        var location = GetUserLocation();
        try
        {
            var currentAsset = await _assetService.getAssetDetail(location, assetCode);
            if (currentAsset == null || currentAsset.State == AssetState.Assigned) return NotFound();

            currentAsset.AssetName = assetModel.AssetName;
            currentAsset.Specification = assetModel.Specification;
            currentAsset.InstalledDate = assetModel.InstalledDate;
            currentAsset.State = assetModel.State;

            var assetUpdate = new AssetDto()
            {
                AssetName = currentAsset.AssetName,
                Specification = currentAsset.Specification,
                InstalledDate = currentAsset.InstalledDate,
                State = currentAsset.State,
            };
            await _assetService.editAssetAsync(assetCode, assetUpdate);
            if (currentAsset != null)
            {
                return Ok(currentAsset);
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}