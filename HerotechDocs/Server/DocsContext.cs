using HerotechDocs.Shared;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HerotechDocs.Server
{
    public class DocsContext : IdentityDbContext
    {

        public DocsContext(DbContextOptions<DocsContext> options)
            : base(options)
        {

        }
        public DbSet<DatabaseFile> DatabaseFiles { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<HerotechDocs.Shared.Category> Category { get; set; }

        public DbSet<Item> Items { get; set; }


    }
}
