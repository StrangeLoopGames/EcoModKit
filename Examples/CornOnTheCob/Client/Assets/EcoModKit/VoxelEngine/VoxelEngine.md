
# Authoring block data

## Meshes

All of the meshes for a block are contained in a BlockMeshLodGroup. LOD0 allows you to set an array of meshes (that
will be chosen from using noise based on the world position.LOD1 and LOD2 meshes can also be set for generating lower
detail chunks.

LOD0 meshes use the materials set in the block, however LOD1 and LOD2 meshes use a single material with different textures.

