using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetManagementWebApi.DTOs;
using AssetManagementWebApi.Models;
using AssetManagementWebApi.Repositories.Entities.Enum;
using AssetManagementWebApi.Repositories.Requests;
using AssetManagementWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementWebApi.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/category")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet("")]
        public async Task<ActionResult> GetCategory()
        {

            var listDto = await _categoryService.getCategory();
            if(listDto == null) return BadRequest();
            var listModel = (from dto in listDto
                            select new CategoryModel(){
                                CategoryName = dto.CategoryName,
                                CategoryPrefix = dto.CategoryPrefix
                            }).ToList();
            return Ok(listModel);
        }
        [HttpPost("")]
        public async Task<ActionResult> PostCategory(CategoryRequest model)
        {
            if(ModelState.IsValid){
                var cateDto = new CategoryDto() {
                    CategoryName = model.CategoryName,
                    CategoryPrefix = model.CategoryPrefix.ToUpper()
                };
                var result = await _categoryService.postCategory(cateDto);
                switch (result) {
                    case CategoryErrorState.CATEGORY_NAME_EXIST:{
                        return BadRequest(new { Message = "Category is already exist. Please enter a different category"});
                    }
                    case CategoryErrorState.CATEGORY_PREFIX_EXIST:{
                        return BadRequest(new { Message = "Prefix is already exist. Please enter a different prefix"});
                    }
                    case CategoryErrorState.CATEGORY_CREATE_SUCCESS:{
                        return Ok(new { Message = "Category create successfully"});
                    }
                    default: case CategoryErrorState.CATEGORY_CREATE_FAIL:{
                        return BadRequest(new { Message = "Category create fail"});
                    }
                }
            }
            return BadRequest();
        }
        
    }
}