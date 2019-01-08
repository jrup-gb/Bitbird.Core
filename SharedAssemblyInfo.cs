﻿using System;

using System.Reflection;

using System.Runtime.CompilerServices;

using System.Runtime.InteropServices;


// General Information about an assembly is controlled through the following

// set of attributes. Change these attribute values to modify the information

// associated with an assembly.

[assembly: AssemblyCompany("Bitbird GmbH")]

[assembly: AssemblyProduct("Bitbird.Core")]

[assembly: AssemblyCopyright("Copyright © Bitbird GmbH 2019")]

[assembly: AssemblyTrademark("")]


// Make it easy to distinguish Debug and Release (i.e. Retail) builds;

// for example, through the file properties window.

#if DEBUG

[assembly: AssemblyConfiguration("Debug")]
    

#else

[assembly: AssemblyConfiguration("Release")]


#endif


// Setting ComVisible to false makes the types in this assembly not visible

// to COM components.  If you need to access a type in this assembly from

// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]


// Note that the assembly version does not get incremented for every build

// to avoid problems with assembly binding (or requiring a policy or

// <bindingRedirect> in the config file).

//

// The AssemblyFileVersionAttribute is incremented with every build in order

// to distinguish one build from another. AssemblyFileVersion is specified

// in AssemblyVersionInfo.cs so that it can be easily incremented by the

// automated build process.

[assembly: AssemblyVersion(bla.VersionNumberHack.VersionNumber)]

// By default, the "Product version" shown in the file properties window is

// the same as the value specified for AssemblyFileVersionAttribute.

// Set AssemblyInformationalVersionAttribute to be the same as

// AssemblyVersionAttribute so that the "Product version" in the file

// properties window matches the version displayed in the GAC shell extension.

[assembly: AssemblyInformationalVersion(bla.VersionNumberHack.VersionNumber)] // a.k.a. "Product version"

[assembly: AssemblyFileVersion(bla.VersionNumberHack.VersionNumber)]


namespace bla
{
    internal static class VersionNumberHack
    {
        public const string VersionNumber = "1.1.3.0";
    }
}