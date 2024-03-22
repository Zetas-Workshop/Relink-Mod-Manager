# Relink Mod Manager

The Relink Mod Manager is a tool designed for providing easy to use file mod support for Granblue Fantasy: Relink. It has a number of features to give users a robust yet streamlined experience:

- [Mod Importing](#mod-importing)
- [Mod Removing](#mod-removingdeleting)
- [Mod Priorities and Conflicts](#mod-priority-and-conflicts)
- [Previewing Changes before Mod Installation](#effective-changes)
- [Installing](#installing-mods)/[Uninstalling](#uninstalling-mods) Mods
- [Mod Pack Creation](#mod-pack-creation)
- [Editing Existing Mod Packs](#mod-pack-editing)

> [!IMPORTANT]
> Please check out our [**GitHub Wiki**](https://github.com/Relink-Mod-Manager/Relink-Mod-Manager/wiki) for more detailed instructions on using the Mod Manager, it even includes pictures!
>
> The Wiki also will be more up-to-date than this ReadMe file for using the Mod Manager.

Relink Mod Manager uses the generic ZIP Archive format for its Mod Packs. However, it is only intended to be used with Mod Packs either created by it or other tools that support its specific file structure. Mods created for other Mod Managers are NOT supported by it. However, [converting Mods](#how-to-convert-a-mod-made-for-another-mod-manager-to-work-with-relink-mod-manager) made for other Mod Managers to work with Relink Mod Manager is a quick and easy process.

## Mod Importing
Getting a Mod Pack imported is a simple process and the first step to getting the game modified.

1. Launch Relink Mod Manager.
2. Click the Import Mod (Green Plus) button on the bottom bar.
   - You can also find an Import Mods option in the File menu.
3. Select the Mod Pack ZIP archive you want to import.
   - Multiple Mod Packs can be selected at once by holding down CTRL when clicking them.

When a Mod Pack is imported, it gets copied into a location known as the Mod Archive Storage. The Mod Packs here are what ultimately get used when installing mods to your game. By default this location is a folder named `Relink Mod Archives` in your Documents directory.

## Mod Removing/Deleting
Sometimes you imported a Mod and decided it isn't something you want to keep around. Don't worry, removing it is quick and easy!

1. Launch Relink Mod Manager.
2. Select the Mod you wish to remove from the Imported Mods list on the left.
3. Click the Remove Mod (Red Trash Can) button on the bottom bar.
4. Confirm removing the Mod.

## Mod Priority and Conflicts
No one is ever satisfied with just one Mod. However, once you start using a lot of mods you quickly realize some of them want to replace the same file as each other. This creates a Conflict between the Mods. Thankfully, resolving this conflict is a simple process. By using the `Priority` value on each imported Mod, you can control which Mod gets installed first. Mods with a higher priority are installed before those with a lower one.
Example: If `Mod A` is `Priority 1` and `Mod B` is `Priority 0`, `Mod A` will be installed, then `Mod B` will be installed.

It's important to know that Mods installed later do not replace the files of Mods that were installed before them. For example, if you have two Mods one that edits Zeta's model and another that changes Zeta's skin. You would want to set the model edit at a lower priority than the skin mod so that the model is first swapped and then the skin is changed, allowing both Mods to be applied simultaneously.

1. Launch Relink Mod Manager.
2. Select a Mod from the **Imported Mods** list on the left.
3. Change the **Priority** value in the right side Mod Details panel, under the **Settings** tab.

> [!NOTE]
> When multiple mods have the same priority number, they are installed based on the order they were imported into the Mod Manager.

## Effective Changes
Once some imported Mod Packs have been enabled, you may want to know just what files exactly they are going to be changing in your game installation. Enter the Effective Changes feature. In the top menu bar, you can open up a new window that shows exactly what Game Files (left side) will be replaced by Mod Files (right side) and the order of those file replaces that will occur when the currently enabled Mod Packs are installed. This list even takes into account Mod Conflicts and how they've been resolved.

1. Launch Relink Mod Manager.
2. Have at least one Mod enabled.
3. Click the **Effective Changes** button in the top menu bar.

> [!NOTE]
> Clicking on a file path allows you to copy it to your clipboard for easy reference.

## Installing Mods
You've enabled some Mods and now you're ready to apply them to your game install. This process like the others is quite straight forward.

1. Launch Relink Mod Manager.
2. Have at least one Mod enabled.
3. Click the **Install Mods** button in the top menu bar.
4. Wait for the confirmation prompt that Mods were installed.
   - This can be between instant and a few seconds depending on Mod sizes and computer hardware.

## Uninstalling Mods
When you're ready to return to a clean unmodified game state, it's only a click away.

1. Launch Relink Mod Manager.
2. Click the **Uninstall All Mods** button in the top menu bar.
3. Wait for the confirmation prompt that Mods were uninstalled.
   - This typically only takes a few moments at most with the longest part being restoring [volatile files](#what-is-a-volatile-mod).

## Mod Pack Creation
For Mod Authors, Creating an entirely new Mod Pack is just a few clicks away.

1. Launch Relink Mod Manager.
2. Open the **File** menu in the top left.
3. Select **Create Mod Pack**.
4. Begin your new creation!

## Mod Pack Editing
Once a Mod Pack has been created, its life has only just begun. From time to time mods will want to be updated but no one wants to start an update from scratch. With Mod Editing, you can load up an extracted Mod Pack and begin changing its contents and metadata.

1. Start with an extracted Relink Mod Pack
   - If you don't have one, they're just simple ZIP archives.
   - Right click an existing Mod Pack and select to extract it in your File Explorer.
2. Launch Relink Mod Manager.
3. Open the **File** Menu in the top left.
4. Select **Edit Mod Pack**.
5. Select the `ModConfig.json` file in the extracted Mod Pack files.
   - This should be located in the root of the extracted files, often times next to a `data` folder.
6. Begin editing the existing mod!

## FAQ

### How to convert a Mod made for another Mod Manager to work with Relink Mod Manager
Converting Mods is not a scary process and can be done in just a few steps. Before starting, take your desired Mod to convert and extract it into a new location.

1. Launch Relink Mod Manager.
2. Select **Create Mod Pack** from the **File** menu in the top left.
3. Fill in the Mod details such as **Name**, **Description**, **Author**, **Version**, etc.
   - Only the Name field is _required_ but the more details you provide the better the experience for everyone!
4. Click the **Set Directory** button.
5. Select the root of the extracted mod. If it contains a `data` folder, you want to select the folder containing that, not the data folder itself.
   - Ex: If you have a structure such as `MyMod/GBFR/data/UI...` you would select `GBFR` as the directory.
6. Under **Group Options**, enter a name such as "Core Files" in the **New Group Name** text box then click the **Add Group** (Green Plus) button to the right.
   - Making additional Groups is a great way to organize different sets of Mod Options for the end user.
7. Enter in a name for the first Option in the Group, such as "Base Files", then click the **Add Option** (Green Plus) button to the right.
   - You can have as many Mod Options as you want! Making use of them can help provide users with selecting different choices for how your Mod will be installed and what changes specifically they want applied.
8. Click the **Edit** (Orange Pencil) button to the right of your newly added Option to open the **Option Bindings** window.
9. Click the **Add All Unreferenced Files As New Bindings** button.
10. Close the window.
11. Click **Save Mod Pack**.
12. Give your Mod Pack file a good name and complete the process.
    - All Mod Packs are stored in basic ZIP archives, with the .ZIP extension even, feel free to open them up in your favorite ZIP manager and give them a look through!

> [!IMPORTANT]
> The steps here result in creating a basic Mod Pack where there is only a single `Enable` checkbox and toggling it will toggle installing all Mod files. It is strongly recommended if you have a Mod that changes multiple aspects such as both a Model and UI textures, that you make use of additional Groups and Options. Options only need File Bindings for the specific files that Option makes use of, not every single Mod File again. [Check the Wiki for more details]((https://github.com/Relink-Mod-Manager/Relink-Mod-Manager/wiki)).

You've now converted an existing mod and it's ready to import back into the Relink Mod Manager and begin using it!

### What is a Volatile Mod
Mods that replace game files in the `data/Sound/` or `data/UI/` directory are considered to be "volatile" as by default there are files stored in those directories of the game. If a Mod were to overwrite one of these, an actual original game file has now been changed and is not as simple as just deleting the Mod file to restore the game one. Instead, a backup of the game file must be made or Steam must be used to verify and restore the original game files. This makes Mods that change files in these directories a bit volatile to your overall game health in terms of restoring its original state.

Relink Mod Manager will automatically perform a backup of the original game files that get replaced and store them in a folder named `data_volatile_backups` that's located right next to your original `data` folder in the game directory. When a Mod is uninstalled that modified one of those files, it gets restored. However, since this is an additional step for only a specific set of Mods, they get special treatment so users are aware that if something does go wrong, they will need to potentially sort it out.

> [!NOTE]
> You will only ever see the `data_volatile_backups` folder with content in it if a volatile mod is enabled _and_ the file it replaces actually did exist as an external game file. Most files referenced in `data/Sound/` or `data/UI` are stored in Pack files instead of loosely in your data folder so they will not actually be volatile and cause a backup to take place!
