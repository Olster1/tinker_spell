using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyMath
{

	public static Vector3 SmoothStepV3(Vector3 a, Vector3 b, float t) {
		Vector3 result = new Vector3();
		result.x = Mathf.SmoothStep(a.x, b.x, t);
		result.y = Mathf.SmoothStep(a.y, b.y, t);
		result.z = Mathf.SmoothStep(a.z, b.z, t);

		return result;
	}	
}
