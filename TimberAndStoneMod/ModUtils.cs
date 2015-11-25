using Plugin.BlowyAsteroid.TimberAndStoneMod.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Timber_and_Stone;
using Timber_and_Stone.API;
using Timber_and_Stone.BlockData;
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

        private static Vector3 vectorFrom = Vector3.zero;
        public static Coordinate getCoordinateFromBelow(Coordinate coordinate)
        {
            vectorFrom.Set(coordinate.world.x, coordinate.world.y - BLOCK_TO_WORLD_SIZE, coordinate.world.z);

            return Coordinate.FromWorld(vectorFrom);
        }

        public static Coordinate getCoordinateFromAbove(Coordinate coordinate)
        {
            vectorFrom.Set(coordinate.world.x, coordinate.world.y + BLOCK_TO_WORLD_SIZE, coordinate.world.z);

            return Coordinate.FromWorld(vectorFrom);
        }
                
        private static List<String> usedBlockNames = new List<String>();
        private static List<BlockProperties> availableBlockTypes = new List<BlockProperties>();
        private static Regex excludeNames = new Regex("(Technical|Block \\(|Scaffolding Base)+");
        public static List<BlockProperties> getBlockTypes()
        {
            return availableBlockTypes.Count() <= 0
                ? availableBlockTypes = BlockProperties.blocks.Where(b => checkBlockName(b)).ToList() 
                : availableBlockTypes;
        }

        private static bool checkBlockName(BlockProperties blockProperties)
        {
            if (!availableBlockTypes.Contains(blockProperties) 
                && !usedBlockNames.Contains(blockProperties.getName())
                && !excludeNames.IsMatch(blockProperties.getName()))
            {
                usedBlockNames.Add(blockProperties.getName());
                return true;
            }
            else return false;
        }

        private static Regex excludeUnbuildable = new Regex("(Sand|Crop)+");
        public static List<BlockProperties> getUnbuildableBlockTypes()
        {
            return getBlockTypes().Where(b => !b.isBuildable() && !excludeUnbuildable.IsMatch(b.getName())).ToList();
        }

        private static IBlockData[][] tempVariations;
        public static int getVariationIndexFromBlock(IBlock block)
        {
            if (block == null || (tempVariations = block.properties.getVariations()) == null) return 0;

            for (int i = 0; i < tempVariations.Count(); i++)
            {
                if (((BlockDataVariant)tempVariations[i][0]).variant == block.getMeta<BlockDataVariant>().variant)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}
