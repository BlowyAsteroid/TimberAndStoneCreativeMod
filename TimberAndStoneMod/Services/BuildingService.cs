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

        private static readonly List<IBlock> EMPTY_BLOCK_LIST = new List<IBlock>();
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
