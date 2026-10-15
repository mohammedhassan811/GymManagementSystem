using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.PlanViewModel;
using GymManagement.DAL.Data.Models;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IPlanServices
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);
        Task<PlanViewModel?> GetPlanByIdAsync(int planId ,CancellationToken ct = default);
        Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default);
        Task<Result> ToggleActivationAsync(int planId,CancellationToken ct = default);
        Task<Result> UpdatePlanAsync(int planId,UpdatePlanViewModel model,CancellationToken ct= default);

    }
}
