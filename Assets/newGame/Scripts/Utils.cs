using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static string getValueInName(string source, string key)
    {
        int indexof = source.IndexOf(key);
        if (indexof == -1) return "";
        return source.Remove(0, indexof + key.Length);
    }

    public static float ruleOfFive(float inputDataA, float outputDataA, float inputDataB, float outputDataB, float input, bool clamp)
    {
        float t = (input - inputDataA) / (inputDataB - inputDataA);
        return outputDataA + (outputDataB - outputDataA) * (clamp ? Mathf.Min(Mathf.Max(t, 0), 1) : t);
    }
}
