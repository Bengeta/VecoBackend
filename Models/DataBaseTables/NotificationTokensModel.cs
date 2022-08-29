using System.ComponentModel.DataAnnotations.Schema;

namespace VecoBackend.Models;
[Table("NotificationTokens")]
public class NotificationTokensModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; }
}