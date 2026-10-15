using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.DAL.Repositories.Classes
{
    public class BookingRepository :GenericRepository<Booking>, IBookingRepository
    {
        private readonly GymDbContext _dbContext;

        public BookingRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<List<Booking>> GetBySessionId(int sessionId, CancellationToken ct = default)
        {
            return _dbContext.Bookings
                             .AsNoTracking()
                             .Include(b => b.Member)
                             .Where(b => b.SessionsId == sessionId)
                             .ToListAsync(ct);
        }
    }
}
