using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Project Bueno")]
[assembly: AssemblyProduct("Project Bueno Engine")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyDescription("Project Buenche (née Bueno)")]
[assembly: AssemblyCompany("Not Affiliated Inc. (A Joke)")]
[assembly: AssemblyCopyright("Copyright © Juhan Oskar Hennoste 2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("247c85ad-bda7-4e4a-b708-72f02732c5da")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion(PB.VERSION)]
[assembly: AssemblyFileVersion(PB.VERSION)]

internal static class PB
{
	public const string VERSION = "0.2.0.0";
	public const string VERSION_NAME = "Adelost";
}
