using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
	public static string getValueInName(string source, string key)
	{
		int indexof = source.IndexOf(key);
		if(indexof == -1) return "";
		return source.Remove(0, indexof + key.Length);
	}
}
