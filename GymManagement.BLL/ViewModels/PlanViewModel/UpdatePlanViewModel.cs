using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.ViewModels.PlanViewModel
{
    public class UpdatePlanViewModel
    {
        public string PlanName { get; set; } = default!;
        [Required(ErrorMessage = "Description Is Required")]
        [StringLength(200,MinimumLength =5 ,ErrorMessage = "Description must be between 5 and 200 characters")]
        public string Description { get; set; } = default!;
        [Required(ErrorMessage = "DurationDays Is Required")]
        [Range(1,365,ErrorMessage ="DurationDays must be between 1 and 365")]
        public int DurationDays { get; set; }
        [Required(ErrorMessage = "Price Is Required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
    }
}
