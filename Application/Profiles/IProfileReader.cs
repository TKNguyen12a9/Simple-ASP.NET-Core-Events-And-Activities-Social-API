using System.Threading.Tasks;

namespace Application.Profiles
{
    public interface IProfileReader
    {
        Task<AttendeeProfile> ReadProfile(string username);
    }
}