# History
The [Apple IIe](http://en.wikipedia.org/wiki/Apple_IIe) left an indelible mark on my teenage psyche. Virtu was born out of an interest in emulators and wanting to wallow in nostalgia. Over time it has also become a vehicle to explore developer environments.

Originally developed for [RISC OS](http://en.wikipedia.org/wiki/RISC_OS) 3.11 on the [Acorn Archimedes](http://en.wikipedia.org/wiki/Acorn_Archimedes) in 1995 using some [C](http://en.wikipedia.org/wiki/C_(programming_language)) but mostly [ARM](http://en.wikipedia.org/wiki/ARM_architecture) assembly language. Published on the [cover disk](http://cid-66272a9ce9cb8f75.skydrive.live.com/self.aspx/Virtu/AcornUser.Article.jpg) of the October 1997 issue of [Acorn User](http://en.wikipedia.org/wiki/Acorn_User). Later that year we started porting Virtu to [Microsoft Windows](http://en.wikipedia.org/wiki/Microsoft_Windows) 95 on the ['PC'](http://en.wikipedia.org/wiki/IBM_PC_compatible) using [C++](http://en.wikipedia.org/wiki/C%2B%2B) with [DirectX](http://en.wikipedia.org/wiki/DirectX). A port to [Microsoft Windows CE](http://en.wikipedia.org/wiki/Microsoft_Windows_CE) 2.11 soon followed. These were tweaked over the next couple of years but never published. Fast forward to the present and the latest incarnation of Virtu, this time ported to the [Microsoft .NET Framework](http://en.wikipedia.org/wiki/.NET_Framework) using [C#](http://en.wikipedia.org/wiki/C_Sharp_(programming_language)) with [Silverlight](http://en.wikipedia.org/wiki/Microsoft_Silverlight) (on Windows, [Mac OS X](http://en.wikipedia.org/wiki/Mac_OS_X), and [Windows Phone 7](http://en.wikipedia.org/wiki/Windows_Phone_7)); [WPF](http://en.wikipedia.org/wiki/Windows_Presentation_Foundation) (on Windows only); and [XNA](http://en.wikipedia.org/wiki/Microsoft_XNA) (on Windows, [Xbox 360](http://en.wikipedia.org/wiki/Xbox_360), and Windows Phone 7).

# Platforms
The target platform has been abstracted to a software framework which can take advantage of the various implementations and subsets of the framework to get more portability. I'm currently interested in all of the above for various reasons and couldn't choose which to target, so I thought it would be interesting to target them all, and thereby compare and contrast them to learn the pros and cons of each. Check out the Virtu [Silverlight](Docs/Silverlight/ReadMe.md), [WPF](Docs/WPF/ReadMe.md) & [XNA](Docs/XNA/ReadMe.md) wiki pages for platform specific details.

# Usage
After a successful build, you should be able to run the emulator and perform a self test by pressing the key combination Control+OpenApple+CloseApple+Reset. Boot to the monitor (after Reset) where you can program interactively using [Applesoft BASIC](http://en.wikipedia.org/wiki/Applesoft_BASIC) or CALL-151 to use [6502](http://en.wikipedia.org/wiki/MOS_Technology_6502) assembly language. Of course disk images can also be booted to complete the nostalgia experience.

The current release is an early work in progress and has many rough edges. Most notably, there is no user interface for configuring the emulator, but this can be worked around by editing the default machine settings source at build time, or in some cases pressing a hotkey at runtime. Any platform specific issues will be listed on their wiki page.

# Recommendations
[Xbox 360 Controller](http://en.wikipedia.org/wiki/Xbox_360_Controller) for Windows, or
[Xbox 360 Console](http://en.wikipedia.org/wiki/Xbox_360) and
[Xbox 360 Chatpad](http://en.wikipedia.org/wiki/Xbox_360_Controller#Xbox_360_Messenger_Kit)

# Silverlight Demo
~{silverlight:url=http://virtu.jellyfish.co.nz/Jellyfish.Virtu.xap,width=640,height=480}~

See the Virtu [Silverlight](Docs/Silverlight/ReadMe.md) wiki page for key bindings. For the best image, set the browser zoom to 100%, 200% etc, or install and run out of browser, which can also run fullscreen.