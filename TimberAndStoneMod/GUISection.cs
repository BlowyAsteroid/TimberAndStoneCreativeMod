using Plugin.BlowyAsteroid.TimberAndStoneMod.Utils;
using System;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public class GUISection
    {
        public enum TextColor { BLACK, WHITE }
        public enum FlowDirection { VERTICAL, HORIZONTAL }
        public enum Overflow { HIDDEN, SCROLL, WRAP }
        
        public const float DEFAULT_CONTROL_WIDTH = 256f;
        public const float DEFAULT_CONTROL_HEIGHT = 32f;
        public const float DEFAULT_CONTROL_MARGIN = 16f;
        public const float DEFAULT_CONTROL_PADDING = 4f;
        
        public FlowDirection Direction { get; set; }
        public Overflow Flow { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float ControlPadding { get; set; }
        public float ControlMargin { get; set; }
        public float ControlWidth { get; set; }
        public float ControlHeight { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public bool isHasBegun { get; private set; }
        public bool isScrolling { get; private set; }
        
        public bool hasChildren { get { return this.currentXIndex > 0 || this.currentYIndex > 0; } }

        private int currentXIndex { get; set; }
        private int currentYIndex { get; set; }

        private Rect scrollContainer = new Rect();
        private Rect scrollViewContainer = new Rect();
        private Vector2 scrollPosition = Vector2.zero;

        private readonly GUIManager guiManager = GUIManager.getInstance();

        public GUISection()
        {
            this.ControlPadding = DEFAULT_CONTROL_PADDING;
            this.ControlMargin = DEFAULT_CONTROL_MARGIN;
            this.ControlWidth = DEFAULT_CONTROL_WIDTH;
            this.ControlHeight = DEFAULT_CONTROL_HEIGHT;
            this.Direction = GUISection.FlowDirection.VERTICAL;
            this.Flow = GUISection.Overflow.SCROLL;
        }

        private const float SCROLL_BAR_SIZE = 20f;
        public void Begin(float startX, float startY, float width, float height)
        {
            this.X = startX + this.ControlMargin;
            this.Y = startY + this.ControlMargin;
            this.Width = width - this.ControlMargin * 2;
            this.Height = height - this.ControlMargin * 2; ;
            this.currentXIndex = 0;
            this.currentYIndex = 0;

            if (this.Flow == Overflow.SCROLL || this.Flow == Overflow.WRAP)
            {
                if (!this.isHasBegun)
                {
                    this.ResetScrollContainer();
                }

                this.scrollViewContainer.Set(this.X, this.Y, this.Width, this.Height);
                scrollPosition = GUI.BeginScrollView(scrollViewContainer, scrollPosition, scrollContainer);
            }

            this.isScrolling = false;
            this.isHasBegun = true;
        }

        public void ResetScrollContainer()
        {
            this.currentXIndex = 0;
            this.currentYIndex = 0;
            this.scrollContainer.Set(this.X, this.Y, this.Width, this.Height);
            this.scrollPosition = Vector2.zero;
        }

        public void End()
        {
            if (this.isHasBegun)
            {
                if (this.Flow == Overflow.SCROLL || this.Flow == Overflow.WRAP)
                {
                    if (!this.isScrolling)
                    {
                        switch (this.Direction)
                        {
                            case FlowDirection.HORIZONTAL:
                                this.Width = this.ControlXPosition - this.X;
                                break;

                            case FlowDirection.VERTICAL:
                                this.Height = this.ControlYPosition - this.Y;
                                break;
                        }
                    }

                    if (!this.hasChildren)
                    {
                        this.ResetScrollContainer();
                    }

                    GUI.EndScrollView();
                }
            }
        }

        public void Background(GUIStyle style)
        {
            GUI.Box(RectangleUtils.get(this.X, this.Y, this.Width, this.Height, this.ControlPadding), String.Empty, style);
        }

        public void LabelCentered(String text, TextColor color = TextColor.WHITE)
        {
            switch (color)
            {
                case TextColor.WHITE:
                    guiManager.DrawTextCenteredWhite(this.getControlRect(), text);
                    break;
                    
                case TextColor.BLACK:
                    guiManager.DrawTextCenteredBlack(this.getControlRect(), text);
                    break;
            }
            
            updateControlIndexes();
        }

        public void LabelLeft(String text, TextColor color = TextColor.WHITE)
        {
            switch (color)
            {
                case TextColor.WHITE:
                    guiManager.DrawTextLeftWhite(this.getControlRect(), text);
                    break;

                case TextColor.BLACK:
                    guiManager.DrawTextLeftBlack(this.getControlRect(), text);
                    break;
            }
            
            updateControlIndexes();
        }

        public bool Button(String text)
        {
            return updateControlIndexes(guiManager.DrawButton(this.getControlRect(), text));
        }

        public bool Button(String text, ref bool doOnClick)
        {
            return updateControlIndexes(guiManager.DrawButton(this.getControlRect(), text) ? (doOnClick = true) : doOnClick);            
        }

        public bool CheckBox(String text, ref bool toggled)
        {
            return updateControlIndexes(guiManager.DrawCheckBox(this.getControlRect(), text, ref toggled));            
        }

        public bool CheckBox(String text, ref bool toggled, ref bool doOnClick)
        {
            return updateControlIndexes(guiManager.DrawCheckBox(this.getControlRect(), text, ref toggled) ? (doOnClick = true) : doOnClick);
        }

        private float tempFloatValue;
        public void addSection(GUISection section)
        {
            if (this.Direction == FlowDirection.VERTICAL)
            {
                tempFloatValue = this.ControlYPosition;
                while (this.ControlYPosition < tempFloatValue + section.Height) this.currentYIndex++;
                this.Y += this.ControlPadding;
            }
            else if (this.Direction == FlowDirection.HORIZONTAL)
            {
                tempFloatValue = this.ControlXPosition;
                while (this.ControlXPosition < tempFloatValue + section.Width) this.currentXIndex++;
                this.X += this.ControlPadding;
            }
        }

        private GUINumberSelect numberSelect = new GUINumberSelect();
        public bool NumberSelect(String label, float value, out float newValue, float min = 0, float max = 100, float increment = 1f, bool showMinMax = false)
        {
            numberSelect.Name = label;
            numberSelect.Value = value;
            numberSelect.Min = min;
            numberSelect.Max = max;
            numberSelect.Increment = increment;

            newValue = value;

            if (updateControlIndexes(numberSelect.Draw(this.getControlRect(), showMinMax)))
            {
                newValue = numberSelect.Value;
                return true;
            }
               
            return false;           
        }

        private void updateScrollContainer(float width, float height)
        {
            this.scrollContainer.Set(this.X, this.Y, width, height);
        }

        private bool updateControlIndexes(bool flag = false)
        {
            if (this.Direction == FlowDirection.VERTICAL)
            {
                this.currentYIndex++;

                switch (this.Flow)
                {
                    case Overflow.WRAP:
                        if (this.ControlYPosition + this.ControlHeight > this.Y + this.Height)
                        {
                            this.currentYIndex = 0;
                            this.currentXIndex++;

                            if (this.ControlXPosition + this.ControlWidth > this.Width)
                            {
                                this.isScrolling = true;     
                            }

                            updateScrollContainer(this.ControlXPosition - this.X, this.scrollViewContainer.height - SCROLL_BAR_SIZE);
                        }

                        break;

                    case Overflow.SCROLL:
                        if (this.ControlYPosition > this.Y + this.Height)
                        {
                            this.isScrolling = true;                                
                        }

                        updateScrollContainer(this.scrollViewContainer.width - SCROLL_BAR_SIZE, this.ControlYPosition - this.Y);

                        break;
                }
               
            }
            else if (this.Direction == FlowDirection.HORIZONTAL)
            {
                this.currentXIndex++;

                switch (this.Flow)
                {
                    case Overflow.WRAP:
                        if (this.ControlXPosition + this.ControlWidth > this.X + this.Width)
                        {
                            this.currentXIndex = 0;
                            this.currentYIndex++;

                            if (this.ControlYPosition + this.ControlHeight > this.Height)
                            {
                                this.isScrolling = true;
                            }

                            updateScrollContainer(this.scrollViewContainer.width - SCROLL_BAR_SIZE, this.ControlYPosition - this.Y);
                        }

                        break;

                    case Overflow.SCROLL:
                        if (this.ControlXPosition > this.X + this.Width)
                        {
                            this.isScrolling = true;
                        }

                        updateScrollContainer(this.ControlXPosition - this.X, this.scrollViewContainer.height - SCROLL_BAR_SIZE);

                        break;
                }  
            }

            return flag;
        }
        
        public float ControlYPosition
        {
            get { return this.Y + this.currentYIndex * (this.ControlHeight + this.ControlPadding); }
        }

        public float ControlXPosition
        {
            get { return this.X + this.currentXIndex * (this.ControlWidth + this.ControlPadding);}
        }

        private Rect getControlRect() 
        {
            return RectangleUtils.get(this.ControlXPosition, this.ControlYPosition, this.ControlWidth, this.ControlHeight); 
        }
    }
}
