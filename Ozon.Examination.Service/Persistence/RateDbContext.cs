using Microsoft.EntityFrameworkCore;
using Ozon.Examination.Service.Persistence.Entities;

namespace Ozon.Examination.Service.Persistence
{
    public class RateDbContext : DbContext
    {
        public RateDbContext(DbContextOptions<RateDbContext> options):
            base(options)
        {
        }

        public virtual DbQuery<RateStatistics> RateStatistics { get; set; }

        public virtual DbSet<Rate> Rates { get; set; }
    }
}
