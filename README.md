# Alfheim
A simple JSON RPC library for developers of Valheim mods.

## Building
### Environment
It is expected your environment define a variable called `VALHEIM_DATA` set to the path where Valheim's data files are located.

On Windows machines, it's typically in `C:\Program Files (x86)\Steam\steamapps\common\Valheim\valheim_Data`.

If you want to specifically target the server, it's at `C:\Program Files (x86)\Steam\steamapps\common\Valheim dedicated server\valheim_server_Data`.

Linux users, consult your Steam installation or server manager.

### BepInEx
BepInEx is required to be installed on your Valheim instance. You may source a version particularly suited for Valheim (with installation instructions) [here](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/).

### Game Libraries
To reference game libraries, you must extract their public API.
Install the [publicizer mod](https://github.com/elliotttate/Bepinex-Tools/releases/tag/1.0.1-Publicizer) into your Valheim plugins and special stub libraries will be generated at `$VALHEIM_DATA/Managed/publicized_assemblies`.
