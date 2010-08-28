namespace Jellyfish.Virtu
{
#if WINDOWS || XBOX
    static class MainApp
    {
        static void Main()
        {
            using (var game = new MainGame())
            {
                game.Run();
            }
        }
    }
#endif
}
