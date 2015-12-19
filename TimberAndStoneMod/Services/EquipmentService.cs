using Plugin.BlowyAsteroid.Collections.TimberAndStoneMod;
using Timber_and_Stone;
using Timber_and_Stone.Profession.Human;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Services
{
    public sealed class EquipmentService
    {
        private static readonly EquipmentService instance = new EquipmentService();
        public static EquipmentService getInstance() { return instance; }

        private ResourceManager resourceManager = ResourceManager.getInstance();

        private EquipmentService() { }
        
        public void equipGoblinWeapons(ALivingEntity entity, bool isArcher = false)
        {
            entity.inventory.Add(ResourceId.FOOD, 5);
            entity.inventory.Add(ResourceId.getRandomGoblinMeleeWeapon(), 1);
            entity.inventory.Add(ResourceId.getRandomGoblinShield(), 1);

            if (isArcher)
            {
                entity.inventory.Add(ResourceId.getRandomGoblinRangedWeapon(), 1);
                entity.inventory.Add(ResourceId.BROADHEAD_ARROW, 25);
            }
        }

        public void equipSkeletonWeapons(ALivingEntity entity, bool isArcher = false)
        {
            entity.inventory.Add(ResourceId.FOOD, 5);
            entity.inventory.Add(ResourceId.getRandomSkeletonMeleeWeapon(), 1);
            entity.inventory.Add(ResourceId.SKELETAL_ROUND_SHIELD, 1);

            if (isArcher)
            {
                entity.inventory.Add(ResourceId.SKELETAL_BOW, 1);
                entity.inventory.Add(ResourceId.BROADHEAD_ARROW, 25);
            }
        }

        public void equipHumanWeapons(ALivingEntity entity, bool isArcher = false)
        {
            entity.inventory.Add(ResourceId.FOOD, 5);
            entity.inventory.Add(ResourceId.SHORTSWORD, 1);
            entity.inventory.Add(ResourceId.BUCKLER, 1);
            
            if (isArcher)
            {
                entity.inventory.Add(ResourceId.SHORTBOW, 1);
                entity.inventory.Add(ResourceId.BROADHEAD_ARROW, 25);
            }
        }

        public void equipNPCWeapons(ALivingEntity entity, bool isArcher = false)
        {
            if (entity is HumanEntity)
            {
                equipHumanWeapons(entity, isArcher);
            }
            else if (entity is GoblinEntity)
            {
                equipGoblinWeapons(entity, isArcher);
            }
            else if (entity is SkeletonEntity)
            {
                equipSkeletonWeapons(entity, isArcher);
            }
        }

        public void equipPlayerUnit(APlayableEntity entity)
        {
            entity.inventory.Add(ResourceId.FOOD, 5);

            AProfession profession = entity.getProfession();

            if (profession is Archer)
            {
                equipHumanWeapons(entity, true);
            }
            else if (profession is Infantry)
            {
                equipHumanWeapons(entity);
            }
            else if (profession is Blacksmith)
            {
                entity.inventory.Add(ResourceId.STRONG_HAMMER, 1);
                entity.inventory.Add(ResourceId.STRONG_TONGS, 1);
            }
            else if (profession is Builder || profession is Carpenter || profession is StoneMason || profession is Engineer)
            {
                entity.inventory.Add(ResourceId.STRONG_HAMMER, 1);
            }
            else if (profession is Farmer)
            {
                entity.inventory.Add(ResourceId.STRONG_HOE, 1);
                entity.inventory.Add(ResourceId.CARROT_SEED, 5);
                entity.inventory.Add(ResourceId.CORN_SEED, 5);
                entity.inventory.Add(ResourceId.COTTON_SEED, 5);
                entity.inventory.Add(ResourceId.FLAX_SEED, 5);
                entity.inventory.Add(ResourceId.POTATO_SEED, 5);
                entity.inventory.Add(ResourceId.PUMPKIN_SEED, 5);
                entity.inventory.Add(ResourceId.TURNIP_SEED, 5);
                entity.inventory.Add(ResourceId.WHEAT_SEED, 5);
            }
            else if (profession is Fisherman)
            {
                entity.inventory.Add(ResourceId.STRONG_FISHING_ROD, 1);
            }
            else if (profession is Forager)
            {
                entity.inventory.Add(ResourceId.SHARP_KNIFE, 1);
            }
            else if (profession is Herder)
            {
                entity.inventory.Add(ResourceId.HERDING_CROOK, 1);
            }
            else if (profession is Miner)
            {
                entity.inventory.Add(ResourceId.SHARP_PICK, 1);
            }
            else if (profession is Tailor)
            {
                entity.inventory.Add(ResourceId.SHARP_SHEARS, 1);
            }
            else if (profession is WoodChopper)
            {
                entity.inventory.Add(ResourceId.SHARP_AXE, 1);
            }
        }

    }
}
