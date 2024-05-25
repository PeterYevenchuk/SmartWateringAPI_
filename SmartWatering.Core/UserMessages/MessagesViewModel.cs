using SmartWatering.DAL.Models;

namespace SmartWatering.Core.UserMessages;

public class MessagesViewModel
{
    public List<MessageModel> Messages { get; set; }

    public int CountUnRead { get; set; }
}
