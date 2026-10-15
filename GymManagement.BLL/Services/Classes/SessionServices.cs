using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Data.Models.Enums;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionServices : ISessionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionServices(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate <= model.StartDate) return Result.Validation("EndDate Must Be After StartDate ");
            if(model.StartDate <= DateTime.Now) return Result.Validation("StartDate Must Be In The Future ");
            if (model.Capacity < 1 || model.Capacity > 25) return Result.Validation("Capacity Must be Between 1 and 25");

            var trainer =await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer is null) return Result.NotFound("Trainer Not Found");
            var category =await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId);
            if (category is null) return Result.NotFound("Category Not Found");
            var isValid = Enum.TryParse<Specialty>(category.CategoryName, true, out var CategorySpecialty);
            if (!isValid || trainer.Specialty != CategorySpecialty) return Result.Validation("Can not Create This Session to this Trainer ");
            var session = _mapper.Map<CreateSessionViewModel,Session>(model);

            _unitOfWork.GetRepository<Session>().Add(session);
           var result = await _unitOfWork.SaveChangesAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Create Session");
        }

        public async Task<Result> DeleteSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var session =await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session == null)
                return Result.NotFound("Session Not Found");
            if (session.EndDate >= DateTime.Now)
                return Result.Fail("Can not Delete Session That Has Not Ended Yet");

            var bookingCount =await _unitOfWork.SessionRepository.GetCountOfBookedSlots(sessionId, ct);
            if (bookingCount > 0)
                return Result.Fail("Can Not Delete Session That Has Bookings");
            _unitOfWork.SessionRepository.Delete(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete Sessiom");
        }

        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
          var  sessionRepo =_unitOfWork.SessionRepository;
            var sessions =await sessionRepo.GetAllSessionsAndTrainerAndCategory(ct);
            if (sessions == null || !sessions.Any()) return null;

            var mappedSessions = sessions.Select(s => new SessionViewModel()
            {
                Id = s.Id,
                Capacity = s.Capacity,
                CategoryName = s.Category.CategoryName,
                TrainerName = s.Trainer.Name,
                Description = s.Description,
                EndDate = s.EndDate,
                StartDate = s.StartDate
            });

            foreach(var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity -await sessionRepo.GetCountOfBookedSlots(session.Id,ct);
            }
            return mappedSessions;
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoryForDropDownAsync(CancellationToken ct = default)
        {
            var result =await _unitOfWork.GetRepository<Category>().GetAllAsync(ct:ct);

            return  _mapper.Map<IEnumerable<CategorySelectViewModel>>(result);

        }

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionByIdWithTrainerAndCategory(sessionId, ct);
            if (session is null)
                return Result<SessionViewModel>.NotFound("Session Not Found");
            else
            {
                var mappedSession = _mapper.Map<Session,SessionViewModel>(session);
                mappedSession.AvailableSlots = mappedSession.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlots(sessionId, ct);
                return Result<SessionViewModel>.Ok(mappedSession);
            }

        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            var session =await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session == null) 
                return Result<UpdateSessionViewModel>.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now) 
                return Result<UpdateSessionViewModel>.Fail("Cannot Update Session that has already started");

            var bookingCount =await _unitOfWork.SessionRepository.GetCountOfBookedSlots(sessionId,ct);
            if(bookingCount > 0)
                return Result<UpdateSessionViewModel>.Fail("Cannot Update Session that has Already has Booking");
            var mappedSession= _mapper.Map<Session,UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.Ok(mappedSession);
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDownAsync(CancellationToken ct = default)
        {
            var result = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);

            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(result);
        }

        public async Task<Result> UpdateSessionAsync(int sessionId, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session =await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session == null)
                return Result.NotFound("Session Not Found");
            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot Update Session that has already started");
            if (model.EndDate < model.StartDate)
                return Result.Fail("EndDate Must be After StartDate");
            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlots(sessionId, ct);
            if (bookingCount > 0)
                return Result.Fail("Cannot Update Session that has Already has Booking");
            if (model.StartDate <= DateTime.Now)
                return Result.Fail("StartDate Must be In The Future");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer is null) return Result.NotFound("Trainer Not Found");
            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId);
            
            var isValid = Enum.TryParse<Specialty>(category?.CategoryName, true, out var CategorySpecialty);
            if (!isValid || trainer.Specialty != CategorySpecialty) return Result.Validation("Can not Create This Session to this Trainer ");

            _mapper.Map(model,session);
            session.UpdatedAt = DateTime.Now;
            _unitOfWork.SessionRepository.Update(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Update Session");

        }
    }
}
