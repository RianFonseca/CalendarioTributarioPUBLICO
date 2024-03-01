using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CalendarioTributario.AutorizacaoEAutentificacao
{
    public class ApplicationUser : IdentityUser
    {
    }
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public IEnumerable<object> Calendario { get; set; }
    }
}
