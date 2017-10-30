using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Blog.Models
{
    public class BlogDbContex : IdentityDbContext<ApplicationUser>
    {
        public BlogDbContex()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public virtual IDbSet<Article> Articles { get; set; }

        public static BlogDbContex Create()
        {
            return new BlogDbContex();
        }
    }
}