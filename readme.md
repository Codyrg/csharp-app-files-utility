# Application Files Manager for .NET Applications

This is a simple application that allows you to manage your .NET applications files. Import this application into your project using NuGet with one of the following methods:

- Package Manager: `Install-Package AppFiles`
- .NET CLI: `dotnet add package AppFiles`
- Package Reference: `<PackageReference Include="AppFiles" Version="1.0.6" />`

## Usage

This library contains a single publicly exposed class, `AppFiles`. This class should be instantiated in your application and then used to access the files in your application. The constructor of this class takes the following parameters:

- `string appName`: The name of your application. This is used to create a folder in the company folder created in the user's `AppData` folder to store your application's files.
- `string company`: The name of your company. This is used to create a folder in the user's `AppData` folder to store your application folders.
- `string appsRoot`: (optional) This is the root to store your company folder in. This defaults to the user's `AppData` folder, but can be changed to any folder you want. **NOTE: This folder must exist before you instantiate the `AppFiles` class.**

Example:
```csharp
var appFiles = new AppFiles("My Application", 
                            "My Company");

// Create an application text file
appFiles.SaveTextFile("MyFile.txt", 
                      "This is the contents of my file.");
// C:\Users\MyUser\AppData\Local\My Company\My Application\MyFile.txt

// Create an application binary file in an optional subfolder
appFiles.SaveBinaryFile("MyFile.bin", 
                        new byte[] { 0x00, 0x01, 0x02, 0x03 }, 
                        subfolderPath: "path/to/subfolder");
// C:\Users\MyUser\AppData\Local\My Company\My Application\path\to\subfolder\MyFile.bin

// Get the contents of the subfolder binary file
var bytes = appFiles.LoadBinaryFile("MyFile.bin", 
                                    subfolderPath: "path/to/subfolder");
// bytes = { 0x00, 0x01, 0x02, 0x03 }

// List all binary files in the subfolder
var files = appFiles.ListDirectory(subfolderPath: "path/to/subfolder", 
                                   pattern: "*.bin");
// files = { "C:\Users\MyUser\AppData\Local\My Company\My Application\path\to\subfolder\MyFile.bin" }
```

## Publishing

To publish this library, you must be authenticated to NuGet with the `nuget.org` source and have the appropriate access key setup.
Then, from the repository root, run the `./tools/PublishNuGet`


## License

MIT


