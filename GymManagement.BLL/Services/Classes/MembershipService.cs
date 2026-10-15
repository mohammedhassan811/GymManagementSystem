using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MembershipViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembershipService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<MembershipViewModel>> GetAllMembershipsAsync(CancellationToken ct = default)
        {
            var membership =await _unitOfWork.MembershipRepository.GetMembershipWithMembersAndPlan(m => m.EndDate > DateTime.Now, ct);
            return _mapper.Map<IEnumerable<MembershipViewModel>>(membership);
        }
        public async Task<Result> CreateMembershipAsync(CreateMembershipViewModel model, CancellationToken ct = default)
        {
            var memberExists =await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Id == model.MemberId, ct);
            if (!memberExists)
                return Result.NotFound("Member Not Found ");

            var planExists =await _unitOfWork.GetRepository<Plan>().AnyAsync(p=>p.Id ==model.PlanId, ct);
            if (!planExists)
                return Result.NotFound("Plan Not Found ");

            var  hasActiveMembership =await _unitOfWork.MembershipRepository.AnyAsync(m=>m.MemberId == model.MemberId && m.EndDate>DateTime.Now, ct);
            if (hasActiveMembership)
                return Result.Fail("Member already has Active Membership");

            var plan =await _unitOfWork.GetRepository<Plan>().GetByIdAsync(model.PlanId, ct);
            if (!plan.IsActive)
                return Result.Fail("Plan Is Not Active");

            var membership =_mapper.Map<MemberShip>(model);
            membership.EndDate = (model.StartDate ?? DateTime.Now).AddDays(plan.DurationDays);

            _unitOfWork.MembershipRepository.Add(membership);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Create Membership");


        }

        public async Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropDownList(CancellationToken ct = default)
        {
            var members =await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<MemberSelectListViewModel>>(members);
        }

        public async Task<IEnumerable<PlanSelectListViewModel>> GetPlansForDropDownList(CancellationToken ct = default)
        {
            var plans =await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<PlanSelectListViewModel>>(plans);
        }

        public async Task<Result> DeleteActiveMembership(int memberId, CancellationToken ct = default)
        {
            var activeMembership =await _unitOfWork.MembershipRepository.FirstOrDefaultAsync(m=>m.MemberId == memberId && m.EndDate>DateTime.Now,true);
            if (activeMembership is null)
                return Result.Fail("Active Membership Not Found for this Member",ResultKind.NotFound);

            _unitOfWork.MembershipRepository.Delete(activeMembership);
            var result =await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Delete Membership");
        }

       

      
    }
}
