using Plugin.BlowyAsteroid.TimberAndStoneMod.Services;
using System;
using System.Linq;
using Timber_and_Stone.Tasks;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Components
{
    public class FriendlyNpcComponent : ModComponent
    {        
        private UnitManager unitManager = UnitManager.getInstance();
        private WorldManager worldManager = WorldManager.getInstance();
        private static EquipmentService equipmentService = EquipmentService.getInstance();

        private const int PARENT_CONTAINER_ID = 103;
        private const float CONTAINER_WIDTH = BUTTON_WIDTH;
        private static readonly float CONTAINER_HEIGHT = WINDOW_TITLE_HEIGHT + BUTTON_HEIGHT;

        private Rect parentContainer = new Rect(Screen.width/2, Screen.height/2, CONTAINER_WIDTH + DOUBLE_PADDING, CONTAINER_HEIGHT + DOUBLE_PADDING);

        private Vector2 mouseDownPosition = Vector2.zero;
        private ALivingEntity selectedEntity;

        private MonoBehaviour selectedObject { get { return worldManager.PlayerFaction.selectedObject; } }

        private bool doRemoveNPC = false;

        public void Start()
        {
            setUpdatesPerMinute(30);

            isComponentVisible = false;
        }

        public void Update()
        {
            if (isComponentVisible)
            {
                translateMouse();
            }

            if (Input.GetKeyUp(KeyCode.Tab) || Input.GetKeyUp(KeyCode.Escape)
                || (Input.GetMouseButtonUp(Mouse.RIGHT) && !controlPlayer.cameraRotate))
            {
                selectedEntity = null;
                isComponentVisible = false;
            }

            if (Input.GetMouseButtonUp(Mouse.LEFT) && !isMouseInGui)
            {
                if (selectedObject != null)
                {
                    if (selectedEntity == null || selectedEntity != selectedObject.GetComponent<ALivingEntity>())
                    {
                        selectedEntity = selectedObject.GetComponent<ALivingEntity>();
                    }

                    if (UnitPreference.isFriendlyNPC(selectedEntity))
                    {
                        mouseDownPosition = translateMouse();
                        parentContainer.x = mouseDownPosition.x + BUTTON_PADDING;
                        parentContainer.y = mouseDownPosition.y + parentContainer.height;
                        isComponentVisible = true;
                    }
                    else selectedEntity = null;
                }
                else selectedEntity = null;
            }
            
            if (doRemoveNPC)
            {
                doRemoveNPC = false;

                if (selectedEntity != null)
                {
                    selectedEntity.Destroy();
                    selectedEntity = null;
                }

                isComponentVisible = false;
            }

            if (!isTimeToUpdate(DateTime.Now.Ticks)) return;

            foreach (ALivingEntity entity in unitManager.allUnits.Where(u => UnitPreference.isFriendlyNPC(u)))
            {
                if(!entity.taskStackContains(typeof(TaskAttack)) 
                    && !entity.taskStackContains(typeof(TaskAttackTarget)))
                {
                    entity.interruptTask(new TaskWait(10));
                }
                
                entity.spottedTimer = 15f;
                entity.hitpoints = entity.maxHP;
                entity.hunger = 0f;

                entity.inventory.Clear();
                equipmentService.equipNPCWeapons(entity, UnitPreference.isArcherNPC(entity));
            }
        }

        public static void updateNPCsNear(ALivingEntity entity)
        {
            if (entity == null) return;

            foreach (ALivingEntity unit in entity.getNearbyUnits(3f, false).Where(u => UnitPreference.isFriendlyNPC(u)))
            {
                if (unit == null) continue;

                unit.interruptTask(new TaskWait(10));

                unit.inventory.Clear();
                equipmentService.equipNPCWeapons(unit, UnitPreference.isArcherNPC(unit));
            }
        }


        public void OnGUI()
        {
            if (isGameRunning)
            {
                if (isComponentVisible)
                { 
                    parentContainer = createWindow(PARENT_CONTAINER_ID, parentContainer, drawWindow);
                    updateMouseForUI(parentContainer);
                }
            }
        }

        private void drawWindow(int id)
        {
            if (selectedEntity == null)
            {
                isComponentVisible = false;
                return;
            }

            Window(parentContainer, selectedEntity.unitName);

            Button("Remove NPC", ref doRemoveNPC);
        }
    }
}
