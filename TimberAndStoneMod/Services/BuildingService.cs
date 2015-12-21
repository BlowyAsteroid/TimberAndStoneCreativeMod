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

        public IBlock getBlock(Vector3 worldPosition)
        {
            return chunkManager.GetBlock(Coordinate.FromWorld(worldPosition));
        }

        private IBlock tempRemoveBlock;
        public bool removeBlock(Coordinate coordinate)
        {
            tempRemoveBlock = getBlock(coordinate);

            if(coordinate.absolute.y > 0 && ModUtils.isBuildable(tempRemoveBlock.properties, true))
            {
                return buildBlock(coordinate, BlockAir.BlockAir);
            }
            else return false;
        }        

        public List<Coordinate> getSelectedCoordinates(bool excludeAirBlocks = false, bool onlyAirBlocks = false)
        {
            if (excludeAirBlocks)
            {
                return ModUtils.createCoordinatesFromSelection(designManager.selectedBlocks)
                    .Where(c => chunkManager.GetBlock(c).properties.GetType() != typeof(BlockAir)).ToList();
            }
            else if (onlyAirBlocks)
            {
                return ModUtils.createCoordinatesFromSelection(designManager.selectedBlocks)
                    .Where(c => chunkManager.GetBlock(c).properties.GetType() == typeof(BlockAir)).ToList();
            }
            else return ModUtils.createCoordinatesFromSelection(designManager.selectedBlocks);
        }
                
        public void buildBlocks(List<Coordinate> coordinates, BlockProperties properties, IBlockData data = null)
        {
            foreach (Coordinate coordinate in coordinates)
            {
                buildBlock(coordinate, properties, data);
            }
        }
        
        public bool buildBlock(Coordinate coordinate, BlockProperties properties, IBlockData data = null)
        {
            if (chunkManager.isCoordInMap(coordinate))
            {
                if (setBlock(coordinate, properties, data))
                {
                    designManager.ReValidateBlock(coordinate);

                    return true;
                }
            }

            return false;
        }

        public bool setBlock(Coordinate coordinate, BlockProperties properties, IBlockData data = null)
        {
            return data != null ? chunkManager.SetBlock(coordinate, properties, data) : chunkManager.SetBlock(coordinate, properties);
        }
        
        public void smoothBlocks(List<Coordinate> coordinates)
        {
            IBlock tempSmoothBlock, tempCompareBlock;
            IBlockData tempBlockData;
            BlockProperties tempBlockProperties;
            int variationIndex = -1;

            foreach(Coordinate coordinate in coordinates)
            {
                if (!chunkManager.isCoordInMap(coordinate)) continue;

                tempSmoothBlock = getBlock(coordinate);

                if (!isValidAirBlock(tempSmoothBlock) || !isValidCompareBlock(tempSmoothBlock.relative(0, -1, 0))) continue;

                variationIndex = getVariationIndexForSmoothing(tempSmoothBlock, out tempCompareBlock);
               
                if (tempCompareBlock == null || variationIndex < 0) continue;

                tempBlockProperties = getSlopeBlockPropertiesFromCubeBlock(tempCompareBlock.properties);

                if (tempBlockProperties.getVariations() == null) continue;

                tempBlockData = tempBlockProperties.getVariations()[variationIndex][0];
                
                buildBlock(coordinate, tempBlockProperties, tempBlockData);
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
        private BlockProperties getSlopeBlockPropertiesFromCubeBlock(BlockProperties blockProperties)
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

            return tempBlockProperties;
        }

        private BlockProperties getCubeBlockPropertiesFromSlopeBlock(BlockProperties blockProperties)
        {
            BlockProperties tempBlockProperties = blockProperties;

            if (GRASS_BLOCKS.Contains(blockProperties.getID()))
            {
                tempBlockProperties = BlockProperties.BlockGrass;
            }
            else if (STONE_BLOCKS.Contains(blockProperties.getID()))
            {
                tempBlockProperties = BlockProperties.BlockStone;
            }
            else if (DIRT_BLOCKS.Contains(blockProperties.getID()))
            {
                tempBlockProperties = BlockProperties.BlockDirt;
            }
            else if (SAND_BLOCKS.Contains(blockProperties.getID()))
            {
                tempBlockProperties = BlockProperties.BlockSand;
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
        
        public void replaceBlocks(List<Coordinate> coordinates, BlockProperties properties, IBlockData data = null)
        {
            IBlock tempReplaceBlock;
            IBlockData tempReplaceBlockData;
            BlockProperties tempReplaceBlockProperties;

            foreach (Coordinate coordinate in coordinates)
            {
                if (!chunkManager.isCoordInMap(coordinate)) continue;

                tempReplaceBlock = getBlock(coordinate);

                if (!ModUtils.isBuildable(tempReplaceBlock.properties, true)) continue;

                if (properties.getVariations() != null && tempReplaceBlock.properties.getVariations() == null) continue;

                if (tempReplaceBlock.properties.getVariations() != null && properties.getVariations() != null)
                {
                    if (tempReplaceBlock.properties.isTransparent())
                    {
                        tempReplaceBlockProperties = getSlopeBlockPropertiesFromCubeBlock(properties);
                        tempReplaceBlockData = tempReplaceBlock.properties.getVariations()[ModUtils.getVariationIndexFromBlock(tempReplaceBlock)][0];
                    }
                    else continue;
                }
                else if (tempReplaceBlock.properties.getVariations() != null)
                {
                    continue;
                }
                else
                {
                    tempReplaceBlockProperties = properties;
                    tempReplaceBlockData = data;
                }

                buildBlock(coordinate, tempReplaceBlockProperties, tempReplaceBlockData);

                tempReplaceBlockProperties = null;
                tempReplaceBlockData = null;                
            }
        }
                
        public List<TreeFlora> getSelectedTrees()
        {
            return terrainManager.treeObjects.Where(tree => tree.inChopQue).ToList();
        }

        public List<Shrub> getSelectedShrubs()
        {
            return terrainManager.shrubObjects.Where(shrub => shrub.inChopQue).ToList();
        }
        
        public bool removeSelectedTrees()
        {
            return getSelectedTrees().Where(tree => removeTree(tree)).Count() > 0;
        }

        public bool removeSelectedShrubs()
        {
            return getSelectedShrubs().Where(shrub => removeShrub(shrub)).Count() > 0;
        }

        public void removeAllTreeItems()
        {
            terrainManager.ClearAll();
        }

        public bool removeTree(TreeFlora tree)
        {
            if (isValidCompareBlock(getBlock(Coordinate.FromChunkBlock(tree.chunkPos, tree.blockPos)), true))
            {
                terrainManager.RemoveTree(tree, 1);
            }
            else terrainManager.RemoveTree(tree, 0);

            return true;
        }

        public bool removeShrub(Shrub shrub)
        {
            if (isValidCompareBlock(getBlock(Coordinate.FromChunkBlock(shrub.chunkPos, shrub.blockPos)), true))
            {
                terrainManager.RemoveShrub(shrub, 1);
            }
            else terrainManager.RemoveShrub(shrub, 0);

            return true;
        }

        private Transform tempFloraTransform;
        private IBlock tempFloraBlock;
        public void addTree(Coordinate coordinate)
        {
            if (!isValidAirBlock(tempFloraBlock = getBlock(coordinate)))
            {
                coordinate = tempFloraBlock.relative(0, 1, 0).coordinate;
            }

            tempFloraTransform = ModUtils.createTransform(terrainManager.treeObject);
            tempFloraTransform.transform.parent = getChunkData(coordinate).chunkObj.transform;
            tempFloraTransform.position = ModUtils.getSlightlyOffsetY(coordinate.world);

            TreeFlora tempAddTree = tempFloraTransform.GetComponent<TreeFlora>();
            tempAddTree.blockPos = ModUtils.getSlightlyOffsetY(coordinate.block);
            tempAddTree.chunkPos = ModUtils.getSlightlyOffsetY(coordinate.chunk);
            tempAddTree.health = 100f;

            terrainManager.AddTree(tempAddTree);
            tempAddTree.Init();
        }       

        public void addShrub(Coordinate coordinate)
        {
            if (!isValidAirBlock(tempFloraBlock = getBlock(coordinate)))
            {
                coordinate = tempFloraBlock.relative(0, 1, 0).coordinate;
            }

            tempFloraTransform = ModUtils.createTransform(terrainManager.shrubObject);
            tempFloraTransform.transform.parent = getChunkData(coordinate).chunkObj.transform;
            tempFloraTransform.position = ModUtils.getSlightlyOffsetY(coordinate.world);

            Shrub tempAddShrub = tempFloraTransform.GetComponent<Shrub>();
            tempAddShrub.blockPos = ModUtils.getSlightlyOffsetY(coordinate.block);
            tempAddShrub.chunkPos = ModUtils.getSlightlyOffsetY(coordinate.chunk);
            tempAddShrub.health = 100f;
            tempAddShrub.berryCount = 50;

            terrainManager.AddShrub(tempAddShrub);
            tempAddShrub.Init(false);
        }

        private ChunkData getChunkData(Coordinate coordinate)
        {
            return chunkManager.chunkArray[coordinate.chunk.x, coordinate.chunk.y, coordinate.chunk.z];
        }
        
        public void buildStructure(ref BuildStructure structure)
        {
            structure.isBuilt = true;
            structure.buildProgress = 100f;
            structure.RenderTextured();
            structure.HideAccessPoints();
            structure.AddBlocks(98);            
        }

        public void setZLevel(float position)
        {
            if (position > 0 && position < chunkManager.worldSize.y)
            {
                while (chunkManager.shownZLevel < position) chunkManager.ZLevelUp(true);
                while (chunkManager.shownZLevel > position) chunkManager.ZLevelDown();
            }
        }
    }
}
