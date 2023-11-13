using HospitalRegistry.Infrastructure.DatabaseContexts;
using HospitalReqistry.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalRegistry.Infrastructure.DatabaseInitializer
{
    public class DbInitializer : IDatabaseInitializer
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(ApplicationContext context, ILogger<DbInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while applying migrations for data base: {ex.Message}");
            }
        }
    }
}
