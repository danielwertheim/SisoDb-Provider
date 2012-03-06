using System.Reflection;

#if DEBUG
[assembly: AssemblyProduct("SisoDb (Debug)")]
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyProduct("SisoDb (Release)")]
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyDescription("SisoDb - Simple Structure Oriented Db")]
[assembly: AssemblyCompany("Daniel Wertheim")]
[assembly: AssemblyCopyright("Copyright © Daniel Wertheim")]
[assembly: AssemblyTrademark("")]

[assembly: AssemblyVersion("10.4.0")]
[assembly: AssemblyFileVersion("10.4.0")]