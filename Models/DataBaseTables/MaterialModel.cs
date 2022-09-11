using System.ComponentModel.DataAnnotations.Schema;
using VecoBackend.Enums;

namespace VecoBackend.Models;
[Table("Materials")]
public class MaterialModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Author { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public bool IsSeen { get; set; }
    public MaterialCategory Category { get; set; }
    public List<MaterialImageModel> Images { get; set; }
    

}