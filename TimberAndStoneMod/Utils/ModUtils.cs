using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Timber_and_Stone;
using Timber_and_Stone.API;
using Timber_and_Stone.BlockData;
using Timber_and_Stone.Blocks;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Utils
{
    public static class ModUtils
    {
        public static IEnumerable<Coordinate> createCoordinatesFromSelection(List<Vector3> designManagerSelectedBlocks)
        {
            if (isDesignManagerSelectedBlocks(designManagerSelectedBlocks))
            {
                Vector3 startPosition = getSlightlyOffsetY(designManagerSelectedBlocks[0]);

                foreach (Vector3 position in designManagerSelectedBlocks.GetRange(1, designManagerSelectedBlocks.Count() - 1))
                {
                    yield return Coordinate.FromWorld(startPosition + position * ChunkManager.getInstance().voxelSize);
                }
            }           
        }

        public static bool isDesignManagerSelectedBlocks(List<Vector3> designManagerSelectedBlocks)
        {
            return designManagerSelectedBlocks != null
                && designManagerSelectedBlocks.Count() > 1
                && designManagerSelectedBlocks[0] != null
                && designManagerSelectedBlocks[1] == Vector3.zero;
        }

        public static Vector3 getSlightlyOffsetY(Vector3 position, int direction = -1)
        {
            direction = direction > 0 ? 1 : -1;
            return new Vector3(position.x, position.y + (ChunkManager.getInstance().voxelSize / 2) * direction, position.z);
        }
                
        private static List<String> usedBlockNames = new List<String>();
        private static List<BlockProperties> availableBlockTypes = new List<BlockProperties>();
        public static List<BlockProperties> getBlockTypes()
        {
            return availableBlockTypes.Count() <= 0
                ? availableBlockTypes = BlockProperties.blocks.Where(b => checkBlockName(b)).ToList() 
                : availableBlockTypes;
        }

        private static bool checkBlockName(BlockProperties blockProperties)
        {
            if (isBuildable(blockProperties)
                && !availableBlockTypes.Contains(blockProperties)
                && !usedBlockNames.Contains(blockProperties.getName()))
            {
                usedBlockNames.Add(blockProperties.getName());
                return true;
            }
            else return false;
        }

        private static Regex excludeNames = new Regex("(Technical|Block \\(|Scaffolding Base|Flowing )+");
        private static readonly List<int> alternateBlockIds = new List<int>() { 63, 64 };
        public static bool isBuildable(BlockProperties blockProperties, bool includeAlternates = false)
        {
            if (!excludeNames.IsMatch(blockProperties.getName()) 
                || (includeAlternates && alternateBlockIds.Contains(blockProperties.getID())))
            {
                return true;
            }
            
            return false;
        }

        private static Regex excludeUnbuildable = new Regex("(Sand|Crop)+");
        public static List<BlockProperties> getUnbuildableBlockTypes()
        {
            return getBlockTypes().Where(b => !b.isBuildable() && !excludeUnbuildable.IsMatch(b.getName())).ToList();
        }

        private static IBlockData[][] tempVariations;
        public static int getVariationIndex(IBlock block)
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

        public static Transform createTransform(UnityEngine.Object unityObject)
        {
            return UnityEngine.Object.Instantiate(unityObject, Vector3.zero, Quaternion.identity) as Transform;
        }

        public static Transform createTransform(UnityEngine.Object unityObject, Vector3 position, Quaternion rotation)
        {
            return UnityEngine.Object.Instantiate(unityObject, position, rotation) as Transform;
        }

        public static void generateResourceConstantsAndWriteToFile()
        {
            String result = "";
            Resource resource;

            for (int i = 1; i < 255; i++)
            {
                try
                {
                    if ((resource = Resource.FromID(i)) != null)
                    {
                        result += String.Format("public const int {0} = {1};{2}",
                            resource.name.Replace(' ', '_').Replace("'", String.Empty).ToUpper(), resource.index, Environment.NewLine);
                    }
                }
                catch { }
            }

            System.IO.File.WriteAllText("resource_constants.txt", result);
        }
    }
}
