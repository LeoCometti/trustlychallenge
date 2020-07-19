using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrustlyChallenge.Model;

namespace TrustlyChallenge.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<GroupedExtension> GroupedFiles { get; set; }
        public DbSet<GitHubRepository> GitHubRepo { get; set; }
    }
}
