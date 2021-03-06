﻿using Plugin.BlowyAsteroid.TimberAndStoneMod.Collections;
using Plugin.BlowyAsteroid.TimberAndStoneMod.Services;
using Plugin.BlowyAsteroid.TimberAndStoneMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Timber_and_Stone;
using Timber_and_Stone.API;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.BlockData;
using Timber_and_Stone.Blocks;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Components
{
    public class CreativeMenuComponent : GUIPluginComponent
    {        
        private const KeyCode KEY_BUILD = KeyCode.E;
        private const KeyCode KEY_CHOP = KeyCode.C;
        private const KeyCode KEY_RAISE_SELECTION = KeyCode.R;
        private const KeyCode KEY_LOWER_SELECTION = KeyCode.F;
        private const KeyCode KEY_CHANGE_VIEW = KeyCode.V;
        private const KeyCode KEY_PICK_BLOCK = KeyCode.Q;

        private ModSettings modSettings = ModSettings.getInstance();
        private BuildingService buildingService = BuildingService.getInstance();
        private UnitService unitService = UnitService.getInstance();
        private GameSaveService gameSaveService = GameSaveService.getInstance();

        private bool isChopping { get { return controlPlayer.designing && controlPlayer.designType == eDesignType.CHOP; } }
        private bool isMining { get { return controlPlayer.designing && controlPlayer.designType == eDesignType.MINE; } }
        private bool isBuilding { get { return controlPlayer.designing && controlPlayer.designType == eDesignType.BUILD; } }
        private bool isStructuring { get { return controlPlayer.designing && controlPlayer.designType == eDesignType.STRUCTURE; } }
        private bool isSelecting { get { return controlPlayer.selecting; } }
        private bool isDesigning { get { return controlPlayer.designing; } }

        private bool isScrolling { get { return Input.GetAxis(Mouse.SCROLL_WHEEL) != 0; } }

        private BlockProperties selectedBlockType;
        private IBlockData selectedBlockData;
        private List<BlockProperties> availableBlockTypes;
        
        private bool doCreateBlocks = false;
        private bool doReplaceBlocks = false;
        private bool doRemoveBlocks = false;
        private bool doRemoveTrees = false;
        private bool doRemoveAllTrees = false;    
        private bool doSaveGame = false;
        private bool doSaveBackup = false;
        private bool doSmoothTerrain = false;
        private bool doBuildStructures = false;
        private bool doRemoveEntity = false;
        private bool doSetPlayerUnitSettings = false;

        private bool doApplyPreferences = true;

        private bool hasRemovedTrees = true;
        private bool hasBuiltStructures = true;

        private bool isMouseInGUIOverride = false;
        private bool isScrollOverride = false;
        
        private UnitHuman selectedUnitType;
        private bool isSelectingHumanType = false;
        private bool isPlacingHuman = false;
        private bool doPlaceHuman = false;

        private UnitEnemy selectedEnemyType;
        private bool isSelectingEnemyType = false;
        private bool isPlacingEnemy = false;
        private bool doPlaceEnemy = false;

        private UnitAnimal selectedAnimalType;
        private bool isSelectingAnimalType = false;
        private bool isPlacingAnimal = false;
        private bool doPlaceAnimal = false;

        private bool isSelectingUnitType { get { return isSelectingHumanType || isSelectingEnemyType || isSelectingAnimalType; } }
        private bool isPlacingUnitType { get { return isPlacingHuman || isPlacingHuman; } }
        
        private MonoBehaviour selectedObject { get { return worldManager.PlayerFaction.selectedObject; } }
        private ALivingEntity selectedEntity { get { return isLivingEntitySelected ? selectedObject as ALivingEntity : null; } }
        private APlayableEntity selectedUnit { get { return isPlayableUnitSelected ? selectedObject as APlayableEntity : null; } }

        private bool isObjectSelected { get { return selectedObject != null; } }
        private bool isLivingEntitySelected { get { return isObjectSelected && selectedObject is ALivingEntity; } }
        private bool isPlayableUnitSelected { get { return isObjectSelected && selectedObject is APlayableEntity; } }

        private int playerFactionUnitCount { get { return worldManager.PlayerFaction.units.Where(u => u.isAlive()).Count(); } }
        
        private bool shiftClickUp = false;

        private IBlock tempBlock;
        private PlayerUnitSettings playerUnitTraitSettings;
        private Vector3 mouseWorldPosition = Vector3.zero;

        private GUISection sectionScroll = new GUISection();

        #region Events
        [Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
        public void onMigrantAcceptNormal(EventMigrantAccept evt)
        {
            applyPlayerPreferences(evt.unit);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
        public void onGameLoad(EventGameLoad evt)
        {
            
        }
        #endregion

        public override void OnStart()
        {
            setWindowSize(260f + sectionMain.ControlMargin * 2, Screen.height / 2);
            setWindowPosition(Screen.width - this.containerWidth, 0f);
            setUpdatesPerSecond(3);

            isVisibleInGame = true;
            isVisibleInMainMenu = false;
            isVisibleDuringGameOver = false;

            sectionMain.Direction = GUISection.FlowDirection.VERTICAL;
            sectionMain.Flow = GUISection.Overflow.HIDDEN;

            sectionScroll.Direction = GUISection.FlowDirection.VERTICAL;
            sectionScroll.Flow = GUISection.Overflow.SCROLL;

            availableBlockTypes = ModUtils.getUnbuildableBlockTypes();
        }

        private void applyPlayerPreferences(ALivingEntity entity)
        {
            if (!UnitPreference.getPreference(entity, UnitPreference.IS_PLAYER_UNIT))
            {
                UnitPreference.setPreference(entity, UnitPreference.IS_PLAYER_UNIT, true);
                UnitPreference.setPreference(entity, UnitPreference.WAIT_IN_HALL_WHILE_IDLE, true);
                UnitPreference.setPreference(entity, UnitPreference.TRAIN_UNDER_LEVEL_3, true);
            }
        }

        public override void OnInput()
        {         
            if (isLivingEntitySelected && (isDesigning || isSelecting || isSelectingUnitType || isPlacingUnitType))
            {
                controlPlayer.CancelDesigning(true);
                isSelectingEnemyType = false;
                isSelectingHumanType = false;
                isSelectingAnimalType = false;
                isPlacingHuman = false;
                isPlacingEnemy = false;
                isPlacingAnimal = false;
            }

            if (isDesigning && controlPlayer.designType == eDesignType.PATROL) return;

            if ((Input.GetMouseButtonUp(Mouse.RIGHT) && !controlPlayer.cameraRotate)
                || Input.GetKeyUp(KeyCode.Tab) || Input.GetKeyUp(KeyCode.Escape))
            {
                if (isSelecting)
                {
                    controlPlayer.CancelDesigning(false);
                    controlPlayer.StartDesigning(controlPlayer.designType);
                }
                else
                {                   
                    controlPlayer.CancelDesigning(true);
                }

                if (isPlacingHuman)
                {
                    isPlacingHuman = false;
                }
                else if (isSelectingHumanType)
                {
                    isSelectingHumanType = false;
                }

                if (isPlacingEnemy)
                {
                    isPlacingEnemy = false;
                }
                else if (isSelectingEnemyType)
                {
                    isSelectingEnemyType = false;
                }

                if (isPlacingAnimal)
                {
                    isPlacingAnimal = false;
                }
                else if (isSelectingAnimalType)
                {
                    isSelectingAnimalType = false;
                }

                isMouseInGUIOverride = false;
            }
            
            if (Input.GetKey(KeyCode.Z))
            {
                if (getBlockAtMouse(out tempBlock))
                {
                    buildingService.setDepthLevel(tempBlock.coordinate.absolute.y + 2);
                }
            }
            else if (Input.GetKeyUp(KEY_CHANGE_VIEW))
            {
                controlPlayer.SwitchCamera();
                log("Camera switched.");
            }

            if (!isSelecting && !isSelectingUnitType && (modSettings.isCreativeEnabled ? !isChopping : true))
            {
                if (Input.GetKeyDown(KEY_BUILD))
                {
                    controlPlayer.StartDesigning(eDesignType.MINE);
                }
                else if (Input.GetKeyUp(KEY_CHOP))
                {
                    controlPlayer.chopType = eChopType.CLEARCUT;
                    controlPlayer.StartDesigning(eDesignType.CHOP);
                }
                else if (Input.GetKeyUp(KEY_PICK_BLOCK))
                {
                    if (getBlockAtMouse(out tempBlock))
                    {
                        if (ModUtils.isBuildable(tempBlock.properties))
                        {
                            updateControlPlayerBlockProperties(tempBlock.properties, tempBlock);
                            controlPlayer.StartDesigning(eDesignType.BUILD);
                        }

                        if (Input.GetKey(KeyCode.LeftControl))
                        {
                            log("ID: " + tempBlock.properties.getID());
                            log("Variation: " + controlPlayer.buildingVariationIndex);
                            log("Name: " + tempBlock.properties.getName());
                        }
                    }
                }
            }
            else if (isSelecting)
            {
                if (Input.GetKeyUp(KEY_RAISE_SELECTION))
                {
                    controlPlayer.RaiseSelectBox(controlPlayer.currentSelectBox);
                }
                else if (Input.GetKeyUp(KEY_LOWER_SELECTION))
                {
                    controlPlayer.LowerSelectBox(controlPlayer.currentSelectBox);
                }
            } 

            if (modSettings.isCreativeEnabled)
            {
                shiftClickUp = (Input.GetMouseButtonUp(Mouse.LEFT) && Input.GetKey(KeyCode.LeftShift));

                if (isStructuring && isDesigning && !shiftClickUp)
                {
                    hasBuiltStructures = false;                      
                }
                else if (!hasBuiltStructures)
                {
                    if (!shiftClickUp)
                    {
                        hasBuiltStructures = true;
                    }

                    doBuildStructures = true;
                }

                if (isPlacingHuman)
                {
                    if (Input.GetKeyDown(KEY_BUILD))
                    {
                        doPlaceHuman = true;
                    }
                }
                else if (isPlacingEnemy)
                {
                    if (Input.GetKeyDown(KEY_BUILD))
                    {
                        doPlaceEnemy = true;
                    }
                }
                else if (isPlacingAnimal)
                {
                    if (Input.GetKeyDown(KEY_BUILD))
                    {
                        doPlaceAnimal = true;
                    }
                }
                
                if (!isSelecting && !isSelectingUnitType)
                {  
                    if (isChopping)
                    {
                        if (Input.GetKeyUp(KEY_BUILD))
                        {
                            buildingService.addTree(Coordinate.FromWorld(controlPlayer.WorldPositionAtMouse()));
                        }
                        else if (Input.GetKeyUp(KEY_PICK_BLOCK))
                        {
                            buildingService.addShrub(Coordinate.FromWorld(controlPlayer.WorldPositionAtMouse()));
                        }
                    }  
                }
                else if (isSelecting)
                {
                    if (isScrolling && isMouseInGUIOverride)
                    {
                        isScrollOverride = true;
                    }    

                    if (isMining)
                    {
                        if (Input.GetMouseButtonUp(Mouse.LEFT) && !isMouseInGUIOverride)
                        {
                            isMouseInGUIOverride = true;
                        }
                        else if (Input.GetMouseButtonUp(Mouse.LEFT))
                        {
                            doRemoveBlocks = true;
                            isMouseInGUIOverride = false;

                            controlPlayer.CancelDesigning(false);
                            controlPlayer.StartDesigning(eDesignType.MINE);
                        }
                    }
                    else if (isBuilding)
                    {
                        if (Input.GetMouseButtonUp(Mouse.LEFT) && !isMouseInGUIOverride)
                        {
                            isMouseInGUIOverride = true;
                        }
                        else if (Input.GetMouseButtonUp(Mouse.LEFT))
                        {
                            if (Input.GetKey(KEY_BUILD))
                            {
                                doReplaceBlocks = true;
                            }
                            else if (Input.GetKey(KEY_PICK_BLOCK))
                            {
                                doSmoothTerrain = true;
                            }
                            else doCreateBlocks = true;

                            isMouseInGUIOverride = false;

                            controlPlayer.CancelDesigning(false);
                            controlPlayer.StartDesigning(eDesignType.BUILD);                            
                        }
                    }
                    else if (isChopping)
                    {
                        hasRemovedTrees = false;                 
                    }
                }                

                if (!hasRemovedTrees && !isSelecting)
                {
                    doRemoveTrees = true;
                    hasRemovedTrees = true;
                } 
            }
        }

        public override void OnUpdate()
        {
            if (isScrollOverride && !isScrolling)
            {
                isScrollOverride = false;
            }

            if (Time.timeSinceLevelLoad > 12f && doApplyPreferences)
            {
                doApplyPreferences = false;

                foreach (ALivingEntity entity in worldManager.PlayerFaction.units)
                {
                    applyPlayerPreferences(entity);
                }
            }

            if (doSaveGame || doSaveBackup)
            {
                doSaveGame = false;

                if (Time.timeSinceLevelLoad > 12f)
                {
                    worldManager.SaveGame();

                    if (doSaveBackup)
                    {
                        doSaveBackup = false;

                        if (!modSettings.isAutoBackupsEnabled)
                        {
                            GameSaveService.SaveGameInfo saveGameInfo = gameSaveService.getSaveGameInfoFromSettlementName(worldManager.settlementName);
                            gameSaveService.createBackup(saveGameInfo);
                            log("Backup saved for: " + saveGameInfo.Name);
                        }
                    }
                }
                else log("Unable to save. Press play until save button is visible then try again.");
            } 

            if (modSettings.isCreativeEnabled)
            {
                if (doCreateBlocks || doReplaceBlocks)
                {
                    selectedBlockType = controlPlayer.buildingMaterial;

                    if (selectedBlockType.getVariations() != null)
                    {
                        selectedBlockData = selectedBlockType.getVariations()[controlPlayer.buildingVariationIndex][0];
                    }
                    else
                    {
                        BlockDataTextureVariant textureData = new BlockDataTextureVariant(TextureVariant.None);
                        textureData.setVariant(TextureVariant.Pillar, controlPlayer.buildingPillarless);
                        textureData.setVariant(TextureVariant.Trimless, controlPlayer.buildingTrimless);

                        selectedBlockData = textureData;
                    }
                }                

                if (doBuildStructures)
                {
                    doBuildStructures = false;
                    controlPlayer.structures.Where(s => !s.isBuilt).ToList()
                        .ForEach(s => buildingService.buildStructure(ref s, worldManager.PlayerFaction));                                 
                }
                else if (doReplaceBlocks)
                {
                    doReplaceBlocks = false;
                    buildingService.replaceBlocksInSelection(selectedBlockType, selectedBlockData);
                }
                else if (doSmoothTerrain)
                {
                    doSmoothTerrain = false;
                    buildingService.smoothBlocksInSelection();
                }
                else if (doCreateBlocks)
                {
                    doCreateBlocks = false;
                    buildingService.buildBlocksInSelection(selectedBlockType, selectedBlockData);
                }
                else if (doRemoveBlocks)
                {
                    doRemoveBlocks = false;
                    buildingService.removeBlocksInSelection();
                }
                else if (doRemoveTrees)
                {
                    doRemoveTrees = false;
                    buildingService.removeSelectedTrees();
                    buildingService.removeSelectedShrubs();
                }
                else if (doRemoveAllTrees)
                {
                    doRemoveAllTrees = false;
                    buildingService.removeAllTreeItems();
                    controlPlayer.CancelDesigning(true);
                }
                else if (doPlaceHuman)
                {
                    doPlaceHuman = false;
                    if (isMouseInWorld(out mouseWorldPosition))
                    {
                        unitService.addHuman(selectedUnitType, mouseWorldPosition, autoAccept: true);
                    }                    
                }
                else if (doPlaceEnemy)
                {
                    doPlaceEnemy = false;
                    if (isMouseInWorld(out mouseWorldPosition))
                    {
                        unitService.addEnemy(selectedEnemyType, mouseWorldPosition);
                    }
                }
                else if (doPlaceAnimal)
                {
                    doPlaceAnimal = false;
                    if (isMouseInWorld(out mouseWorldPosition))
                    {
                        unitService.addAnimal(selectedAnimalType, mouseWorldPosition);
                    }
                }
                else if (doRemoveEntity)
                {
                    doRemoveEntity = false;
                    if (UnitService.isFriendly(selectedEntity) && playerFactionUnitCount <= 1)
                    {
                        log("Unable to remove player unit. There must be at least one unit in the player faction.");
                        return;
                    }
                    selectedEntity.Destroy();
                }
                else if (doSetPlayerUnitSettings)
                {
                    doSetPlayerUnitSettings = false;
                    if (isPlayableUnitSelected)
                    {
                        PlayerUnitSettings.setPlayerUnitSettings(playerUnitTraitSettings, selectedUnit, UnitTrait.List);
                    }
                }
                else if (isPlayableUnitSelected)
                {
                    PlayerUnitSettings.updatePlayerUnitSettings(ref playerUnitTraitSettings, selectedUnit, UnitTrait.List);
                }  
            }
        }

        private float newLevelValue;
        public override void OnDraw(int windowId)
        {
            Window(this.title);

            CheckBox(sectionMain.ControlMargin, WINDOW_TITLE_HEIGHT + sectionMain.ControlMargin + sectionMain.ControlPadding,
                sectionMain.ControlWidth / 2, sectionMain.ControlHeight, "Creative", ref modSettings.isCreativeEnabled);

            sectionMain.Begin(0, WINDOW_TITLE_HEIGHT + sectionMain.ControlHeight + sectionMain.ControlPadding, this.width, this.height - WINDOW_TITLE_HEIGHT - sectionMain.ControlHeight);

            if (isMouseHover || isDesigning || isSelectingUnitType || isLivingEntitySelected)
            {
                if (isMouseHover && !isSelecting && !isDesigning && !isSelectingUnitType && !isLivingEntitySelected)
                {
                    if (Time.timeSinceLevelLoad > 12f)
                    {
                        sectionMain.Button("Save Game", ref doSaveGame);
                        if (!modSettings.isAutoBackupsEnabled)
                        {
                            sectionMain.Button("Save Backup", ref doSaveBackup);
                        }
                    }
                }

                if (modSettings.isCreativeEnabled)
                {
                    if (isSelectingHumanType && !isPlacingHuman)
                    {
                        sectionMain.LabelCentered("Player Units");
                    }
                    else if (isSelectingEnemyType && !isPlacingEnemy)
                    {
                        sectionMain.LabelCentered("Enemy Units");
                    }
                    else if (isSelectingAnimalType && !isPlacingAnimal)
                    {
                        sectionMain.LabelCentered("Animal Units");
                    }
                    else if (isPlacingHuman)
                    {
                        sectionMain.LabelCentered(selectedUnitType.Name);
                        sectionMain.LabelCentered(getKeyString(KEY_BUILD) + " To Place");
                    }
                    else if (isPlacingEnemy)
                    {
                        sectionMain.LabelCentered("Enemy: " + selectedEnemyType.Name);
                        sectionMain.LabelCentered(getKeyString(KEY_BUILD) + " To Place");
                    }
                    else if (isPlacingAnimal)
                    {
                        sectionMain.LabelCentered("Animal: " + selectedAnimalType.Name);
                        sectionMain.LabelCentered(getKeyString(KEY_BUILD) + " To Place");
                    }
                    else if (isBuilding)
                    {
                        if (isSelecting)
                        {
                            if (Input.GetKey(KEY_BUILD))
                            {
                                sectionMain.LabelCentered("Replacing");
                            }
                            else if (Input.GetKey(KEY_PICK_BLOCK))
                            {
                                sectionMain.LabelCentered("Smoothing");
                            }
                            else
                            {
                                sectionMain.LabelCentered("Hold " + getKeyString(KEY_BUILD) + " to Replace");
                                sectionMain.LabelCentered("Hold " + getKeyString(KEY_PICK_BLOCK) + " to Smooth");
                            }
                        }
                        else if (isDesigning)
                        {
                            sectionMain.LabelCentered(controlPlayer.buildingMaterial.getName());
                        }
                    }
                    else if (isChopping)
                    {
                        sectionMain.LabelCentered(getKeyString(KEY_BUILD) + " Place Tree");
                        sectionMain.LabelCentered(getKeyString(KEY_PICK_BLOCK) + " Place Shrub");
                        sectionMain.Button("Remove All Trees", ref doRemoveAllTrees);
                    }
                    else if (isLivingEntitySelected)
                    {
                        if (isPlayableUnitSelected && UnitService.isFriendly(selectedEntity))
                        {
                            sectionMain.LabelCentered(selectedUnit.getProfession().getProfessionName());

                            if (selectedUnit.isAlive())
                            {
                                if (sectionMain.NumberSelect("Level: ", selectedUnit.getProfession().getLevel(), out newLevelValue, 
                                    increment: Input.GetKey(KeyCode.LeftShift) ? 5 : 1, min: 1, max: AProfession.maxLevel, showMinMax: true))
                                {
                                    selectedUnit.getProfession().setLevel(newLevelValue);
                                }

                                sectionMain.LabelCentered("Traits");
                                sectionScroll.Background(guiManager.windowBoxStyle);
                            }
                        }
                        else
                        {
                            sectionMain.LabelCentered(selectedEntity.unitName);
                            drawSelectedUnitButtons();
                        }
                    }
                    else if (!isSelecting && !isDesigning && !isSelectingUnitType)
                    {
                        if (sectionMain.Button("Add Player Units"))
                        {
                            isSelectingHumanType = true;
                        }

                        if (!modSettings.isPeacefulEnabled)
                        {
                            if (sectionMain.Button("Add Enemy Units"))
                            {
                                isSelectingEnemyType = true;
                            }
                        }

                        if (sectionMain.Button("Add Animal Units"))
                        {
                            isSelectingAnimalType = true;
                        }
                    }

                    if (isSelecting)
                    {
                        if (!isScrollOverride)
                        {
                            guiManager.mouseInGUI = isMouseInGUIOverride;
                        }
                    }
                }
            }

            sectionScroll.Begin(0, sectionMain.ControlYPosition - sectionScroll.ControlMargin, this.ParentContainer.width, sectionMain.ControlHeight * 10);

            if (isMouseHover || isDesigning || isSelectingUnitType || isLivingEntitySelected)
            {
                if (modSettings.isCreativeEnabled)
                {
                    if (isPlayableUnitSelected && UnitService.isFriendly(selectedUnit))
                    {                        
                        if (selectedUnit.isAlive())
                        {
                            foreach (IUnitSettingCollectionItem item in UnitTrait.List)
                            {
                                sectionScroll.CheckBox(item.Description, ref playerUnitTraitSettings.getSetting(item).Enabled, ref doSetPlayerUnitSettings);
                            }
                        }
                    }
                    else if (isSelectingHumanType && !isPlacingHuman)
                    {
                        //Place Human List
                        foreach (UnitHuman profession in UnitHuman.List)
                        {
                            if (sectionScroll.Button(profession.Name))
                            {
                                selectedUnitType = profession;
                                isPlacingHuman = true;
                            }
                        }
                    }
                    else if (isSelectingEnemyType && !isPlacingEnemy)
                    {
                        //Place Enemy List
                        foreach (UnitEnemy enemyType in UnitEnemy.List)
                        {
                            if (sectionScroll.Button(enemyType.Name))
                            {
                                selectedEnemyType = enemyType;
                                isPlacingEnemy = true;
                            }
                        }
                    }
                    else if (isSelectingAnimalType && !isPlacingAnimal)
                    {
                        //Place Animal List
                        foreach (UnitAnimal animalType in UnitAnimal.List)
                        {
                            if (sectionScroll.Button(animalType.Name))
                            {
                                selectedAnimalType = animalType;
                                isPlacingAnimal = true;
                            }
                        }
                    }
                    else if (isDesigning && !isSelecting)
                    {
                        if (isBuilding)
                        {
                            foreach (BlockProperties properties in availableBlockTypes)
                            {
                                if (sectionScroll.Button(properties.getName()))
                                {
                                    updateControlPlayerBlockProperties(properties);
                                }
                            }
                        }
                    }
                }
            }

            sectionScroll.End();

            if (sectionScroll.hasChildren)
            {
                sectionMain.addSection(sectionScroll);
            }

            if (isMouseHover || isDesigning || isSelectingUnitType || isLivingEntitySelected)
            {
                if (modSettings.isCreativeEnabled)
                {
                    if (isPlayableUnitSelected && UnitService.isFriendly(selectedUnit))
                    {
                        drawSelectedUnitButtons();
                    }
                }
            }

            sectionMain.End();

            if (sectionMain.hasChildren)
            {
                this.containerHeight = sectionMain.ControlYPosition + sectionMain.ControlMargin;
            }
            else this.containerHeight = sectionMain.ControlYPosition;
        }

        private void drawSelectedUnitButtons()
        {
            if (!selectedEntity.isAlive())
            {
                if (sectionMain.Button("Revive Unit"))
                {
                    unitService.reviveUnit(selectedEntity);
                }
            }
            else if ((UnitService.isFriendly(selectedEntity)
                ? (!modSettings.isPeacefulEnabled && playerFactionUnitCount > 1) : true))
            {
                if (sectionMain.Button("Kill Unit"))
                {
                    selectedEntity.hitpoints = 0f;
                }
            }

            sectionMain.Button("Remove Unit", ref doRemoveEntity);
        }

        private String getKeyString(KeyCode key)
        {
            return "[" + key + "]";
        }

        private BlockDataTextureVariant tempBlockDataTextureVariant;
        private void updateControlPlayerBlockProperties(BlockProperties properties, IBlock block = null)
        {
            controlPlayer.buildingMaterial = properties;
            controlPlayer.buildTile = properties.getID();
            controlPlayer.buildingVariations = properties.getVariations(); 
            controlPlayer.buildingVariationIndex = ModUtils.getVariationIndex(block);

            if (block != null && (tempBlockDataTextureVariant = block.getMeta<BlockDataTextureVariant>()) != null)
            {
                controlPlayer.buildingPillarless = tempBlockDataTextureVariant.checkVariant(TextureVariant.Pillar);                
                controlPlayer.buildingTrimless = tempBlockDataTextureVariant.checkVariant(TextureVariant.Trimless);
            }
            else
            {
                controlPlayer.buildingPillarless = false;
                controlPlayer.buildingTrimless = false;
            }
        }

        private bool getBlockAtMouse(out IBlock block, bool isAirAllowed = false)
        {
            if (isMouseInWorld(out mouseWorldPosition))
            {
                block = buildingService.getBlock(Coordinate.FromWorld(mouseWorldPosition));

                if (!isAirAllowed && block.properties.GetType() == typeof(BlockAir))
                {
                    block = block.relative(0, -1, 0);
                }
            }
            else block = null;

            return block != null;
        }
    }
}
