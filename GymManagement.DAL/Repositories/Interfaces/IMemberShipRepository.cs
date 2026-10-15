using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GymManagement.DAL.Data.Models;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IMemberShipRepository : IGenericRepository<MemberShip>
    {
        Task<IEnumerable<MemberShip>> GetMembershipWithMembersAndPlan(Expression<Func<MemberShip, bool>>? filter = null , CancellationToken ct = default);
    }
}
