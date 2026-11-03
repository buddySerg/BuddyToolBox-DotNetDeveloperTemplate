using System.IO;
using System.Text.Json;

using Microsoft.Extensions.Logging;

namespace BTB.Core.Repositories;

/// <summary>
/// Provides a file-based repository for JSON serialized data.
/// </summary>
public sealed class JsonRepository<T> : IJsonRepository<T>, IDisposable where T : class, new()
{
    #region Private Fields
    private const int BufferSize = 4096;
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;
    private readonly ILogger<JsonRepository<T>> _logger;

    // Prevent concurrent writes from racing with each other.
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets the path to the JSON file.
    /// </summary>
    public string FilePath => _filePath;
    #endregion

    /// <summary>
    /// Cunstructor
    /// </summary>
    public JsonRepository(string path, ILogger<JsonRepository<T>> logger, JsonSerializerOptions? options = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(logger);

        _filePath = path;
        _logger = logger;

        _options = options ?? new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    #region Public Methods
    /// <summary>
    /// Checks if the JSON file exists.
    /// </summary>
    public bool Exists()
    {
        return File.Exists(_filePath);
    }

    /// <summary>
    /// Asynchronously loads the JSON data from the file.
    /// </summary>
    public async Task<T> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return new T();
        }

        try
        {
            await using FileStream stream = new(
                _filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                BufferSize,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            return await JsonSerializer.DeserializeAsync<T>(stream, _options, cancellationToken) ?? new T();

        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid JSON in file: {Path}", _filePath);
            throw;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading JSON file: {Path}", _filePath);
            throw;
        }
    }

    /// <summary>
    /// Asynchronously serializes the specified model to JSON and saves it to the configured file.
    /// </summary>
    public async Task SaveAsync(T model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        string? directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await _writeLock.WaitAsync(cancellationToken);

        string tempFile = "";

        try
        {
            tempFile = CreateTempFilePath();

            cancellationToken.ThrowIfCancellationRequested();

            await WriteTempFileAsync(tempFile, model, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            // Replace the destination only after the complete JSON document has been successfully written.
            File.Move(tempFile, _filePath, overwrite: true);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving JSON file: {Path}", _filePath);
            throw;
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(tempFile))
            {
                TryDeleteTempFile(tempFile);
            }
            _writeLock.Release();
        }
    }

    /// <summary>
    /// Deletes the JSON file if it exists.
    /// </summary>
    public void Delete()
    {
        try
        {
            File.Delete(_filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting JSON file: {Path}", _filePath);
            throw;
        }
    }

    /// <summary>
    /// Creates a deep copy of the specified data by serializing and deserializing it.
    /// </summary>
    public T Clone(T data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string json = JsonSerializer.Serialize(data);

        return JsonSerializer.Deserialize<T>(json)
               ?? throw new InvalidOperationException(
                   $"Failed to clone {typeof(T).Name}.");
    }

    /// <summary>
    /// Determines whether two instances are equal based on their JSON representations.
    /// </summary>
    public bool AreEqual(T left, T right)
    {
        return JsonSerializer.Serialize(left) == JsonSerializer.Serialize(right);
    }

    /// <summary>
    /// Releases resources used by the repository.
    /// </summary>
    public void Dispose()
    {
        _writeLock.Dispose();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Creates a unique path for a temporary file next to the configured JSON file.
    /// </summary>
    private string CreateTempFilePath()
    {
        return $"{_filePath}.{Guid.NewGuid():N}.tmp";
    }

    /// <summary>
    /// Asynchronously serializes the specified model to a temporary file and flushes all buffered data to disk.
    /// </summary>
    private async Task WriteTempFileAsync(string tempFile, T model, CancellationToken cancellationToken)
    {
        await using FileStream stream = new(
            tempFile,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            BufferSize,
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        await JsonSerializer.SerializeAsync(
            stream,
            model,
            _options,
            cancellationToken);

        // Ensure all buffered data is flushed before the file becomes the new destination.
        await stream.FlushAsync(cancellationToken);
    }

    /// <summary>
    /// Attempts to delete a temporary file and logs a warning if the operation fails.
    /// </summary>
    private void TryDeleteTempFile(string tempFile)
    {
        try
        {
            File.Delete(tempFile);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to delete temporary JSON file: {Path}", tempFile);
        }
    }
    #endregion
}
