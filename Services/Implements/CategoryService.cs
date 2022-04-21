using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetManagementWebApi.DTOs;
using AssetManagementWebApi.Repositories.EFContext;
using AssetManagementWebApi.Repositories.Entities;
using AssetManagementWebApi.Repositories.Entities.Enum;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementWebApi.Services.Implements
{
    public class CategoryService : ICategoryService
    {
        private readonly AssetManagementDBContext _dbContext;
        private readonly ILogger<AssetService> _logger;
        private readonly IConfiguration _config;
        public CategoryService(
            AssetManagementDBContext dbContext,
            ILogger<AssetService> logger,
            IConfiguration config
        )
        {
            _dbContext = dbContext;
            _logger = logger;
            _config = config;
        }

        public async Task<List<CategoryDto>> getCategory()
        {
            return await (_dbContext.CategoryEntity
                                    .Select(x => new CategoryDto()
                                    {
                                        CategoryName = x.CategoryName,
                                        CategoryPrefix = x.CategoryPrefix
                                    })
                                    .ToListAsync());
        }

        public async Task<CategoryErrorState> postCategory(CategoryDto categoryDto)
        {
            var category = new CategoryEntity()
            {
                CategoryName = categoryDto.CategoryName,
                CategoryPrefix = categoryDto.CategoryPrefix.ToUpper()
            };
            bool isNameExist = _dbContext.CategoryEntity.Where(x => x.CategoryName == category.CategoryName).Count() > 0;
            if (isNameExist) return CategoryErrorState.CATEGORY_NAME_EXIST;
            bool isPrefixExist = _dbContext.CategoryEntity.Where(x => x.CategoryPrefix == category.CategoryPrefix).Count() > 0;
            if (isPrefixExist) return CategoryErrorState.CATEGORY_PREFIX_EXIST;
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await _dbContext.CategoryEntity.AddAsync(category);
                _dbContext.SaveChanges();
                await transaction.CommitAsync();
                return CategoryErrorState.CATEGORY_CREATE_SUCCESS;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                _logger.LogError(e.Message);
                return CategoryErrorState.CATEGORY_CREATE_FAIL;
            }
        }
    }
}