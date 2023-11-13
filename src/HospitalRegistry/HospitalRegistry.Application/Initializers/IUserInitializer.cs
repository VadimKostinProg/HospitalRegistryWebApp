namespace HospitalRegistry.Application.Initializers
{
    /// <summary>
    /// Initializer for adding default user roles and system admin.
    /// </summary>
    public interface IUserInitializer
    {
        /// <summary>
        /// Method for adding user roles and asustem admin if they does not exist.
        /// </summary>
        void Initialize();
    }
}
