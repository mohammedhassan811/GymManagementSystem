using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    [Authorize]
    public class SessionsController : Controller
    {
        private readonly ISessionServices _sessionServices;

        public SessionsController(ISessionServices sessionServices)
        {
            _sessionServices = sessionServices;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await _sessionServices.GetAllSessionsAsync(ct);
            return View(sessions);
        }

        #region CreateSession
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropDownListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model , CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownListAsync();
                return View(model);
            }

            var result =await _sessionServices.CreateSessionAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            
            TempData["ErrorMessage"] = result.error;
            await PopulateDropDownListAsync();
            return View(model);


        }

        private async Task PopulateDropDownListAsync()
        {
            ViewBag.Trainers = new SelectList(await _sessionServices.GetTrainerForDropDownAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _sessionServices.GetCategoryForDropDownAsync(), "Id", "CategoryName");
        }
        #endregion
        [HttpGet]
        public async Task<IActionResult> Details(int id ,CancellationToken ct)
        {
            var result =await _sessionServices.GetSessionByIdAsync(id, ct);
            if(result.success)
            {
                return View(result.value);
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        #region EditSession
        [HttpGet]
        public async Task<IActionResult> Edit(int id,CancellationToken ct)
        {
            
            var result =await _sessionServices.GetSessionToUpdateAsync(id, ct);
            if(result.success)
            {
                ViewBag.Trainers = new SelectList(await _sessionServices.GetTrainerForDropDownAsync(), "Id", "Name");
                return View(result.value);
                
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id,UpdateSessionViewModel model,CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Trainers = new SelectList(await _sessionServices.GetTrainerForDropDownAsync(), "Id", "Name");
                return View(model);
            }

            var result = await _sessionServices.UpdateSessionAsync(id, model, ct);
            if(result.success)
            {
                TempData["SuccessMessage"] = "Session Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                ViewBag.Trainers = new SelectList(await _sessionServices.GetTrainerForDropDownAsync(), "Id", "Name");
                return View(model);
            }
            
        }
        #endregion

        #region DeleteSession
        [HttpGet]
        public async Task<IActionResult> Delete(int id ,CancellationToken ct)
        {
            var result =await _sessionServices.GetSessionByIdAsync(id);

            if (result.success)
            {
                return View(result.value);
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id ,CancellationToken ct)
        {
            var result =await _sessionServices.DeleteSessionAsync(id, ct);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "Session Deleted Successfully" : result.error;
            return RedirectToAction(nameof(Index));
        }
        #endregion

    }
}
