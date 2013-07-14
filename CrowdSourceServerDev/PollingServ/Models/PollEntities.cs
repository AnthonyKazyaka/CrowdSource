using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace PollingServ.Models
{
    /// <summary>
    /// Database Context. (This is the database in the eyes of the application)
    /// </summary>
    public class PollEntities : DbContext
    {
        
        public DbSet<Question> Question { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<QuestionMetaOptions> QuestionMetaOptions { get; set; }
        public DbSet<QuestionMetaValues> QuestionMetaValues { get; set; }
        public DbSet<FreeResponses> FreeResponses { get; set; }
        public DbSet<PollResponses> PollResponses { get; set; }
    }
}