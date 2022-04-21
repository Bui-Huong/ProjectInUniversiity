
using AssetManagementWebApi.DTOs;
using AssetManagementWebApi.Models;
using AssetManagementWebApi.Repositories.Requests;

namespace AssetManagementWebApi.Services;
public interface IAssignmentService{
    Task<PagingResult<AssignmentDto>> GetAssignmentsAsync(
    string role, string location,string listOf,
    PagingRequest pageRequest, SortRequest requestSort = null,
    FilterRequest filterRequest = null, string code = null);
    Task<AssignmentDto> GetAssignmentAsync(Guid id,string role,string code);
    Task<string?> CreateAssignment(AssignmentCreateModel model);
    Task<string?> EditAssignment(AssignmentModel model,Guid id );
    Task<string> ResponseAssignment(Guid id, string respond,string code);
    Task<string> DeleteAssignmentAsync(Guid id);
    Task<string> ReturnAsset(ReturningRequestModel model);
    Task<string> AcceptReturnAsset(ReturningRequestModel model);
    Task<string> RejectReturnAsset(ReturningRequestModel model);
}