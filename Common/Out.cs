using System;
using System.Collections.Generic;

namespace PixelWorld;

public static class Out
{
    private static readonly List<Action<String>> logTargets = [];
    private static readonly Object lockObj = new();

    public static void Write(String output) {
        lock (lockObj)
            foreach (var logTarget in logTargets)
                logTarget(output);
    }

    public static void Attach(Action<String> logTarget) {
        lock (lockObj)
            logTargets.Add(logTarget);
    }
}