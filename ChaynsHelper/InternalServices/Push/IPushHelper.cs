using System.Threading.Tasks;

namespace ChaynsHelper.InternalServices.Push
{
    public interface IPushHelper
    {
        Task SendPushMessageToUser(PushMessage message);
    }
}