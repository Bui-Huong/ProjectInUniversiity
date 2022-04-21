
using System.Data;
using System.Linq;
using AssetManagementWebApi.DTOs;
using AssetManagementWebApi.Repositories.EFContext;
using AssetManagementWebApi.Repositories.Entities.Enum;
using AssetManagementWebApi.Repositories.Requests;
using AssetManagementWebApi.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementWebApi.Services.Implements;
public class ReportService : IReportService
{   
    private readonly ILogger<ReportService> _logger;
    private readonly AssetManagementDBContext _assetManagementDBContext;
    public ReportService(AssetManagementDBContext assetManagementDBContext,
    ILogger<ReportService> logger)
    {
        _logger = logger;
        _assetManagementDBContext = assetManagementDBContext;
    }
    public async Task<PagingResult<ReportDTO>> ReportAsync(string location,PagingRequest pageRequest, SortRequest sortRequest = null )
    {   
        var categories = await _assetManagementDBContext.CategoryEntity.ToListAsync();
        var reports = new List<ReportDTO>();
        for(int i = 0; i < categories.Count; i++)
        {
            var assets = await _assetManagementDBContext.AssetEntity.Include(x => x.Category).Where(x => x.Location == location && x.Category.CategoryName == categories[i].CategoryName ).ToListAsync();
            var categoryName = categories.Find(x => x.Id == categories[i].Id).CategoryName;
            var totalCount = assets.Count();
            var availableCount = assets.Where(x => x.State == AssetState.Available ).Count() ;
            var notAvailableCount = assets.Where(x => x.State == AssetState.NotAvailable ).Count();
            var assigned = assets.Where(x => x.State == AssetState.Assigned ).Count();
            var waitingForRecyclingCount = assets.Where(x => x.State == AssetState.WaitingForRecycling ).Count();
            var recycledCount = assets.Where(x => x.State == AssetState.Recycled ).Count();
            var report = new ReportDTO(){
                CategoryName = categoryName,
                TotalCount = totalCount,
                AssignedCount = assigned,
                AvailableCount = availableCount,
                NotAvailableCount = notAvailableCount,
                WaitingForRecyclingCount = waitingForRecyclingCount,
                RecycledCount = recycledCount
            };
            reports.Add(report);
        }
        var result =  AddFilterQuery(sortRequest,reports.AsQueryable());
        List<ReportDTO> data;
        int total = result.Count();
        if(pageRequest.PageIndex != 0 && pageRequest.PageIndex != 0)
        {
            data = result.Skip((int)((pageRequest.PageIndex - 1) * pageRequest.PageSize)).Take((int)pageRequest.PageSize).ToList();
        }
        else{
            data = result.ToList();
        }
        var pageResult = new PagingResult<ReportDTO>()
        {
            Items = data,
            TotalRecords = total,
            PageSize = pageRequest.PageSize,
            PageIndex = pageRequest.PageIndex,
        };
        return pageResult;
    }
    private IQueryable<ReportDTO> AddFilterQuery(SortRequest? requestSort, IQueryable<ReportDTO> report)
    {
        if (!string.IsNullOrEmpty(requestSort.SortField))
        {
            if (requestSort.OrderBy == "ascend")
            {
                if (requestSort.SortField == "categoryName")
                {
                    report = report.OrderBy(x => x.CategoryName);
                }
                if (requestSort.SortField == "totalCount")
                {
                    report = report.OrderBy(x => x.TotalCount);
                }
                if (requestSort.SortField == "assignedCount")
                {
                    report = report.OrderBy(x => x.AssignedCount);
                }
                if (requestSort.SortField == "availableCount")
                {
                    report = report.OrderBy(x => x.AvailableCount);
                }
                if (requestSort.SortField == "notAvailableCount")
                {
                    report = report.OrderBy(x => x.NotAvailableCount);
                }
                if (requestSort.SortField == "waitingForRecyclingCount")
                {
                    report = report.OrderBy(x => x.WaitingForRecyclingCount);
                }
                if (requestSort.SortField == "recycledCount")
                {
                    report = report.OrderBy(x => x.RecycledCount);
                }
            }
            if (requestSort.OrderBy == "descend")
            {
                if (requestSort.SortField == "categoryName")
                {
                    report = report.OrderByDescending(x => x.CategoryName);
                }
                if (requestSort.SortField == "totalCount")
                {
                    report = report.OrderByDescending(x => x.TotalCount);
                }
                if (requestSort.SortField == "assignedCount")
                {
                    report = report.OrderByDescending(x => x.AssignedCount);
                }
                if (requestSort.SortField == "availableCount")
                {
                    report = report.OrderByDescending(x => x.AvailableCount);
                }
                if (requestSort.SortField == "notAvailableCount")
                {
                    report = report.OrderByDescending(x => x.NotAvailableCount);
                }
                if (requestSort.SortField == "waitingForRecyclingCount")
                {
                    report = report.OrderByDescending(x => x.WaitingForRecyclingCount);
                }
                if (requestSort.SortField == "recycledCount")
                {
                    report = report.OrderByDescending(x => x.RecycledCount);
                }
            }
        }
        if(string.IsNullOrEmpty(requestSort.SortField))
        {
            requestSort.SortField = "categoryName";
            requestSort.OrderBy = "ascend";
            report =  report.OrderBy(x => x.CategoryName);
        }
        return report;
    }
    public async Task<FileContentResult> ExportExcel(string location, SortRequest sortRequest = null)
    {   
        PagingRequest pageRequest = new PagingRequest(){
            PageIndex = 0,
            PageSize = 0,
        };
        DataTable dt = new DataTable("AssetsByCategoriesWithState");
        dt.Columns.AddRange(new DataColumn[7] { new DataColumn("Category"),
                                        new DataColumn("Total"),
                                        new DataColumn("Assigned"),
                                        new DataColumn("Available"),
                                        new DataColumn("Not available"),
                                        new DataColumn("Waiting for recycling"),
                                        new DataColumn("Recycled")});
        var report =await ReportAsync(location,pageRequest, sortRequest);
        foreach (var row in report.Items)
        {
            dt.Rows.Add(row.CategoryName,row.TotalCount,row.AssignedCount,row.AvailableCount,row.NotAvailableCount,row.WaitingForRecyclingCount,row.RecycledCount);
        }
        var today = DateTime.Now.ToString("dd/MM/yyyy");
        using (XLWorkbook wb = new XLWorkbook())
        {
            wb.Worksheets.Add(dt);
            using (MemoryStream stream = new MemoryStream())
            {
                wb.SaveAs(stream);
                return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"Asset-Sort-{sortRequest.SortField}-{sortRequest.OrderBy}-{today}"
                };
            }
        }
    }
}