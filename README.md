KinectGamePlayer
================

# Pre-Compilation
* KinectGamePlayer depends on the Kinect 1.8 libraries. These can acquired via the [SDK](http://www.microsoft.com/en-us/download/details.aspx?id=40278) or the [Runtime](http://www.microsoft.com/en-us/download/details.aspx?id=40277) (untested with the runtime only).
* Compilation of KinectGamePlayer depends on the .NET Framework, and was developed using the 4.5 version, available [here](http://msdn.microsoft.com/en-us/vstudio/aa496123.aspx). It may work with lower versions of .NET, but has not been tested.
* A Kinect 1.8 is necessary, for obvious reasons.
* An installation of Visual Studio is required. The [latest version](http://www.visualstudio.com/products/visual-studio-community-vs), which is also free, is recommended.

# Compilation
1. Open KinectGamePlayer/KinectGamePlayer.sln
2. In Visual Studio, select Build -> (Re)Build Solution from the dropdown menus

# Running
1. Navigate to KinectGamePlayer/bin/x86/Debug
2. Copy the two text files from preprocessedData at the root of the repository here (You can also use the SkeletonRecorder and HistogrammerRunner programs to record and prepare different data)
3. You can now run the utility from Visual Studio or via the executable binary KinectGamePlayer.exe in the folder

# Supporting Projects In The Solution
* Histogrammer: Contains classes useful for histogramming raw skeleton data. Library (dll) only.
* HistogrammerRunner: Processes raw skeleton logs in the folder data/, expected to be inside the program's current working directory
* LibSVMSharp (+Demo): Wrappers around the LibSVM library. Included for convenience because the maintainer does not provide readily available binaries or NuGet packages.
* SkeletonRecorder: Records raw skeleton data from the Kinect into log files which can be processed later into training data. Very little protection against overwriting recorded data, be careful with this one!
