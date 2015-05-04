using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

/// <summary>
/// provides formation classes (with individual attributes)
/// </summary>
public class Formation
{
	/// <summary>
	/// constructor, creates formation object with formation type, and formation and unit scale.
	/// </summary>
	public Formation(Formations f, float width, Vector3 unitScale)
	{
		formation = f;
		scale.x = width;
		this.unitScale = unitScale;
	}

	public enum Formations
	{
		Grid,
		None,
		Wedge,
	}
	public Formations formation;

	private Vector3 scale = new Vector3(0, 0, 0);
	private Vector3 unitScale = new Vector3(1, 1, 1);

	public Vector3 GetFormationScale()
	{
		return scale;
	}

	public Vector3 GetUnitScale()
	{
		return unitScale;
	}

	/// <summary>
	/// gets local normalized positions of units in the formation, compared to the formation origin, which is decided per-formation.
	/// </summary>
	/// <param name="numUnits">number of units to format</param>
	/// <param name="formationWidthInUnits">widthInMeters of formation, if it makes sense</param>
	/// <returns>list of relative positions, ordered by columns/rows (text writing direction)</returns>
	public List<Vector3> GetRelativePositions(int numUnits, int formationWidthInUnits = -1)
	{
		List<Vector3> posList = new List<Vector3>();
		switch (formation) {
		case Formations.Grid:

			int w, h, rest;
			if (formationWidthInUnits == -1) {
				w = Mathf.FloorToInt(Mathf.Sqrt(numUnits));
			} else {
				w = formationWidthInUnits;
			}
			w = Mathf.Clamp(w, 1, numUnits);
			h = Mathf.FloorToInt(numUnits / w);
			rest = numUnits - w * h;
			for (int j = 0; j < h; j++) {
				for (int i = 0; i < w; i++) {
					posList.Add(new Vector3(-0.5f + i / (float)Mathf.Clamp((w - 1), 1, float.MaxValue), 0, j / (float)h));

				}
			}
			for (int i = 0; i < rest; i++) {
				posList.Add(new Vector3(-0.5f + i / (float)Mathf.Clamp((w - 1), 1, float.MaxValue) + (w - rest) / (float)2 / (float)Mathf.Clamp((w - 1), 1, float.MaxValue), 0, 1));
			}
			scale.z = h * unitScale.z;

			break;
		case Formations.Wedge:
			var rowLength = 1;
			var row = 0;
			var col = 0;
			for (int i = 0; i < numUnits; i++) {
				posList.Add(new Vector3(-rowLength / 2f + row / rowLength, 0, col));
				row++;
				if (row % rowLength == 0) {
					row = 0;
					rowLength++;
					col++;
				}
			}
			scale.z = col * unitScale.z;
			scale.x = rowLength;
			break;
		default:
			// nothing returned. units will keep their old positions.
			// scale is just left alone.
			break;
		}

		return posList;
	}

	/// <summary>
	/// transforms each entry of targetUnitPositions by the formation transform. formationTransform contains world position of formation, world rotation and world widthInMeters.
	/// </summary>
	public List<Vector3> AbsolutePositions(List<Vector3> relativePositions, Matrix4x4 formationTransform)
	{
		var absPoss = new List<Vector3>();
		for (int i = 0; i < relativePositions.Count; i++) {
			absPoss.Add(formationTransform.MultiplyPoint3x4(relativePositions[i]));

		}
		return absPoss;
	}

	/// <summary>
	/// maps targetPositions to units
	/// </summary>
	/// <param name="targetUnitPositions"></param>
	/// <param name="targetPositions"></param>
	/// <returns></returns>
	public List<Vector3> AssignedPositions(List<Vector3> unitPositions, List<Vector3> targetPositions)
	{

		// from the furthest target position to the closest
		Vector3 sumUnitPositions = Vector3.zero;
		for (int i = 0; i < unitPositions.Count; i++) {
			sumUnitPositions += unitPositions[i];
		}
		var averageUnitPositions = sumUnitPositions / unitPositions.Count;

		Vector3 sumTargetPositions = Vector3.zero;
		for (int i = 0; i < targetPositions.Count; i++) {
			sumTargetPositions += targetPositions[i];
		}
		var averageTargetPositions = sumTargetPositions / targetPositions.Count;

		var unitTravelVector = averageTargetPositions - averageUnitPositions;

		List<Vector3> orderedUnitPos = new List<Vector3>();
		for (int unitIndex = 0; unitIndex < unitPositions.Count; unitIndex++) {
			var uPos = unitPositions[unitIndex];
			// for each unit, ordered by distance to average unit position, find closest position between unitPos + deltaUnitTargetAverage and ListTargetPos.
			var closestPos = targetPositions.Aggregate((t1, t2) => ((uPos + unitTravelVector) - t1).sqrMagnitude < ((uPos + unitTravelVector) - t2).sqrMagnitude ? t1 : t2);
			targetPositions.Remove(closestPos);
			orderedUnitPos.Add(closestPos);

		}

		return orderedUnitPos;

		// helper algorithms

		// find distance from each unit to its target position
		float totalDist = 0;
		for (int totalDistCount = 0; totalDistCount < unitPositions.Count; totalDistCount++) {
			totalDist += (unitPositions[totalDistCount] - targetPositions[totalDistCount]).sqrMagnitude;
		}

		// map a unit 0'unit position to the closest target position to it
		int closestTarget = 0;
		float closestDist = float.MaxValue;
		for (int i = 0; i < targetPositions.Count; i++) {
			var dist = (targetPositions[i] - unitPositions[0]).sqrMagnitude;
			if (dist < closestDist) {
				closestDist = dist;
				closestTarget = i;
			}
		}

	}

	/// <summary>
	/// returns a list of final positions for units, provided the required data about units and formation.
	/// </summary>
	/// <param name="unitsWithPositions">list of transforms of units to be moved. they must contain their current position.</param>
	/// <param name="formationWidthInUnits">how many units form the formation's width</param>
	/// <param name="formationTransform">how is the formation positioned in the world</param>
	/// <returns>list of assigned positions for units, in the given order in unitsWithPositions</returns>
	public List<Vector3> GetFinalPositions(List<Transform> unitsWithPositions, int formationWidthInUnits, Matrix4x4 formationTransform)
	{
		var r = GetRelativePositions(unitsWithPositions.Count, formationWidthInUnits);
		var a = AbsolutePositions(r, formationTransform);
		var ass = AssignedPositions(unitsWithPositions.Select(t => t.position).ToList(), a);
		return ass;
	}
}