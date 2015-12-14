using Plugin.BlowyAsteroid.TimberAndStoneMod.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Timber_and_Stone;
using Timber_and_Stone.API;
using Timber_and_Stone.BlockData;
using Timber_and_Stone.Blocks;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Components
{
    public class CreativeMenuComponent : ModComponent
    {        
        private const KeyCode PRIMARY_KEY = KeyCode.E;
        private const KeyCode CHOP_KEY = KeyCode.C;
        private const KeyCode RAISE_SELECTION_KEY = KeyCode.R;
        private const KeyCode LOWER_SELECTION_KEY = KeyCode.F;
        private const KeyCode CHANGE_VIEW_KEY = KeyCode.V;
        private const KeyCode PICK_BLOCK_KEY = KeyCode.Q;

        private const String PARENT_CONTAINER_TITLE = "";
        private const int PARENT_CONTAINER_ID = 102;
        private const float CONTAINER_WIDTH = BUTTON_WIDTH + SCROLL_BAR_SIZE;
        private static readonly float CONTAINER_HEIGHT = WINDOW_TITLE_HEIGHT;

        private BuildingService buildingService = BuildingService.getInstance();
        private UnitService unitService = UnitService.getInstance();

        private Rect parentContainer = new Rect(
            Screen.width - CONTAINER_WIDTH - BUTTON_PADDING * 2, 
            MAIN_MENU_HEADER_HEIGHT - WINDOW_TITLE_HEIGHT, 
            CONTAINER_WIDTH + DOUBLE_PADDING, 
            CONTAINER_HEIGHT
        );

        private Rect scrollContainer = new Rect(
            BUTTON_PADDING, BUTTON_PADDING, 
            CONTAINER_WIDTH - SCROLL_BAR_SIZE - DOUBLE_PADDING, 
            CONTAINER_HEIGHT - WINDOW_TITLE_HEIGHT - DOUBLE_PADDING
        );

        private Rect scrollViewContainer = new Rect(
            BUTTON_PADDING, WINDOW_TITLE_HEIGHT, CONTAINER_WIDTH, 
            CONTAINER_HEIGHT - WINDOW_TITLE_HEIGHT - DOUBLE_PADDING
        );

        private bool isChopping { get { return controlPlayer.designing && controlPlayer.designType == eDesignType.CHOP; } }
        private bool isMining { get { return controlPlayer.designing && controlPlayer.designType == eDesignType.MINE; } }
        private bool isBuilding { get { return controlPlayer.designing && controlPlayer.designType == eDesignType.BUILD; } }
        private bool isStructuring { get { return controlPlayer.designing && controlPlayer.designType == eDesignType.STRUCTURE; } }
        private bool isSelecting { get { return controlPlayer.selecting; } }
        private bool isDesigning { get { return controlPlayer.designing; } }
        private bool isSelectingUnitType { get { return isSelectingHumanType || isSelectingEnemyType; } }
        private bool isPlacingUnitType { get { return isPlacingHuman || isPlacingHuman; } }

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
        private bool doSmoothTerrain = false;
        private bool doBuildStructures = false;

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

        private MonoBehaviour selectedObject { get { return worldManager.PlayerFaction.selectedObject; } }
        private bool isObjectSelected { get { return selectedObject != null; } }
        private bool isLivingEntitySelected { get { return isObjectSelected && selectedObject is ALivingEntity; } }
        private ALivingEntity selectedEntity { get { return isLivingEntitySelected ? selectedObject as ALivingEntity : null; } }        
        private bool isPlayableUnitSelected { get { return isObjectSelected && selectedObject is APlayableEntity; } }
        private APlayableEntity selectedUnit { get { return isPlayableUnitSelected ? selectedObject as APlayableEntity : null; } }
        private int playerFactionUnitCount { get { return worldManager.PlayerFaction.units.Where(u => u.isAlive()).Count(); } }
        private bool doRemoveEntity = false;
        private bool doSetPlayerUnitSettings = false;

        public void Start()
        {
            setUpdatesPerSecond(5);

            availableBlockTypes = ModUtils.getUnbuildableBlockTypes();
        }

        private IBlock tempBlock;
        private bool shiftClickUp = false;
        public void Update()
        {         
            if (isComponentVisible)
            {
                translateMouse();
            }

            if (isLivingEntitySelected && (isDesigning || isSelecting || isSelectingUnitType || isPlacingUnitType))
            {
                controlPlayer.CancelDesigning(true);
                isSelectingEnemyType = false;
                isSelectingHumanType = false;
                isPlacingHuman = false;
                isPlacingEnemy = false;
            }

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

                isMouseInGUIOverride = false;
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
                    if (Input.GetKeyDown(PRIMARY_KEY))
                    {
                        doPlaceHuman = true;
                    }
                }
                else if (isPlacingEnemy)
                {
                    if (Input.GetKeyDown(PRIMARY_KEY))
                    {
                        doPlaceEnemy = true;
                    }
                }
                
                if (!isSelecting && !isSelectingUnitType)
                {    
                    if (Input.GetKeyUp(CHANGE_VIEW_KEY))
                    {
                        controlPlayer.SwitchCamera();
                        log("Camera switched.");
                    }

                    if (isChopping)
                    {
                        if (Input.GetKeyUp(PRIMARY_KEY))
                        {
                            buildingService.addTree(Coordinate.FromWorld(controlPlayer.WorldPositionAtMouse()));
                        }
                        else if (Input.GetKeyUp(PICK_BLOCK_KEY))
                        {
                            buildingService.addShrub(Coordinate.FromWorld(controlPlayer.WorldPositionAtMouse()));
                        }
                    }                   
                    else if (Input.GetKeyDown(PRIMARY_KEY))
                    {
                        controlPlayer.StartDesigning(eDesignType.MINE);
                    }
                    else if (Input.GetKeyUp(CHOP_KEY))
                    {
                        controlPlayer.chopType = eChopType.CLEARCUT;
                        controlPlayer.StartDesigning(eDesignType.CHOP);
                    }
                    else if (Input.GetKeyUp(PICK_BLOCK_KEY))
                    {
                        //Pick Block
                        tempBlock = buildingService.getBlock(controlPlayer.WorldPositionAtMouse());

                        if (tempBlock.properties.GetType() == typeof(BlockAir))
                        {
                            tempBlock = tempBlock.relative(0, -1, 0);
                        }

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
                else if (isSelecting)
                {
                    if (Input.GetKeyUp(RAISE_SELECTION_KEY))
                    {
                        controlPlayer.RaiseSelectBox(controlPlayer.currentSelectBox);
                    }
                    else if (Input.GetKeyUp(LOWER_SELECTION_KEY))
                    {
                        controlPlayer.LowerSelectBox(controlPlayer.currentSelectBox);
                    }

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
                            if (Input.GetKey(PRIMARY_KEY))
                            {
                                doReplaceBlocks = true;
                            }
                            else if (Input.GetKey(PICK_BLOCK_KEY))
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

            try
            {
                postUpdate();
            }
            catch (Exception e)
            {
                log(e.Message);
                e.StackTrace.Split(Environment.NewLine.ToCharArray()).ToList().ForEach(line => log(line));
            }
        }

        public void postUpdate()
        {
            if (!isTimeToUpdate(DateTime.Now.Ticks)) return;

            if (isScrollOverride && !isScrolling)
            {
                isScrollOverride = false;
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
                    //Build Non-Built Structures
                    controlPlayer.structures.Where(s => !s.isBuilt).ToList().ForEach(s => buildingService.buildStructure(ref s));                                 
                }
                else if (doReplaceBlocks)
                {
                    doReplaceBlocks = false;
                    //Replace Selected Blocks
                    buildingService.replaceBlocks(buildingService.getSelectedCoordinates(excludeAirBlocks: true), selectedBlockType, selectedBlockData);
                }
                else if (doSmoothTerrain)
                {
                    doSmoothTerrain = false;
                    //Smooth Selected Blocks
                    buildingService.smoothBlocks(buildingService.getSelectedCoordinates(onlyAirBlocks: true));
                }
                else if (doCreateBlocks)
                {
                    doCreateBlocks = false;
                    //Build Selected Blocks
                    buildingService.buildBlocks(buildingService.getSelectedCoordinates(onlyAirBlocks: true), selectedBlockType, selectedBlockData);
                }
                else if (doRemoveBlocks)
                {
                    doRemoveBlocks = false;
                    //Remove Selected Blocks
                    buildingService.getSelectedCoordinates(excludeAirBlocks: true).ForEach(c => buildingService.removeBlock(c));
                }
                else if (doRemoveTrees)
                {
                    doRemoveTrees = false;
                    //Remove Selected Trees
                    buildingService.removeSelectedTrees();
                    buildingService.removeSelectedShrubs();
                }
                else if (doRemoveAllTrees)
                {
                    doRemoveAllTrees = false;
                    //Remove All Trees
                    buildingService.removeAllTreeItems();
                    controlPlayer.CancelDesigning(true);
                }
                else if (doPlaceHuman)
                {
                    doPlaceHuman = false;
                    if (isMouseInWorld(out mouseWorldPosition))
                    {
                        //Place Selected Unit Type At Mouse Position
                        unitService.addHuman(selectedUnitType, mouseWorldPosition, autoAccept: true);
                    }                    
                }
                else if (doPlaceEnemy)
                {
                    doPlaceEnemy = false;
                    if (isMouseInWorld(out mouseWorldPosition))
                    {
                        //Place Selected Enemy Type At Mouse Position
                        unitService.addEnemy(selectedEnemyType, mouseWorldPosition);
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
                    //Remove Entity
                    selectedEntity.Destroy();
                }
                else if (doSetPlayerUnitSettings)
                {
                    doSetPlayerUnitSettings = false;

                    if (isPlayableUnitSelected)
                    {
                        setPlayerUnitSettings(playerUnitTraitSettings, selectedUnit, UnitTrait.List);
                    }
                }
                else if (doSaveGame)
                {
                    doSaveGame = false;

                    if (Time.timeSinceLevelLoad > 12f)
                    {
                        log("Saving: " + worldManager.settlementName);
                        worldManager.SaveGame();
                    }
                    else log("Unable to save. Press play until save button is visible then try again.");
                }
                else if (isPlayableUnitSelected)
                {
                    updatePlayerUnitSettings(ref playerUnitTraitSettings, selectedUnit, UnitTrait.List);
                }
            }
        }        
        
        public void OnGUI()
        {
            if (isGameRunning)
            {
                if (isComponentVisible)
                { 
                    parentContainer = createWindow(PARENT_CONTAINER_ID, parentContainer, drawBuildWindow);
                    isMouseHover = updateMouseForUI(parentContainer);
                }
            }
        }

        private void drawBuildWindow(int id)
        {
            Window(parentContainer, PARENT_CONTAINER_TITLE);

            CheckBox("Creative", ref modSettings.isCreativeEnabled);

            if (isMouseHover || isDesigning || isSelectingUnitType || isLivingEntitySelected)
            {
                if (modSettings.isCreativeEnabled)
                {
                    if (isSelectingHumanType && !isPlacingHuman)
                    {
                        Label("Player Units");
                    }
                    else if (isSelectingEnemyType && !isPlacingEnemy)
                    {
                        Label("Enemy Units");
                    }
                    else if (isPlacingHuman)
                    {
                        Label(selectedUnitType.Name);
                        Label(getKeyString(PRIMARY_KEY) + " To Place");
                    }
                    else if (isPlacingEnemy)
                    {
                        Label("Enemy: " + selectedEnemyType.Name);
                        Label(getKeyString(PRIMARY_KEY) + " To Place");
                    }

                    else if (isBuilding)
                    {
                        if (isSelecting)
                        {
                            if (Input.GetKey(PRIMARY_KEY))
                            {
                                Label("Replacing");
                            }
                            else if (Input.GetKey(PICK_BLOCK_KEY))
                            {
                                Label("Smoothing");
                            }
                            else 
                            {
                                Label("Hold " + getKeyString(PRIMARY_KEY) + " to Replace");
                                Label("Hold " + getKeyString(PICK_BLOCK_KEY) + " to Smooth");
                            }
                        }
                        else if (isDesigning)
                        {
                            Label(controlPlayer.buildingMaterial.getName());
                        }
                    }
                    else if (isChopping)
                    {
                        Label(getKeyString(PRIMARY_KEY) + " Place Tree");
                        Label(getKeyString(PICK_BLOCK_KEY) + " Place Shrub");
                        Button("Remove All Trees", ref doRemoveAllTrees);
                    }
                    else if (isLivingEntitySelected)
                    {
                        Label(selectedEntity.unitName);

                        if (isPlayableUnitSelected)
                        {
                            if (selectedUnit.isAlive() && UnitService.isFriendly(selectedEntity))
                            {
                                Label("Lvl." + selectedUnit.getProfession().getLevel() + " " + selectedUnit.getProfession().getProfessionName());

                                if (Button("Max Current Profession"))
                                {
                                    UnitHuman.setCurrentProfessionMax(selectedUnit);
                                }

                                if (Button("Best Traits"))
                                {
                                    UnitTrait.setBestTraits(selectedUnit);
                                }
                            }
                            else if (UnitPreference.getPreference(selectedEntity, UnitPreference.IS_PLAYER_UNIT))
                            {
                                if (Button("Revive Unit"))
                                {
                                    UnitService.reviveUnit(selectedUnit, worldManager.PlayerFaction);
                                }
                            }
                        }

                        if (selectedEntity.isAlive() && (UnitService.isFriendly(selectedEntity)
                            ? (!modSettings.isPeacefulEnabled && playerFactionUnitCount > 1) : true))
                        {
                            if (Button("Kill Unit"))
                            {
                                selectedEntity.hitpoints = 0f;
                            }
                        }

                        Button("Remove Unit", ref doRemoveEntity);
                    }
                    else if (!isSelecting && !isDesigning && !isSelectingUnitType)
                    {
                        if (Time.timeSinceLevelLoad > 12f)
                        {
                            Button("Save Game", ref doSaveGame);
                        }

                        if (Button("Add Player Units"))
                        {
                            isSelectingHumanType = true;
                        }

                        if (!modSettings.isPeacefulEnabled)
                        {
                            if (Button("Add Enemy Units"))
                            {
                                isSelectingEnemyType = true;
                            }
                        }                        
                    }

                    if (isSelecting)
                    {
                        if (!isScrollOverride)
                        {
                            guiManager.mouseInGUI = isMouseInGUIOverride;
                        }
                    }               

                    BeginScrollView(scrollViewContainer, scrollContainer);

                    if (isDesigning && !isSelecting)
                    {
                        if (isBuilding)
                        {
                            foreach (BlockProperties properties in availableBlockTypes)
                            {
                                if (Button(properties.getName()))
                                {
                                    updateControlPlayerBlockProperties(properties);
                                    break;
                                }
                            }
                        }
                    }
                    else if (isPlayableUnitSelected && selectedUnit.isAlive())
                    {
                        drawPlayerUnitSettings(playerUnitTraitSettings, selectedUnit, UnitTrait.List, ref doSetPlayerUnitSettings);
                    }
                    else if (!isSelecting && !isDesigning && !isSelectingUnitType)
                    {

                    }
                    else if (isSelectingHumanType && !isPlacingHuman)
                    {
                        //Place Human List
                        foreach (UnitHuman profession in UnitHuman.List)
                        {
                            if (Button(profession.Name))
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
                            if (Button(enemyType.Name))
                            {
                                selectedEnemyType = enemyType;
                                isPlacingEnemy = true;
                            }
                        }
                    }
                }
            }

            EndScrollView(ref parentContainer, ref scrollContainer, ref scrollViewContainer, CONTAINER_WIDTH);
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
            controlPlayer.buildingVariationIndex = ModUtils.getVariationIndexFromBlock(block);

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

        private PlayerUnitSettings playerUnitTraitSettings;
        //private PlayerUnitSettings playerUnitPreferenceSettings;
        private void drawPlayerUnitSettings(PlayerUnitSettings playerUnitSettings, APlayableEntity entity, IModCollectionItem[] collection, ref bool onClick)
        {
            if (collection != null && UnitService.isFriendly(entity))
            {
                foreach (IModCollectionItem item in collection)
                {
                    CheckBox(item.Description, ref playerUnitSettings.getSetting(item).Enabled, ref onClick);
                }
            }
        }

        private void updatePlayerUnitSettings(ref PlayerUnitSettings playerUnitSettings, APlayableEntity entity, IModCollectionItem[] collection)
        {
            if (UnitPreference.isPlayableEntity(entity))
            {
                playerUnitSettings = PlayerUnitSettings.fromEntity(entity, collection);
            }
            else
            {
                playerUnitSettings = null;
            }
        }

        private void setPlayerUnitSettings(PlayerUnitSettings playerUnitSettings, APlayableEntity entity, IModCollectionItem[] collection)
        {
            if (playerUnitSettings != null)
            {
                foreach (IModCollectionItem item in collection)
                {
                    UnitPreference.setPreference(entity, item.Name, playerUnitSettings.getSetting(item).Enabled);
                }
            }
        }
    }
}
