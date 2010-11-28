namespace Jellyfish.Virtu.Services
{
    public abstract class VideoService : MachineService
    {
        protected VideoService(Machine machine) : 
            base(machine)
        {
        }

        public abstract void SetFullScreen(bool isFullScreen);
        public abstract void SetPixel(int x, int y, uint color);
        public abstract void Update(); // main thread
    }
}
