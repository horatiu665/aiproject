using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(UnitSelectionModule))]
public class UnitCommandModule : MonoBehaviour
{
	private UnitSelectionModule sel;
	public Transform test;
	public float rayDur = 1;

	public Formation.Formations formation;

	public ParticleSystem unitPositionPreviewParticles;

	public Transform ground;

	public Vector3 unitScale = new Vector3(1, 1, 1);

	private bool inputCanceled = false;

	void Start()
	{
		sel = GetComponent<UnitSelectionModule>();
		AssignEvents(null);
		
	}

	void AssignEvents(RtsInputModule.InputData data)
	{
		RtsInputModule.RightMouseUp += RtsInputModule_RightMouseUp;
		RtsInputModule.RightMouseMove += RtsInputModule_RightMouseMove;
	}

	void UnAssignEvents(RtsInputModule.InputData data)
	{
		RtsInputModule.RightMouseUp -= RtsInputModule_RightMouseUp;
		RtsInputModule.RightMouseMove -= RtsInputModule_RightMouseMove;
	}

	/// <summary>
	/// draws particles where the units will be moved if mouse was released.
	/// </summary>
	/// <param name="data"></param>
	void RtsInputModule_RightMouseMove(RtsInputModule.InputData data)
	{
		
		// if no units, cancel action
		if (sel.selection.Count == 0)
			return;

var targetUnitPositions = SetupFormationData(data);

		// no need to assign position to units, we are just previewing.
		//var curUnitPositions = movableUnits.Select(t => t.position).ToList();
		//targetUnitPositions = formation.AssignedPositions(curUnitPositions, targetUnitPositions);

		// use particle system to draw gizmo shit
		ParticleSystem.Particle[] p = new ParticleSystem.Particle[targetUnitPositions.Count];

		int unitIndex = 0;
		foreach (var unit in targetUnitPositions) {
			var upos = targetUnitPositions[unitIndex];
			ParticleSystem.Particle part = new ParticleSystem.Particle();
			part.position = PositionOnObject.GetPositionOnObject(upos + Vector3.up * 50, Vector3.down, ground);
			part.color = Color.red;
			part.size = 1f;
			p[unitIndex] = part;
			unitIndex++;
		}
		unitPositionPreviewParticles.SetParticles(p, p.Length);

	}

	void KillParticles()
	{
		// kill particles
		unitPositionPreviewParticles.SetParticles(new ParticleSystem.Particle[0], 0);
	}

	/// <summary>
	/// sends units towards formation positions based on calculations upon input data.
	/// </summary>
	/// <param name="data"></param>
	void RtsInputModule_RightMouseUp(RtsInputModule.InputData data)
	{
		KillParticles();
		
		// if no units, cancel action
		if (sel.selection.Count == 0)
			return;
		
		Formation formation;
		// units that will be moved
		IEnumerable<Transform> movableUnits;
		var targetUnitPositions = SetupFormationData(data, out formation, out movableUnits);

		var curUnitPositions = movableUnits.Select(t => t.position).ToList();
		targetUnitPositions = formation.AssignedPositions(curUnitPositions, targetUnitPositions);


		int unitIndex = 0;
		foreach (var unit in movableUnits) {
			SetTargetPosForUnit(unit, targetUnitPositions[unitIndex]);

			unitIndex++;
		}

	}

	List<Vector3> SetupFormationData(RtsInputModule.InputData data)
	{
		Formation form;
		IEnumerable<Transform> movableUnits;
		return SetupFormationData(data, out form, out movableUnits);
	}

	List<Vector3> SetupFormationData(RtsInputModule.InputData data, out Formation formation, out IEnumerable<Transform> movableUnits)
	{
		var worldData = data.ToWorld();

		// calculate placement data
		// ################################ PLACEMENT DATA ###########################################
		// target position, rotation and widthInMeters of the formation
		var pos = (worldData.startPos + worldData.endPos) / 2;
		var vectorForYAngleCalculation = (worldData.startPos - worldData.endPos);
		vectorForYAngleCalculation.Scale(new Vector3(1, 0, 1));
		// angle is only between 0-180, so we must check further for quadrant 3 and 4
		var yAngle = Vector3.Angle(Vector3.right, vectorForYAngleCalculation);
		if (worldData.startPos.z > worldData.endPos.z) {
			yAngle = 360 - yAngle;
		}
		var rot = Quaternion.Euler(0, yAngle, 0);

		//set widthInMeters in unity units, as well as in units units: distance/unitWidth = widthInMeters in units
		var widthInMeters = (worldData.endPos - worldData.startPos).magnitude;
		int widthInUnits = Mathf.FloorToInt(widthInMeters / unitScale.x);
		var unitZScale = unitScale.z;

		// set new formation
		formation = GetFormation(widthInMeters, unitScale);

		// units that will be moved
		movableUnits = sel.selection.Where(t => t.GetComponentInChildren<StateMoveToCharacterMotor>() != null || t.GetComponentInChildren<StateMoveToNavMesh>() != null);
		var targetUnitPositions = formation.GetRelativePositions(movableUnits.Count(), widthInUnits);
		var zScaleInMeters = formation.GetFormationScale().z;
		var formationTransformation = new Matrix4x4();
		formationTransformation.SetTRS(pos, rot, new Vector3(widthInMeters, 0, zScaleInMeters));
		targetUnitPositions = formation.AbsolutePositions(targetUnitPositions, formationTransformation);

		return targetUnitPositions;
	}

	// sends unit towards position.
	void SetTargetPosForUnit(Transform unit, Vector3 pos)
	{
		var moveTo = unit.GetComponentInChildren<StateMoveToCharacterMotor>();
		if (moveTo != null) {
			var ai = unit.GetComponent<PlantmanAI4.ControllerState>();
			moveTo.targetPosition = pos;
			ai.states.RemoveAll(s => (s.GetType()) == (moveTo.GetType()));
			ai.states.Add(moveTo);
			moveTo.Exit += (sender, args) => {
				ai.states.Remove(moveTo);
			};
		}
		var moveTo2 = unit.GetComponentInChildren<StateMoveToNavMesh>();
		if (moveTo2 != null) {
			var ai = unit.GetComponent<PlantmanAI4.ControllerState>();
			moveTo2.targetPosition = pos;
			ai.states.RemoveAll(s => (s.GetType()) == (moveTo2.GetType()));
			ai.states.Add(moveTo2);
			moveTo2.Exit += (sender, args) => {
				ai.states.Remove(moveTo2);
			};
		}
	}

	Formation GetFormation(float formationWidth, Vector3 unitScale)
	{
		return new Formation(formation, formationWidth, unitScale);
	}

	Vector3 GetPosOnGround(Vector3 mousePos)
	{
		return Physics.RaycastAll(Camera.main.ScreenPointToRay(mousePos))
			.Aggregate((r1, r2) => (r1.point - Camera.main.transform.position).sqrMagnitude > (r2.point - Camera.main.transform.position).sqrMagnitude ? r2 : r1).point;
	}

}
