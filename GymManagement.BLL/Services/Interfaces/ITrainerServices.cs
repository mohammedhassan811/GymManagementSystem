using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.TrainerViewModel;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ITrainerServices
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);
        Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default);
        Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId,CancellationToken ct = default);
        Task<Result> CreateTrainerAsync(CreateTrainerViewModel model,CancellationToken ct = default);
        Task<Result> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct = default);
        Task<Result> DeleteTrainerAsync(int trainerId, CancellationToken ct = default);



    }
}
