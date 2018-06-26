using System;
using System.Collections.Generic;

namespace PixelWorld
{
    public static class Out
    {
        private static readonly List<Action<string>> logTargets = new List<Action<string>>();

        public static void Write(string output) {
            foreach (var logTarget in logTargets)
                logTarget(output);
        }

        public static void Attach(Action<string> logTarget) {
            logTargets.Add(logTarget);
        }
    }
}
