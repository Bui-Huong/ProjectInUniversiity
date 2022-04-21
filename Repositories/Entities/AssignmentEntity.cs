using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AssetManagementWebApi.Repositories.Entities.Enum;

namespace AssetManagementWebApi.Repositories.Entities;

public class AssignmentEntity{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public List<AppUser>? RelatedUsers { get; set; }
    public string? Location { get; set; }
    public Guid AssetId { get; set; }
    public AssetEntity? Asset { get; set; }
    [DataType(DataType.Date)]
    public DateTime AssignedDate { get; set; }
    public string? Note { get; set; }
    public AsmState AssignmentState { get; set; }
    public string? AssigneeCode { get; set; }
    public string? AssignerCode { get; set; }
    public string? RequesterCode { get; set; }
    public string? VerifierCode { get; set; }
    [DataType(DataType.Date)]
    public DateTime ReturnDate { get; set; }
    public ReturnState ReturnState { get; set; }

}