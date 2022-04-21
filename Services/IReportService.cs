using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetManagementWebApi.DTOs;
using AssetManagementWebApi.Repositories.Entities.Enum;
using AssetManagementWebApi.Repositories.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementWebApi.Services
{
    public interface IReportService
    {
        Task<PagingResult<ReportDTO>> ReportAsync(string location,PagingRequest request, SortRequest sortRequest = null );
        Task<FileContentResult> ExportExcel(string location, SortRequest? sortRequest);
    }
}