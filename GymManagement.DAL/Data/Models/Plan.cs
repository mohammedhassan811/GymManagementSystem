using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Data.Models
{
    public class Plan : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int DurationDays { get; set; }
        public decimal price { get; set; }
        public bool IsActive { get; set; }

        public ICollection<MemberShip> PlanMembers { get; set; } = default!;

    }
}
