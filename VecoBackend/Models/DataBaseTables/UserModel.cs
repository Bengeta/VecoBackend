using System.ComponentModel.DataAnnotations.Schema;

namespace VecoBackend.Models;

[Table("Users")]
public class UserModel
{
    public int id { get; set; }

    public string name { get; set; }
    
    public Boolean isAdmin { get; set; }
}