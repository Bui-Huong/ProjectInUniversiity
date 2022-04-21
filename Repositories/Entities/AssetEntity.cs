using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AssetManagementWebApi.Repositories.Entities.Enum;

namespace AssetManagementWebApi.Repositories.Entities;

public class AssetEntity{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string? AssetCode { get; set; }
    public string? AssetName { get; set; }
    public string? Location { get; set; }
    public Guid CategoryId { get; set; }
    public CategoryEntity? Category { get; set; }
    public string? Specification { get; set; }
    [DataType(DataType.Date)]
    public DateTime InstalledDate { get; set; }
    public AssetState State { get; set; }
    public List<AssignmentEntity>? HistoricalAssignments { get; set; }
}