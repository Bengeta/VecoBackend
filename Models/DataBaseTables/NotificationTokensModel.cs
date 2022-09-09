using System.ComponentModel.DataAnnotations.Schema;

namespace VecoBackend.Models;

[Table("NotificationTokens")]
public class NotificationTokensModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }

    [ForeignKey("UserModel")] public int UserId { get; set; }
    public UserModel user { get; set; }
    public string Token { get; set; }
}