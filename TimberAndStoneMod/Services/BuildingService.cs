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

        public static IBlock getBlockFromPosition(Vector3 position)
        {
            return instance.getBlock(Coordinate.FromWorld(position));
        }

        private TerrainObjectManager terrainManager = TerrainObjectManager.getInstance();
        private ChunkManager chunkManager = ChunkManager.getInstance();
        private DesignManager designManager = DesignManager.getInstance();

        private BuildingService() { }

        private const int MAX_BLOCK_INDEX = 255;
        private BlockProperties tempProperties = BlockProperties.fromID(0);
        private String tempName = String.Empty;
        private List<String> usedBlockNames = new List<String>();
        private List<BlockProperties> availableBlockTypes;
        public List<BlockProperties> getBlockTypes()
        {
            if (availableBlockTypes != null) return availableBlockTypes;

            availableBlockTypes = new List<BlockProperties>();

            for (int i = 0; i < MAX_BLOCK_INDEX; i++)
            {
                if ((tempProperties = BlockProperties.fromID(i)) != null)
                {
                    tempName = tempProperties.getName().ToLower().Trim();

                    if (!availableBlockTypes.Contains(tempProperties)
                        && !usedBlockNames.Contains(tempName)
                        && !tempName.Contains("technical")
                        && !tempName.Contains("block (")
                        && !tempName.Contains("scaffolding"))
                    {
                        usedBlockNames.Add(tempName);
                        availableBlockTypes.Add(tempProperties);
                    }
                }
            }

            return availableBlockTypes;
        }

        public List<BlockProperties> getUnbuildableBlockTypes()
        {
            return getBlockTypes().Where(t => !t.isBuildable() && !t.getName().Contains("Sand") && !t.getName().Contains("Crop")).ToList();
        }

        public IBlock getBlock(Coordinate coordinate)
        {
            return chunkManager.GetBlock(coordinate);
        }

        public IBlock getBlock(Vector3 worldPosition)
        {
            return chunkManager.GetBlock(Coordinate.FromWorld(worldPosition));
        }

        public bool buildBlock(Coordinate coordinate, BlockProperties properties, IBlockData data = null)
        {
            if (coordinate.absolute.y > 0)
            {
                if (data != null ? chunkManager.SetBlock(coordinate, properties, data) : chunkManager.SetBlock(coordinate, properties))
                {
                    designManager.ReValidateBlock(coordinate);

                    return true;
                }
            }

            return false;
        }
        
        public bool removeBlock(Coordinate coordinate)
        {
            return buildBlock(coordinate, BlockAir.BlockAir);
        }

        public void fillBuildDesignationsWith(BlockProperties properties, IBlockData data = null)
        {
            foreach (Coordinate c in designManager.getBuildPoolBlocks())
            {
                buildBlock(c, properties, data);
            }
        }

        private const float OFFSET_Y = 0.1f;
        private List<Coordinate> selectedCoordinates = new List<Coordinate>();
        private Vector3 startPosition, tempPosition;
        private Coordinate tempCoordinate;
        private IBlock tempBlock;
        public List<Coordinate> getSelectedCoordinates(bool isOffsetY = false, bool excludeAirBlocks = true)
        {
            selectedCoordinates.Clear();

            if (designManager.selectedBlocks.Count() > 1)
            {
                startPosition = designManager.selectedBlocks[0];
                startPosition.y += isOffsetY ? -OFFSET_Y : 0;

                for (int i = 1; i < designManager.selectedBlocks.Count(); i++)
                {
                    tempPosition = designManager.selectedBlocks[i];
                    tempCoordinate = Coordinate.FromWorld(startPosition + tempPosition * 0.2f);

                    if ((tempBlock = chunkManager.GetBlock(tempCoordinate)) != null)
                    {
                        if (excludeAirBlocks && tempBlock.properties.GetType() == typeof(BlockAir)) continue;
                       
                        selectedCoordinates.Add(tempCoordinate);
                    }
                }
            }

            return selectedCoordinates;
        }

        public void buildStructures()
        {
            throw new NotImplementedException();
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
