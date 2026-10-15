using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.BookingViewModel;
using GymManagement.BLL.ViewModels.MembershipViewModel;
using GymManagement.BLL.ViewModels.SessionViewModel;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default);
        Task<IEnumerable<MemberForSessionViewModel>> GetMembersForSessionAsync(int sessionId, CancellationToken ct = default);
        Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropdownList(int sessionId, CancellationToken ct = default);
        Task<Result> CreateBookingAsync(CreateBookingViewModel model, CancellationToken ct = default);
        Task<Result> MarkIsAttendedAsync(int memberId, int sessionId, CancellationToken ct = default);
        Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default);

    }
}
