using System;
using System.Collections.Generic;

namespace PixelWorld;

public static class Out
{
    private static readonly List<Action<String>> logTargets = [];

    public static void Write(String output) {
        foreach (var logTarget in logTargets)
            logTarget(output);
    }

    public static void Attach(Action<String> logTarget) {
        logTargets.Add(logTarget);
    }
}