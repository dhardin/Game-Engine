using System;

namespace Game {
    static class Program {
        static void Main (string[] args) {
            using (TopDownGame game = new TopDownGame())
            {
                game.Run();
            }
        }
    }
}

