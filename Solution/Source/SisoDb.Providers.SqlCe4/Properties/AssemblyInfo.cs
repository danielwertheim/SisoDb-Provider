using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SisoDb.Providers.SqlCe4")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Daniel Wertheim")]
[assembly: AssemblyProduct("SisoDb.Providers.SqlCe4")]
[assembly: AssemblyCopyright("Copyright © Daniel Wertheim 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("fcf681dc-e84e-4f64-b685-2f8c5bfc5737")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.2.0.0")]
[assembly: AssemblyFileVersion("0.2.0.0")]
#if DEPLOY
#else
[assembly: InternalsVisibleTo("SisoDb.TestUtils")]
[assembly: InternalsVisibleTo("SisoDb.Tests.IntegrationTests")]
[assembly: InternalsVisibleTo("SisoDb.Tests.UnitTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif
