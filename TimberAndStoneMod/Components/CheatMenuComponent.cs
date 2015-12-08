using Plugin.BlowyAsteroid.TimberAndStoneMod.Services;
using System;
using Timber_and_Stone;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Components
{
    public class CheatMenuComponent : ModComponent
    {
        private const String PARENT_CONTAINER_TITLE = "";
        private const int PARENT_CONTAINER_ID = 101;
        private const float CONTAINER_WIDTH = BUTTON_WIDTH + BUTTON_PADDING * 2;
        private const float CONTAINER_HEIGHT = WINDOW_TITLE_HEIGHT;

        private const float START_X = BUTTON_PADDING;
        private const float START_Y = WINDOW_TITLE_HEIGHT + BUTTON_PADDING;

        private const float FILL_STORAGE_PERCENTAGE = .70f;
        private const float MINIMUM_RESOURCE_MASS = 0.01f;

        private Rect parentContainer = new Rect(
            0f, MAIN_MENU_HEADER_HEIGHT - WINDOW_TITLE_HEIGHT, CONTAINER_WIDTH, CONTAINER_HEIGHT
        );

        private bool isCheatsEnabled = false;
        private bool isUnlimitedStorageEnabled = false;
        private bool isInfiniteMaterialsEnabled = false;
        private bool isAlwaysDayTimeEnabled = false;
        private bool isRealTimeEnabled = false;
        private bool isPeacefulEnabled = false;
        private bool isHungerEnabled = true;

        private bool doSpawnInvasion = false;
        private bool doKillEnemies = false;
        private bool doBestTraits = false;
        private bool doMaxCurrentProfessions = false;
        private bool doMaxAllProfessions = false;
        private bool doRemoveOvereaterTrait = false;
        private bool doSpawnMigrant = false;
        private bool doSpawnMerchant = false;
        private bool doSpawnAnimal = false;
        private bool doUnlimitedStorage = false;
        private bool doAlwaysDaytime = false;
        private bool doRealTime = false;
        private bool doCheats = false;
        private bool doInfiniteMaterials = false;

        public void Start()
        {
            setUpdatesPerSecond(5);
        }

        private ResourceService resourceService = ResourceService.getInstance();
        private UnitService unitService = UnitService.getInstance();
        
        public void Update()
        {
            if (!isTimeToUpdate(DateTime.Now.Ticks)) return;

            if (isComponentVisible)
            {
                translateMouse();
            }

            if (doCheats)
            {
                doCheats = false;

                modSettings.isCheatsEnabled = isCheatsEnabled;

                if (!isCheatsEnabled)
                {
                    //Disable all cheats
                    isCheatsEnabled = false;
                    isUnlimitedStorageEnabled = false;
                    isInfiniteMaterialsEnabled = false;
                    isAlwaysDayTimeEnabled = modSettings.isAlwaysDayTimeEnabled = false;
                    isRealTimeEnabled = modSettings.isRealTimeEnabled = false;
                    isPeacefulEnabled = modSettings.isPeacefulEnabled = false;
                    isHungerEnabled = true;

                    if (isUnlimitedStorageEnabled)
                    {
                        isUnlimitedStorageEnabled = false;
                        doUnlimitedStorage = true;
                    }

                    if (isInfiniteMaterialsEnabled)
                    {
                        isInfiniteMaterialsEnabled = false;
                        doInfiniteMaterials = true;
                    }
                }
            }

            if (doAlwaysDaytime)
            {
                doAlwaysDaytime = false;

                modSettings.isAlwaysDayTimeEnabled = isAlwaysDayTimeEnabled;
                modSettings.isRealTimeEnabled = isRealTimeEnabled = false;
            }
            else if (doRealTime)
            {
                doRealTime = false;

                modSettings.isRealTimeEnabled = isRealTimeEnabled;
                modSettings.isAlwaysDayTimeEnabled = isAlwaysDayTimeEnabled = false;
            }

            if (doUnlimitedStorage)
            {
                doUnlimitedStorage = false;
                isInfiniteMaterialsEnabled = false;
                //Restore Storage Caps
                resourceService.restoreStorageCaps();
            }
            
            if (doInfiniteMaterials)
            {
                doInfiniteMaterials = false;
                isUnlimitedStorageEnabled = false;
                //doUnlimitedStorage = true;
                if (isInfiniteMaterialsEnabled)
                {//Lower Resource Mass                    
                    resourceService.getResources().ForEach(r => resourceService.lowerResourceMass(r));
                }//Restore Resource Mass
                else resourceService.getResources().ForEach(r => resourceService.restoreResourceMass(r));

                //Refill Storage Containers
                resourceService.getMaterials().ForEach(r => resourceService.fillResourceStorage(r, FILL_STORAGE_PERCENTAGE));
            }

            if (isCheatsEnabled)
            {
                if (isPeacefulEnabled)
                {
                    //Destroy All Enemy Units
                    unitService.getEnemyUnits().ForEach(u => u.Destroy());
                }

                if (!isHungerEnabled || isPeacefulEnabled)
                {
                    //Fix Unit Status
                    unitService.getFriendlyUnits().ForEach(u => fixUnitStatus(u));
                }

                if (isInfiniteMaterialsEnabled)
                {
                    //Refill Storage Containers
                    resourceService.getMaterials().ForEach(r => resourceService.fillResourceStorage(r, FILL_STORAGE_PERCENTAGE));
                }
                
                if (isUnlimitedStorageEnabled)
                {
                    //Update Storage Caps
                    resourceService.makeStorageRoom();
                }

                if (doRemoveOvereaterTrait)
                {
                    doRemoveOvereaterTrait = false;
                    //Remove Overeater Trait From All Units
                    unitService.getPlayableUnits().ForEach(u => u.preferences[UnitTrait.Overeater.Name] = false);
                }
                else if (doSpawnMigrant)
                {
                    doSpawnMigrant = false;
                    //Spawn Migrant
                    if (!unitService.spawnMigrant())
                    {
                        log("Roads are needed before a migrant can be spawned.");
                    }
                }
                else if (doSpawnMerchant)
                {
                    doSpawnMerchant = false;
                    //Spawn Merchant
                    if (!unitService.spawnMerchant())
                    {
                        log("Roads are needed before a merchant can be spawned.");
                    }
                }
                else if (doSpawnAnimal)
                {
                    doSpawnAnimal = false;
                    //Spawn Animal
                    unitService.spawnAnimal();
                }
                else if (doSpawnInvasion)
                {
                    doSpawnInvasion = false;
                    //Spawn Invasion
                    unitService.spawnInvasion();
                }
                else if (doKillEnemies)
                {
                    doKillEnemies = false;
                    //Kill All Enemy Units
                    unitService.getEnemyUnits().ForEach(u => u.hitpoints = 0f);
                }
                else if (doBestTraits)
                {
                    doBestTraits = false;
                    //Set Best Unit Traits
                    unitService.getPlayableUnits().ForEach(u => UnitPreference.setBestTraits(u));
                }
                else if (doMaxCurrentProfessions)
                {
                    doMaxCurrentProfessions = false;
                    //Set Units Current Profession To Max
                    unitService.getPlayableUnits().ForEach(u => u.getProfession().setLevel(AProfession.maxLevel));
                }
                else if (doMaxAllProfessions)
                {
                    doMaxAllProfessions = false;
                    //Set All Unit Professions To Max
                    unitService.getPlayableUnits().ForEach(u => UnitProfession.setAllProfessionsMax(u));
                }
            }
        }

        private void fixUnitStatus(ALivingEntity entity)
        {
            if(!isHungerEnabled)
            {
                entity.hunger = 0f;
            }

            if (isPeacefulEnabled)
            {
                if (entity.fatigue <= .5f) entity.fatigue = 1f;
                if (entity.morale <= .5f) entity.morale = 1f;
                if (entity.hitpoints <= entity.maxHP * .5f) entity.hitpoints = entity.maxHP;
            }
        } 
        
        public void OnGUI()
        {
            if (isGameRunning)
            {
                if (isComponentVisible)
                {
                    parentContainer = createWindow(PARENT_CONTAINER_ID, parentContainer, drawCheatWindow);

                    isMouseHover = updateMouseForUI(parentContainer);                    
                }
            }
        }        

        private void drawCheatWindow(int id)
        {
            Window(parentContainer, PARENT_CONTAINER_TITLE);

            CheckBox(BUTTON_WIDTH - 94f, START_Y, "Cheats", ref isCheatsEnabled, ref doCheats);

            getNextWindowControlYPosition();

            if (isMouseHover && isCheatsEnabled)
            {
                if (!isUnlimitedStorageEnabled)
                {
                    CheckBox("Infinite Materials", ref isInfiniteMaterialsEnabled, ref doInfiniteMaterials);
                }

                if (!isInfiniteMaterialsEnabled)
                {
                    CheckBox("Unlimited Storage", ref isUnlimitedStorageEnabled, ref doUnlimitedStorage);
                }

                if (!isRealTimeEnabled)
                {
                    CheckBox("Always Daytime", ref isAlwaysDayTimeEnabled, ref doAlwaysDaytime);
                }

                if (!isAlwaysDayTimeEnabled)
                {
                    CheckBox("Real Time", ref isRealTimeEnabled, ref doRealTime);
                }

                CheckBox("Peaceful", ref isPeacefulEnabled);
                CheckBox("Hunger", ref isHungerEnabled);

                Button("Spawn Migrant", ref doSpawnMigrant);
                Button("Spawn Merchant", ref doSpawnMerchant);
                Button("Spawn Animal", ref doSpawnAnimal);

                if (!isPeacefulEnabled)
                {
                    Button("Spawn Invasion", ref doSpawnInvasion);
                    Button("Kill All Enemies", ref doKillEnemies);
                }

                Button("Remove Overeater Trait", ref doRemoveOvereaterTrait);
                Button("Best Traits", ref doBestTraits);
                Button("Max Current Professions", ref doMaxCurrentProfessions);
                Button("Max All Professions", ref doMaxAllProfessions);
            }

            parentContainer.height = WINDOW_TITLE_HEIGHT + BUTTON_HEIGHT * currentControlIndex + BUTTON_PADDING * 2;
        }        
    }
}
