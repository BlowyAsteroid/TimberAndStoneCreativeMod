using Plugin.BlowyAsteroid.TimberAndStoneMod.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private bool isLowerResourceMassEnabled = false;
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
        private bool doLowerResourceMass = false;
        private bool doUnlimitedStorage = false;
        private bool doAlwaysDaytime = false;
        private bool doRealTime = false;
        private bool doCheats = false;

        public void Start()
        {
            setUpdatesPerSecond(5);

            modSettings.isCheatsEnabled = isCheatsEnabled;
            modSettings.isPeacefulEnabled = isPeacefulEnabled;
            modSettings.isAlwaysDayTimeEnabled = isAlwaysDayTimeEnabled;
            modSettings.isRealTimeEnabled = isRealTimeEnabled;
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

                if (doLowerResourceMass)
                {
                    doLowerResourceMass = false;
                    //Lower Resource Mass
                    if (isLowerResourceMassEnabled)
                    {
                        resourceService.getResources().ForEach(r => resourceService.lowerResourceMass(r));
                    }
                    else resourceService.getResources().ForEach(r => resourceService.restoreResourceMass(r));
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
                else if (doUnlimitedStorage)
                {
                    doUnlimitedStorage = false;
                    //Restore Storage Caps
                    resourceService.restoreStorageCaps();
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
                    unitService.getPlayableUnits().ForEach(u => unitService.setBestTraits(u));
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
                    unitService.getPlayableUnits().ForEach(u => unitService.setAllProfessionsMax(u));
                }
            }
        }

        private void fixUnitStatus(ALivingEntity entity)
        {
            entity.hunger = isHungerEnabled ? entity.hunger : 0f;
            entity.fatigue = isPeacefulEnabled ? entity.fatigue : 1f;
            entity.morale = isPeacefulEnabled ? entity.morale : 1f;
            entity.hitpoints = isPeacefulEnabled ? entity.hitpoints : entity.maxHP;
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
        
        private int buttonIndex;
        private void drawCheatWindow(int id)
        {
            Window(parentContainer, PARENT_CONTAINER_TITLE);

            buttonIndex = 0;

            CheckBox(BUTTON_WIDTH - 94f, START_Y + BUTTON_HEIGHT * buttonIndex++, "Cheats", ref isCheatsEnabled, ref doCheats);

            if (isMouseHover && isCheatsEnabled)
            {                
                CheckBox(START_X, START_Y + BUTTON_HEIGHT * buttonIndex++, "Infinite Materials", ref isInfiniteMaterialsEnabled);
                CheckBox(START_X, START_Y + BUTTON_HEIGHT * buttonIndex++, "Unlimited Storage", ref isUnlimitedStorageEnabled, ref doUnlimitedStorage);
                CheckBox(START_X, START_Y + BUTTON_HEIGHT * buttonIndex++, "Reduce Resource Mass", ref isLowerResourceMassEnabled, ref doLowerResourceMass);

                CheckBox(START_X, START_Y + BUTTON_HEIGHT * buttonIndex++, "Always Daytime", ref isAlwaysDayTimeEnabled, ref doAlwaysDaytime);
                CheckBox(START_X, START_Y + BUTTON_HEIGHT * buttonIndex++, "Real Time", ref isRealTimeEnabled, ref doRealTime);                

                CheckBox(START_X, START_Y + BUTTON_HEIGHT * buttonIndex++, "Peaceful", ref isPeacefulEnabled);
                CheckBox(START_X, START_Y + BUTTON_HEIGHT * buttonIndex++, "Hunger", ref isHungerEnabled);

                Button(START_X, START_Y + BUTTON_HEIGHT * buttonIndex++, "Spawn Migrant", ref doSpawnMigrant);
                Button(START_X, START_Y + BUTTON_HEIGHT * buttonIndex++, "Spawn Merchant", ref doSpawnMerchant);
                Button(START_X, START_Y + BUTTON_HEIGHT * buttonIndex++, "Spawn Animal", ref doSpawnAnimal);

                if (!isPeacefulEnabled)
                {
                    Button(START_X, START_Y + BUTTON_HEIGHT * buttonIndex++, "Spawn Invasion", ref doSpawnInvasion);
                    Button(START_X, START_Y + BUTTON_HEIGHT * buttonIndex++, "Kill All Enemies", ref doKillEnemies);
                }

                Button(START_X, START_Y + BUTTON_HEIGHT * buttonIndex++, "Fix Overeater Trait", ref doRemoveOvereaterTrait);
                Button(START_X, START_Y + BUTTON_HEIGHT * buttonIndex++, "Best Traits", ref doBestTraits);
                Button(START_X, START_Y + BUTTON_HEIGHT * buttonIndex++, "Max Current Professions", ref doMaxCurrentProfessions);
                Button(START_X, START_Y + BUTTON_HEIGHT * buttonIndex++, "Max All Professions", ref doMaxAllProfessions);
            }

            parentContainer.height = WINDOW_TITLE_HEIGHT + BUTTON_HEIGHT * buttonIndex + BUTTON_PADDING * 2;

            GUI.DragWindow();
        }        
    }
}
