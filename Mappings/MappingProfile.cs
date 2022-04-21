using AutoMapper;
using AssetManagementWebApi.DTOs;
using AssetManagementWebApi.Repositories.Entities;
using AssetManagementWebApi.Models;

namespace AssetManagementWebApi.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile(){
        CreateMap<AppUserDTO,AppUser>();
        CreateMap<AppUserModel,AppUserDTO>();
        CreateMap<AppUserDTO,AppUserModel>();
        CreateMap<AppUser,AppUserDTO>();

        CreateMap<AssignmentEntity, AssignmentDto>();
        CreateMap<AssignmentDto, AssignmentModel>();
        CreateMap<AssignmentDto, AssignmentEntity>();
        CreateMap<AssignmentModel, AssignmentDto>();

        CreateMap<AssetEntity, AssetDto>();
        CreateMap<AssetDto, AssetModel>();
        CreateMap<AssetDto, AssetCreateModel>();
    }
}