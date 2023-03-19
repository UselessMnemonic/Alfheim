# Hermodr
My personal Valheim IPC server library for non-game clients.

## Building
### Environment
It is expected your environment defines these variables:

 - `BEPINEX_UNSTRIPPED` set to the location of the unstripped libraries
 - `BEPINEX_CORE` set to the location of the core BepInEx libraries
 - `VALHEIM_PUBLICIZED` set to the location of your publicized game libraries

### BepInEx
BepInEx is required to be installed on your Valheim instance. You may source a version particularly suited for Valheim (with installation instructions) [here](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/).

### Game Libraries
To reference game libraries, you must extract their public APIs.
Install the [publicizer mod](https://github.com/elliotttate/Bepinex-Tools/releases/tag/1.0.1-Publicizer) into your Valheim plugins and special stub libraries will be generated at `$VALHEIM_DATA/Managed/publicized_assemblies`.
