using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CirAnime.Models;

namespace CirAnime.Data
{
    public class CirAnimeContext : DbContext
    {
        public CirAnimeContext (DbContextOptions<CirAnimeContext> options)
            : base(options)
        {
     
        }

        public DbSet<CirAnime.Models.UploadEntry> UploadEntry { get; set; }
        public DbSet<User> User { get; set; }
    }
}
