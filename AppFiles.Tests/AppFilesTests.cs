namespace AppFiles.Tests;

/// <summary>
/// Tests for the AppFiles class.
/// </summary>
public class AppFilesTests
{
    private string _testingDirectory = string.Empty;
    private const string CompanyName = "Test Company 1";
    private const string ProductNameOne = "Test Product 1";
    private const string ProductNameTwo = "Test Product 2";
    private const string TestTextFileName = "Test.txt";
    private const string TestBinaryFileName = "Test.bin";
    private readonly string Subfolder = Path.Combine("path", "to", "subfolder");
    
    /// <summary>
    /// Setup the test environment.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _testingDirectory = Path.GetTempPath();
        Directory.CreateDirectory(_testingDirectory);
    }

    /// <summary>
    /// Test that the AppFiles class can be instantiated with a valid directory without throwing an exception.
    /// </summary>
    [Test]
    public void TestInstantiation()
    {
        Assert.DoesNotThrow(() =>
        {
            var appFiles = new AppFiles(ProductNameOne, CompanyName, _testingDirectory);
        });
    }
    
    /// <summary>
    /// Test that the AppFiles class throws an exception when instantiated with an invalid app name.
    /// </summary>
    [Test]
    public void TestInstantiationWithInvalidAppName()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var appFiles = new AppFiles("Test Product 1" + '\0', CompanyName, _testingDirectory);
        });
    }
    
    /// <summary>
    /// Test that the AppFiles class throws an exception when instantiated with an invalid company name.
    /// </summary>
    [Test]
    public void TestInstantiationWithInvalidCompanyName()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var appFiles = new AppFiles(ProductNameOne, "Test Company" + '\0', _testingDirectory);
        });
    }
    
    /// <summary>
    /// Test that the AppFiles class throws an exception when instantiated with an invalid apps root.
    /// </summary>
    [Test]
    public void TestInstantiationWithInvalidAppsRoot()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var appFiles = new AppFiles(ProductNameOne, CompanyName, "C:\\Invalid\\Path");
        });
    }
    
    /// <summary>
    /// Test that the AppFiles can be instantiated multiple times without throwing an exception for the same company with different apps.
    /// </summary>
    [Test]
    public void TestInstantiatingMultipleAppFiles()
    {
        Assert.DoesNotThrow(() =>
        {
            var appFilesOne = new AppFiles(ProductNameOne, CompanyName, _testingDirectory);
            var appFilesTwo = new AppFiles(ProductNameTwo, CompanyName, _testingDirectory);
        });
    }
    
    /// <summary>
    /// Test that the app files root folder is created when the AppFiles class is instantiated.
    /// </summary>
    [Test]
    public void TestFolderCreation()
    {
        var appFiles = new AppFiles(ProductNameOne, CompanyName, _testingDirectory);
        Assert.IsTrue(Directory.Exists(appFiles.AppRootPath));
    }
    
    /// <summary>
    /// Test saving a text file to the app files root folder.
    /// </summary>
    [Test]
    public void TestTextFileSaveNoSubfolder()
    {
        var appFiles = new AppFiles(ProductNameOne, CompanyName, _testingDirectory);
        appFiles.SaveTextFile(TestTextFileName, "This is a test file.");
        Assert.IsTrue(File.Exists(Path.Combine(appFiles.AppRootPath, TestTextFileName)));
    }
    
    /// <summary>
    /// Test saving a text file to a subfolder of the app files root folder.
    /// </summary>
    [Test]
    public void TestTextFileSaveWithSubfolder()
    {
        var appFiles = new AppFiles(ProductNameOne, CompanyName, _testingDirectory);
        var saved = appFiles.SaveTextFile(TestTextFileName, "This is a test file.", Subfolder);
        var path = Path.Combine(appFiles.AppRootPath, Subfolder, TestTextFileName);
        var exists = File.Exists(path);
        Assert.IsTrue(exists);
    }

    /// <summary>
    /// Test saving a binary file to the app files root folder.
    /// </summary>
    [Test]
    public void TestBinaryFileSave()
    {
        var appFiles = new AppFiles(ProductNameOne, CompanyName, _testingDirectory);
        appFiles.SaveBinaryFile(TestTextFileName, new byte[] { 0x00, 0x01, 0x02, 0x03 });
        Assert.IsTrue(File.Exists(Path.Combine(appFiles.AppRootPath, Subfolder, TestTextFileName)));
    }
    
    /// <summary>
    /// Test that list directory returns the correct files.
    /// </summary>
    [Test]
    public void TestListDirectory()
    {
        var appFiles = new AppFiles(ProductNameOne, CompanyName, _testingDirectory);
        appFiles.SaveTextFile(TestTextFileName, "This is a test file.", Subfolder);
        appFiles.SaveBinaryFile(TestBinaryFileName, new byte[] { 0x00, 0x01, 0x02, 0x03 }, Subfolder);
        var files = appFiles.ListDirectory(Subfolder, "*.txt").ToList();
        
        var expected = new List<string>
        {
            Path.Combine(appFiles.AppRootPath, Subfolder, TestTextFileName)
        };
        
        Assert.That(files, Is.EquivalentTo(expected));
    }
}