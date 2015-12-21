﻿using Plugin.BlowyAsteroid.TimberAndStoneMod.Collections;
using Plugin.BlowyAsteroid.TimberAndStoneMod.Utils;
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
            if (!(entity is HumanEntity)) return false;

            return instance.getUnitAlignment(entity) == Alignment.Ally 
                || UnitPreference.getPreference(entity, UnitPreference.IS_PLAYER_UNIT);
        }

        public static void reviveUnit(ALivingEntity entity, IFaction faction)
        {
            entity.hitpoints = entity.maxHP;
            entity.hunger = 0f;
            entity.faction = faction;
            entity.interruptTask(new TaskWander(entity));
        }

        public void reviveUnit(ALivingEntity entity)
        {
            if (isFriendly(entity))
            {
                UnitService.reviveUnit(entity, worldManager.PlayerFaction);
            }
            else if (entity is GoblinEntity)
            {
                UnitService.reviveUnit(entity, worldManager.GoblinFaction);
            }
            else if (entity is SkeletonEntity)
            {
                UnitService.reviveUnit(entity, worldManager.UndeadFaction);
            }
            else if (entity is HumanEntity)
            {
                UnitService.reviveUnit(entity, worldManager.NeutralHostileFaction);
            }
            else if (entity is AAnimalEntity)
            {
                UnitService.reviveUnit(entity, worldManager.GaiaFaction);
            }
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
            while (attempts < MAX_SPAWN_ATTEMPTS && unitManager.SpawnMerchant(Vector3.zero) == null) attempts++;

            return attempts < MAX_SPAWN_ATTEMPTS;
        }

        public void spawnAnimal()
        {
            if (unitManager.wildFaunaUnits.Count < MAX_ANIMAL_COUNT)
            {
                addAnimal(UnitAnimal.Random, Vector3.zero);
            }
        }

        public APlayableEntity spawnMigrant()
        {
            return designManager.edgeRoads.Count > 0 ? addHuman(UnitHuman.Random, getRandomRoadPosition()) : null;
        }

        public IInvasion spawnInvasion()
        {
            return worldManager.SpawnInvasion();
        }

        public APlayableEntity addHuman(UnitHuman profession, Vector3 position, bool autoAccept = false)
        {
            APlayableEntity entity = unitManager.AddHumanUnit(profession.Name.ToLower(), position, 
                true, !autoAccept, UnityEngine.Random.Range(0, 3) == 1).GetComponent<APlayableEntity>();

            entity.inventory.Clear();
            equipmentService.equipPlayerUnit(entity);

            UnitPreference.setPreference(entity, UnitPreference.WAIT_IN_HALL_WHILE_IDLE, true);
            UnitPreference.setPreference(entity, UnitPreference.TRAIN_UNDER_LEVEL_3, true);

            return entity;
        }

        public AAnimalEntity addAnimal(UnitAnimal type, Vector3 position)
        {
            return unitManager.AddAnimal(type.Name.ToLower(), ModUtils.getSlightlyOffsetY(position, 1), false);
        }        

        private IBlock tempBlock;
        private Coordinate getCoordinateAboveIfNotAir(Coordinate coordinate)
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
            bool isArcher = type.Name.Contains("Archer");

            switch (type.Name)
            {
                case UnitEnemy.HUMAN_INFANTRY:
                case UnitEnemy.HUMAN_ARCHER:
                    entity = createHumanUnit(isArcher);
                    break;

                case UnitEnemy.GOBLIN_INFANTRY:
                case UnitEnemy.GOBLIN_ARCHER:
                    entity = createGoblinUnit(isArcher);
                    break;

                case UnitEnemy.SKELETON_INFANTRY:
                case UnitEnemy.SKELETON_ARCHER:
                    entity = createSkeletonUnit(isArcher);
                    break;

                case UnitEnemy.NECROMANCER:
                    entity = createNecromancerUnit();
                    break;
            }

            entity.coordinate = getCoordinateAboveIfNotAir(Coordinate.FromWorld(position));

            return entity;
        }

        private ALivingEntity createHumanUnit(bool isArcher = false) 
        {
            HumanEntity unit = createALivingEntity<HumanEntity>(worldManager.NeutralHostileFaction);
            unit.addProfession(getMilitaryProfession(unit, isArcher));
            unit.unitName = isArcher ? UnitEnemy.HUMAN_ARCHER : UnitEnemy.HUMAN_INFANTRY;

            equipmentService.equipHumanWeapons(unit, isArcher);

            return unit;
        }

        private ALivingEntity createGoblinUnit(bool isArcher = false)
        {
            GoblinEntity unit = createALivingEntity<GoblinEntity>(worldManager.GoblinFaction);
            unit.addProfession(new Timber_and_Stone.Profession.Goblin.Infantry(unit, getRandomExperience()));

            equipmentService.equipGoblinWeapons(unit, isArcher);

            return unit;
        }

        private ALivingEntity createSkeletonUnit(bool isArcher = false)
        {
            SkeletonEntity unit = createALivingEntity<SkeletonEntity>(worldManager.UndeadFaction);
            unit.addProfession(new Timber_and_Stone.Profession.Undead.Infantry(unit, getRandomExperience()));

            equipmentService.equipSkeletonWeapons(unit, isArcher);

            return unit;
        }

        private ALivingEntity createNecromancerUnit(bool isArcher = false)
        {
            NecromancerEntity unit = createALivingEntity<NecromancerEntity>(worldManager.UndeadFaction); 
            unit.addProfession(new Timber_and_Stone.Profession.Undead.Infantry(unit, getRandomExperience()));  

            return unit;
        }

        private T createALivingEntity<T>(IFaction faction) where T : ALivingEntity
        {
            T unit = assetManager.InstantiateUnit<T>();
            unit.faction = faction;

            return unit;
        }

        private AProfession getMilitaryProfession(HumanEntity unit, bool isArcher)
        {
            if (isArcher)
            {
                return new Timber_and_Stone.Profession.Human.Archer(unit, getRandomExperience());
            }
            else return new Timber_and_Stone.Profession.Human.Infantry(unit, getRandomExperience());
        }

        private int getRandomExperience()
        {
            return UnityEngine.Random.Range(300, 1500);
        }
    }
}
