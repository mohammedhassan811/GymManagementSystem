using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.AnalyticsViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<AnalyticsViewModel> GetDataAsync(CancellationToken ct = default)
        {
            var now = DateTime.Now;
            var upComingSessions =await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate >  now);
            var onGoingSessions =await  _unitOfWork.GetRepository<Session>().CountAsync(s=>s.StartDate <= now && s.EndDate >= DateTime.Now);
            var completedSessions =await _unitOfWork.GetRepository<Session>().CountAsync(s => s.EndDate < now);
            var totalMembers =await _unitOfWork.GetRepository<Member>().CountAsync(ct: ct);
            var totalTrainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(ct: ct);
            var activeMember = await _unitOfWork.GetRepository<MemberShip>().CountAsync(x => x.EndDate > now, ct);

            return new AnalyticsViewModel()
            {
                TotalMembers = totalMembers,
                ActiveMembers = activeMember,
                CompletedSessions = completedSessions,
                OngoingSessions = onGoingSessions,
                UpcomingSessions = upComingSessions,
                TotalTrainers = totalTrainers,


            };

                
        }
    }
}
