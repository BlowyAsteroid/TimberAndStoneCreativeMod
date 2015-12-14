
using System.Collections.Generic;
namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public static class ResourceId
    {
        public const int DIRT = 1;//First Raw Material
        public const int RAW_STONE = 2;
        public const int RAW_WOOD = 3;
        public const int FOOD = 4;
        public const int SAND = 5;
        public const int BONES = 6;
        public const int FEATHERS = 7;
        public const int SPIDER_SILK = 8;
        public const int COAL = 9;
        public const int ANIMAL_HIDE = 10;
        public const int ANIMAL_FUR = 11;
        public const int FAT = 12;
        public const int WHEAT = 13;
        public const int FLAX_FIBER = 14;
        public const int COTTON = 15;
        public const int WOOL = 16;
        public const int SEEDLING = 17;
        public const int SCRAP_METAL = 18;
        public const int TIN_ORE = 19;
        public const int COPPER_ORE = 20;
        public const int IRON_ORE = 21;
        public const int SILVER_ORE = 22;
        public const int GOLD_ORE = 23;
        public const int MITHRIL_ORE = 24;
        public const int CARROT_SEED = 30;
        public const int CORN_SEED = 31;
        public const int COTTON_SEED = 32;
        public const int FLAX_SEED = 33;
        public const int POTATO_SEED = 34;
        public const int PUMPKIN_SEED = 35;
        public const int TURNIP_SEED = 36;
        public const int WHEAT_SEED = 37;//Last Raw Material
        public const int TIMBER = 40;//First Processed Material
        public const int STRONG_TIMBER = 41;
        public const int TWINE = 42;
        public const int ROPE = 43;
        public const int PLASTER = 44;
        public const int BRICK = 45;
        public const int CLOTH = 46;
        public const int LEATHER = 47;
        public const int WHEEL = 48;
        public const int WOODEN_GEAR = 49;
        public const int METAL_GEAR = 50;
        public const int CHAIN = 51;
        public const int STANDARD_INGOT = 52;
        public const int SOLID_INGOT = 53;
        public const int STRONG_INGOT = 54;
        public const int COIN = 55;
        public const int BANDAGES = 56;//Last Processed Material
        public const int KNIFE = 60;//First Human Gear
        public const int SHARP_KNIFE = 61;
        public const int SHEARS = 62;
        public const int SHARP_SHEARS = 63;
        public const int STONE_TONGS = 64;
        public const int SOLID_TONGS = 65;
        public const int STRONG_TONGS = 66;
        public const int STONE_HAMMER = 67;
        public const int SOLID_HAMMER = 68;
        public const int STRONG_HAMMER = 69;
        public const int STONE_PICK = 70;
        public const int SOLID_PICK = 71;
        public const int SHARP_PICK = 72;
        public const int STONE_HOE = 73;
        public const int SOLID_HOE = 74;
        public const int STRONG_HOE = 75;
        public const int STONE_AXE = 76;
        public const int SOLID_AXE = 77;
        public const int SHARP_AXE = 78;
        public const int FISHING_ROD = 79;
        public const int STRONG_FISHING_ROD = 80;
        public const int STONE_HANDSAW = 81;
        public const int SHARP_HANDSAW = 82;
        public const int TORCH = 83;
        public const int HERDING_CROOK = 84;
        public const int SOLID_HANDSAW = 85;
        public const int SHORTSWORD = 90;
        public const int GLADIUS = 91;
        public const int SPATHA = 92;
        public const int BROADSWORD = 93;
        public const int SHORTBOW = 94;
        public const int SELF_BOW = 95;
        public const int COMPOUND_BOW = 96;
        public const int LONGBOW = 97;
        public const int CROSSBOW = 98;
        public const int STONE_ARROW = 99;
        public const int BODKIN_ARROW = 100;
        public const int BROADHEAD_ARROW = 101;
        public const int FIRE_ARROW = 102;
        public const int CROSSBOW_BOLTS = 103;
        public const int BALLISTA_BOLT = 104;
        public const int SPEAR = 105;
        public const int VOULGE = 106;
        public const int GLAIVE = 107;
        public const int HALBERD = 108;
        public const int CLUB = 109;
        public const int BALL_O_FIRE = 110;
        public const int TUNIC = 120;
        public const int GAMBESON = 121;
        public const int BRIGANDINE = 122;
        public const int HAUBERK = 123;
        public const int BREASTPLATE = 124;
        public const int CUIRASS = 125;
        public const int ARMING_CAP = 126;
        public const int CHAINMAIL_COIF = 127;
        public const int SPANGENHELM = 128;
        public const int BARBUTE_HELM = 129;
        public const int GREAT_HELM = 130;
        public const int ARMET = 131;
        public const int PATTENS = 132;
        public const int LEATHER_BOOTS = 133;
        public const int SABATONS = 134;
        public const int CUISSES = 135;
        public const int GREAVES = 136;
        public const int BUCKLER = 137;
        public const int KITE_SHIELD = 138;
        public const int HEATER_SHIELD = 139;
        public const int TOWER_SHIELD = 140;//Last Human Gear
        public const int GOBLIN_BONECLUB = 150;//First Enemy Gear
        public const int GOBLIN_SHORTSWORD = 151;
        public const int GOBLIN_CLEAVER = 152;
        public const int GOBLIN_SHORTBOW = 153;
        public const int GOBLIN_LONGBOW = 154;
        public const int GOBLIN_PLANK_SHIELD = 170;
        public const int GOBLIN_BUCKLER = 171;
        public const int SKELETAL_SHORTSWORD = 180;
        public const int SKELETAL_CLAYMORE = 181;
        public const int SKELETAL_BOW = 182;
        public const int SKELETAL_ROUND_SHIELD = 190;//Last Enemy Gear


        public static int getRandomGoblinMeleeWeapon()
        {
            return UnityEngine.Random.Range(GOBLIN_BONECLUB, GOBLIN_CLEAVER + 1);
        }

        public static int getRandomGoblinRangedWeapon()
        {
            return UnityEngine.Random.Range(GOBLIN_SHORTBOW, GOBLIN_LONGBOW + 1);
        }

        public static int getRandomGoblinShield()
        {
            return UnityEngine.Random.Range(GOBLIN_PLANK_SHIELD, GOBLIN_BUCKLER + 1);
        }

        public static int getRandomSkeletonMeleeWeapon()
        {
            return UnityEngine.Random.Range(SKELETAL_SHORTSWORD, SKELETAL_CLAYMORE + 1);
        }

        public static readonly List<int> RawMaterialList = getResourceIds(DIRT, WHEAT_SEED);
        public static readonly List<int> ProcessedMaterialList = getResourceIds(TIMBER, BANDAGES);
        public static readonly List<int> AllPlayerResourcesList = getResourceIds(DIRT, TOWER_SHIELD);
        public static readonly List<int> SeedsList = getResourceIds(CARROT_SEED, WHEAT_SEED);
        public static readonly List<int> HumanGearList = getResourceIds(KNIFE, TOWER_SHIELD);
        public static readonly List<int> EnemyGearList = getResourceIds(GOBLIN_BONECLUB, SKELETAL_ROUND_SHIELD);
        
        private static List<int> getResourceIds(int start, int end)
        {
            List<int> ids = new List<int>();

            for (int i = start; i < end + 1; i++)
            {
                ids.Add(i);
            }

            return ids;
        }

    }
}
