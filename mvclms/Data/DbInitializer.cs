using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using mvclms.Models;

namespace mvclms.Data
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context, IServiceProvider services)
        {
            // Get a logger
            var logger = services.GetRequiredService<ILogger<DbInitializer>>();

            // Make sure the database is created
            // We already did this in the previous step
            context.Database.EnsureCreated();

            // if (context.Authors.Any())
            // {
            //     logger.LogInformation("The database was already seeded");
            //     return;
            // }

            logger.LogInformation("Start seeding the database.");
            //
            // var person = new Person
            // {
            //     FirstName = "Johan",
            //     LastName = "Vergeer",
            //     Age = 32
            // };
            //
            // context.People.Add(person);
            // context.SaveChanges();

            logger.LogInformation("Finished seeding the database.");
        }
    }
}