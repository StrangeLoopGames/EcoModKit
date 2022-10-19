
# Creating ModKit items

Note: This doument provides minimal instructions, for more details see the guides online at https://wiki.play.eco/en/Mod_Development

This document covers the client side objects, before they can be used they need to exist in the game server.

Note: As of 0.9.6 Eco is in the process of moving to Addressables for asset loading. Currently the modkit does not support this yet, however there have been some changes like the use of `ModkitPrefabContainer` to reduce the amount of setup done in the scene. Previous mods will still continue to work for now, but it is recommended to use the latest setup instructions for future compatability.

Different types you can create:
* World Objects - These match by name with objects in the game server
* Items - These are the icons for items and world objects
* Block Sets - These match blocks in the game server, and contain variations of meshes for how to build the mesh in game
* Emoji Sets - A set of emoji that are processed on the client

## Setting up the scene

1. Create a new scene
1. Remove everything from scene
1. Create an empty game object in root of scene and name "Objects"
1. Click "Add Component" and type in `ModkitPrefabContainer`
1. Create an empty game object in root of scene and name "Items"
1. Create an empty game object in root of scene and name "Emoji"
1. Click "Add Component" and type in `ChatEmoteSetOld`
1. Create an empty game object in root of scene and name "BlockSets"
1. Click "Add Component" and type in `BlockSetContainer`

## Adding Items

1. In the project browse to "Assets > EcoModKit > Prefabs"
1. Drag "ItemTemplate" onto "Items" in the scene. This will create a child game object called "ItemTemplate"
1. Right click the new "ItemTemplate" game object in the scene, and select "Prefab > Unpack Completely". This will remove the reference to the prefab.
1. Rename the game object to the name of your item
1. Edit the background and forground sprites

## Creating a new World Object

1. Create a new empty game object in the scene
1. Rename the object to the name of your world object
1. Set the tag to "ModObject"
1. Click "Add Component" and type in `WorldObject`
1. Add mesh
1. Drag the game object to the project window to save it as a prefab
1. Delete the game object in the scene

## Adding World Objects

1. Create object as a prefab
1. Click on "Objects" in scene root
1. Expand "Prefabs" In the "Modkit Prefab Container"
1. Click "+" to add a new element to the list
1. Select or drag the prefab onto the item.

## Adding Emojis

1. Click on the "Emoji" game object in the scene
1. Expand the "Emotes" list
1. Click "+" to add a new element to the list
1. Set the name
1. Set any aliases
1. Set the sprite

## Creating a BlockSet

1. Select the Project window
1. Right click in the view and select "Create > VoxelEngine > BlockSet"
1. Rename the asset
1. Create your blocks

## Adding Block Sets

1. Click on the "BlockSets" game object in the scene
1. Expand the "Block Sets" list
1. Click "+" to add a new element to the list
1. Select your BlockSet
