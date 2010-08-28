namespace Jellyfish.Virtu
{
    public sealed class NoSlotClock
    {
        public NoSlotClock()
        {
        }

        public int Read(int address, int data)
        {
            return data;
        }

        public void Write(int address)
        {
        }
    }
}
