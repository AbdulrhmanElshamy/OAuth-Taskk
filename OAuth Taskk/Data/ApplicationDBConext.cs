using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OAuth_Taskk.Data
{
    public class ApplicationDBConext : IdentityDbContext
    {
        public ApplicationDBConext(DbContextOptions<ApplicationDBConext> options) : base(options) 
        { 

        }
    }
}
