using System.Linq;
using AssetManagementWebApi.DTOs;
using AssetManagementWebApi.Models;
using AssetManagementWebApi.Repositories.EFContext;
using AssetManagementWebApi.Repositories.Entities;
using AssetManagementWebApi.Repositories.Entities.Enum;
using AssetManagementWebApi.Repositories.Requests;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementWebApi.Services.Implements;
public class AssignmentService : IAssignmentService
{
    private readonly IMapper _mapper;
    private readonly ILogger<AssignmentService> _logger;
    private readonly AssetManagementDBContext _assetManagementDBContext;
    public AssignmentService(ILogger<AssignmentService> logger, AssetManagementDBContext assetManagementDBContext)
    {
        _logger = logger;
        _assetManagementDBContext = assetManagementDBContext;
    }
    public AppUser GetUserByCode(string code) => _assetManagementDBContext.AppUser.FirstOrDefault(x => x.Code == code);
    public AssetEntity GetAssetByCode(string code) => _assetManagementDBContext.AssetEntity.FirstOrDefault(x => x.AssetCode == code);
       public async Task<PagingResult<AssignmentDto>> GetAssignmentsAsync(
    string role, string location, string listOf,
    PagingRequest pageRequest= null, SortRequest requestSort = null,
    FilterRequest filterRequest = null, string code = null)
    {
        var sea = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, sea);
        IQueryable<AssignmentEntity> assignments = null;
        if (role == "Staff")
        {
            assignments = _assetManagementDBContext.AssignmentEntity.Where(x => x.AssigneeCode == code && x.AssignmentState != (AsmState)1
            && DateTime.Compare(x.AssignedDate.Date, currentTime.Date) <= 0 && x.ReturnState != (ReturnState)1);
        }
        if (role == "Admin")
        {
            if (code != null)
            {
                assignments = _assetManagementDBContext.AssignmentEntity.Where(x => x.AssigneeCode == code && x.AssignmentState != (AsmState)1
                && DateTime.Compare(x.AssignedDate.Date, currentTime.Date) <= 0 && x.ReturnState != (ReturnState)1);
            }
            if (code == null)
            {
                if (listOf == "assignments")
                {
                    assignments = _assetManagementDBContext.AssignmentEntity
                    .Where(x => x.Location == location && x.ReturnState != (ReturnState)1);
                }
                if (listOf == "returnRequests")
                {
                    assignments = _assetManagementDBContext.AssignmentEntity
                   .Where(x => x.Location == location);
                }
            }
        }
        IQueryable<AssignmentDto> result = null;
        if (listOf == "assignments")
        {
            result = AddFilterQuery(requestSort, filterRequest, assignments, "assignments");
        }
        if (listOf == "returnRequests")
        {
            result = AddFilterQuery(requestSort, filterRequest, assignments, "returnRequests").Where(x => x.ReturnState != ReturnState.AUnknown);
        }
        int total = await result.CountAsync();
        var data = await result.Skip((int)((pageRequest.PageIndex - 1) * pageRequest.PageSize)).Take((int)pageRequest.PageSize)
        .ToListAsync();
        var pageResult = new PagingResult<AssignmentDto>()
        {
            Items = data,
            TotalRecords = total,
            PageSize = pageRequest.PageSize,
            PageIndex = pageRequest.PageIndex,
        };
        return pageResult;
    }
 private IQueryable<AssignmentDto> AddFilterQuery(SortRequest? requestSort, FilterRequest? requestFilter, IQueryable<AssignmentEntity>? asmFilter, string dataOf)
    {
        var sea = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        if (!string.IsNullOrEmpty(requestSort.SortField))
        {   
            if(requestSort.OrderBy == null)
            {
                asmFilter = asmFilter.OrderBy(x => x.Asset.AssetCode);
            }
            if (requestSort.OrderBy == "ascend")
            {
                switch (requestSort.SortField)
                {
                    case "assigneeUserName":
                        {
                            asmFilter = asmFilter.OrderBy(x => x.RelatedUsers.FirstOrDefault(a => a.Code == x.AssigneeCode).UserName);
                            break;
                        }
                    case "assignerUserName":
                        {
                            asmFilter = asmFilter.OrderBy(x => x.RelatedUsers.FirstOrDefault(a => a.Code == x.AssignerCode).UserName);
                            break;
                        }
                    case "assignedDate":
                        {
                            asmFilter = asmFilter.OrderBy(x => x.AssignedDate);
                            break;
                        }
                    case "assignmentState":
                        {
                            asmFilter = asmFilter.OrderBy(x => x.AssignmentState);
                            break;
                        }
                    case "assetCode":
                        {
                            asmFilter = asmFilter.OrderBy(x => x.Asset.AssetCode);
                            break;
                        }
                    case "assetName":
                        {
                            asmFilter = asmFilter.OrderBy(x => x.Asset.AssetName);
                            break;
                        }
                    case "returnDate":
                        {
                            asmFilter = asmFilter.OrderBy(x => x.ReturnDate);
                            break;
                        }
                    case "requesterUserName":
                        {
                            asmFilter = asmFilter.OrderBy(x => x.RelatedUsers.FirstOrDefault(a => a.Code == x.RequesterCode).UserName);
                            break;
                        }
                    case "verifierUserName":
                        {
                            asmFilter = asmFilter.OrderBy(x => x.RelatedUsers.FirstOrDefault(a => a.Code == x.VerifierCode).UserName);
                            break;
                        }
                    case "returnState":
                        {
                            asmFilter = asmFilter.OrderBy(x => x.ReturnState);
                            break;
                        }
                    default:  break;
                    
                }
            }
            if (requestSort.OrderBy == "descend")
            {
                switch (requestSort.SortField)
                {
                    case "assigneeUserName":
                        {
                            asmFilter = asmFilter.OrderByDescending(x => x.RelatedUsers.FirstOrDefault(a => a.Code == x.AssigneeCode).UserName);
                            break;
                        }
                    case "assignerUserName":
                        {
                            asmFilter = asmFilter.OrderByDescending(x => x.RelatedUsers.FirstOrDefault(a => a.Code == x.AssignerCode).UserName);
                            break;
                        }
                    case "assignedDate":
                        {
                            asmFilter = asmFilter.OrderByDescending(x => x.AssignedDate);
                            break;
                        }
                    case "assignmentState":
                        {
                            asmFilter = asmFilter.OrderByDescending(x => x.AssignmentState);
                            break;
                        }
                    case "assetCode":
                        {
                            asmFilter = asmFilter.OrderByDescending(x => x.Asset.AssetCode);
                            break;
                        }
                    case "assetName":
                        {
                            asmFilter = asmFilter.OrderByDescending(x => x.Asset.AssetName);
                            break;
                        }
                    case "returnDate":
                        {
                            asmFilter = asmFilter.OrderByDescending(x => x.ReturnDate);
                            break;
                        }
                    case "requesterUserName":
                        {
                            asmFilter = asmFilter.OrderByDescending(x => x.RelatedUsers.FirstOrDefault(a => a.Code == x.RequesterCode).UserName);
                            break;
                        }
                    case "verifierUserName":
                        {
                            asmFilter = asmFilter.OrderByDescending(x => x.RelatedUsers.FirstOrDefault(a => a.Code == x.VerifierCode).UserName);
                            break;
                        }
                    case "returnState":
                        {
                            asmFilter = asmFilter.OrderByDescending(x => x.ReturnState);
                            break;
                        }
                    default: 
                        break;
                }
            }
        }
        if(string.IsNullOrEmpty(requestSort.SortField))
        {   
            requestSort.SortField ="assetCode";
            asmFilter = asmFilter.OrderBy(x => x.Asset.AssetCode);
        }
        if (!string.IsNullOrEmpty(requestFilter?.SearchValue))
        {
            requestFilter.SearchValue = requestFilter.SearchValue.ToLower().Trim();
            if (dataOf == "assignments")
            {
                asmFilter = asmFilter.Where(x => x.Asset.AssetCode.ToLower().Trim().Contains(requestFilter.SearchValue)
                || x.Asset.AssetName.ToLower().Trim().Contains(requestFilter.SearchValue)
                || x.RelatedUsers.FirstOrDefault(a => a.Code == x.AssigneeCode).UserName.Contains(requestFilter.SearchValue));
            }
            if (dataOf == "returnRequests")
            {
                asmFilter = asmFilter.Where(x => x.Asset.AssetCode.ToLower().Trim().Contains(requestFilter.SearchValue)
               || x.Asset.AssetName.ToLower().Trim().Contains(requestFilter.SearchValue)
               || x.RelatedUsers.FirstOrDefault(a => a.Code == x.RequesterCode).UserName.Contains(requestFilter.SearchValue));
            }
        }
        if (dataOf == "assignments")
        {
            if (requestFilter.AsmState != null)
            {
                asmFilter = asmFilter.Where(x => requestFilter.AsmState.Contains(x.AssignmentState));
            }
            if (requestFilter.FilterDate != null)
            {
                asmFilter = asmFilter.Where(x => x.AssignedDate.Date == requestFilter.FilterDate.Value.Date);
            }
            if(requestFilter.AsmState == null)
            {
                asmFilter = asmFilter.Where(x => x.AssignmentState == AsmState.WaitingForAcceptance || x.AssignmentState == AsmState.Accepted);
            }
        }
        if (dataOf == "returnRequests")
        {
            if (requestFilter.ReturnState != null)
            {
                asmFilter = asmFilter.Where(x => requestFilter.ReturnState.Contains(x.ReturnState));
            }
            if (requestFilter.FilterDate != null)
            {
                asmFilter = asmFilter.Where(x => x.ReturnDate.Date == requestFilter.FilterDate.Value.Date);
            }
        }

        return dataOf == "assignments" ?
        asmFilter.Include(x => x.Asset).AsSingleQuery().Select(x => new AssignmentDto()
        {
            Id = x.Id,
            // SerialNumber = asmFilter.ToList().IndexOf(x) + 1,
            AssetCode = x.Asset.AssetCode,
            AssetName = x.Asset.AssetName,
            AssigneeCode = x.AssigneeCode,
            AssigneeUserName = x.RelatedUsers.FirstOrDefault(a => a.Code == x.AssigneeCode).UserName,
            AssignerCode = x.AssignerCode,
            AssignerUserName = x.RelatedUsers.FirstOrDefault(a => a.Code == x.AssignerCode).UserName,
            AssignedDate = x.AssignedDate,
            AssignmentState = x.AssignmentState,
            ReturnState = x.ReturnState,
            Note = x.Note,
            Specification = x.Asset.Specification,
        })
        : asmFilter.Include(x => x.Asset).AsSingleQuery().Select(x => new AssignmentDto()
        {
            Id = x.Id,
            // SerialNumber = asmFilter.ToList().IndexOf(x) + 1,
            AssetCode = x.Asset.AssetCode,
            AssetName = x.Asset.AssetName,
            RequesterCode = x.RequesterCode,
            RequesterUserName = x.RelatedUsers.FirstOrDefault(a => a.Code == x.RequesterCode).UserName,
            VerifierCode = x.VerifierCode,
            VerifierUserName = x.RelatedUsers.FirstOrDefault(a => a.Code == x.VerifierCode).UserName,
            ReturnDate = x.ReturnDate,
            AssignedDate = x.AssignedDate,
            ReturnState = x.ReturnState,
            Note = x.Note,
            Specification = x.Asset.Specification,
        });
    }
    public async Task<AssignmentDto> GetAssignmentAsync(Guid id,string role,string code)
    {   
        AssignmentEntity asm = new AssignmentEntity();
        if(role == "Admin")
        {
            asm = await _assetManagementDBContext.AssignmentEntity.Include(x => x.Asset).AsSingleQuery()
            .Include(x => x.RelatedUsers).AsSingleQuery()
            .FirstOrDefaultAsync(x => x.Id == id && x.ReturnState != (ReturnState)1);
        }
        if(role == "Staff")
        {
            asm = await _assetManagementDBContext.AssignmentEntity.Include(x => x.Asset).AsSingleQuery()
            .Include(x => x.RelatedUsers).AsSingleQuery()
            .FirstOrDefaultAsync(x => x.Id == id && x.AssigneeCode == code && x.ReturnState != (ReturnState)1);
        }
        AssignmentDto result = null;
        if (asm != null)
        {
            result = new AssignmentDto
            {
                Id = asm.Id,
                AssetCode = asm.Asset.AssetCode,
                AssetName = asm.Asset.AssetName,
                AssigneeCode = asm.AssigneeCode,
                AssigneeUserName = asm.RelatedUsers.FirstOrDefault(a => a.Code == asm.AssigneeCode).UserName,
                AssignerCode = asm.AssignerCode,
                AssignerUserName = asm.RelatedUsers.FirstOrDefault(a => a.Code == asm.AssignerCode).UserName,
                AssignedDate = asm.AssignedDate,
                AssignmentState = asm.AssignmentState,
                ReturnState = asm.ReturnState,
                Note = asm.Note,
                Specification = asm.Asset.Specification,
            };
        }
        return result;
    }
    public async Task<string?> EditAssignment(AssignmentModel model, Guid id)
    {   
        var sea = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var targetAsset = await _assetManagementDBContext.AssetEntity.FirstOrDefaultAsync(x => x.AssetCode == model.AssetCode);
        if (targetAsset == null) return "No Asset Found";

        var targetAssignee = await _assetManagementDBContext.AppUser.Where( x => x.IsDisabled == false ).FirstOrDefaultAsync(x => x.Code == model.AssigneeCode);
        if (targetAssignee == null) return "No Assignee Found";
        if(targetAssignee.IsDisabled) return "This user has been disabled!";
        var targetAssigner = await _assetManagementDBContext.AppUser.Where( x => x.IsDisabled == false ).FirstOrDefaultAsync(x => x.Code == model.AssignerCode);
        if (targetAssigner == null) return "No Assigner Found";

        var entity = await _assetManagementDBContext.AssignmentEntity.Include(x => x.RelatedUsers).FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return "No Assingment Found";
        if (entity.AssignmentState != AsmState.WaitingForAcceptance) return "Assignment can not be edited";
        if (entity.ReturnState != ReturnState.AUnknown) return "Assignment can not be edited";
        var oldAsset = await _assetManagementDBContext.AssetEntity.FirstOrDefaultAsync(x => x.Id == entity.AssetId);
        oldAsset.State = AssetState.Available;

        if (targetAsset.State != AssetState.Available) return "Asset Not Availble";

        var newList = new List<AppUser>();
        newList.Add(targetAssignee);
        newList.Add(targetAssigner);
        entity.RelatedUsers = new List<AppUser>();
        entity.RelatedUsers = newList;
        entity.AssetId = targetAsset.Id;
        entity.AssignedDate = TimeZoneInfo.ConvertTimeFromUtc(model.AssignedDate, sea);
        entity.Note = model.Note;
        entity.AssigneeCode = model.AssigneeCode;
        entity.AssignerCode = model.AssignerCode;
        targetAsset.State = AssetState.Assigned;

        _assetManagementDBContext.Entry(entity).State = EntityState.Modified;
        await _assetManagementDBContext.SaveChangesAsync();


        return entity.Id.ToString();
    }
    public async Task<string?> CreateAssignment(AssignmentCreateModel model)
    {   
        var sea = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var targetAsset = await _assetManagementDBContext.AssetEntity.FirstOrDefaultAsync(x => x.AssetCode == model.AssetCode);
        if (targetAsset == null) return "No Asset Found";
        if(targetAsset.State != AssetState.Available) return "Only availble asset can be assigned";

        var targetAssignee = await _assetManagementDBContext.AppUser.Where( x => x.IsDisabled == false ).FirstOrDefaultAsync(x => x.Code == model.AssigneeCode);
        if (targetAssignee == null) return "No Assignee Found";
        if(targetAssignee.IsDisabled) return "This user has been disabled!";
        var targetAssigner = await _assetManagementDBContext.AppUser.Where( x => x.IsDisabled == false ).FirstOrDefaultAsync(x => x.Code == model.AssignerCode);
        if (targetAssigner == null) return "No Assigner Found";

        var newList = new List<AppUser>();
        newList.Add(targetAssignee);
        newList.Add(targetAssigner);

        var entity = new AssignmentEntity
        {
            Location = model.location,
            AssetId = targetAsset.Id,
            AssignedDate = TimeZoneInfo.ConvertTimeFromUtc(model.AssignedDate, sea),
            Note = model.Note,
            AssignmentState = AsmState.WaitingForAcceptance,
            AssigneeCode = model.AssigneeCode,
            AssignerCode = model.AssignerCode,
            RelatedUsers = newList

        };
        await _assetManagementDBContext.AssignmentEntity.AddAsync(entity);
        targetAsset.State = AssetState.Assigned;
        await _assetManagementDBContext.SaveChangesAsync();

        return entity.Id.ToString();
    }
    public async Task<string> ResponseAssignment(Guid id, string respond,string code)
    {   
        var existAssignment = await _assetManagementDBContext.AssignmentEntity.Include(x => x.Asset).AsSingleQuery().FirstOrDefaultAsync(x => x.Id == id && x.AssigneeCode == code);
        var sea = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, sea);
        if (existAssignment != null)
        {
            if (existAssignment.AssignmentState == AsmState.Accepted)
            {
                return "You already accepted this assignments!";
            }
            if (existAssignment.AssignmentState == AsmState.Declined)
            {
                return "You already declined this assignments!";
            }
            if(existAssignment.AssigneeCode != code)
            {
                return "Your assignment has been assigned to another user!";
            }
            if(DateTime.Compare(existAssignment.AssignedDate.Date, currentTime.Date)>0)
            {
                return "You can not respond assignment in the future!";
            }
            if (respond == "Accepted")
            {
                existAssignment.AssignmentState = AsmState.Accepted;
                _assetManagementDBContext.Entry(existAssignment).State = EntityState.Modified;
                await _assetManagementDBContext.SaveChangesAsync();
                return "Response assignment successfully!";
            }
            if (respond == "Declined")
            {   
                var existValidAsm = _assetManagementDBContext.AssignmentEntity.AsSingleQuery().FirstOrDefault(x => x.Id != existAssignment.Id && x.AssetId == existAssignment.AssetId && x.AssignmentState == AsmState.WaitingForAcceptance);
                if(existValidAsm != null)
                {
                    existAssignment.AssignmentState = AsmState.Declined;
                    _assetManagementDBContext.Entry(existAssignment).State = EntityState.Modified;
                    await _assetManagementDBContext.SaveChangesAsync();
                    return "Response assignment successfully!";
                }else{
                    existAssignment.AssignmentState = AsmState.Declined;
                    _assetManagementDBContext.Entry(existAssignment).State = EntityState.Modified;
                    if(existAssignment?.Asset?.State == AssetState.Assigned)
                    {
                        existAssignment.Asset.State = AssetState.Available;
                    }
                    await _assetManagementDBContext.SaveChangesAsync();
                    return "Response assignment successfully!";
                }
            }
        }
        return "This assignment is already delete or You can't respond assignment of others!";
    }
    public async Task<string> DeleteAssignmentAsync(Guid id)
    {
        var assignmentToDelete = await _assetManagementDBContext.AssignmentEntity.Include(x => x.Asset).AsSingleQuery()
                                                                                .Include(x => x.RelatedUsers).AsSingleQuery()
                                                                                .FirstOrDefaultAsync(x => x.Id == id);
        
        var stateOfAssetToChange = assignmentToDelete?.Asset?.AssetCode;

        if(assignmentToDelete == null) 
        {
            return "Not Found assignment";
        }

        var stateOfAssignment = assignmentToDelete.AssignmentState.ToString();

        if(stateOfAssignment == "Accepted") return "Can not delete assignment when state is accepted";
         var stateReturnOfAssignment = assignmentToDelete.ReturnState.ToString();

        if(stateReturnOfAssignment != "AUnknown") return "Can not delete assignment when state is accepted";
        if(stateOfAssignment == "WaitingForAcceptance")
        {
            _assetManagementDBContext.Remove(assignmentToDelete);
            if(assignmentToDelete?.Asset?.State == AssetState.Assigned)
            {
                assignmentToDelete.Asset.State = AssetState.Available;
            }
        }
        if(stateOfAssignment == "Declined")
        {
            var existValidAsm = _assetManagementDBContext.AssignmentEntity.FirstOrDefault(x => x.Id != assignmentToDelete.Id && x.AssetId == assignmentToDelete.AssetId && x.AssignmentState == AsmState.WaitingForAcceptance);
            if(existValidAsm != null)
            {
                _assetManagementDBContext.Remove(assignmentToDelete);
            }else{
                _assetManagementDBContext.Remove(assignmentToDelete);
                if(assignmentToDelete?.Asset?.State == AssetState.Assigned)
                {
                    assignmentToDelete.Asset.State = AssetState.Available;
                }
            }
        }
        await _assetManagementDBContext.SaveChangesAsync();

        return "Assignment Has Been Delete Success";
    }
    public async Task<string> ReturnAsset(ReturningRequestModel model)
    {   
        var sea = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var assignment = await _assetManagementDBContext.AssignmentEntity.Include(x => x.RelatedUsers).FirstOrDefaultAsync(x => x.Id == model.assetId);
        if (assignment == null) return "No Assingment Found";
        if(assignment.RequesterCode != null) return "Request for this asset has already created";
        var requester = await _assetManagementDBContext.AppUser.Where(x => x.IsDisabled == false).FirstOrDefaultAsync(x => x.Code == model.reuesterCode);
        if (requester == null) return "Requester not exist";

        var targetAsset = await _assetManagementDBContext.AssetEntity.FirstOrDefaultAsync(x => x.Id == assignment.AssetId);
        if(assignment.AssignmentState != AsmState.Accepted) return "Only Accepted Asset can be return";
        
        assignment.ReturnState = ReturnState.WaitingForReturning;
        assignment.RequesterCode = model.reuesterCode;
        assignment.RelatedUsers.Add(requester);
        await _assetManagementDBContext.SaveChangesAsync();
        return "Return request created";
    }
    public async Task<string> AcceptReturnAsset(ReturningRequestModel model)
    {   
        var sea = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var assignment = await _assetManagementDBContext.AssignmentEntity.Include(x => x.RelatedUsers).FirstOrDefaultAsync(x => x.Id == model.assetId);
        if (assignment == null) return "No Assingment Found";

        var verifier = await _assetManagementDBContext.AppUser.FirstOrDefaultAsync(x => x.Code == model.reuesterCode);
        if (verifier == null) return "Verifier not exist";

        var targetAsset = await _assetManagementDBContext.AssetEntity.FirstOrDefaultAsync(x => x.Id == assignment.AssetId);

        if(assignment.ReturnState != ReturnState.WaitingForReturning) return "This Request has been Modified!";
        if(targetAsset.State != AssetState.Assigned) return "This Asset has been Modified!";
        assignment.VerifierCode = verifier.Code;
        assignment.RelatedUsers.Add(verifier);
        targetAsset.State = AssetState.Available;
        assignment.ReturnDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, sea);
        assignment.ReturnState = ReturnState.Completed;
        await _assetManagementDBContext.SaveChangesAsync();
        return "Quest has been Accepted";

    }
    public async Task<string> RejectReturnAsset(ReturningRequestModel model)
    {
        var assignment = await _assetManagementDBContext.AssignmentEntity.Include(x => x.RelatedUsers).FirstOrDefaultAsync(x => x.Id == model.assetId);
        if (assignment == null) return "No Assingment Found";

        var verifier = await _assetManagementDBContext.AppUser.FirstOrDefaultAsync(x => x.Code == model.reuesterCode);
        if (verifier == null) return "Verifier not exist";

        var targetAsset = await _assetManagementDBContext.AssetEntity.FirstOrDefaultAsync(x => x.Id == assignment.AssetId);

        if(assignment.ReturnState != ReturnState.WaitingForReturning) return "This Request has been Modified!";
        if(targetAsset.State != AssetState.Assigned) return "This Asset has been Modified!";

        assignment.ReturnState = ReturnState.AUnknown;
        assignment.RequesterCode = null;
        await _assetManagementDBContext.SaveChangesAsync();
        return "Quest has been Rejected";
    }
}