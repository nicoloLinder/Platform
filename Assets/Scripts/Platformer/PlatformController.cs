using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController {

	//	VARS

		//	PUBLIC

	public LayerMask passengerMask;

	public Vector3[] localWaypoints;
	public Vector3[] globalWaypoints;

	public float speed;
	public bool cyclic;
	public float waitTime;

	[Range(0,3)]
	public float easeAmount;

		//	PRIVATE

	List<PassengerMovement> passengerMovement;
	Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();
	int fromWaypointIndex;
	float percentBetweenWaypoints;
	float nextMoveTime;

	//	GETTERS
	//	SETTERS

	//	FACTORY


	//	MONOBEHAIVIOUR

	public override void Start () {
		base.Start();

		globalWaypoints = new Vector3[localWaypoints.Length];
		for(int i = 0; i < localWaypoints.Length; i++) {
			globalWaypoints[i] = localWaypoints[i] + transform.position;
		}
	}
	
	void FixedUpdate () {
		UpdateRaycastOrigins();
		Vector3 velocity = CalculatePlatformMovement();

		CalculatePassengerMovement(velocity);

		MovePassengers(true);
		transform.Translate(velocity);
		MovePassengers(false);

	}

	void OnDrawGizmos() {
		if (localWaypoints != null) {
			Gizmos.color = Color.red;
			float size = 0.3f;
			for(int i = 0; i < localWaypoints.Length; i++) {
				Vector3 globalWaypointPos = (Application.isPlaying)?globalWaypoints[i]:localWaypoints[i] + transform.position;
				Debug.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
				Debug.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);

			}
		}
	}

	//	METHODS

		//	PUBLIC

		//	PRIVATE

	float Ease(float x) {
		float a = easeAmount + 1;
		return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1-x, a));
	} 

	Vector3 CalculatePlatformMovement() {
		if(Time.timeScale < nextMoveTime) {
			return Vector3.zero;
		}

		fromWaypointIndex %= globalWaypoints.Length;
		int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
		float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
		percentBetweenWaypoints += Time.deltaTime * speed/distanceBetweenWaypoints;
		percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);

		float easedPerceBetweenWaypoints = Ease(percentBetweenWaypoints);

		Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPerceBetweenWaypoints);

		if(percentBetweenWaypoints >= 1) {
			percentBetweenWaypoints = 0;
			fromWaypointIndex++;
			if(!cyclic) {
				if(fromWaypointIndex >= globalWaypoints.Length-1) {
					fromWaypointIndex = 0;
					System.Array.Reverse(globalWaypoints);
				}
			}
			nextMoveTime = Time.time + waitTime;
		}

		return newPos - transform.position;
	}

	void MovePassengers(bool beforeMovePlatform) {
		foreach(PassengerMovement passenger in passengerMovement) {
			if(!passengerDictionary.ContainsKey(passenger.passenger)) {
				passengerDictionary.Add(passenger.passenger, passenger.passenger.GetComponent<Controller2D>());
			}
			if (passenger.moveBeforePlatform == beforeMovePlatform) {
				passengerDictionary[passenger.passenger].Move(passenger.velocity, passenger.standingOnPlatform);
			}
		}
	}

	void CalculatePassengerMovement(Vector3 velocity) {
		HashSet<Transform> movedPassangers = new HashSet<Transform>(); 
		passengerMovement = new List<PassengerMovement>();

		float directionX = Mathf.Sign(velocity.x);
		float directionY = Mathf.Sign(velocity.y);

		//	Vertical
		if(velocity.y != 0) {
			float rayLength = Mathf.Abs (velocity.y) + skinWidth;

			for (int i = 0; i < verticalRayCount; i ++) {
				Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

				if(hit && hit.distance != 0) {
					if(!movedPassangers.Contains(hit.transform)){
						float pushX = (directionY == 0)?velocity.x : 0;
						float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
						movedPassangers.Add(hit.transform);
					}
				}
			} 
		}
		//	Horizontal
		if(velocity.x != 0) {
			float rayLength = Mathf.Abs (velocity.x) + skinWidth;
		
			for (int i = 0; i < horizontalRayCount; i ++) {
				Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
				rayOrigin += Vector2.up * (horizontalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

				if(hit && hit.distance != 0) {
					if(!movedPassangers.Contains(hit.transform)){
						float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
						float pushY = -skinWidth;

						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
						movedPassangers.Add(hit.transform);
					}
				}
			}
		}
		if(directionY == -1 || velocity.y == 0 && velocity.x != 0) {
			float rayLength = skinWidth * 2;

			for (int i = 0; i < verticalRayCount; i ++) {
				Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

				if(hit && hit.distance != 0) {
					if(!movedPassangers.Contains(hit.transform)){
						float pushX = velocity.x;
						float pushY = velocity.y;

						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
						movedPassangers.Add(hit.transform);
					}
				}
			} 
		}

	}

	struct PassengerMovement {
		public Transform passenger;
		public Vector3 velocity;
		public bool standingOnPlatform;
		public bool moveBeforePlatform;

		public PassengerMovement(Transform _passenger, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform) {
			passenger = _passenger;
			velocity = _velocity;
			standingOnPlatform = _standingOnPlatform;
			moveBeforePlatform = _moveBeforePlatform;
		}
	}



}