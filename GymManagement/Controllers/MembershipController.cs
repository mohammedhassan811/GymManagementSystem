using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MembershipViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    [Authorize]
    public class MembershipController : Controller
    {
        private readonly IMembershipService _membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }
        public async Task<IActionResult> Index()
        {
            var memberships =await _membershipService.GetAllMembershipsAsync();
            return View(memberships);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropDownListAsync(ct);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMembershipViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownListAsync(ct);
                return View(model);
            }
            var result = await _membershipService.CreateMembershipAsync(model, ct);

            if (result.success)
            {
                TempData["SuccessMessage"] = "Membership Created successfully";

                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.error;
            await PopulateDropDownListAsync(ct);
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id , CancellationToken ct)
        {
            var result = await _membershipService.DeleteActiveMembership(id, ct);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "Membership Canceled Successfully" : result.error;
            return RedirectToAction(nameof(Index));
        }
        private async Task PopulateDropDownListAsync(CancellationToken ct)
        {
            ViewBag.Members = new SelectList(await _membershipService.GetMembersForDropDownList(ct), "Id", "Name");
            ViewBag.Plans = new SelectList(await _membershipService.GetPlansForDropDownList(ct), "Id", "Name");
        }
    }
}
