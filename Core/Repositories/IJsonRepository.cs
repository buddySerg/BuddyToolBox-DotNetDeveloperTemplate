namespace BTB.Core.Repositories
{
    /// <summary>
    /// Defines operations for managing JSON data persisted in a file.
    /// </summary>
    /// <typeparam name="T">The type of data managed by the repository.</typeparam>
    public interface IJsonRepository<T> where T : class, new()
    {
        /// <summary>
        /// Gets a value indicating whether the JSON file exists.
        /// </summary>
        bool Exists();

        /// <summary>
        /// Asynchronously loads the JSON data from the file.
        /// </summary>
        /// <remarks>
        /// Returns a new instance of <typeparamref name="T"/> when the file does not exist.
        /// </remarks>
        Task<T> LoadAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously saves the specified JSON data to the file.
        /// </summary>
        Task SaveAsync(T model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the JSON file if it exists.
        /// </summary>
        void Delete();

        /// <summary>
        /// Creates a copy of the specified data.
        /// </summary>
        T Clone(T data);

        /// <summary>
        /// Determines whether two instances of <typeparamref name="T"/> are equal.
        /// </summary>
        bool AreEqual(T left, T right);
    }
}
