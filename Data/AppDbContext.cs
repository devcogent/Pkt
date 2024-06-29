using Microsoft.EntityFrameworkCore;
using Pkt.Models.Entites;

namespace Pkt.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<ManagePkt> managepkt { get; set; } = default!;
        public DbSet<CommonQuestion> commonquestions { get; set; } = default!;
        public DbSet<ManageQuestion> manageQuestions { get; set; } = default!;
        public DbSet<ManageAnswer> manageAnswer { get; set; } = default!;
        public DbSet<AttemptPkt> attemptPkts { get; set; } = default!;  
        public DbSet<Auth> auths { get; set; } = default!;
        public DbSet<PktFor> pktFors { get; set; } = default!;
        public DbSet<ClientMaster> clientMasters { get; set; } = default!;

    }
}
