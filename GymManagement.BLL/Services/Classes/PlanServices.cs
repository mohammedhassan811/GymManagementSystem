using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.BLL.Services.Classes
{
    public class PlanServices : IPlanServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans =await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct:ct);
            return plans.Select(p => new PlanViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                DurationDays = p.DurationDays,
                IsActive = p.IsActive,
                Price = p.price
            });
        }

        public async Task<PlanViewModel?> GetPlanByIdAsync(int planId, CancellationToken ct = default)
        {
            var plan =await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if (plan == null)
                return null;
            else
                return new PlanViewModel()
                {
                    Id = plan.Id,
                    Name = plan.Name,
                    Description = plan.Description,
                    DurationDays = plan.DurationDays,
                    IsActive = plan.IsActive,
                    Price = plan.price
                };

        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
        {
            var plan =await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if(plan == null||!plan.IsActive) return null;
            if (await HasActiveMemberShipsAsync(planId, ct)) return null;

            else
                return new UpdatePlanViewModel()
                {
                    Description = plan.Description,
                    DurationDays = plan.DurationDays,
                    PlanName = plan.Name,
                    Price = plan.price
                };
        }

        public async Task<Result> ToggleActivationAsync(int planId, CancellationToken ct = default)
        {
            var plan =await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId);
            if (plan == null) return Result.NotFound("Plan Not Found");
            if (plan.IsActive && await HasActiveMemberShipsAsync(planId, ct)) return Result.Validation("Failed , Plan Is Active or has Active MemberShip ");

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;
             _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed");
        }

        public async Task<Result> UpdatePlanAsync(int planId, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan =await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if (plan is null) return Result.NotFound("Plan Not Found");
            if(await HasActiveMemberShipsAsync(planId,ct)) return Result.Fail("Failed Because Plan has Active MemberShip");

            plan.DurationDays = model.DurationDays;
            plan.price = model.Price;
            plan.Description = model.Description;
            plan.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.Ok() : Result.Fail("Failed");

        }

        #region HelperMethods
        private async Task<bool> HasActiveMemberShipsAsync(int planId, CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<MemberShip>().AnyAsync(x => x.PlanId == planId && x.EndDate > DateTime.Now);
        }
        #endregion
    }

}
