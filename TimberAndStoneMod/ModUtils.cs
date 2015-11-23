using Plugin.BlowyAsteroid.TimberAndStoneMod.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timber_and_Stone;
using Timber_and_Stone.Blocks;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public class ModUtils
    {
        private const float BLOCK_TO_WORLD_SIZE = 0.2f;
        private const float BLOCK_TO_CHUNK_SIZE = 0.1f;

        private static readonly List<Coordinate> EMPTY_COORDINATE_LIST = new List<Coordinate>();

        private static List<Coordinate> selectedCoordinates = new List<Coordinate>();
        public static List<Coordinate> createCoordinates(Vector3 startPosition, List<Vector3> offsetPositions)
        {
            selectedCoordinates.Clear();

            offsetPositions.ForEach(v => selectedCoordinates.Add(Coordinate.FromWorld(startPosition + v * BLOCK_TO_WORLD_SIZE)));            

            return selectedCoordinates;
        }

        private static Vector3 tempStartPosition;
        public static List<Coordinate> createCoordinatesFromSelection(List<Vector3> designManagerSelectedBlocks)
        {
            if (!isDesignManagerSelectedBlocks(designManagerSelectedBlocks)) return EMPTY_COORDINATE_LIST;

            tempStartPosition = designManagerSelectedBlocks[0];
            tempStartPosition.y -= BLOCK_TO_CHUNK_SIZE;

            return createCoordinates(tempStartPosition, designManagerSelectedBlocks.GetRange(1, designManagerSelectedBlocks.Count()-1).ToList());            
        }

        public static bool isDesignManagerSelectedBlocks(List<Vector3> designManagerSelectedBlocks)
        {
            return designManagerSelectedBlocks != null
                && designManagerSelectedBlocks.Count() > 1
                && designManagerSelectedBlocks[0] != null
                && designManagerSelectedBlocks[1] == Vector3.zero;
        }

        public static bool isCoordinateWithinWorld(Coordinate coordinate, Vector3 worldSize)
        {
            return coordinate.chunk.x >= 0 && coordinate.chunk.x <= worldSize.x
                && coordinate.chunk.y >= 0 && coordinate.chunk.y <= worldSize.y
                && coordinate.chunk.z >= 0 && coordinate.chunk.z <= worldSize.z;
        }

        private static Vector3 vectorFromBelow = Vector3.zero;
        public static Coordinate getCoordinateFromBelow(Coordinate coordinate)
        {
            vectorFromBelow.Set(coordinate.world.x, coordinate.world.y - BLOCK_TO_WORLD_SIZE, coordinate.world.z);

            return Coordinate.FromWorld(vectorFromBelow);
        }

        private const int MAX_BLOCK_INDEX = 255;
        private static List<String> usedBlockNames = new List<String>();
        private static List<BlockProperties> availableBlockTypes;
        public static List<BlockProperties> getBlockTypes()
        {
            if (availableBlockTypes != null) return availableBlockTypes;

            BlockProperties tempProperties;
            String tempName;

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

        public static List<BlockProperties> getUnbuildableBlockTypes()
        {
            return getBlockTypes().Where(t => !t.isBuildable()
                && !t.getName().Contains("Sand") && !t.getName().Contains("Crop")).ToList();
        }
    }
}
