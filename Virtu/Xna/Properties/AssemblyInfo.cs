using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using Jellyfish.Library;

[assembly: AssemblyTitle("Virtu")]
[assembly: AssemblyDescription("Apple IIe Emulator")]
#if XBOX
[assembly: AssemblyProduct("Jellyfish.Virtu.Xna.Xbox")]
#else
[assembly: AssemblyProduct("Jellyfish.Virtu.Xna")]
#endif
[assembly: AssemblyCompany("Digital Jellyfish Design Ltd")]
[assembly: AssemblyCopyright("Copyright © 1995-2010 Digital Jellyfish Design Ltd")]
[assembly: AssemblyComment("Developed by Sean Fausett & Nick Westgate")]

[assembly: AssemblyVersion("0.7.0.0")]
#if WINDOWS
[assembly: AssemblyFileVersion("0.7.0.0")]
#endif
[assembly: AssemblyInformationalVersion("0.7.0.0")]

[assembly: CLSCompliant(false)]
[assembly: ComVisible(false)]
[assembly: Guid("89a50370-1ed9-4cf1-ad08-043b6e6f3c90")]

[assembly: NeutralResourcesLanguage("en")]
