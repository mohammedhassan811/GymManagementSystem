using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.BookingViewModel;
using GymManagement.BLL.ViewModels.MembershipViewModel;
using GymManagement.BLL.ViewModels.SessionViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions =await _unitOfWork.SessionRepository.GetAllSessionsAndTrainerAndCategory(ct);
            var mappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);
            foreach (var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlots(session.Id, ct);
            }
            return mappedSessions;
        }

        public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var bookings =await _unitOfWork.BookingRepository.GetBySessionId(sessionId, ct);
            var sessions =await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);


            return bookings.Select(bookings => new MemberForSessionViewModel
            {
                MemberId = bookings.MemberId,
                SessionId = bookings.SessionsId,
                BookingDate = bookings.CreatedAt,
                MemberName = bookings.Member.Name,
                IsAttended = sessions.StartDate > DateTime.Now ? false : bookings.IsAttended
                
                
                
            }).ToList();
        }

        public async Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropdownList(int sessionId, CancellationToken ct = default)
        {
            var allBooking = await _unitOfWork.BookingRepository.GetAllAsync(false, ct);

            var booking = allBooking
                .Where(b => b.SessionsId == sessionId);

            var bookedMemberIds = booking
                .Select(b => b.MemberId)
                .ToHashSet();

            var allMembers = await _unitOfWork
                .GetRepository<Member>()
                .GetAllAsync(false, ct);

            var availableMembers = allMembers
                .Where(m => !bookedMemberIds.Contains(m.Id))
                .ToList();

            return _mapper.Map<IEnumerable<MemberSelectListViewModel>>(availableMembers);
        }

        public async Task<Result> CreateBookingAsync(CreateBookingViewModel model, CancellationToken ct = default)
        {
            var session =await _unitOfWork.SessionRepository.GetByIdAsync(model.SessionId);

            if (session is null)
                return Result.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("You Can't Book a Session That already Started");

            var hasActiveMembership =await _unitOfWork.MembershipRepository.AnyAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now);

            if (!hasActiveMembership)
                return Result.Fail("You Don't have Active Membership");

            var alreadyBooked =await _unitOfWork.BookingRepository.AnyAsync(b=>b.SessionsId  == session.Id&& b.MemberId == model.MemberId);

            if (alreadyBooked)
                return Result.Fail("You already Booked this Session");

            var bookedSlots =await _unitOfWork.SessionRepository.GetCountOfBookedSlots(model.SessionId, ct);

            if (bookedSlots >= session.Capacity)
                return Result.Fail("Session is full Capacity");
            var booking = new Booking
            {
                MemberId = model.MemberId,
                SessionsId = model.SessionId,
                IsAttended = false,
                CreatedAt = DateTime.Now
            };
            _unitOfWork.BookingRepository.Add(booking);
            var sessionId = booking.SessionsId;

            var sessionExists = await _unitOfWork.SessionRepository
                .AnyAsync(s => s.Id == sessionId);

           
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Booking");

        }

        public async Task<Result> MarkIsAttendedAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking =await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.SessionsId == sessionId && b.MemberId == memberId, true, ct);
            if (booking is null)
                return Result.NotFound("Booking Not Found");

            booking.IsAttended = true;
            booking.UpdatedAt = DateTime.Now;

            _unitOfWork.BookingRepository.Update(booking);
            var result =await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to mark this Member as Attended ");
        }

        public async Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync( sessionId);
            if (session is null)
                return Result.NotFound("Session Not Found");
            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot Cancel Session That already Started ");

            var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.SessionsId == sessionId && b.MemberId == memberId, true, ct);

            if (booking is null)
                return Result.NotFound("Booking Not Found");

            _unitOfWork.BookingRepository.Delete(booking);
            var result =await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Cancel Booking"); 
        }


       
    }
}
