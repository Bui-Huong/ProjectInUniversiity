using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagementWebApi.Repositories.Entities;

public class CategoryEntity{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryPrefix { get; set; }
}