namespace AppFiles;

using Microsoft.Extensions.Logging;

/// <summary>
/// Provides methods for saving and loading files to the local application data folder.
/// </summary>
public class AppFileManager
{
    private readonly ILogger<AppFileManager> _logger = new LoggerFactory().CreateLogger<AppFileManager>();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AppFileManager"/> class.
    /// </summary>
    /// <param name="appName">The name of the application. This will be created as a folder within the company root folder.</param>
    /// <param name="companyName">The name of the company. This will be created as a folder within the apps root folder.</param>
    /// <param name="appsRoot">The root folder where the company root folder will be created. If not specified, the LocalApplicationData folder will be used.</param>
    /// <exception cref="ArgumentException">Thrown if appName or companyName contain invalid characters.</exception>
    /// <exception cref="Exception">Thrown if the company root folder cannot be created or the app root folder cannot be created.</exception>
    public AppFileManager(string appName, string companyName, string appsRoot = "")
    {
        if(string.IsNullOrEmpty(appsRoot))
        {
            appsRoot = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        // Make sure appName and companyName are valid folder names
        var invalidChars = Path.GetInvalidFileNameChars();
        if(appName.IndexOfAny(invalidChars) >= 0)
        {
            throw new ArgumentException("appName contains invalid characters");
        }

        if(companyName.IndexOfAny(invalidChars) >= 0)
        {
            throw new ArgumentException("companyName contains invalid characters");
        }

        // Make sure appsRoot is a valid path
        if(!Directory.Exists(appsRoot))
        {
            throw new ArgumentException("appsRoot is not a valid path");
        }

        // Create the company root folder if it doesn't exist
        string companyRootPath;
        try
        {
            companyRootPath = Path.Combine(appsRoot, companyName);
            if(!Directory.Exists(companyRootPath))
            {
                Directory.CreateDirectory(companyRootPath);
            }
        }
        catch(Exception ex)
        {
            throw new Exception("Unable to create company root folder", ex);
        }

        // Create the app root folder if it doesn't exist
        string appRootPath;
        try
        {
            appRootPath = Path.Combine(companyRootPath, appName);
            if(!Directory.Exists(appRootPath))
            {
                Directory.CreateDirectory(appRootPath);
            }
        }
        catch(Exception ex)
        {
            throw new Exception("Unable to create app root folder", ex);
        }

        AppRootPath = appRootPath;
    }
    
    public string AppRootPath { get; }

    /// <summary>
    /// Saves a text file to the local application data folder.
    /// </summary>
    /// <param name="fileName">Name of the file to save.</param>
    /// <param name="content">The content to save.</param>
    /// <param name="subfolderPath">The subfolder path to save the file to. If not specified, the file will be saved to the app root folder.</param>
    /// <returns>True if the file was saved successfully, otherwise false.</returns>
    public bool SaveTextFile(string fileName, string content, string? subfolderPath = "")
    {
        if (subfolderPath is null)
        {
            _logger.LogError("Invalid subfolder path: {subfolderPath}", subfolderPath);
            return false;
        }
        
        if(!IsValidSubfolderPath(subfolderPath))
        {
            _logger.LogError("Invalid subfolder path: {subfolderPath}", subfolderPath);
            return false;
        }
        
        var folderPath = Path.Combine(AppRootPath, subfolderPath);
        try
        {
            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to create folder: {folderPath}", folderPath);
            return false;
        }
        
        var filePath = Path.Combine(folderPath, fileName);
        try
        {
            File.WriteAllText(filePath, content);
            return true;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unable to save text file: {filePath}", filePath);
            return false;
        }
    }

    /// <summary>
    /// Loads a text file from the local application data folder.
    /// </summary>
    /// <param name="fileName">Name of the file to load.</param>
    /// <param name="subfolderPath">The subfolder path to load the file from. If not specified, the file will be loaded from the app root folder.</param>
    /// <returns>The content of the file if it was loaded successfully, otherwise an empty string.</returns>
    public string LoadTextFile(string fileName, string? subfolderPath = "")
    {
        if (subfolderPath is null)
        {
            _logger.LogError("Invalid subfolder path: {subfolderPath}", subfolderPath);
            return string.Empty;
        }
        
        if(!IsValidSubfolderPath(subfolderPath))
        {
            _logger.LogError("Invalid subfolder path: {subfolderPath}", subfolderPath);
            return string.Empty;
        }

        var filePath = Path.Combine(AppRootPath, subfolderPath, fileName);
        try
        {
            return File.ReadAllText(filePath);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unable to load text file: {filePath}", filePath);
            return string.Empty;
        }
    }
    
    /// <summary>
    /// Saves a binary file to the local application data folder.
    /// </summary>
    /// <param name="fileName">Name of the file to save.</param>
    /// <param name="content">The content to save.</param>
    /// <param name="subfolderPath">The subfolder path to save the file to. If not specified, the file will be saved to the app root folder.</param>
    /// <returns>True if the file was saved successfully, otherwise false.</returns>
    public bool SaveBinaryFile(string fileName, byte[] content, string? subfolderPath = "")
    {
        if (subfolderPath is null)
        {
            _logger.LogError("Invalid subfolder path: {subfolderPath}", subfolderPath);
            return false;    
        }
        
        if(!IsValidSubfolderPath(subfolderPath))
        {
            _logger.LogError("Invalid subfolder path: {subfolderPath}", subfolderPath);
            return false;
        }

        var filePath = Path.Combine(AppRootPath, subfolderPath, fileName);
        try
        {
            File.WriteAllBytes(filePath, content);
            return true;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unable to save binary file: {filePath}", filePath);
            return false;
        }
    }
    
    /// <summary>
    /// Loads a binary file from the local application data folder.
    /// </summary>
    /// <param name="fileName">Name of the file to load.</param>
    /// <param name="subfolderPath">The subfolder path to load the file from. If not specified, the file will be loaded from the app root folder.</param>
    /// <returns>The content of the file if it was loaded successfully, otherwise an empty byte array.</returns>
    public byte[] LoadBinaryFile(string fileName, string? subfolderPath = "")
    {
        if (subfolderPath is null)
        {
            _logger.LogError("Invalid subfolder path: {subfolderPath}", subfolderPath);
            return Array.Empty<byte>();
        }
        
        if(!IsValidSubfolderPath(subfolderPath))
        {
            _logger.LogError("Invalid subfolder path: {subfolderPath}", subfolderPath);
            return Array.Empty<byte>();
        }

        var filePath = Path.Combine(AppRootPath, subfolderPath, fileName);
        try
        {
            return File.ReadAllBytes(filePath);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unable to load binary file: {filePath}", filePath);
            return Array.Empty<byte>();
        }
    }

    /// <summary>
    /// Lists the files in a directory.
    /// </summary>
    /// <param name="subfolderPath">The subfolder path to list the files from. If not specified, the files will be listed from the app root folder.</param>
    /// <param name="pattern">The pattern to use when listing the files. If not specified, all files will be listed.</param>
    /// <returns>The list of files if the directory was listed successfully, otherwise an empty list.</returns>
    public IEnumerable<string> ListDirectory(string? subfolderPath = "", string pattern = "*")
    {
        if (subfolderPath is null)
        {
            _logger.LogError("Invalid subfolder path: {subfolderPath}", subfolderPath);
            return Array.Empty<string>();
        }
        
        if(!IsValidSubfolderPath(subfolderPath))
        {
            _logger.LogError("Invalid subfolder path: {subfolderPath}", subfolderPath);
            return Array.Empty<string>();
        }

        var directoryPath = Path.Combine(AppRootPath, subfolderPath);
        try
        {
            return Directory.EnumerateFiles(directoryPath, pattern);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unable to list directory: {directoryPath}", directoryPath);
            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Checks if the specified subfolder path is valid.
    /// </summary>
    /// <param name="subfolderPath">The subfolder path to check.</param>
    /// <returns>True if the subfolder path is valid, otherwise false.</returns>
    private static bool IsValidSubfolderPath(string? subfolderPath)
    {
        if(subfolderPath == null)
            return false;
        if(subfolderPath.Length == 0)
            return true;

        var folders = subfolderPath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
        var invalidChars = Path.GetInvalidFileNameChars();
        return folders.All(f => f.IndexOfAny(invalidChars) < 0);
    }
}