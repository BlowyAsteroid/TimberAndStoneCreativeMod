using System;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Components
{
    public class TimeComponent : ModComponent
    {
        private const int DAY_TIME_START = 8;
        private const int DAY_TIME_END = 17;
        private const float TIME_SCALE = .6f;

        public void Start() 
        {
            setUpdatesPerSecond(5);
        }

        private double tempMinutes;
        public void Update()
        {
            if (!isTimeToUpdate(DateTime.Now.Ticks)) return;

            if (modSettings.isAlwaysDaytimeEnabled)
            {
                if (timeManager.hour >= DAY_TIME_END || timeManager.hour < DAY_TIME_START)
                {
                    timeManager.time = timeManager.hour = DAY_TIME_START;
                }                
            }

            tempMinutes = ((timeManager.time - timeManager.hour) * 100) * TIME_SCALE;

            guiManager.UpdateTime(timeManager.day, getTimeString(timeManager.hour, tempMinutes));
        }        
        
        private String getTimeString(int hour, double minutes)
        {
            return String.Format("{0}:{1}{2}", getTwelveHour(hour), getLeadingZero((int)minutes), hour >= 12 ? "PM" : "AM");
        }

        private int getTwelveHour(int hour)
        {
            return hour <= 0 ? 12 : hour > 12 ? hour - 12 : hour;
        }

        private String getLeadingZero(int value)
        {
            return String.Format("{0}{1}", value > 9 ? String.Empty : "0", value);
        }
    }
}
