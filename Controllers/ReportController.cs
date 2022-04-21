using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using AssetManagementWebApi.DTOs;
using AssetManagementWebApi.Models;
using AssetManagementWebApi.Repositories.Requests;
using AssetManagementWebApi.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementWebApi.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {      
        [NonAction]
        public string GetUserLocation()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            return claimsIdentity.FindFirst(ClaimTypes.Locality).Value;
        }
        private readonly IReportService? _reportService;
        public ReportController(IReportService? reportService)
        {
            _reportService = reportService;
        }
        [HttpGet("list")]
        public async Task<IActionResult> Reports( 
            [FromQuery] SortRequest sortRequest,
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
            return Ok(await _reportService.ReportAsync(location, request, sortRequest));
        }
        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportExcel( [FromQuery] SortRequest sortRequest)
        {   
            var location = GetUserLocation();
            return Ok(await _reportService.ExportExcel(location,sortRequest));
        }

    }

}
