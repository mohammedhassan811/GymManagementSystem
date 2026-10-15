using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.SessionViewModel;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ISessionServices
    {
        Task<Result<SessionViewModel>> GetSessionByIdAsync(int sessionId, CancellationToken ct = default);
        Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default);
        Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default);
        Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDownAsync(CancellationToken ct= default);
        Task<IEnumerable<CategorySelectViewModel>> GetCategoryForDropDownAsync(CancellationToken ct = default);
        Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default);
        Task<Result> UpdateSessionAsync(int sessionId,UpdateSessionViewModel model,CancellationToken ct = default);
        Task<Result> DeleteSessionAsync(int sessionId, CancellationToken ct = default);


    }
}
