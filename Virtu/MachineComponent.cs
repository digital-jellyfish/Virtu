namespace Jellyfish.Virtu
{
    public abstract class MachineComponent
    {
        protected MachineComponent(Machine machine)
        {
            Machine = machine;
        }

        public virtual void Initialize()
        {
        }

        public virtual void Reset()
        {
        }

        public virtual void Uninitialize()
        {
        }

        protected Machine Machine { get; private set; }
    }
}
