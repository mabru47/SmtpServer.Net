using System.Threading.Tasks;

namespace Tireless.Net.Mail.Plugins
{
    public interface IInitializable
    {
        Task InitializeAsync();
    }
}
