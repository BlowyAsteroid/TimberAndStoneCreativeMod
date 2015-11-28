using System;
using System.Collections.Generic;
using System.Linq;
using Timber_and_Stone;
using Timber_and_Stone.API;
using Timber_and_Stone.Invasion;
using Timber_and_Stone.Utility;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Services
{
    public sealed class UnitService
    {
        private static readonly UnitService instance = new UnitService();
        public static UnitService getInstance() { return instance; }

        public static bool isFriendly(ALivingEntity entity)
        {
            return instance.getUnitAlignment(entity) == Alignment.Ally;
        }

        private const int MAX_SPAWN_ATTEMPTS = 5;
        private const int MAX_ANIMAL_COUNT = 250;

        private Dictionary<APlayableEntity, APlayableEntity.Preferences> originalTraits 
            = new Dictionary<APlayableEntity, APlayableEntity.Preferences>();
        
        private Dictionary<APlayableEntity, Dictionary<AProfession, int>> originalProfessions 
            = new Dictionary<APlayableEntity, Dictionary<AProfession, int>>();

        private UnitManager unitManager = UnitManager.getInstance();
        private WorldManager worldManager = WorldManager.getInstance();
        private DesignManager designManager = DesignManager.getInstance();

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

        public void spawnAnimal()
        {
            if (unitManager.wildFaunaUnits.Count < MAX_ANIMAL_COUNT)
            {
                unitManager.AddAnimal("random", Vector3.zero, false);
            }
        }

        public APlayableEntity spawnMigrant()
        {
            return designManager.edgeRoads.Count > 0 ? addUnit(UnitProfession.Random, getRandomRoadPosition()) : null;
        }

        public IInvasion spawnInvasion()
        {
            return worldManager.SpawnInvasion();
        }

        public APlayableEntity addUnit(UnitProfession profession, Vector3 position, bool autoAccept = false)
        {
            APlayableEntity entity = unitManager.AddHumanUnit(profession.Name.ToLower(), position, 
                true, !autoAccept, UnityEngine.Random.Range(0, 3) == 1).GetComponent<APlayableEntity>();

            unitManager.AddMigrantResources(entity);

            return entity;
        }

        public Vector3 getRandomRoadPosition()
        {
            return designManager.edgeRoads.Count > 0 
                ? designManager.edgeRoads[UnityEngine.Random.Range(0, designManager.edgeRoads.Count)].world
                : Vector3.zero;
        }


        APlayableEntity.Preferences originalPreferences;
        public void setBestTraits(APlayableEntity entity)
        {
            originalPreferences = null;

            if (!originalTraits.ContainsKey(entity))
            {
                originalPreferences = new APlayableEntity.Preferences();
            }

            foreach (UnitTrait trait in UnitTrait.List)
            {
                if (originalPreferences != null)
                {
                    originalPreferences[trait.Name] = entity.preferences[trait.Name];
                }

                entity.preferences[trait.Name] = trait.Type == UnitTraitType.GOOD;
            }

            if (originalPreferences != null)
            {
                originalTraits.Add(entity, originalPreferences);
            }
        }

        private APlayableEntity.Preferences existingPreferences;
        public void restoreTraits(APlayableEntity entity)
        {
            if (originalTraits.TryGetValue(entity, out existingPreferences))
            {
                foreach (UnitTrait trait in UnitTrait.List)
                {
                    entity.preferences[trait.Name] = existingPreferences[trait.Name];
                }

                originalTraits.Remove(entity);
            }
        }

        Dictionary<AProfession, int> professions;
        public void setAllProfessionsMax(APlayableEntity entity)
        {
            professions = null;

            if (!originalProfessions.ContainsKey(entity))
            {
                professions = new Dictionary<AProfession, int>();
            }

            foreach (KeyValuePair<Type, AProfession> key in entity.professions)
            {
                if (professions != null)
                {
                    professions.Add(key.Value, key.Value.getLevel());
                }

                key.Value.setLevel(AProfession.maxLevel);
            }

            if (professions != null)
            {
                originalProfessions.Add(entity, professions);
            }
        }

        private Dictionary<AProfession, int> existingProfessions;
        private int existingLevel;
        public void restoreProfessions(APlayableEntity entity)
        {
            if (originalProfessions.TryGetValue(entity, out existingProfessions))
            {
                foreach (KeyValuePair<Type, AProfession> key in entity.professions)
                {
                    if (existingProfessions.TryGetValue(key.Value, out existingLevel))
                    {
                        key.Value.setLevel(existingLevel);
                    }
                }

                originalProfessions.Remove(entity);
            }
        }



        private BuildingService buildingSerive = BuildingService.getInstance();
        IBlock tempEnemyBlock;
        public ALivingEntity addEnemy(EnemyUnit type, Vector3 position)
        {
            if(type == null || position == null) return null;

            ALivingEntity entity = null;

            switch (type.Name)
            {
                case EnemyUnit.GOBLIN_UNARMED:
                    entity = createGoblinUnit();
                    break;

                case EnemyUnit.GOBLIN_INFANTRY:
                    entity = createGoblinUnit();
                    equipGoblin(entity);
                    break;

                case EnemyUnit.GOBLIN_ARCHER:
                    entity = createGoblinUnit();
                    equipGoblin(entity, true);
                    break;

                case EnemyUnit.SKELETON_UNARMED:
                    entity = createSkeletonUnit();
                    break;

                case EnemyUnit.SKELETON_INFANTRY:
                    entity = createSkeletonUnit();
                    equipSkeleton(entity);
                    break;

                case EnemyUnit.SKELETON_ARCHER:
                    entity = createSkeletonUnit();
                    equipSkeleton(entity, true);
                    break;

                case EnemyUnit.NECROMANCER:
                    entity = createNecromancerUnit();
                    break;
            }

            Coordinate coordinate = Coordinate.FromWorld(position);
            if ((tempEnemyBlock = buildingSerive.getBlock(coordinate)).properties.getID() != 0)
            {
                coordinate = tempEnemyBlock.relative(0, 1, 0).coordinate;
            }

            entity.coordinate = coordinate;

            return entity;
        }

        private AssetManager assetManager = AssetManager.getInstance();
        private ALivingEntity createGoblinUnit()
        {
            GoblinEntity unit = assetManager.InstantiateUnit<GoblinEntity>();
            unit.addProfession((AProfession)new Timber_and_Stone.Profession.Goblin.Infantry(unit, 100));
            unit.faction = (IFaction)worldManager.GoblinFaction;
            return unit;
        }

        private ALivingEntity createSkeletonUnit()
        {
            SkeletonEntity unit = assetManager.InstantiateUnit<SkeletonEntity>();
            unit.addProfession((AProfession)new Timber_and_Stone.Profession.Undead.Infantry(unit, 100));
            unit.faction = (IFaction)worldManager.UndeadFaction;
            return unit;
        }

        private ALivingEntity createNecromancerUnit()
        {
            NecromancerEntity unit = assetManager.InstantiateUnit<NecromancerEntity>();
            unit.addProfession((AProfession)new Timber_and_Stone.Profession.Undead.Infantry(unit, 100));
            unit.faction = (IFaction)worldManager.UndeadFaction;
            return unit;
        }

        private ResourceManager resourceManager = ResourceManager.getInstance();
        private void equipGoblin(ALivingEntity entity, bool addBow = false)
        {
            entity.inventory.Add(UnityEngine.Random.Range(150, 152 + 1), 1);
            entity.inventory.Add(UnityEngine.Random.Range(170, 171 + 1), 1);

            if (addBow)
            {
                entity.inventory.Add(UnityEngine.Random.Range(153, 154 + 1), 1);
                entity.inventory.Add(Resource.FromID(Extension_Methods.RandomElement<int>(resourceManager.listIndexArrowBest)), UnityEngine.Random.Range(6, 23));        
            }
        }

        private void equipSkeleton(ALivingEntity entity, bool addBow = false)
        {
            entity.inventory.Add(UnityEngine.Random.Range(180, 181 + 1), 1);
            entity.inventory.Add(190, 1);

            if (addBow)
            {
                entity.inventory.Add(182, 1);
                entity.inventory.Add(Resource.FromID(Extension_Methods.RandomElement<int>(resourceManager.listIndexArrowBest)), UnityEngine.Random.Range(6, 23));
            }
        }
    }
}
