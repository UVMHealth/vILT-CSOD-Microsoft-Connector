using System.Threading.Tasks;

namespace vILT.Domain
{
/// <summary>
/// Interface to allow InstructorAPI and SessionApi to potentially use any meeting provider.
/// </summary>
    public interface IViltSessionService
    {
        Task CreateSessionAsync(SessionRequest request);
        Task UpdateSessionAsync(string id, SessionRequest request);
        Task<ViltSession> GetSessionAsync(string id);
        Task AddAttendeeAsync(string id, string email);
        Task<bool> AddInstructorAsync(AddInstructorRequest request);
        Task<bool> UpdateInstructorAsync(UpdateInstructorRequest request);
    }
}
