using System;
using System.Collections.Generic;
using System.Linq;
using Timber_and_Stone;
using Timber_and_Stone.API;
using Timber_and_Stone.Invasion;
using Timber_and_Stone.Tasks;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Services
{
    public sealed class UnitService
    {
        private static readonly UnitService instance = new UnitService();
        public static UnitService getInstance() { return instance; }

        public static bool isFriendly(ALivingEntity entity)
        {
            return instance.getUnitAlignment(entity) == Alignment.Ally || instance.getUnitAlignment(entity) == Alignment.Neutral;
        }

        private const int MAX_SPAWN_ATTEMPTS = 10;
        private const int MAX_ANIMAL_COUNT = 250;

        private UnitManager unitManager = UnitManager.getInstance();
        private WorldManager worldManager = WorldManager.getInstance();
        private DesignManager designManager = DesignManager.getInstance();
        private ResourceManager resourceManager = ResourceManager.getInstance();
        private AssetManager assetManager = AssetManager.getInstance();

        private EquipmentService equipmentService = EquipmentService.getInstance();
        private BuildingService buildingSerive = BuildingService.getInstance();

        private UnitService() { }

        public List<ALivingEntity> getUnitsByAlignment(Alignment alignment)
        {
            return unitManager.allUnits.Where(unit => getUnitAlignment(unit) == alignment).ToList();            
        }

        public List<ALivingEntity> getFriendlyUnits()
        {
            return worldManager.PlayerFaction.units.ToList();
        }

        public List<ALivingEntity> getEnemyUnits()
        {
            return getUnitsByAlignment(Alignment.Enemy);
        }

        public List<ALivingEntity> getDeadUnits()
        {
            return unitManager.allUnits.Where(unit => !unit.isAlive()).ToList();
        }

        public Alignment getUnitAlignment(ALivingEntity entity)
        {
            return worldManager.PlayerFaction.getAlignmentToward(entity.faction);
        }

        public List<APlayableEntity> getPlayableUnits()
        {
            return worldManager.PlayerFaction.units.OfType<APlayableEntity>().ToList();
        }
        
        public bool spawnMerchant()
        {
            if (designManager.edgeRoads.Count < 1) return false;

            int attempts = 0;
            while (attempts < MAX_SPAWN_ATTEMPTS && unitManager.SpawnMerchant(Vector3.zero) != null) attempts++;

            return attempts < MAX_SPAWN_ATTEMPTS;
        }

        private const String ANIMAL_RANDOM = "random";
        public void spawnAnimal()
        {
            if (unitManager.wildFaunaUnits.Count < MAX_ANIMAL_COUNT)
            {
                unitManager.AddAnimal(ANIMAL_RANDOM, Vector3.zero, false);
            }
        }

        public APlayableEntity spawnMigrant()
        {
            return designManager.edgeRoads.Count > 0 ? addHuman(UnitProfession.Random, getRandomRoadPosition()) : null;
        }

        public IInvasion spawnInvasion()
        {
            return worldManager.SpawnInvasion();
        }

        public APlayableEntity addHuman(UnitProfession profession, Vector3 position, bool autoAccept = false)
        {            
            APlayableEntity entity = unitManager.AddHumanUnit(profession.Name.ToLower(), position, 
                true, !autoAccept, UnityEngine.Random.Range(0, 3) == 1).GetComponent<APlayableEntity>();

            entity.inventory.Clear();
            equipmentService.equipPlayerUnit(entity);

            return entity;
        }

        public ALivingEntity addFriendlyNPC(UnitFriendly profession, Vector3 position)
        {
            if (profession == null || position == null) return null;

            ALivingEntity entity = null;
            bool isArcher = false;

            switch (profession.Name)
            {
                case UnitFriendly.HUMAN_INFANTRY:
                    entity = createHumanUnit();                  
                    entity.unitName = UnitFriendly.HUMAN_INFANTRY;
                    break;

                case UnitFriendly.HUMAN_ARCHER:
                    entity = createHumanUnit(isArcher: true);
                    entity.unitName = UnitFriendly.HUMAN_ARCHER;
                    isArcher = true;
                    break;

                case UnitFriendly.GOBLIN_INFANTRY:
                    entity = createGoblinUnit();
                    equipmentService.equipGoblinWeapons(entity);
                    break;

                case UnitFriendly.GOBLIN_ARCHER:
                    entity = createGoblinUnit();
                    isArcher = true;
                    break;

                case UnitFriendly.SKELETON_INFANTRY:
                    entity = createSkeletonUnit();
                    equipmentService.equipSkeletonWeapons(entity);
                    break;

                case UnitFriendly.SKELETON_ARCHER:
                    entity = createSkeletonUnit();
                    isArcher = true;
                    break;
            }

            UnitPreference.set(entity, UnitPreference.NPC_FRIENDLY, true);
            UnitPreference.set(entity, UnitPreference.NPC_ARCHER, isArcher);

            equipmentService.equipNPCWeapons(entity, isArcher);

            entity.faction = worldManager.MigrantFaction;
            entity.hitpoints = entity.maxHP;
            entity.coordinate = getCoordinateAboveIfAir(Coordinate.FromWorld(position));
            entity.interruptTask(new TaskWait(10));
            entity.spottedTimer = 10f;

            return entity;
        }

        IBlock tempBlock;
        private Coordinate getCoordinateAboveIfAir(Coordinate coordinate)
        {
            if ((tempBlock = buildingSerive.getBlock(coordinate)).properties.getID() != 0)
            {
                return tempBlock.relative(0, 1, 0).coordinate;
            }

            return coordinate;
        }

        public Vector3 getRandomRoadPosition()
        {
            return designManager.edgeRoads.Count > 0 
                ? designManager.edgeRoads[UnityEngine.Random.Range(0, designManager.edgeRoads.Count)].world
                : Vector3.zero;
        }

        public ALivingEntity addEnemy(UnitEnemy type, Vector3 position)
        {
            if(type == null || position == null) return null;

            ALivingEntity entity = null;

            switch (type.Name)
            {
                case UnitEnemy.HUMAN_INFANTRY:
                    entity = createHumanUnit();
                    equipmentService.equipHumanWeapons(entity);
                    entity.name = UnitEnemy.HUMAN_INFANTRY;
                    break;

                case UnitEnemy.HUMAN_ARCHER:
                    entity = createHumanUnit(isArcher: true);
                    equipmentService.equipHumanWeapons(entity, true);
                    entity.name = UnitEnemy.HUMAN_ARCHER;
                    break;

                case UnitEnemy.GOBLIN_INFANTRY:
                    entity = createGoblinUnit();
                    equipmentService.equipGoblinWeapons(entity);
                    break;

                case UnitEnemy.GOBLIN_ARCHER:
                    entity = createGoblinUnit();
                    equipmentService.equipGoblinWeapons(entity, true);
                    break;

                case UnitEnemy.SKELETON_INFANTRY:
                    entity = createSkeletonUnit();
                    equipmentService.equipSkeletonWeapons(entity);
                    break;

                case UnitEnemy.SKELETON_ARCHER:
                    entity = createSkeletonUnit();
                    equipmentService.equipSkeletonWeapons(entity, true);
                    break;

                case UnitEnemy.NECROMANCER:
                    entity = createNecromancerUnit();
                    break;
            }

            entity.coordinate = getCoordinateAboveIfAir(Coordinate.FromWorld(position));

            return entity;
        }

        private ALivingEntity createHumanUnit(bool isArcher = false)
        {
            HumanEntity unit = assetManager.InstantiateUnit<HumanEntity>();
            
            if (isArcher)
            {
                unit.addProfession(new Timber_and_Stone.Profession.Human.Archer(unit, getRandomeExperience()));
            }
            else unit.addProfession(new Timber_and_Stone.Profession.Human.Infantry(unit, getRandomeExperience()));

            unit.unitName = "Human";
            unit.faction = worldManager.NeutralHostileFaction;

            return unit;
        }

        private ALivingEntity createGoblinUnit()
        {
            GoblinEntity unit = assetManager.InstantiateUnit<GoblinEntity>();
            unit.addProfession(new Timber_and_Stone.Profession.Goblin.Infantry(unit, getRandomeExperience()));
            
            unit.faction = worldManager.GoblinFaction;           

            return unit;
        }

        private ALivingEntity createSkeletonUnit()
        {
            SkeletonEntity unit = assetManager.InstantiateUnit<SkeletonEntity>();
            unit.addProfession(new Timber_and_Stone.Profession.Undead.Infantry(unit, getRandomeExperience()));

            unit.faction = worldManager.UndeadFaction;

            return unit;
        }

        private ALivingEntity createNecromancerUnit()
        {
            NecromancerEntity unit = assetManager.InstantiateUnit<NecromancerEntity>();
            unit.addProfession(new Timber_and_Stone.Profession.Undead.Infantry(unit, getRandomeExperience()));
           
            unit.faction = worldManager.UndeadFaction;

            return unit;
        }

        private int getRandomeExperience()
        {
            return UnityEngine.Random.Range(300, 1500);
        }
    }
}
