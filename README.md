Pathfinding Framework
===

[![RimWorld](https://img.shields.io/badge/RimWorld-1.4-informational)](https://rimworldgame.com/) [![Steam Downloads](https://img.shields.io/steam/downloads/ToDo)](https://steamcommunity.com/sharedfiles/filedetails/?id=2813426619) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.1-4baaaa.svg)](CODE_OF_CONDUCT.md)

Ever wished your colonists in [RimWorld](https://rimworldgame.com/) could do more than just walk? With the [Pathfinding Framework](https://steamcommunity.com/sharedfiles/filedetails/?id=ToDo) mod, they can! The mod introduces a variety of new movement types, allowing pawns to move across the world in entirely new ways.

You only need to add this mod to your modlist, along with any other mods of your choice that have support for this framework (such as for example mods in the Biomes! series). Pathfinding Framework will then automatically assign appropriate movement types to pawns based on their characteristics, apparel, and other factors.

**Discord server:** https://discord.gg/HB3KyzStgp

### Features

* **Diverse movement types:** Introduces a variety of movement types, including aquatic, flying, and drilling.


* **Terrain interaction:** Pawns intelligently interact with terrains based on their movement type. Watch as birds effortlessly fly over obstacles or fish navigate through deep waters.


* **Extensible framework for modders:** Modders can easily patch movement type support for their pawns, apparel genes or other items, or even integrate and define custom movement types. Check the [Modding wiki](https://github.com/joseasoler/Pathfinding-Framework/wiki/Modding) for details.


* **Compatible and performance-friendly:** Designed to play well with other mods and optimized for smooth gameplay.

### Frequenty Asked Questions (FAQ)

**Can I safely add this mod to an existing save?**

Yes.

**Can I safely remove this mod from an existing save?**

No.

**Is this mod compatible with...**

Pathfinding Framework should be compatible with almost every mod. Other pathfinding mods can be found in the table below.

| Mod                                                                                                        | Compatible  |
|------------------------------------------------------------------------------------------------------------|-------------|
| [Clean Pathfinding 2](https://steamcommunity.com/sharedfiles/filedetails/?id=2603765747)                   | Yes         |
| [Vanilla Expanded Framework](https://steamcommunity.com/sharedfiles/filedetails/?id=2023507013)            | Yes         |
| [Vanilla Furniture Expanded - Security](https://steamcommunity.com/sharedfiles/filedetails/?id=1845154007) | Partial (1) |
| [SwimmingKit](https://steamcommunity.com/sharedfiles/filedetails/?id=1542399915)                           | No          |
| [TerrainMovementKit](https://steamcommunity.com/sharedfiles/filedetails/?id=2048567351)                    | No          |

1. Pawns move at normal speed over trenches. See https://github.com/joseasoler/Pathfinding-Framework/issues/7

Development
---

To compile this mod on Windows, you will need to install the [.NET Framework 4.8 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48). On Linux the packages you need vary depending on your distribution of choice. Dependencies are managed using NuGet. Your checkout must be placed in the RimWorld/Mods folder to let it find the RimWorld assemblies required for compilation.

Contributions
---

This project encourages community involvement and contributions. Check the [CONTRIBUTING](CONTRIBUTING.md) file for details.

• [MSeal](https://github.com/MSeal/): Original implementation of [TerrainMovementKit](https://github.com/MSeal/RimworldTerrainMovementKit) and [SwimmingKit](https://github.com/MSeal/RimworldSwimming).

• [joseasoler](https://github.com/joseasoler): New implementation of the Pathfinding Framework.

Other contributors can be checked in the [contributors list](https://github.com/joseasoler/pathfinding-framework/graphs/contributors).

License
---

This project is licensed under the MIT license. Check the [LICENSE](LICENSE) file for details.

Acknowledgements
---

Read the [ACKNOWLEDGEMENTS](ACKNOWLEDGEMENTS.md) file for details.
