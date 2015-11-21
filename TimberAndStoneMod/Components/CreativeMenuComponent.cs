using Plugin.BlowyAsteroid.TimberAndStoneMod.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timber_and_Stone;
using Timber_and_Stone.API;
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

        private const String PARENT_CONTAINER_TITLE = "";
        private const int PARENT_CONTAINER_ID = 102;
        private const float CONTAINER_WIDTH = BUTTON_WIDTH + SCROLL_BAR_SIZE;
        private static readonly float CONTAINER_HEIGHT = Screen.height * 0.25f;

        private BuildingService buildingService = BuildingService.getInstance();

        private Rect parentContainer = new Rect(
            Screen.width - CONTAINER_WIDTH - BUTTON_PADDING, 
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

        private eDesignType currentDesignType;
        private BlockProperties selectedBlockType;
        private IBlockData selectedBlockData;
        private List<BlockProperties> availableBlockTypes;

        private bool isCreativeEnabled = false;

        private bool doCreateBlocks = false;
        private bool doReplaceBlocks = false;
        private bool doRemoveBlocks = false;
        private bool doRemoveTrees = false;
        private bool doRemoveAllTrees = false;        
        private bool doMineStart = false;
        private bool doChopStart = false;
        private bool doSaveGame = false;

        public void Start()
        {
            setUpdatesPerSecond(5);

            availableBlockTypes = buildingService.getUnbuildableBlockTypes();       
        }

        private IBlock tempBlock;
        public void Update()
        {         
            if (isComponentVisible)
            {
                translateMouse();
            }

            if (!isDesigning && !isSelecting)
            {
                if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl) || doSaveGame)
                {
                    doSaveGame = false;

                    if (Time.timeSinceLevelLoad > 12f)
                    {
                        log("Saving: " + WorldManager.getInstance().settlementName);
                        WorldManager.getInstance().SaveGame();
                    }
                }
            }

            if (isCreativeEnabled)
            {
                currentDesignType = controlPlayer.designType;

                if (Input.GetKeyUp(CHANGE_VIEW_KEY))
                {
                    controlPlayer.SwitchCamera();
                    log("Camera switched.");
                }

                if (!isDesigning)
                {
                    if (Input.GetKeyUp(PRIMARY_KEY) || doMineStart)
                    {
                        doMineStart = false;
                        controlPlayer.StartDesigning(eDesignType.MINE);
                    }
                    else if (Input.GetKeyUp(CHOP_KEY) || doChopStart)
                    {
                        doChopStart = false;
                        controlPlayer.chopType = eChopType.CLEARCUT;
                        controlPlayer.StartDesigning(eDesignType.CHOP);
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

                    if (isMining)
                    {
                        if (Input.GetKeyUp(PRIMARY_KEY))
                        {
                            doRemoveBlocks = true;

                            controlPlayer.CancelDesigning(false);
                            controlPlayer.StartDesigning(eDesignType.MINE);
                        }
                    }
                    else if (isBuilding)
                    {
                        if (Input.GetKeyUp(PRIMARY_KEY))
                        {
                            //Replace Blocks
                            doReplaceBlocks = true;

                            updateSelectedBlockInfo();
                            controlPlayer.CancelDesigning(false);
                            controlPlayer.StartDesigning(eDesignType.BUILD);
                        }
                    }
                }
                else if(isDesigning)
                {
                    if (isBuilding)
                    {
                        if (Input.GetKeyDown(PRIMARY_KEY))
                        {
                            //Pick Block
                            tempBlock = buildingService.getBlock(controlPlayer.WorldPositionAtMouse());

                            if (tempBlock.properties.GetType() != typeof(BlockAir))
                            {
                                selectBlockProperties(tempBlock.properties);
                            }
                        }

                        doCreateBlocks = true;
                        updateSelectedBlockInfo();
                    }
                    else if (isChopping)
                    {
                        if (!doRemoveAllTrees)
                        {
                            //Remove Selected Trees
                             doRemoveTrees = true;
                        }
                    }  
                }              
            }           

            postUpdate();
        }
        
        public void postUpdate()
        {
            if (!isTimeToUpdate(DateTime.Now.Ticks)) return;

            if (isCreativeEnabled)
            {
                if (doReplaceBlocks)
                {
                    doReplaceBlocks = false;
                    //Replace Selected Blocks
                    buildingService.getSelectedCoordinates(true).ForEach(c =>
                        buildingService.buildBlock(c, selectedBlockType, selectedBlockData));
                }
                else if (doCreateBlocks)
                {
                    doCreateBlocks = false;
                    //Build Selected Blocks
                    buildingService.fillBuildDesignationsWith(selectedBlockType, selectedBlockData);
                }
                else if (doRemoveBlocks)
                {
                    doRemoveBlocks = false;
                    //Remove Selected Blocks
                    buildingService.getSelectedCoordinates(true).ForEach(c => buildingService.removeBlock(c));
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
            }
        }

        private void updateSelectedBlockInfo()
        {
            selectedBlockType = controlPlayer.buildingMaterial;

            if (controlPlayer.buildingMaterial.isTransparent() && controlPlayer.buildingMaterial.getVariations() != null)
            {
                selectedBlockData = controlPlayer.buildingVariations[controlPlayer.buildingVariationIndex][0];
            }
            else selectedBlockData = null;
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
                if (isSelecting)
                {
                    Label(controlPlayer.designType + " Select");

                    if (isMining)
                    {
                        Label(getKeyString(RAISE_SELECTION_KEY) + " Raise Selection");
                        Label(getKeyString(LOWER_SELECTION_KEY) + " Lower Selection");
                        Label(getKeyString(PRIMARY_KEY) + " Mine Selection");
                    }
                    else if (isBuilding)
                    {
                        Label(getKeyString(RAISE_SELECTION_KEY) + " Raise Selection");
                        Label(getKeyString(LOWER_SELECTION_KEY) + " Lower Selection");
                        Label(getKeyString(PRIMARY_KEY) + " Replace Selection");
                        Label("Left Click To Build");
                    }
                    else if (isChopping)
                    {
                        Label("Left Click To Chop");
                    }
                }
                else if (isDesigning)
                {
                    Label(controlPlayer.designType + " Design");

                    if (isBuilding)
                    {
                        Label(getKeyString(PRIMARY_KEY) + " Select Block At Mouse");
                        Label("(Non-Buildable Blocks)");
                    }
                }
            }

            BeginScrollView(scrollViewContainer, scrollContainer, ref scrollPosition);

            if (isCreativeEnabled)
            {
                if (isSelecting)
                {
                    
                }
                else if (isDesigning)
                {
                    if (isBuilding)
                    {
                        foreach (BlockProperties properties in availableBlockTypes)
                        {
                            if(Button(properties.getName()))
                            {
                                selectBlockProperties(properties);
                                break;
                            }
                        }
                    }
                    else if (isChopping)
                    {
                        Button("Remove All Trees", ref doRemoveAllTrees);
                    }
                }
                else
                {
                    Button(getKeyString(PRIMARY_KEY) + " Mine", ref doMineStart);
                    Button(getKeyString(CHOP_KEY) + " Chop", ref doChopStart);

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

        private void selectBlockProperties(BlockProperties properties)
        {
            controlPlayer.buildingMaterial = properties;
            controlPlayer.buildTile = properties.getID();
            controlPlayer.buildingVariations = properties.getVariations();
            controlPlayer.buildingVariationIndex = 0;
        }
    }
}
