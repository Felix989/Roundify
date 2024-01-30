Note: If you are looking for a release for the version that runs in the background with a tray icon, please look at the releases tab. 

The following documentation is out of date and will not be updated. You do still need to install the runtime however. 


~Description:<br/>
This is a program that makes your screen's window rounded just like displaperture but for windows.

  ~Disclamer: <br/>
The program is in strong Alpha state, but I'm planning on continuing this project.<br/>
Currently it only supports 1920*1080 (16:9), but im planning to make it universal and also the corner radius to be adjusted.<br/>

A deployed version is partly added, asap I'm ready with it I will commit it to the "Release" branch.<br/>
The Release branch contains a "bin" Folder with an EXE file and a Settings(Probably, still under planning stage).<br/>
If you want to deploy for yourself you will need Visual Studio, .NET 5 SDK and Runtime.<br/>

  ~Usage (Currently):<br/>
You have to clone the Release branch and inside the Bin folder there are the two distributions which you can run as an exe.<br/>
YOU WILL NEED .NET 5 FRAMEWORK RUNTIME SUPPORT x64 OR x86<br/>
Download here: https://dotnet.microsoft.com/download/dotnet/5.0 (.NET Desktop Runtime 5.0.11)<br/>
Use the following command to deploy for yourself: *dotnet publish -c Release --self-contained -r win-x64 -p:PublishSingleFile=true*<br/>


  ~Features to be added:<br/>
-New Icon<br/>
-Run invisibly at background<br/>
-Reduce cpu and memory usage<br/>
-Adjustable borders<br/>
-Start as service in background, when windows boots<br/>
-Config file<br/>
-Support more resolution<br/>
-Set custom color for border<br/>
-Custom overlay support for games<br/>


  ~Remarks:<br/>
Please if you can help develop my project or have any ideas, make an Issue ticket or <br/>
do a pull request with detailed descriptions of your code, what you did and why.<br/>
