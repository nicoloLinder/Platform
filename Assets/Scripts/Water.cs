using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{

	//	VARS

	//	PUBLIC

	public float width;
	public float baseHeight;
	public float maxDisplacement;

	//	PRIVATE

	Vector2 [] positions;
	float [] velocities;
	float [] accelerations;

	EdgeCollider2D waterSurface;

	WaterNode [] waterNodes;

	Mesh mesh;
	LineRenderer foamLine;

	//	CONST

	public float springconstant = 0.02f;
	public float damping = 0.1f;
	public float spread = 0.05f;
	public float z = -1f;



	// Use this for initialization
	void Start ()
	{
		InitializeWater ();
	}

	// Update is called once per frame

	void FixedUpdate ()
	{
		for (int i = 0; i < waterNodes.Length; i++) {
			float force = springconstant * (waterNodes [i].Position.y - baseHeight) + waterNodes [i].Velocity * damping;
			waterNodes [i].Acceleration = -force;
			waterNodes [i].Position += new Vector2 (0, waterNodes [i].Velocity);
			waterNodes [i].Velocity += waterNodes [i].Acceleration;
		}

		float [] leftDeltas = new float [waterNodes.Length];
		float [] rightDeltas = new float [waterNodes.Length];

		for (int j = 0; j < 8; j++) {
			for (int i = 0; i < waterNodes.Length; i++) {
				if (i > 0) {
					leftDeltas [i] = spread * (waterNodes [i].Position.y - waterNodes [i - 1].Position.y);
					waterNodes [i - 1].Velocity += leftDeltas [i];
				}
				if (i < waterNodes.Length - 1) {
					rightDeltas [i] = spread * (waterNodes [i].Position.y - waterNodes [i + 1].Position.y);
					waterNodes [i + 1].Velocity += rightDeltas [i];
				}
			}

			for (int i = 0; i < waterNodes.Length; i++) {
				if (i > 0) {
					waterNodes [i - 1].Position += new Vector2 (0, leftDeltas [i]);
				}
				if (i < waterNodes.Length - 1) {
					waterNodes [i + 1].Position += new Vector2 (0, rightDeltas [i]);
				}
			}
		}

		UpdateMesh ();
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		Splash (other.transform.position.x, other.GetComponent<Rigidbody2D> ().mass * other.GetComponent<Rigidbody2D> ().velocity.y / 40);
	}

	void OnDrawGizmos ()
	{
		if (waterNodes != null) {
			Gizmos.color = Color.blue;
			foreach (WaterNode waterNode in waterNodes) {
				Gizmos.DrawCube (transform.TransformPoint (waterNode.Position), Vector3.one * 0.1f);
			}
		}
	}

	public void InitializeWater ()
	{
		int edgeCount = Mathf.RoundToInt (width) * 5;
		int nodeCount = edgeCount + 1;

		waterSurface = gameObject.AddComponent<EdgeCollider2D> ();
		//waterSurface.points = new Vector2 [nodeCount];
		waterSurface.isTrigger = true;
		waterSurface.points = new Vector2 [] { new Vector2 (-width / 2f, baseHeight), new Vector2 (width / 2f, baseHeight) };

		foamLine = GetComponent<LineRenderer> ();


		waterNodes = new WaterNode [nodeCount];
		foamLine.positionCount = nodeCount;

		for (int i = 0; i < nodeCount; i++) {
			waterNodes [i] = new WaterNode (new Vector2 ((-width / 2f) + width * i / edgeCount, baseHeight), 0, 0);
			foamLine.SetPosition (i, transform.TransformPoint (waterNodes [i].Position));
		}

		mesh = new Mesh ();
		Vector3 [] vertices = new Vector3 [nodeCount * 2];
		Vector2 [] UVs = new Vector2 [nodeCount * 2];
		int [] tris = new int [(vertices.Length - 1) * 3];

		int triangleIndex = 0;

		for (int i = 0; i < nodeCount - 1; i++) {
			vertices [i] = waterNodes [i].Position;
			vertices [i + 1] = waterNodes [i + 1].Position;
			vertices [i + nodeCount] = new Vector3 (waterNodes [i].Position.x, 0);
			vertices [i + 1 + nodeCount] = new Vector3 (waterNodes [i + 1].Position.x, 0);

			UVs [i] = new Vector2 ((float)i / nodeCount, 1);
			UVs [i + 1] = new Vector2 ((float)(i + 1) / nodeCount, 1);
			UVs [i + nodeCount] = new Vector2 ((float)i / nodeCount, 0);
			UVs [i + nodeCount + 1] = new Vector2 ((float)(i + 1) / nodeCount, 0);

			tris [triangleIndex++] = i;
			tris [triangleIndex++] = i + nodeCount + 1;
			tris [triangleIndex++] = i + nodeCount;
			tris [triangleIndex++] = i + nodeCount + 1;
			tris [triangleIndex++] = i;
			tris [triangleIndex++] = i + 1;
		}
		mesh.vertices = vertices;
		mesh.uv = UVs;
		mesh.triangles = tris;

		GetComponent<MeshFilter> ().mesh = mesh;
	}

	void UpdateMesh ()
	{
		Vector3 [] vertices = mesh.vertices;
		int nodeCount = vertices.Length / 2;
		int vertexIndex = 0;
		for (int i = 0; i < waterNodes.Length - 1; i++) {
			if(waterNodes[i].Position.y > baseHeight + maxDisplacement){
				waterNodes [i].Position = new Vector2 (waterNodes [i].Position.x, baseHeight + maxDisplacement);
			}
			vertices [vertexIndex] = waterNodes [i].Position;
			vertices [vertexIndex + 1] = waterNodes [i + 1].Position;
			vertexIndex++;
			foamLine.SetPosition (i, transform.TransformPoint(waterNodes [i].Position));
			foamLine.SetPosition (i+1, transform.TransformPoint (waterNodes [i+1].Position));
		}

		mesh.vertices = vertices;
		GetComponent<MeshFilter> ().mesh = mesh;

	}

	public void Splash (float xPosition, float velocity)
	{

		if (xPosition >= waterNodes [0].Position.x && xPosition <= waterNodes [waterNodes.Length - 1].Position.x) {
			xPosition -= waterNodes [0].Position.x;

			int index = Mathf.RoundToInt ((waterNodes.Length - 1) * (xPosition / (waterNodes [waterNodes.Length - 1].Position.x - waterNodes [0].Position.x)));

			waterNodes [index].Velocity += velocity;
		}
	}

}

class WaterNode
{
	Vector2 position;
	float velocity;
	float acceleration;

	public Vector2 Position { get { return position; } set { position = value; } }
	public float Velocity { get { return velocity; } set { velocity = value; } }
	public float Acceleration { get { return acceleration; } set { acceleration = value; } }

	public WaterNode (Vector2 _position, float _velocity, float _acceleration)
	{
		position = _position;
		velocity = _velocity;
		acceleration = _acceleration;
	}
}
