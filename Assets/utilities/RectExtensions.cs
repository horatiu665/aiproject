using UnityEngine;
using System.Collections;

public static class RectExtensions {

	public static Rect Add(this Rect r, Rect rect)
	{
		return new Rect(r.x + rect.x, r.y + rect.y, rect.width + r.width, rect.height + r.height);
	}
}
