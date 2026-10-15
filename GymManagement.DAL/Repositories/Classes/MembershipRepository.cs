using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.DAL.Repositories.Classes
{
    public class MembershipRepository :GenericRepository<MemberShip>, IMemberShipRepository
    {
        private readonly GymDbContext _dbContext;

        public MembershipRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<MemberShip>> GetMembershipWithMembersAndPlan(Expression<Func<MemberShip, bool>>? filter = null , CancellationToken ct = default)
        {
            var query =  _dbContext.MemberShips
                                  .Include(m => m.Member)
                                  .Include(m => m.Plan)
                                  .AsNoTracking();

            if(filter is not null)
                query = query.Where(filter);

            return await query.ToListAsync(ct);
        }
    }
}
