using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;
using System.Linq;
using Falcon_Blog.Models;

namespace Falcon_Blog.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Falcon_Blog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Falcon_Blog.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                                new RoleStore<IdentityRole>(context));
          
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            #region Add User creatgion here
            #endregion

            #region Add Role assignment here
            #endregion


            //I need a section to create users
            //I need a sction to assign these users to a role


            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            #region Add Users and Assign Roles
            #endregion

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            if (!context.Users.Any(u => u.Email == "pf@hotmail.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "pf@hotmail.com",
                    Email = "pf@hotmail.com",
                    FirstName = "Peter",
                    LastName = "Fralick",
                    DisplayName = "Admin"
                };

                //This line creaes the User in the DB, password
                userManager.Create(user, "Abc&123!");

                //This line attaches the Role of Admin to this specific user
                userManager.AddToRoles(user.Id, "Admin");
            }

            if (!context.Users.Any(u => u.Email == "ARussell@coderfoundry.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "ARussell@coderfoundry.com",
                    Email = "ARussell@coderfoundry.com",
                    FirstName = "Andrew",
                    LastName = "Russell",
                    DisplayName = "Andy Stache"
                };

                //This line creaes the User in the DB
                userManager.Create(user, "Abc&121!");

                //This line attaches the Role of Moderator to this specific user
                userManager.AddToRoles(user.Id, "Moderator");

            }

            if (!context.Users.Any(v => v.Email == "JasonTwichell@coderfoundry.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "JasonTwichell@coderfoundry.com",
                    Email = "JasonTwichell@coderfoundry.com",
                    FirstName = "Jason",
                    LastName = "Twichell",
                    DisplayName = "JasonTwichell"
                };

                //This line creaes the User in the DB
                userManager.Create(user, "Abc&124!");

                //This line attaches the Role of Moderator to this specific user
                userManager.AddToRoles(user.Id, "Moderator");

            }

        }
    }
}
