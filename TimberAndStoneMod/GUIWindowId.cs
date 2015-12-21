
namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public sealed class GUIWindowId
    {
        public const int STARTING_WINDOW_ID = 1001;

        private static GUIWindowId instance = new GUIWindowId();        
        public static int Next { get { return instance.getNext(); } }

        private int currentWindowId = STARTING_WINDOW_ID;

        private GUIWindowId() { }

        public int getNext()
        {
            return this.currentWindowId++;
        }
    }
}
