using Plugin.BlowyAsteroid.TimberAndStoneMod.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private bool isChopping { get { return controlPlayer.designType == eDesignType.CHOP; } }
        private bool isMining { get { return controlPlayer.designType == eDesignType.MINE; } }
        private bool isBuilding { get { return controlPlayer.designType == eDesignType.BUILD; } }
        private bool isSelecting { get { return controlPlayer.selecting; } }
        private bool isDesigning { get { return controlPlayer.designing; } }

        private bool isScrolling { get { return Input.GetAxis(Mouse.SCROLL_WHEEL) != 0; } }

        private BlockProperties selectedBlockType;
        private IBlockData selectedBlockData;
        private List<BlockProperties> availableBlockTypes;

        private bool isCreativeEnabled = false;

        private bool doCreateBlocks = false;
        private bool doReplaceBlocks = false;
        private bool doRemoveBlocks = false;
        private bool doRemoveTrees = false;
        private bool doRemoveAllTrees = false;    
        private bool doSaveGame = false;
        private bool doSmoothTerrain = false;

        private bool hasRemovedTrees = false;

        private bool isMouseInGUIOverride = false;
        private bool isScrollOverride = false;

        public void Start()
        {
            setUpdatesPerSecond(5);

            availableBlockTypes = ModUtils.getUnbuildableBlockTypes();       
        }

        private IBlock tempBlock;
        public void Update()
        {         
            if (isComponentVisible)
            {
                translateMouse();
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

                isMouseInGUIOverride = false;
            }

            if (isCreativeEnabled)
            {
                if (!isSelecting)
                {
                    if (Input.GetKeyUp(CHANGE_VIEW_KEY))
                    {
                        controlPlayer.SwitchCamera();
                        log("Camera switched.");
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

                        updateControlPlayerBlockProperties(tempBlock.properties, tempBlock);
                        controlPlayer.StartDesigning(eDesignType.BUILD);
                    }
                }
                else if (!isDesigning && !isSelecting)
                {
                    if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl) && !doSaveGame)
                    {
                        doSaveGame = true;
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
            }
        }
        
        public void postUpdate()
        {
            if (!isTimeToUpdate(DateTime.Now.Ticks)) return;
            
            if (isScrollOverride && !isScrolling)
            {
                isScrollOverride = false;
            }

            if (isCreativeEnabled)
            {
                if (doCreateBlocks || doReplaceBlocks)
                {
                    selectedBlockType = controlPlayer.buildingMaterial;

                    if (selectedBlockType.isTransparent() && selectedBlockType.getVariations() != null)
                    {
                        selectedBlockData = controlPlayer.buildingVariations[controlPlayer.buildingVariationIndex][0];
                    }
                    else selectedBlockData = null;                  
                }

                if (doReplaceBlocks)
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
                }
                else if (doRemoveAllTrees)
                {
                    doRemoveAllTrees = false;
                    //Remove All Trees
                    buildingService.removeAllTreeItems();
                }
                else if (doSaveGame)
                {
                    doSaveGame = false;

                    if (Time.timeSinceLevelLoad > 12f)
                    {
                        log("Saving: " + WorldManager.getInstance().settlementName);
                        WorldManager.getInstance().SaveGame();
                    }
                    else log("Unable to save. Press play until save button is visible then try again.");
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
                    updateMouseForUI(parentContainer);
                }
            }
        }
       
        private void drawBuildWindow(int id)
        {
            Window(parentContainer, PARENT_CONTAINER_TITLE);

            CheckBox("Creative", ref isCreativeEnabled);

            if (isCreativeEnabled)
            {
                if (isBuilding && isDesigning)
                {
                    Label(controlPlayer.buildingMaterial.getName());
                }

                if (isSelecting)
                {
                    if (!isScrollOverride)
                    {
                        guiManager.mouseInGUI = isMouseInGUIOverride;
                    }
                }
            }

            BeginScrollView(scrollViewContainer, scrollContainer);

            if (isCreativeEnabled)
            {
                if (isDesigning && !isSelecting)
                {
                    if (isBuilding)
                    {
                        foreach (BlockProperties properties in availableBlockTypes)
                        {
                            if(Button(properties.getName()))
                            {
                                updateControlPlayerBlockProperties(properties);
                                break;
                            }
                        }
                    }
                    else if (isChopping)
                    {
                        Button("Remove All Trees", ref doRemoveAllTrees);
                    }
                }
                else if(!isSelecting && !isDesigning)
                {
                    if (Time.timeSinceLevelLoad > 12f)
                    {
                        Button("Save Game", ref doSaveGame);
                    }                    
                }
            }

            EndScrollView(ref parentContainer, ref scrollContainer, ref scrollViewContainer, CONTAINER_WIDTH);
        }

        private String getKeyString(KeyCode key)
        {
            return "[" + key + "]";
        }        

        private void updateControlPlayerBlockProperties(BlockProperties properties, IBlock block = null)
        {
            controlPlayer.buildingMaterial = properties;
            controlPlayer.buildTile = properties.getID();
            controlPlayer.buildingVariations = properties.getVariations();           
            controlPlayer.buildingVariationIndex = ModUtils.getVariationIndexFromBlock(block);
        }

        

    }
}
