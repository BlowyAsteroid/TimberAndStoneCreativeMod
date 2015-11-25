using Plugin.BlowyAsteroid.TimberAndStoneMod.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Timber_and_Stone;
using Timber_and_Stone.API;
using Timber_and_Stone.Blocks;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Services
{
    public class BuildingService
    {
        private static BuildingService instance = new BuildingService();
        public static BuildingService getInstance() { return instance; }

        private TerrainObjectManager terrainManager = TerrainObjectManager.getInstance();
        private ChunkManager chunkManager = ChunkManager.getInstance();
        private DesignManager designManager = DesignManager.getInstance();

        private BuildingService() { }        

        public IBlock getBlock(Coordinate coordinate)
        {
            return chunkManager.GetBlock(coordinate);
        }

        public IBlock getBlock(Vector3 worldPosition)
        {
            return chunkManager.GetBlock(Coordinate.FromWorld(worldPosition));
        }
        
        public bool removeBlock(Coordinate coordinate)
        {
            return coordinate.absolute.y > 0 ? buildBlock(coordinate, BlockAir.BlockAir) != null : false;
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
                
        private List<IBlock> builtBlockList = new List<IBlock>();
        public List<IBlock> buildBlocks(List<Coordinate> coordinates, BlockProperties properties, IBlockData data = null)
        {
            builtBlockList.Clear();

            foreach (Coordinate coordinate in coordinates)
            {
                builtBlockList.Add(buildBlock(coordinate, properties, data));
            }

            return builtBlockList.Where(b => b != null).ToList();
        }
        
        public IBlock buildBlock(Coordinate coordinate, BlockProperties properties, IBlockData data = null)
        {
            if (chunkManager.isCoordInMap(coordinate))
            {
                if (setBlock(coordinate, properties, data))
                {
                    designManager.ReValidateBlock(coordinate);

                    return chunkManager.GetBlock(coordinate);
                }
            }

            return null;
        }

        public bool setBlock(Coordinate coordinate, BlockProperties properties, IBlockData data = null)
        {
            return data != null ? chunkManager.SetBlock(coordinate, properties, data) : chunkManager.SetBlock(coordinate, properties);
        }

        private IBlock tempSmoothBlock, tempCompareBlock;
        private IBlockData tempBlockData;
        private BlockProperties tempBlockProperties;
        private int variationIndex = 0;
        public void smoothBlocks(List<Coordinate> coordinates)
        {
            foreach(Coordinate coordinate in coordinates)
            {
                if (!chunkManager.isCoordInMap(coordinate)) continue;

                tempSmoothBlock = getBlock(coordinate);
                variationIndex = -1;

                if (tempSmoothBlock.properties.GetType() != typeof(BlockAir) || !isValidCompareBlock(tempSmoothBlock.relative(0, -1, 0))) continue;
                                
                
                if (isValidCompareBlock(tempSmoothBlock.relative(-1, 0, 0)))//Left
                {
                    variationIndex = 0;
                }
                else if (isValidCompareBlock(tempSmoothBlock.relative(1, 0, 0)))//Right
                {
                    variationIndex = 6;
                }
                else if (isValidCompareBlock(tempSmoothBlock.relative(0, 0, -1)))//Front
                {
                    variationIndex = 9;
                }
                else if (isValidCompareBlock(tempSmoothBlock.relative(0, 0, 1)))//Back
                {
                    variationIndex = 3;
                }


                else if (isValidCornerBlock(tempSmoothBlock, -1, 1))//Front-Left-Corner
                {
                    variationIndex = 1;
                }
                else if (isValidCornerBlock(tempSmoothBlock, -1, -1))//Back-Left-Corner
                {
                    variationIndex = 10;
                }
                else if (isValidCornerBlock(tempSmoothBlock, 1, 1))//Front-Right-Corner
                {
                    variationIndex = 4;
                }
                else if (isValidCornerBlock(tempSmoothBlock, 1, -1))//Back-Right-Corner
                {
                    variationIndex = 7;
                }
                else continue;
                

                tempBlockProperties = tempCompareBlock.properties;
                

                if (variationIndex == 0 && isValidCompareBlock(tempSmoothBlock.relative(0, 0, -1)))//Left-Front
                {
                    variationIndex = 11;                   
                }
                else if (variationIndex == 0 && isValidCompareBlock(tempSmoothBlock.relative(0, 0, 1)))//Left-Back
                {
                    variationIndex = 2;
                }
                else if (variationIndex == 6 && isValidCompareBlock(tempSmoothBlock.relative(0, 0, -1)))//Right-Front
                {
                    variationIndex = 8;
                }
                else if (variationIndex == 6 && isValidCompareBlock(tempSmoothBlock.relative(0, 0, 1)))//Right-Back
                {
                    variationIndex = 5;
                }


                switch (tempBlockProperties.getID())
                {
                    case 1:
                        tempBlockProperties = BlockProperties.SlopeGrass;
                        tempBlockData = tempBlockProperties.getVariations()[variationIndex][0];
                        break;

                    case 4:
                        tempBlockProperties = BlockProperties.SlopeDirt;
                        tempBlockData = tempBlockProperties.getVariations()[variationIndex][0];
                        break;

                    case 6:
                        tempBlockProperties = BlockProperties.SlopeSand;
                        tempBlockData = tempBlockProperties.getVariations()[variationIndex][0];
                        break;

                    case 8:
                        tempBlockProperties = BlockProperties.SlopeStone;
                        tempBlockData = tempBlockProperties.getVariations()[variationIndex][0];
                        break;

                    default:
                        continue;
                }

                buildBlock(coordinate, tempBlockProperties, tempBlockData);
            }
            
        }

        private bool isValidCompareBlock(IBlock block, bool includeTransparent = false)
        {
            return (tempCompareBlock = block).properties.getID() != 0 && includeTransparent ? true : !block.properties.isTransparent();
        }

        private bool isValidAirBlock(IBlock block)
        {
            return block.properties.getID() == 0;
        }

        private bool isValidCornerBlock(IBlock block, int x, int z)
        {
            return isValidCompareBlock(block.relative(x, 0, z))
                && (block.relative(x, 0, 0).properties.isTransparent() || isValidAirBlock(block.relative(x, 0, 0)))
                && (block.relative(0, 0, z).properties.isTransparent() || isValidAirBlock(block.relative(0, 0, z)));
        }





        private IBlock tempReplaceBlock;
        private IBlockData tempReplaceBlockData;
        private BlockProperties tempReplaceBlockProperties;
        private int tempReplaceVariationIndex;
        private List<IBlock> replacedBlocks = new List<IBlock>();
        public List<IBlock> replaceBlocks(List<Coordinate> coordinates, BlockProperties properties, IBlockData data = null)
        {
            replacedBlocks.Clear();

            foreach (Coordinate coordinate in coordinates)
            {
                if (!chunkManager.isCoordInMap(coordinate)) continue;

                tempReplaceBlock = getBlock(coordinate);

                if (tempReplaceBlock.properties.getVariations() != null)
                {
                    switch (properties.getID())
                    {
                        case 1:
                            tempReplaceBlockProperties = BlockProperties.SlopeGrass;
                            break;

                        case 4:
                            tempReplaceBlockProperties = BlockProperties.SlopeDirt;
                            break;

                        case 6:
                            tempReplaceBlockProperties = BlockProperties.SlopeSand;
                            break;

                        case 8:
                            tempReplaceBlockProperties = BlockProperties.SlopeStone;
                            break;

                        default:
                            tempReplaceBlockProperties = properties;
                            break;
                    }                    

                    tempReplaceBlockData = tempReplaceBlock.properties.getVariations()[ModUtils.getVariationIndexFromBlock(tempReplaceBlock)][0];

                    replacedBlocks.Add(buildBlock(coordinate, tempReplaceBlockProperties, tempReplaceBlockData));
                }
                else
                {
                    if (properties.getVariations() != null)
                    {
                        switch (properties.getID())
                        {
                            case 86:
                                tempReplaceBlockProperties = BlockProperties.BlockGrass;
                                break;

                            case 87:
                                tempReplaceBlockProperties = BlockProperties.BlockDirt;
                                break;

                            case 88:
                                tempReplaceBlockProperties = BlockProperties.BlockSand;
                                break;

                            case 85:
                                tempReplaceBlockProperties = BlockProperties.BlockStone;
                                break;

                            default:
                                tempReplaceBlockProperties = tempReplaceBlock.properties;
                                break;
                        }

                        tempReplaceBlockData = properties.getVariations()[ModUtils.getVariationIndexFromBlock(tempReplaceBlock)][0];

                    }
                    else
                    {
                        tempReplaceBlockProperties = properties;
                        data = null;
                    }

                    replacedBlocks.Add(buildBlock(coordinate, tempReplaceBlockProperties, data));
                }

                tempReplaceBlockProperties = null;
                tempReplaceBlockData = null;                
            }

            return replacedBlocks;
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
            terrainManager.RemoveTree(tree, 1);
            return true;
        }

        public bool removeShrub(Shrub shrub)
        {
            terrainManager.RemoveShrub(shrub, 1);
            return true;
        }
    }
}
