using Plugin.BlowyAsteroid.TimberAndStoneMod.Utils;
using System;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public class GUINumberSelect
    {
        private const float BUTTON_SIZE = 20f;

        public String Name { get; set; }
        public float Value { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }
        public float Increment { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        private readonly GUIManager guiManager = GUIManager.getInstance();
        
        public GUINumberSelect(String name = "", float value = 0f, float min = 0f, float max = 100f, float increment = 1f)
        {
            this.Name = name;
            this.Value = value;
            this.Min = min;
            this.Max = max;
            this.Increment = increment;
        }

        public bool Draw(Rect container, bool showMinMax = false)
        {
            return this.Draw(container.x, container.y, container.width, container.height, showMinMax);
        }

        private float tempX, tempWidth;
        private bool isValueChanged;
        public bool Draw(float x, float y, float width, float height, bool showMinMax = false)        
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;

            isValueChanged = false;
            tempX = x;
            tempWidth = 0;

            if (this.Name.Trim() != String.Empty)
            {
                tempWidth = this.Name.Length * 10;
                guiManager.DrawTextLeftBlack(RectangleUtils.get(tempX, y, tempWidth), this.Name);
                tempX += tempWidth;
            }

            if (showMinMax)
            {
                if (GUI.Button(RectangleUtils.get(tempX, y, BUTTON_SIZE * 2, BUTTON_SIZE), "Min", guiManager.skin.button) && this.Value > this.Min)
                {
                    this.Value = this.Min;
                    isValueChanged = true;
                }

                tempX += BUTTON_SIZE * 2;
                tempWidth += BUTTON_SIZE * 4;
            }

            if (GUI.Button(RectangleUtils.get(tempX, y, BUTTON_SIZE), "-", guiManager.skin.button) && this.Value > this.Min)
            {
                if ((this.Value -= this.Increment) < this.Min) this.Value = this.Min;
                isValueChanged = true;
            }      

            tempX += BUTTON_SIZE;

            tempWidth = width - BUTTON_SIZE * 2 - tempWidth;
            guiManager.DrawTextCenteredBlack(RectangleUtils.get(tempX, y, tempWidth), this.Value.ToString());
            tempX += tempWidth;

            if (GUI.Button(RectangleUtils.get(tempX, y, BUTTON_SIZE), "+", guiManager.skin.button) && this.Value < this.Max)
            {
                if ((this.Value += this.Increment) > this.Max) this.Value = this.Max;
                isValueChanged = true;
            }

            tempX += BUTTON_SIZE;

            if (showMinMax)
            {
                if (GUI.Button(RectangleUtils.get(tempX, y, BUTTON_SIZE * 2, BUTTON_SIZE), "Max", guiManager.skin.button) && this.Value < this.Max)
                {
                    this.Value = this.Max;
                    isValueChanged = true;
                }
            }

            return isValueChanged;
        }
    }
}
