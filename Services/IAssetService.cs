using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetManagementWebApi.Repositories;
using AssetManagementWebApi.Repositories.Requests;
using AssetManagementWebApi.DTOs;
using AssetManagementWebApi.Repositories.Entities.Enum;

namespace AssetManagementWebApi.Services;
public interface IAssetService
{
    Task<AssetPagingResult<AssetDto>> getAssetList(string location,AssetPagingRequest request);

    Task<AssetDto> getAssetDetail (string location, string assetCode);
    Task<AssetDto> postAsset (string location, AssetDto assetDto);
    Dictionary<string, int> getAssetState ();
    Dictionary<string, int> getAssetCreateState ();
    Task<String> DeleteAsset (string assetCode);
    Task<AssetDto> editAssetAsync(string code, AssetDto asset);
}