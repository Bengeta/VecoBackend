using System.ComponentModel.DataAnnotations.Schema;

namespace VecoBackend.Models;

[Table("Users")]
public class UserModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    public string token { get; set; }
    public string username { get; set; }

    public string password { get; set; }
    
    public string salt { get; set; }

    public string name { get; set; }

    public Boolean isAdmin { get; set; }
    
    public int points { get; set; }
    
    public string email { get; set; }
}