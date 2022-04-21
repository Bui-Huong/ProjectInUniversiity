using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetManagementWebApi.DTOs;
using AssetManagementWebApi.Repositories.Entities;
using AssetManagementWebApi.Repositories.Requests;
using Microsoft.AspNetCore.Identity;
using AssetManagementWebApi.Repositories.EFContext;
using Microsoft.EntityFrameworkCore;
using AssetManagementWebApi.Repositories.Entities.Enum;
using AutoMapper;

namespace AssetManagementWebApi.Services.Implements;

public class AssetService : IAssetService
{
    private readonly AssetManagementDBContext _dbContext;
    private readonly ILogger<AssetService> _logger;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;
    public AssetService(
        AssetManagementDBContext dbContext,
        ILogger<AssetService> logger,
        IConfiguration config,
        IMapper mapper
    )
    {
        _dbContext = dbContext;
        _logger = logger;
        _config = config;
        _mapper = mapper;
    }

    public Dictionary<string, int> getAssetCreateState()
    {
        Dictionary<string, int> state = Enum.GetValues(typeof(AssetState))
                                        .Cast<AssetState>()
                                        .ToDictionary(k => k.ToString(), v => (int)v)
                                        .AsQueryable()
                                        .Where(x => x.Key == AssetState.Available.ToString() || x.Key == AssetState.NotAvailable.ToString())
                                        .ToDictionary(k => k.Key, v => v.Value);
        return state;
    }

    public async Task<AssetDto?> getAssetDetail(string location, string assetCode)
    {
        var assetEntity = await _dbContext.AssetEntity
                                            .Where(x => x.Location == location)
                                            .Where(x => x.AssetCode == assetCode)
                                            .FirstOrDefaultAsync();
        if (assetEntity == null) return null;
        _dbContext.Entry(assetEntity).Reference<CategoryEntity>(x => x.Category).Load();
        var assetId = assetEntity.Id;
        assetEntity.HistoricalAssignments = await _dbContext.AssignmentEntity
                                                            .Where(x => x.AssetId == assetId)
                                                            .ToListAsync();

        var history = new List<AssignmentDto>();
        foreach (AssignmentEntity item in assetEntity.HistoricalAssignments)
        {
            history.Add(
                _mapper.Map<AssignmentDto>(item)
            );
        }
        var assetDto = new AssetDto()
        {
            AssetCode = assetEntity.AssetCode,
            AssetName = assetEntity.AssetName,
            Category = assetEntity.Category,
            Specification = assetEntity.Specification,
            InstalledDate = assetEntity.InstalledDate,
            State = assetEntity.State,
            HistoricalAssignments = history
        };
        return assetDto;
    }

    public async Task<AssetPagingResult<AssetDto>> getAssetList(string location, AssetPagingRequest request)
    {
        var sortBy = request.Sort;
        var sortField = request.SortField;
        var search = request.Search;
        var categoryPrefix = request.CategoryPrefix;
        var state = request.State;
        var pageIndex = request.PageIndex;
        var pageSize = request.PageSize;
        var totalRecords = 0;

        var assetList = _dbContext.AssetEntity.Where(x => x.Location == location)
                                .Select(x => new AssetDto()
                                {
                                    AssetCode = x.AssetCode,
                                    AssetName = x.AssetName,
                                    Category = x.Category,
                                    CategoryName = x.Category.CategoryName,
                                    CategoryPrefix = x.Category.CategoryPrefix,
                                    Specification = x.Specification,
                                    InstalledDate = x.InstalledDate,
                                    State = x.State,
                                    HistoricalAssignments = null
                                });
        if (sortBy == "asc")
        {
            switch (sortField)
            {
                case "assetcode":
                    {
                        assetList = assetList.OrderBy(x => x.AssetCode);
                        break;
                    }
                case "assetname":
                    {
                        assetList = assetList.OrderBy(x => x.AssetName);
                        break;
                    }
                case "category":
                    {
                        assetList = assetList.OrderBy(x => x.Category.CategoryName);
                        break;
                    }
                case "state":
                    {
                        assetList = assetList.OrderBy(x => x.State);
                        break;
                    }
                default:
                    {
                        sortField = "assetcode";
                        assetList = assetList.OrderBy(x => x.AssetCode);
                        break;
                    }
            }
        }
        if (sortBy != "asc")
        {
            switch (sortField)
            {
                case "assetcode":
                    {
                        assetList = assetList.OrderByDescending(x => x.AssetCode);
                        break;
                    }
                case "assetname":
                    {
                        assetList = assetList.OrderByDescending(x => x.AssetName);
                        break;
                    }
                case "category":
                    {
                        assetList = assetList.OrderByDescending(x => x.Category.CategoryName);
                        break;
                    }
                case "state":
                    {
                        assetList = assetList.OrderByDescending(x => x.State);
                        break;
                    }
                default:
                    {
                        sortField = "assetcode";
                        assetList = assetList.OrderByDescending(x => x.AssetCode);
                        break;
                    }
            }
        }


        if (!String.IsNullOrEmpty(search))
        {
            assetList = assetList.Where(x => EF.Functions.Like(x.AssetCode, $"%{search}%") || EF.Functions.Like(x.AssetName, $"%{search}%"));
        }
        // if (!String.IsNullOrEmpty(categoryPrefix))
        if (categoryPrefix != null && categoryPrefix.Length > 0)
        {
            assetList = assetList.Where(x => categoryPrefix.Contains(x.Category.CategoryPrefix));
        }
        // if (Enum.IsDefined(typeof(AssetState), state))
        if (state != null && state.Length > 0)
        {
            assetList = assetList.Where(x => state.Contains((int)x.State));
        }
        else
        {   
            if(String.IsNullOrEmpty(search))
            {
                assetList = assetList.Where(x => x.State == (AssetState)0 || x.State == (AssetState)1 || x.State == (AssetState)2);
            }else{
                assetList = assetList.Where(x => x.State == (AssetState)0 || x.State == (AssetState)1 || x.State == (AssetState)2|| x.State == (AssetState)3|| x.State == (AssetState)4);
            }
        }

        totalRecords = assetList.Count();
        if (totalRecords < pageSize)
        {
            pageIndex = 1;
        }

        if (pageSize > 0)
        {
            assetList = assetList
                        .Skip((int)((pageIndex - 1) * pageSize))
                        .Take((int)pageSize);
        }
        var assetListPaging = await assetList.ToListAsync();

        var result = new AssetPagingResult<AssetDto>()
        {
            Items = assetListPaging,
            TotalRecords = totalRecords,
            PageIndex = pageIndex,
            PageSize = pageSize,
            Search = search,
            CategoryPrefix = categoryPrefix,
            Sort = sortBy,
            SortField = sortField,
            State = state
        };
        return result;
    }

    public Dictionary<string, int> getAssetState()
    {
        Dictionary<string, int> state = Enum.GetValues(typeof(AssetState))
                                        .Cast<AssetState>()
                                        .ToDictionary(k => k.ToString(), v => (int)v);
        return state;
    }

    public async Task<AssetDto> postAsset(string location, AssetDto assetDto)
    {   
        var sea = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var categoryPrefix = assetDto.CategoryPrefix;
            var newAsset = new AssetEntity()
            {
                Location = location,
                AssetCode = null,
                AssetName = assetDto.AssetName,
                Specification = assetDto.Specification,
                InstalledDate = TimeZoneInfo.ConvertTimeFromUtc(assetDto.InstalledDate, sea),
                State = assetDto.State,
                CategoryId = Guid.Empty
            };
            var category = await _dbContext.CategoryEntity.Where(x => x.CategoryPrefix == categoryPrefix).FirstOrDefaultAsync();
            if (category == null) throw new Exception("Category is not exists");

            newAsset.CategoryId = category.Id;

            var codeLength = categoryPrefix.Length + 6;
            var maxAssetCode = 0;
            var currentAssets = await _dbContext.AssetEntity.Where(x=>x.CategoryId == newAsset.CategoryId).CountAsync();
            if(currentAssets != 0)
            {
                var maxAsset = await _dbContext.AssetEntity.Where(x => x.CategoryId == newAsset.CategoryId)
                                                                .OrderByDescending(x => Convert.ToInt32(x.AssetCode.Substring(codeLength - 6)))
                                                                .FirstAsync();
                maxAssetCode = Convert.ToInt32(maxAsset.AssetCode.Substring(maxAsset.AssetCode.Length - 6)) + 1;
            }
            newAsset.AssetCode = $"{categoryPrefix.ToUpper()}{maxAssetCode.ToString("D6")}";
            var existAssetWithCode = _dbContext.AssetEntity.FirstOrDefault(x => x.AssetCode == newAsset.AssetCode);
            if(existAssetWithCode != null) throw new Exception("Duplicate Asset");
            _dbContext.AssetEntity.Add(newAsset);
            var result = _dbContext.SaveChanges();
            await transaction.CommitAsync();
            return new AssetDto()
            {
                AssetCode = newAsset.AssetCode,
                AssetName = newAsset.AssetName,
                Specification = newAsset.Specification,
                InstalledDate = newAsset.InstalledDate,
                State = newAsset.State,
                CategoryPrefix = category.CategoryPrefix,
                CategoryName = category.CategoryName,
                Category = category,
                HistoricalAssignments = new List<AssignmentDto>()
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            _logger.LogError(e.Message);
            return null;
        }
    }
    public async Task<String> DeleteAsset(string assetCode)
    {
        var assetEntity = await _dbContext.AssetEntity
                                           .Where(x => x.AssetCode == assetCode)
                                           .FirstOrDefaultAsync();
        if (assetEntity == null) return "No data found";
        var assetId = assetEntity.Id;
        assetEntity.HistoricalAssignments = await _dbContext.AssignmentEntity
                                                            .Where(x => x.AssetId == assetId)
                                                            .ToListAsync();

        var history = new List<AssignmentDto>();
        foreach (AssignmentEntity item in assetEntity.HistoricalAssignments)
        {
            history.Add(
                _mapper.Map<AssignmentDto>(item)
            );
        }
        if (history.Count() != 0) return "Can Not Delete Assigned Asset!";
        _dbContext.AssetEntity.Remove(assetEntity);
        await _dbContext.SaveChangesAsync();
        return "Asset Has Been Deleted";
    }
    public async Task<AssetDto> editAssetAsync(string assetCode, AssetDto asset)
    {   
        var sea = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        AssetDto result = null;
        var existAsset = await _dbContext.AssetEntity
                                          .Where(x => x.AssetCode == assetCode)
                                          .FirstOrDefaultAsync();
        using var transaction = _dbContext.Database.BeginTransaction();
        try
        {
            if (existAsset == null) throw new IndexOutOfRangeException();
            if (existAsset.State == AssetState.Assigned) return null;
            existAsset.AssetName = asset.AssetName;
            existAsset.Specification = asset.Specification;
            existAsset.InstalledDate = TimeZoneInfo.ConvertTimeFromUtc(asset.InstalledDate, sea);
            existAsset.State = asset.State;

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            result = new AssetDto()
            {
                AssetName = existAsset.AssetName,
                Specification = existAsset.Specification,
                InstalledDate = existAsset.InstalledDate,
                State = existAsset.State,
            };
        }
        catch (Exception e)
        {
            _logger.LogError("Error");
        }
        return result;
    }
}