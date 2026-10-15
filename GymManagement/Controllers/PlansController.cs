using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModel;
using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Controllers
{
    [Authorize]
    public class PlansController : Controller
    {
        private readonly IPlanServices _planServices;

        public PlansController(IPlanServices planServices)
        {
            _planServices = planServices;
        }
        
        public async Task<IActionResult> Index(CancellationToken ct)
            => View(await _planServices.GetAllPlansAsync(ct));

        [HttpGet]
        public async Task<IActionResult> Details(int id , CancellationToken ct)
        {
            var plan =await _planServices.GetPlanByIdAsync(id,ct);
            if(plan == null)
            {
                TempData["ErrorMessage"] = "Plan not found";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id,CancellationToken ct)
        {
            var plan =await _planServices.GetPlanToUpdateAsync(id,ct);
            if(plan is null)
            {
                TempData["ErrorMessage"] = "Plan cannot be edited (not found , inactive , or has active membership)";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id,UpdatePlanViewModel model,CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result =await _planServices.UpdatePlanAsync(id, model, ct);
            if (result.success)
                TempData["SuccessMessage"] = "Plan Updated Successfully";
            else
                TempData["ErrorMessage"] = "Plan Updated Failed to edit plan";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Activate(int id,CancellationToken ct)
        {
            var result =await _planServices.ToggleActivationAsync(id,ct);
            if (result.success)
                TempData["SuccessMessage"] = "Plan status changed";
            else
                TempData["ErrorMessage"] = "Failed to Toggle Plan Status";
            return RedirectToAction(nameof(Index));

        }


    }
}
