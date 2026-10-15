using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.BookingViewModel;
using GymManagement.BLL.ViewModels.MembershipViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }


        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions =await _bookingService.GetAllSessionsAsync(ct);
            return View(sessions);
        }

        [HttpGet]
        public async Task<IActionResult> GetMembersForUpComingSession(int id,CancellationToken ct)
        {
            var members = await _bookingService.GetMembersForSessionAsync(id, ct);
            return View(members);
        }


        [HttpGet]
        public async Task<IActionResult> GetMembersForOngoingSession(int id, CancellationToken ct)
        {
            var members = await _bookingService.GetMembersForSessionAsync(id, ct);
            return View(members);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int Id, CancellationToken ct)
        {
            var members =await _bookingService.GetMembersForDropdownList(Id, ct);
            ViewBag.Members = new SelectList(members, "Id", "Name");
            ViewBag.SessionId = Id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingViewModel model, CancellationToken ct)
        {
          
           var result = await _bookingService.CreateBookingAsync(model, ct);

             TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "Booking Created Successfully" : result.error;
            return RedirectToAction(nameof(GetMembersForUpComingSession), new { id = model.SessionId });
        }

        [HttpPost]
        public async Task<IActionResult> Attended(int sessionId,int memberId, CancellationToken ct)
        {

            var result = await _bookingService.MarkIsAttendedAsync(memberId,sessionId,  ct);

            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "Marked Attended Successfully" : result.error;
            return RedirectToAction(nameof(GetMembersForOngoingSession), new {id= sessionId });
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int sessionId, int memberId, CancellationToken ct)
        {

            var result = await _bookingService.CancelBookingAsync(memberId, sessionId, ct);

            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "Booking Canceled Successfully" : result.error;
            return RedirectToAction(nameof(GetMembersForUpComingSession), new { id = sessionId });
        }



        private async Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropdownList(int sessionId,CancellationToken ct)
        {
            var members =await _bookingService.GetMembersForDropdownList(sessionId, ct);
            return members;
           
        }
    }
}
