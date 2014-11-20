using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

[assembly: AssemblyProduct("Toqe.Downloader")]
[assembly: AssemblyCompany("Tobias Ebert")]
[assembly: AssemblyCopyright("Tobias Ebert")]

[assembly: AssemblyVersion("1.0.0.0")]

#if DEBUG
[assembly : AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif