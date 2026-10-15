using System.Threading.Tasks;
using GymManagement.BLL.Attachment;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MembersController : Controller
    {
        private readonly IMemberServices _memberServices;
        private readonly IAttachmentService _attachmentService;

        public MembersController(IMemberServices memberServices , IAttachmentService attachmentService)
        {
            _memberServices = memberServices;
            _attachmentService = attachmentService;
        }
        //GET BaseURL/Members/index
        //index ->list all members
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberServices.GetAllMemberAsync(ct);
            return View(members);
        }

        #region Get Member Photo
        [HttpGet]
        public async Task<IActionResult> Picture(int id)
        {
            var member =await _memberServices.GetMemberDetailsByIdAsync(id);
            if(member == null || string.IsNullOrWhiteSpace(member.value.Photo))
               return NotFound();
            var result =_attachmentService.GetFile(member.value.Photo, "MembersPhoto");
            if (result == null) return NotFound();
            return File(result.Value.stream, result.Value.contentType);
            
        }
        #endregion


        //GET BaseURL/Members/memberDetails/{id}
        //MemberDetails -> show one member details

        public async Task<IActionResult> MemberDetails(int id ,CancellationToken ct)
        {
            var member = await _memberServices.GetMemberDetailsByIdAsync(id, ct);
            if(member == null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(member.value);
        }

        //GET BaseURL/Members/HealthRecordDetails/{id}
        //HealthRecordDetails -> show one member details
        public async Task<IActionResult> HealthRecordDetails(int id,CancellationToken ct)
        {
            var result =await  _memberServices.GetMemberHealthRecordAsync(id, ct);
            if(result == null)
            {
                TempData["ErrorMessage"] = "Health Record Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(result);
        }


        #region Create Member
        //Get BaseURL/Members/Create
        //Create -> show empty form
        [HttpGet]
        public IActionResult Create() => View();
        //Post BaseURL/Members/Create {Member}
        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model,CancellationToken ct)
        {
            if (!ModelState.IsValid) 
                return View(nameof(Create), model);

            var result =await _memberServices.CreateMemberAsync(model, ct);
            if (result.success)
                TempData["SuccessMessage"] = "Member Created Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Create Member ";

            return RedirectToAction(nameof(Index));

        }
        //CreateMember -> Submit form
        #endregion

        #region Edit Member
        //Get BaseURL/Members/Edit/{id}
        //Edit -> display edit form
        [HttpGet]
        public async Task<IActionResult> EditMember(int id,CancellationToken ct)
        {
            var member =await _memberServices.GetMemberToUpdateAsync(id, ct);
            if(member==null)
            {
                TempData["Error Message"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member.value);
        }

        //Post BaseURL/Members/Edit {Member}
        //Edit-> Submit form
        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute]int id, MemberToUpdateViewModel model,CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result =await  _memberServices.UpdateMemberDetailsAsync(id, model, ct);
            if (result.success)
                TempData["SuccessMessage"] = "Member Updated Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Update Member";
            return RedirectToAction(nameof(Index));

        }
        #endregion

        #region Delete Member
        //Get BaseURL/Members/Delete/{id}
        //Delete ->  delete form
        [HttpGet]
        public async Task<IActionResult> Delete(int id,CancellationToken ct)
        {
            var member=await _memberServices.GetMemberDetailsByIdAsync(id, ct);

            if(member == null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }


        //Post BaseURL/Members/DeleteConfirmed/{id}
        //DeleteConfirmed-> Submit form

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id ,CancellationToken ct = default)
        {
            var result =await _memberServices.RemoveMemberAsync(id, ct);
            if (result.success)
                TempData["SuccessMessage"] = "Member Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Delete Member";

            return RedirectToAction(nameof(Index));
        }
        #endregion

    }
}
