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

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
      options.UseSqlite("Data Source=ciranime.db");
        }
        public DbSet<CirAnime.Models.UploadEntry> UploadEntry { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<CirAnime.Models.MediaInfo> MediaInfo { get; set; }
        public DbSet<ProcessingJob> ProcessingJob { get; set; }
    }
}
