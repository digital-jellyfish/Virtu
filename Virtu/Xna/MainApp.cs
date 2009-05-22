using System;

namespace Jellyfish.Virtu
{
    static class MainApp
    {
        static void Main()
        {
            using (MainGame game = new MainGame())
            {
                game.Run();
            }
        }
    }
}
