using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetManagementWebApi.DTOs;
using AssetManagementWebApi.Repositories.Entities.Enum;

namespace AssetManagementWebApi.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> getCategory();
        Task<CategoryErrorState> postCategory(CategoryDto categoryDto);
    }
}