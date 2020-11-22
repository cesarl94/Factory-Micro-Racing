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

    public static float getInterpolatedValueInVectors(Vector2[] inputOutput, float input)
    {
        if (inputOutput.Length == 1)
        {
            return inputOutput[0].y;
        }

        for (int i = 0; i < inputOutput.Length - 1; i++)
        {
            Vector2 previous = inputOutput[i];
            Vector2 next = inputOutput[i + 1];

            if (input >= previous.x && input < next.x)
            {
                return Utils.ruleOfFive(previous.x, previous.y, next.x, next.y, input, false);
            }
        }

        return inputOutput[inputOutput.Length - 1].y;
    }

    public static Transform findNode(Transform parent, string name)
    {
        Transform findedNode = parent.Find(name);
        if (findedNode == null)
        {
            Debug.LogError("Error: \"" + name + "\" node wasn't finded");
            Debug.Break();
        }
        return findedNode;
    }

    public static int SortByNameValue(Transform p1, Transform p2, string key)
    {
        return int.Parse(Utils.getValueInName(p1.name, key)).CompareTo(int.Parse(Utils.getValueInName(p2.name, key)));
    }

}
