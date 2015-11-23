using System;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Components
{
    public abstract class ModComponent : MonoBehaviour
    {
        public static void log(String message) { guiManager.AddTextLine(message); }
        public static void log<T>(T obj) { log(obj.ToString()); }

        public static Component addComponent(Type componentType)
        {
            return guiManager.gameObject.AddComponent(componentType);
        }        

        public const float MAIN_MENU_HEADER_HEIGHT = 44f;
        public const float WINDOW_TITLE_HEIGHT = 32f;
        public const float LABEL_HEIGHT = 36f;
        public const float TUTORIAL_MENU_WIDTH = 64f;
        public const float SCROLL_BAR_SIZE = 16f;
        public const float BUTTON_PADDING = 8f;
        public const float DOUBLE_PADDING = BUTTON_PADDING * 2f;
        public const float BUTTON_WIDTH = 270f;
        public const float BUTTON_HEIGHT = 32f;
        public const float BUTTON_SIZE = 20f;

        private const long DEFAULT_UPDATES_PER_SECOND = 5;
        private const long TICKS_PER_SECOND = 10000000;

        protected static readonly GUIManager guiManager = GUIManager.getInstance();

        protected readonly ModSettings modSettings = ModSettings.getInstance();
        protected readonly TimeManager timeManager = TimeManager.getInstance();
        protected readonly ControlPlayer controlPlayer = guiManager.controllerObj.GetComponent<ControlPlayer>();

        private const float START_X = BUTTON_PADDING;
        private const float START_Y_SCROLL = BUTTON_PADDING;
        private const float START_Y_WINDOW = START_Y_SCROLL + WINDOW_TITLE_HEIGHT;
        private static readonly float MAX_CONTAINER_HEIGHT = Screen.height * 0.7f;

        protected Vector2 scrollPosition = Vector2.zero;
       
        protected bool isComponentVisible = true;
        protected bool isMouseHover = false;
               
        protected bool isGameRunning { get { return guiManager.inGame && !guiManager.gameOver; } }
        protected static bool isMouseInGui { get { return guiManager.mouseInGUI; } }
        
        private long updatesPerSecond = DEFAULT_UPDATES_PER_SECOND;
        private long previousTicks = DateTime.Now.Ticks;
        protected void setUpdatesPerSecond(long updatesPerSecond)
        {
            this.updatesPerSecond = updatesPerSecond > 0 ? updatesPerSecond : DEFAULT_UPDATES_PER_SECOND;
        }

        protected bool isTimeToUpdate(long ticks)
        {
            if (previousTicks + TICKS_PER_SECOND / updatesPerSecond <= ticks)
            {
                previousTicks = ticks;
            }

            return previousTicks == ticks;
        }       

        private Vector2 mousePosition = Vector2.zero;
        protected Vector2 translateMouse()
        {
            mousePosition.Set(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            return mousePosition;
        }
        
        protected bool updateMouseForUI(Rect container)
        {
            return container.Contains(mousePosition) ? guiManager.mouseInGUI = true : false;
        }

        protected bool Button(float x, float y, String text)
        {
            return guiManager.DrawButton(getRectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), text);
        }

        protected bool Button(float x, float y, String text, ref bool doOnClick)
        {
            return Button(x, y, text) ? (doOnClick = true) : doOnClick;          
        }
        
        protected bool CheckBox(float x, float y, String text, ref bool toggled)
        {
            return guiManager.DrawCheckBox(getRectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), text, ref toggled);
        }

        protected bool CheckBox(float x, float y, String text, ref bool toggled, ref bool doOnClick)
        {
            return CheckBox(x, y, text, ref toggled) ? (doOnClick = true) : doOnClick;
        }

        protected Rect createWindow(int id, Rect parentContainer, GUI.WindowFunction func)
        {
            return GUI.Window(id, parentContainer, func, String.Empty, guiManager.windowBoxStyle);
        }       

        protected void Window(Rect parentContainer, String title, ref bool isVisible)
        {
            guiManager.DrawWindow(getRectangle(0, 0, parentContainer.width, parentContainer.height), title, false);
            CloseWindowButton(parentContainer.width - (BUTTON_SIZE + BUTTON_PADDING), BUTTON_PADDING, ref isVisible);
        }

        protected void Label(float x, float y, String text)
        {
            guiManager.DrawTextCenteredWhite(getRectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT), text);
        }        

        protected void LargeLabel(float x, float y, float width, float height, String text)
        {
            guiManager.DrawLargeTextCenteredWhite(getRectangle(x, y, width, height), text);
        }

        protected bool CloseWindowButton(float x, float y)
        {
            return GUI.Button(getRectangle(x, y, BUTTON_SIZE, BUTTON_SIZE), String.Empty, guiManager.closeWindowButtonStyle);
        }

        protected void CloseWindowButton(float x, float y, ref bool referenceVariable)
        {
            if (CloseWindowButton(x, y))
            {
                referenceVariable = false;
            }
        }        

        private Rect locationRectangle = new Rect();
        private Rect getRectangle(float x, float y, float width, float height)
        {
            locationRectangle.x = x;
            locationRectangle.y = y;
            locationRectangle.width = width;
            locationRectangle.height = height;
            return locationRectangle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentContainer"></param>
        /// <param name="title"></param>
        protected void Window(Rect parentContainer, String title)
        {
            windowControlIndex = 0;
            scrollControlIndex = 0;
            guiManager.DrawWindow(getRectangle(0, 0, parentContainer.width, parentContainer.height), title, false);
        }

        protected int currentControlIndex { get { return windowControlIndex + scrollControlIndex; } }
        private int windowControlIndex = 0;
        protected float getNextWindowControlYPosition()
        {
            return START_Y_WINDOW + BUTTON_HEIGHT * windowControlIndex++;
        }

        private int scrollControlIndex = 0;
        protected float getNextScrollControlYPosition()
        {
            return START_Y_SCROLL + BUTTON_HEIGHT * scrollControlIndex++;
        }

        private float getNextControlYPosition()
        {
            return isScrollView ? getNextScrollControlYPosition() : getNextWindowControlYPosition();
        }               

        protected void Label(String text)
        {
            guiManager.DrawTextCenteredWhite(getRectangle(START_X, getNextControlYPosition(), BUTTON_WIDTH, BUTTON_HEIGHT), text);
        }

        protected bool Button(String text)
        {
            return Button(START_X, getNextControlYPosition(), text);
        }

        protected bool Button(String text, ref bool doOnClick)
        {
            return Button(START_X, getNextControlYPosition(), text) ? (doOnClick = true) : doOnClick;
        }

        protected bool CheckBox(String text, ref bool toggled)
        {
            return guiManager.DrawCheckBox(getRectangle(START_X, getNextControlYPosition(), BUTTON_WIDTH, BUTTON_HEIGHT), text, ref toggled);
        }

        protected bool CheckBox(String text, ref bool toggled, ref bool doOnClick)
        {
            return CheckBox(text, ref toggled) ? (doOnClick = true) : doOnClick;           
        }

        private bool isScrollView = false;
        protected Vector2 BeginScrollView(Rect scrollViewContainer, Rect scrollContainer, ref Vector2 scrollPosition)
        {
            isScrollView = true;
            scrollViewContainer.y = START_Y_WINDOW + BUTTON_HEIGHT * windowControlIndex;
            return scrollPosition = GUI.BeginScrollView(scrollViewContainer, scrollPosition, scrollContainer);
        }

        protected void EndScrollView(ref Rect parentContainer, ref Rect scrollContainer, ref Rect scrollViewContainer, float originalWidth)
        {
            isScrollView = false;

            parentContainer.height = WINDOW_TITLE_HEIGHT + BUTTON_HEIGHT * (windowControlIndex + scrollControlIndex) + DOUBLE_PADDING;
            scrollContainer.height = BUTTON_HEIGHT * scrollControlIndex;

            if (parentContainer.height > MAX_CONTAINER_HEIGHT)
            {
                parentContainer.x = Screen.width - originalWidth - BUTTON_PADDING;
                parentContainer.height = MAX_CONTAINER_HEIGHT;
                scrollViewContainer.height = parentContainer.height - WINDOW_TITLE_HEIGHT - BUTTON_HEIGHT * windowControlIndex - DOUBLE_PADDING;
            }
            else
            {
                parentContainer.x = Screen.width - originalWidth - BUTTON_PADDING + SCROLL_BAR_SIZE;
                scrollViewContainer.height = parentContainer.height - BUTTON_HEIGHT * windowControlIndex - DOUBLE_PADDING;
            }

            GUI.EndScrollView();
        }
    }
}
