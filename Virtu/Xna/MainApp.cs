namespace Jellyfish.Virtu
{
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
}
