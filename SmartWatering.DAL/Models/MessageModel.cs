using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartWatering.DAL.Models;

public class MessageModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public string DayTime { get; set; }

    [Required]
    public string DateTime { get; set; }

    [Required]
    public string Message { get; set; }

    public bool IsRead { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }
}
