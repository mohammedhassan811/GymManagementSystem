using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagement.DAL.Data.Models;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsAndTrainerAndCategory(CancellationToken ct = default);
        Task<int> GetCountOfBookedSlots(int sessionId, CancellationToken ct = default);
        Task<Session?> GetSessionByIdWithTrainerAndCategory(int sessionId, CancellationToken ct = default);

    }
}
