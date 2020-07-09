﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
	public static float sphereLegRadius = 0.08f;
	public static float pixelsPerUnitLegs = 250f;
	public static float legSphereImageScale = 1.25f;
	public static float fingerDrawSpriteScale = 1.5f;
	public static int pixelPerFingerSample = 10;
	public static Color drawTrailColor = new Color(0.2745098f, 0.2745098f, 0.2745098f);
	public static Color legsColor = new Color(0.2745098f, 0.2745098f, 0.2745098f);
}

// export function ruleOfFive(inputDataA: number, outputDataA: number, inputDataB: number, outputDataB: number, input: number, clamp: boolean): number {
// 	const t: number = (input - inputDataA) / (inputDataB - inputDataA);
// 	return outputDataA + (outputDataB - outputDataA) * (clamp ? Math.min(Math.max(t, 0), 1) : t);
// }