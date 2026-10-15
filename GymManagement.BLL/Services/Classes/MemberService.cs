using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagement.BLL.Attachment;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberServices
    {
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork,IMapper mapper,IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            // Check Email
            var emailExist =await _unitOfWork.GetRepository<Member>().AnyAsync(x=>x.Email == model.Email , ct);
            //Check Phone
            var PhoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone , ct);
            // Email or phone Exist return false
            if(emailExist||PhoneExist) return Result.Validation("Cannot Create This Member,Phone or Email already exist");

            //upload Photo
            var storedPhotoName = await _attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "MembersPhoto");
            if (string.IsNullOrWhiteSpace(storedPhotoName.value)) return Result.Fail("Photo Name Is Empty");

            //Else return true add member
            var member = _mapper.Map<Member>(model);
            member.Photo = storedPhotoName.value;

            _unitOfWork.GetRepository<Member>().Add(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            if(result > 0)
            {
                return Result.Ok();
            }
            else
            {
                _attachmentService.Delete(storedPhotoName.value, "MembersPhoto");
               return  Result.Fail("Failed to Create Member");
            }

                
            
            
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMemberAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);

            if (!members.Any()) return [];
            var memberViewModels = _mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(members);
            return memberViewModels;
        }

        public async Task<Result<MemberViewModel>> GetMemberDetailsByIdAsync(int memberId, CancellationToken ct = default)
        {
            var member =await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);

            if (member == null) return Result<MemberViewModel>.NotFound("Member Not Found");
            var model = _mapper.Map<Member,MemberViewModel>(member);

            var activeMemberShip =await _unitOfWork.GetRepository<MemberShip>().FirstOrDefaultAsync(x => x.Id == memberId && x.EndDate > DateTime.Now);

            if(activeMemberShip is not null)
            {
                var activePlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMemberShip.PlanId, ct);
                model.PlanName = activePlan?.Name;
                model.MemberShipStartDate = activeMemberShip.CreatedAt.ToString();
                model.MemberShipEndDate = activeMemberShip.EndDate.ToString();
            }
            return Result<MemberViewModel>.Ok(model);


        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId , CancellationToken ct = default)
        {
            var record=await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(x=>x.Id == memberId,ct:ct );
            if (record == null) return null;
            else
                return _mapper.Map<HealthRecord, HealthRecordViewModel>(record);


        }

        public async Task<Result<MemberToUpdateViewModel>> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member =await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (member == null) return Result<MemberToUpdateViewModel>.NotFound("Member Not Found");
            
              var  mappedMember = _mapper.Map<Member, MemberToUpdateViewModel>(member);
            return Result<MemberToUpdateViewModel>.Ok(mappedMember);
        }

        public async Task<Result> RemoveMemberAsync(int memberId, CancellationToken ct = default)
        {
            var member =await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if(member == null) return Result.NotFound("Member Not Found");

            var hasFutureBooking =await _unitOfWork.GetRepository<Booking>().AnyAsync(x=>x.MemberId == memberId&& x.Session.StartDate > DateTime.Now ,ct);
            if (hasFutureBooking) return Result.Fail("Cannot Remove Member has Future Booking");

             _unitOfWork.GetRepository<Member>().Delete(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Remove Member");

        }

        public async Task<Result> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member =await _unitOfWork.GetRepository<Member>().GetByIdAsync(id,ct);

            if (member == null) return Result.NotFound("Member Not Found");

            var emailExist=await _unitOfWork.GetRepository<Member>().AnyAsync(x=>x.Email == model.Email&&x.Id!=x.Id);
            var phoneExist =await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone && x.Id != x.Id);

            if(emailExist || phoneExist) return Result.Validation("Email or Phone already Exist");

            _mapper.Map(model, member);
            member.UpdatedAt = DateTime.Now;

             _unitOfWork.GetRepository<Member>().Update(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Update Member");
            
        }
    }
}
