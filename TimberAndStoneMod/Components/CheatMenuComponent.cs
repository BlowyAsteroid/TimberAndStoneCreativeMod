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

        private bool doCheats = false;
        private bool doInfiniteMaterials = false;
        private bool doUnlimitedStorage = false;
        private bool doPeaceful = false;
        private bool doSpawnInvasion = false;
        private bool doKillEnemies = false;
        private bool doBestTraits = false;
        private bool doMaxCurrentProfessions = false;
        private bool doSpawnMigrant = false;
        private bool doSpawnMerchant = false;
        private bool doSpawnAnimal = false;

        private bool isSettingsLoaded = false;

        public void Start()
        {
            setUpdatesPerSecond(1);
        }

        private ResourceService resourceService = ResourceService.getInstance();
        private UnitService unitService = UnitService.getInstance();
        
        public void Update()
        {
            if (isComponentVisible)
            {
                translateMouse();
            }

            if (!isTimeToUpdate(DateTime.Now.Ticks)) return;

            if (modSettings.isHasSettings && !isSettingsLoaded)
            {
                isSettingsLoaded = true;
                if (modSettings.isCheatsEnabled)
                {
                    doInfiniteMaterials = modSettings.isInfiniteMaterialsEnabled;
                    doUnlimitedStorage = modSettings.isUnlimitedStorageEnabled;
                    doPeaceful = modSettings.isPeacefulEnabled;
                }
                else doCheats = true;
            }

            if (doCheats)
            {
                doCheats = false;

                if (!modSettings.isCheatsEnabled)
                {
                    //Disable all cheats
                    modSettings.isAlwaysDaytimeEnabled = false;
                    modSettings.isPeacefulEnabled = false;
                    modSettings.isShowEnemiesEnabled = false;

                    if (modSettings.isUnlimitedStorageEnabled)
                    {
                        modSettings.isUnlimitedStorageEnabled = false;
                        doUnlimitedStorage = true;
                    }

                    if (modSettings.isInfiniteMaterialsEnabled)
                    {
                        modSettings.isInfiniteMaterialsEnabled = false;
                        doInfiniteMaterials = true;
                    }
                }
            }

            if (doPeaceful)
            {
                doPeaceful = false;

                if (modSettings.isPeacefulEnabled)
                {
                    //Destroy All Enemy Units  
                    foreach (ALivingEntity entity in unitService.getEnemyUnits())
                    {
                        if (!entity.isAlive() && UnitPreference.getPreference(entity, UnitPreference.IS_PLAYER_UNIT))
                        {
                            UnitService.reviveUnit(entity, worldManager.PlayerFaction);
                        }
                        else entity.Destroy();
                    }
                }
            }     

            if (doUnlimitedStorage)
            {
                doUnlimitedStorage = false;
                modSettings.isInfiniteMaterialsEnabled = false;
                //Restore Storage Caps
                resourceService.restoreStorageCaps();
            }
            
            if (doInfiniteMaterials)
            {
                doInfiniteMaterials = false;
                modSettings.isUnlimitedStorageEnabled = false;
                //doUnlimitedStorage = true;
                if (modSettings.isInfiniteMaterialsEnabled)
                {//Lower Resource Mass                    
                    resourceService.getAllResources().ForEach(r => resourceService.lowerResourceMass(r));
                }//Restore Resource Mass
                else resourceService.getAllResources().ForEach(r => resourceService.restoreResourceMass(r));

                //Refill Storage Containers
                resourceService.getAllResources().ForEach(r => resourceService.fillResourceStorage(r, FILL_STORAGE_PERCENTAGE));
            }

            if (modSettings.isCheatsEnabled)
            {
                if (modSettings.isPeacefulEnabled)
                {
                    //Fix Unit Status
                    unitService.getFriendlyUnits().ForEach(u => fixUnitStatus(u));
                }
                else if (modSettings.isShowEnemiesEnabled)
                {
                    //Show Enemy Units
                    unitService.getEnemyUnits().ForEach(u => u.spottedTimer = 3f);
                }

                if (modSettings.isInfiniteMaterialsEnabled)
                {
                    //Refill Storage Containers
                    resourceService.getAllResources().ForEach(r => resourceService.fillResourceStorage(r, FILL_STORAGE_PERCENTAGE));
                }

                if (modSettings.isUnlimitedStorageEnabled)
                {
                    //Update Storage Caps
                    resourceService.makeStorageRoom();
                }

                if (doSpawnMigrant)
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
                    unitService.getPlayableUnits().ForEach(u => UnitTrait.setBestTraits(u));
                }
                else if (doMaxCurrentProfessions)
                {
                    doMaxCurrentProfessions = false;
                    //Set Units Current Profession To Max
                    unitService.getPlayableUnits().ForEach(u => u.getProfession().setLevel(AProfession.maxLevel));
                }
            }
        }

        private void fixUnitStatus(ALivingEntity entity)
        {
            entity.hunger = 0f;
            if (entity.fatigue <= .5f) entity.fatigue = 1f;
            if (entity.morale <= .5f) entity.morale = 1f;
            if (entity.hitpoints <= entity.maxHP / 2f) entity.hitpoints = entity.maxHP;            
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

            CheckBox(BUTTON_WIDTH - 94f, START_Y, "Cheats", ref modSettings.isCheatsEnabled, ref doCheats);

            getNextWindowControlYPosition();

            if (isMouseHover && modSettings.isCheatsEnabled)
            {
                if (!modSettings.isUnlimitedStorageEnabled)
                {
                    CheckBox("Infinite Materials", ref modSettings.isInfiniteMaterialsEnabled, ref doInfiniteMaterials);
                }

                if (!modSettings.isInfiniteMaterialsEnabled)
                {
                    CheckBox("Unlimited Storage", ref modSettings.isUnlimitedStorageEnabled, ref doUnlimitedStorage);
                }

                CheckBox("Always Daytime", ref modSettings.isAlwaysDaytimeEnabled);
                CheckBox("Peaceful", ref modSettings.isPeacefulEnabled, ref doPeaceful);

                if (!modSettings.isPeacefulEnabled)
                {
                    CheckBox("Show Enemies", ref modSettings.isShowEnemiesEnabled); 
                }

                Button("Spawn Migrant", ref doSpawnMigrant);
                Button("Spawn Merchant", ref doSpawnMerchant);
                Button("Spawn Animal", ref doSpawnAnimal);

                if (!modSettings.isPeacefulEnabled)
                {
                    Button("Spawn Invasion", ref doSpawnInvasion);
                    Button("Kill All Enemies", ref doKillEnemies);
                }

                Button("Best Traits", ref doBestTraits);
                Button("Max Current Professions", ref doMaxCurrentProfessions);
            }

            parentContainer.height = WINDOW_TITLE_HEIGHT + BUTTON_HEIGHT * currentControlIndex + DOUBLE_PADDING;
        }        
    }
}
