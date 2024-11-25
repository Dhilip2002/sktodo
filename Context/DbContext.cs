using Microsoft.EntityFrameworkCore;
using sktodo.Models;

namespace sktodo.Context
{
    public class NLPContext: DbContext
    {

        public NLPContext(DbContextOptions<NLPContext> options) : base(options)
        {

        }
        public DbSet<ToDo> ToDos => Set<ToDo>();
        public DbSet<Light> Lights => Set<Light>();   
    }
}