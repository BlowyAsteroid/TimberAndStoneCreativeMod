using Plugin.BlowyAsteroid.TimberAndStoneMod.Collections;
using Plugin.BlowyAsteroid.TimberAndStoneMod.Services;
using System;
using Timber_and_Stone;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Event;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Components
{
    public class CheatMenuComponent : GUIPluginComponent
    {
        private const float FILL_STORAGE_PERCENTAGE = 0.7f;

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

        private ModSettings modSettings = ModSettings.getInstance();
        private ResourceService resourceService = ResourceService.getInstance();
        private UnitService unitService = UnitService.getInstance();

        #region Events
        [Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
        public void onInvasionNormal(EventInvasion evt)
        {
            if (!modSettings.isPeacefulEnabled) return;

            evt.result = Result.Deny;
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void onInvasionMonitor(EventInvasion evt)
        {
            if (evt.result != Result.Deny) return;

            log(String.Format("A {0} invasion has been cancelled.", evt.invasion.getName()));
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
        public void onEntityDeathNormal(EventEntityDeath evt)
        {
            if (!modSettings.isPeacefulEnabled || !UnitPreference.getPreference(evt.getUnit(), UnitPreference.IS_PLAYER_UNIT)) return;

            evt.result = Result.Deny;
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void onEntityDeathMonitor(EventEntityDeath evt)
        {
            if (evt.result != Result.Deny) return;

            UnitService.reviveUnit(evt.getUnit(), worldManager.PlayerFaction);
        }
        #endregion

        public override void OnStart()
        {
            setWindowSize(260f + sectionMain.ControlMargin * 2, Screen.height / 2);
            setWindowPosition(0f, 0f);
            setUpdatesPerSecond(1);

            isVisibleInGame = true;
            isVisibleInMainMenu = false;
            isVisibleDuringGameOver = false;

            sectionMain.Direction = GUISection.FlowDirection.VERTICAL;
            sectionMain.Flow = GUISection.Overflow.HIDDEN;
        }

        public override void OnDraw(int windowId)
        {
            Window(this.title);

            CheckBox(this.containerWidth - 94f - sectionMain.ControlMargin, WINDOW_TITLE_HEIGHT + sectionMain.ControlMargin + sectionMain.ControlPadding,
                sectionMain.ControlWidth / 2, sectionMain.ControlHeight, "Cheats", ref modSettings.isCheatsEnabled, ref doCheats);

            sectionMain.Begin(0, WINDOW_TITLE_HEIGHT + sectionMain.ControlHeight + sectionMain.ControlPadding, this.ParentContainer.width, this.ParentContainer.height);

            if (isMouseHover && modSettings.isCheatsEnabled)
            {
                if (!modSettings.isUnlimitedStorageEnabled)
                {
                    sectionMain.CheckBox("Infinite Materials", ref modSettings.isInfiniteMaterialsEnabled, ref doInfiniteMaterials);
                }

                if (!modSettings.isInfiniteMaterialsEnabled)
                {
                    sectionMain.CheckBox("Unlimited Storage", ref modSettings.isUnlimitedStorageEnabled, ref doUnlimitedStorage);
                }

                sectionMain.CheckBox("Always Daytime", ref modSettings.isAlwaysDaytimeEnabled);
                sectionMain.CheckBox("Peaceful", ref modSettings.isPeacefulEnabled, ref doPeaceful);

                if (!modSettings.isPeacefulEnabled)
                {
                    sectionMain.CheckBox("Show Enemies", ref modSettings.isShowEnemiesEnabled);
                }

                sectionMain.Button("Spawn Migrant", ref doSpawnMigrant);
                sectionMain.Button("Spawn Merchant", ref doSpawnMerchant);
                sectionMain.Button("Spawn Animal", ref doSpawnAnimal);

                if (!modSettings.isPeacefulEnabled)
                {
                    sectionMain.Button("Spawn Invasion", ref doSpawnInvasion);
                    sectionMain.Button("Kill All Enemies", ref doKillEnemies);
                }

                sectionMain.Button("Best Traits", ref doBestTraits);
                sectionMain.Button("Max Current Professions", ref doMaxCurrentProfessions);
            }

            sectionMain.End();

            if (sectionMain.hasChildren)
            {
                this.containerHeight = sectionMain.ControlYPosition + sectionMain.ControlMargin;
            }
            else this.containerHeight = sectionMain.ControlYPosition;
        } 
        
        public override void OnUpdate()
        {
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
                resourceService.restoreStorageCaps();
            }
            
            if (doInfiniteMaterials)
            {
                doInfiniteMaterials = false;
                modSettings.isUnlimitedStorageEnabled = false;

                if (modSettings.isInfiniteMaterialsEnabled)
                {                
                    resourceService.getAllResources().ForEach(r => resourceService.lowerResourceMass(r));
                }
                else resourceService.getAllResources().ForEach(r => resourceService.restoreResourceMass(r));

                resourceService.getAllResources().ForEach(r => resourceService.fillResourceStorage(r, FILL_STORAGE_PERCENTAGE));
            }

            if (modSettings.isCheatsEnabled)
            {
                if (modSettings.isPeacefulEnabled)
                {
                    unitService.getFriendlyUnits().ForEach(u => fixUnitStatus(u));
                }
                else if (modSettings.isShowEnemiesEnabled)
                {
                    unitService.getEnemyUnits().ForEach(u => u.spottedTimer = 3f);
                }

                if (modSettings.isInfiniteMaterialsEnabled)
                {
                    resourceService.getAllResources().ForEach(r => resourceService.fillResourceStorage(r, FILL_STORAGE_PERCENTAGE));
                }

                if (modSettings.isUnlimitedStorageEnabled)
                {
                    resourceService.makeStorageRoom();
                }

                if (doSpawnMigrant)
                {
                    doSpawnMigrant = false;
                    if (!unitService.spawnMigrant())
                    {
                        log("Roads are needed before a migrant can be spawned.");
                    }
                }
                else if (doSpawnMerchant)
                {
                    doSpawnMerchant = false;
                    if (!unitService.spawnMerchant())
                    {
                        log("Roads are needed before a merchant can be spawned.");
                    }
                }
                else if (doSpawnAnimal)
                {
                    doSpawnAnimal = false;
                    unitService.spawnAnimal();
                }
                else if (doSpawnInvasion)
                {
                    doSpawnInvasion = false;
                    unitService.spawnInvasion();
                }
                else if (doKillEnemies)
                {
                    doKillEnemies = false;
                    unitService.getEnemyUnits().ForEach(u => u.hitpoints = 0f);
                }
                else if (doBestTraits)
                {
                    doBestTraits = false;
                    unitService.getPlayableUnits().ForEach(u => UnitTrait.setBestTraits(u));
                }
                else if (doMaxCurrentProfessions)
                {
                    doMaxCurrentProfessions = false;
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
    }
}
