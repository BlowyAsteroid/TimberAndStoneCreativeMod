using Plugin.BlowyAsteroid.TimberAndStoneMod.Utils;
using System.Collections.Generic;
using System.Linq;
using Timber_and_Stone;
using Timber_and_Stone.API;
using Timber_and_Stone.Blocks;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Services
{
    public sealed class BuildingService
    {
        private static readonly BuildingService instance = new BuildingService();
        public static BuildingService getInstance() { return instance; }

        private TerrainObjectManager terrainManager = TerrainObjectManager.getInstance();
        private ChunkManager chunkManager = ChunkManager.getInstance();
        private DesignManager designManager = DesignManager.getInstance();
        private CraftingManager craftingManager = CraftingManager.getInstance();

        private BuildingService() { }        

        public IBlock getBlock(Coordinate coordinate)
        {
            return chunkManager.GetBlock(coordinate);
        }

        public void removeBlocksInSelection()
        {
            IBlock tempRemoveBlock;

            foreach (Coordinate coordinate in getSelectedCoordinates(excludeAirBlocks: true))
            {
                tempRemoveBlock = getBlock(coordinate);

                if(coordinate.absolute.y > 0 && ModUtils.isBuildable(tempRemoveBlock.properties, true))
                {
                    setBlock(coordinate, BlockAir.BlockAir);
                }
            }
        }

        public IEnumerable<Coordinate> getSelectedCoordinates(bool excludeAirBlocks = false, bool onlyAirBlocks = false)
        {
            foreach (Coordinate coordinate in ModUtils.createCoordinatesFromSelection(designManager.selectedBlocks))
            {
                if (!chunkManager.isCoordInMap(coordinate)) continue;

                if (excludeAirBlocks)
                {
                    if (!(chunkManager.GetBlock(coordinate).properties is BlockAir))
                    {
                        yield return coordinate;
                    }
                }
                else if (onlyAirBlocks)
                {
                    if (chunkManager.GetBlock(coordinate).properties is BlockAir)
                    {
                        yield return coordinate;
                    }
                }
                else yield return coordinate;
            }
        }

        public void buildBlocksInSelection(BlockProperties properties, IBlockData data = null)
        {
            foreach (Coordinate coordinate in getSelectedCoordinates(onlyAirBlocks: true))
            {
                setBlock(coordinate, properties, data);               
            }
        }

        public bool setBlock(Coordinate coordinate, BlockProperties properties, IBlockData data = null)
        {
            if (!chunkManager.isCoordInMap(coordinate)) return false;

            return data != null 
                ? chunkManager.SetBlock(coordinate, properties, data) 
                : chunkManager.SetBlock(coordinate, properties);
        }        

        public void smoothBlocksInSelection()
        {
            IBlock tempSmoothBlock, tempCompareBlock;
            IBlockData tempBlockData;
            BlockProperties tempBlockProperties;
            int variationIndex;

            foreach (Coordinate coordinate in getSelectedCoordinates(onlyAirBlocks: true))
            {
                tempSmoothBlock = getBlock(coordinate);

                if (!isValidCompareBlock(tempSmoothBlock.relative(0, -1, 0))) continue;

                variationIndex = getVariationIndexForSmoothing(tempSmoothBlock, out tempCompareBlock);
               
                if (tempCompareBlock == null || variationIndex < 0) continue;

                tempBlockProperties = getSlopeBlockPropertiesForBlock(tempCompareBlock.properties);

                if (tempBlockProperties.getVariations() == null) continue;

                tempBlockData = tempBlockProperties.getVariations()[variationIndex][0];
                
                setBlock(coordinate, tempBlockProperties, tempBlockData);
            }            
        }

        private int getVariationIndexForSmoothing(IBlock block, out IBlock compareBlock)
        {
            int variationIndex = -1;

            if (isValidInnerCornerBlock(block, -1, -1))//Front-Left-Corner
            {
                compareBlock = block.relative(-1, 0, -1);
                variationIndex = 11;
            }
            else if (isValidInnerCornerBlock(block, -1, 1))//Back-Left-Corner
            {
                compareBlock = block.relative(-1, 0, 1);
                variationIndex = 2;
            }
            else if (isValidInnerCornerBlock(block, 1, 1))//Back-Right-Corner
            {
                compareBlock = block.relative(1, 0, 1);
                variationIndex = 5;
            }
            else if (isValidInnerCornerBlock(block, 1, -1))//Front-Right-Corner
            {
                compareBlock = block.relative(1, 0, -1);
                variationIndex = 8;
            }


            else if (isValidCompareBlock(block.relative(-1, 0, 0)))//Left
            {
                compareBlock = block.relative(-1, 0, 0);
                variationIndex = 0;
            }
            else if (isValidCompareBlock(block.relative(1, 0, 0)))//Right
            {
                compareBlock = block.relative(1, 0, 0);
                variationIndex = 6;
            }
            else if (isValidCompareBlock(block.relative(0, 0, -1)))//Front
            {
                compareBlock = block.relative(0, 0, -1);
                variationIndex = 9;
            }
            else if (isValidCompareBlock(block.relative(0, 0, 1)))//Back
            {
                compareBlock = block.relative(0, 0, 1);
                variationIndex = 3;
            }


            else if (isValidOuterCornerBlock(block, -1, 1))//Front-Left-Corner
            {
                compareBlock = block.relative(-1, 0, 1);
                variationIndex = 1;
            }
            else if (isValidOuterCornerBlock(block, -1, -1))//Back-Left-Corner
            {
                compareBlock = block.relative(-1, 0, -1);
                variationIndex = 10;
            }
            else if (isValidOuterCornerBlock(block, 1, 1))//Front-Right-Corner
            {
                compareBlock = block.relative(1, 0, 1);
                variationIndex = 4;
            }
            else if (isValidOuterCornerBlock(block, 1, -1))//Back-Right-Corner
            {
                compareBlock = block.relative(1, 0, -1);
                variationIndex = 7;
            }


            else compareBlock = null;

            return variationIndex;
        }

        private static readonly int[] GRASS_BLOCKS = new int[] { 1, 2, 10, 64, 86 };
        private static readonly int[] STONE_BLOCKS = new int[] { 8, 66, 67, 68, 69, 70, 71, 72, 85 };
        private static readonly int[] DIRT_BLOCKS = new int[] { 4, 40, 87 };
        private static readonly int[] SAND_BLOCKS = new int[] { 6, 88 };
        private static readonly int[] THEME_BLOCKS_1 = new int[] { 24, 34, 89 };
        private static readonly int[] THEME_BLOCKS_2 = new int[] { 26, 90 };
        private static readonly int[] THEME_BLOCKS_3 = new int[] { 15, 93 };
        private static readonly int[] THEME_BLOCKS_4 = new int[] { 25, 30, 92 };
        private BlockProperties getSlopeBlockPropertiesForBlock(BlockProperties blockProperties)
        {
            BlockProperties tempBlockProperties = blockProperties;

            if (GRASS_BLOCKS.Contains(blockProperties.getID()))
            {
                tempBlockProperties = BlockProperties.SlopeGrass;
            }
            else if (STONE_BLOCKS.Contains(blockProperties.getID()))
            {
                tempBlockProperties = BlockProperties.SlopeStone;
            }
            else if (DIRT_BLOCKS.Contains(blockProperties.getID()))
            {
                tempBlockProperties = BlockProperties.SlopeDirt;
            }
            else if (SAND_BLOCKS.Contains(blockProperties.getID()))
            {
                tempBlockProperties = BlockProperties.SlopeSand;
            }
            else if (THEME_BLOCKS_1.Contains(blockProperties.getID()))
            {
                tempBlockProperties = BlockProperties.SlopeCeramic;
            }
            else if (THEME_BLOCKS_2.Contains(blockProperties.getID()))
            {
                tempBlockProperties = BlockProperties.SlopeWoodTile;
            }
            else if (THEME_BLOCKS_3.Contains(blockProperties.getID()))
            {
                tempBlockProperties = BlockProperties.SlopeNordicShingles;
            }
            else if (THEME_BLOCKS_4.Contains(blockProperties.getID()))
            {
                tempBlockProperties = BlockProperties.SlopeCeramic2;
            }

            return tempBlockProperties;
        }

        private bool isValidCompareBlock(IBlock block, bool includeTransparent = false)
        {
            return !isValidAirBlock(block) && (includeTransparent ? true : !block.properties.isTransparent());
        }

        private bool isValidAirBlock(IBlock block)
        {
            return block != null && block.properties.GetType() == typeof(BlockAir);
        }

        private bool isValidOuterCornerBlock(IBlock block, int x, int z)
        {
            return isValidCompareBlock(block.relative(x, 0, z))
                && (block.relative(x, 0, 0).properties.isTransparent() || isValidAirBlock(block.relative(x, 0, 0)))
                && (block.relative(0, 0, z).properties.isTransparent() || isValidAirBlock(block.relative(0, 0, z)));
        }

        private bool isValidInnerCornerBlock(IBlock block, int x, int z)
        {
            return isValidCompareBlock(block.relative(x, 0, 0))
                && isValidCompareBlock(block.relative(0, 0, z))
                && (block.relative(-x, 0, 0).properties.isTransparent() || isValidAirBlock(block.relative(-x, 0, 0)))
                && (block.relative(0, 0, -z).properties.isTransparent() || isValidAirBlock(block.relative(0, 0, -z)));
        }        
        
        public void replaceBlocksInSelection(BlockProperties properties, IBlockData data = null)
        {
            IBlock tempReplaceBlock;
            IBlockData tempReplaceBlockData;
            BlockProperties tempReplaceBlockProperties;

            foreach (Coordinate coordinate in getSelectedCoordinates(excludeAirBlocks: true))
            {
                tempReplaceBlock = getBlock(coordinate);

                if (!ModUtils.isBuildable(tempReplaceBlock.properties, includeAlternates: true)) continue;
                                
                if (properties.getVariations() != null && tempReplaceBlock.properties.getVariations() != null)                    
                {                    
                    tempReplaceBlockProperties = getSlopeBlockPropertiesForBlock(properties);
                    tempReplaceBlockData = tempReplaceBlock.properties.getVariations()[ModUtils.getVariationIndex(tempReplaceBlock)][0];                   
                }
                else if (properties.getVariations() == null && tempReplaceBlock.properties.getVariations() == null)
                {
                    tempReplaceBlockProperties = properties;
                    tempReplaceBlockData = data;
                }
                else continue;

                setBlock(coordinate, tempReplaceBlockProperties, tempReplaceBlockData);              
            }
        }
        
        public void removeSelectedTrees()
        {
            foreach(TreeFlora tree in terrainManager.treeObjects.Where(tree => tree.inChopQue).ToList())
            {
                if (isValidCompareBlock(getBlock(Coordinate.FromChunkBlock(tree.chunkPos, tree.blockPos)), true))
                {
                    terrainManager.RemoveTree(tree, 1);
                }
                else terrainManager.RemoveTree(tree, 0);
            }
        }

        public void removeSelectedShrubs()
        {
            foreach(Shrub shrub in terrainManager.shrubObjects.Where(shrub => shrub.inChopQue).ToList())
            {
                if (isValidCompareBlock(getBlock(Coordinate.FromChunkBlock(shrub.chunkPos, shrub.blockPos)), true))
                {
                    terrainManager.RemoveShrub(shrub, 1);
                }
                else terrainManager.RemoveShrub(shrub, 0);
            }
        }

        public void removeAllTreeItems()
        {
            terrainManager.ClearAll();
        }
        
        public void addTree(Coordinate coordinate)
        {
            TreeFlora tempAddTree = createFlora<TreeFlora>(coordinate, terrainManager.treeObject);
            terrainManager.AddTree(tempAddTree);
            tempAddTree.Init();
        }       

        public void addShrub(Coordinate coordinate)
        {
            Shrub tempAddShrub = createFlora<Shrub>(coordinate, terrainManager.shrubObject);
            terrainManager.AddShrub(tempAddShrub);
            tempAddShrub.Init(false);
        }

        private T createFlora<T>(Coordinate coordinate, Transform parent) where T : MonoBehaviour
        {
            Transform tempFloraTransform;
            IBlock tempFloraBlock;

            if (!isValidAirBlock(tempFloraBlock = getBlock(coordinate)))
            {
                coordinate = tempFloraBlock.relative(0, 1, 0).coordinate;
            }

            tempFloraTransform = ModUtils.createTransform(parent);
            tempFloraTransform.transform.parent = getChunkData(coordinate).chunkObj.transform;
            tempFloraTransform.position = coordinate.world;

            T tempFlora = tempFloraTransform.GetComponent<T>();

            if (tempFlora is TreeFlora)
            {
                TreeFlora tempTree = tempFlora as TreeFlora;
                tempTree.blockPos = coordinate.block;
                tempTree.chunkPos = ModUtils.getSlightlyOffsetY(coordinate.chunk);
                tempTree.health = 100f;
            }
            else if (tempFlora is Shrub)
            {
                Shrub tempShrub = tempFlora as Shrub;
                tempShrub.blockPos = coordinate.block;
                tempShrub.chunkPos = ModUtils.getSlightlyOffsetY(coordinate.chunk);
                tempShrub.health = 100f;
                tempShrub.berryCount = 100;
            }
            
            return tempFlora;
        }

        private ChunkData getChunkData(Coordinate coordinate)
        {
            return chunkManager.chunkArray[coordinate.chunk.x, coordinate.chunk.y, coordinate.chunk.z];
        }        
        
        public void buildStructure(ref BuildStructure structure, IFaction faction)
        {
            structure.beingBuilt = false;
            structure.isBuilt = true;
            structure.buildProgress = 1f;
            structure.RenderTextured();
            structure.HideAccessPoints();
            structure.AddBlocks(98);

            ResourceService.getInstance().addStructureToFactionStorage(structure, faction);
        }

        public void setDepthLevel(float position)
        {
            if (position > 0 && position < chunkManager.worldSize.y)
            {
                while (chunkManager.shownZLevel < position) chunkManager.ZLevelUp(true);
                while (chunkManager.shownZLevel > position) chunkManager.ZLevelDown();
            }
        }
    }
}
