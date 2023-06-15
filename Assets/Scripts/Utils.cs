using UnityEngine;

public static class Utils
{
	public static float RoundTo(float value, float multipleOf)
	{
		return Mathf.Round(value / multipleOf) * multipleOf;
	}

	public static Vector2 RoundTo(Vector2 value, Vector2 multipleOf)
	{
		return new Vector2(RoundTo(value.x, multipleOf.x), RoundTo(value.y, multipleOf.y));
	}
}
