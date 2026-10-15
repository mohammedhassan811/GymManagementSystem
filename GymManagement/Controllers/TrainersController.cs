using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles ="SuperAdmin")]
    public class TrainersController : Controller
    {
        private readonly ITrainerServices _trainerServices;

        public TrainersController(ITrainerServices trainerServices)
        {
            _trainerServices = trainerServices;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
           => View(await _trainerServices.GetAllTrainersAsync(ct));

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model ,CancellationToken ct)
        {
            if(!ModelState.IsValid) return View(model);

            var result =await _trainerServices.CreateTrainerAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Trainer Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
                TempData["ErrorMessage"] = "Trainer Failed Create";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id,CancellationToken ct)
        {
            var trainer =await _trainerServices.GetTrainerDetailsAsync(id, ct);
            if(trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken ct)
        {
            var trainer =await _trainerServices.GetTrainerToUpdateAsync(id, ct);
            if (trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id,TrainerToUpdateViewModel model, CancellationToken ct)
        {

            if (!ModelState.IsValid) return View(model);

            var result =await _trainerServices.UpdateTrainerDetailsAsync(id, model, ct);
            if (result.success)
                TempData["SuccessMessage"] = "Trainer Updated Successfully";
            else
                TempData["ErrorMessage"] = "Trainer failed to update";
            return RedirectToAction(nameof(Index));


        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id , CancellationToken ct)
        {
            var trainer = await _trainerServices.GetTrainerDetailsAsync(id, ct);
            if(trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id , CancellationToken ct)
        {
            var result =await  _trainerServices.DeleteTrainerAsync(id, ct);
            if (result.success)
                TempData["SuccessMessage"] = "Trainer Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Failed to Delete Trainer";
            return RedirectToAction(nameof(Index));
        }


    }
}
