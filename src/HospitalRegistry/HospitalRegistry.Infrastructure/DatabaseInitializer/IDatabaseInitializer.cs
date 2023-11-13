namespace HospitalRegistry.Infrastructure.DatabaseInitializer
{
    /// <summary>
    /// Seed data initializer for the data base.
    /// </summary>
    public interface IDatabaseInitializer
    {
        /// <summary>
        /// Method for applying all pending migrations and initializing data base with seed data.
        /// </summary>
        void Initialize();
    }
}
