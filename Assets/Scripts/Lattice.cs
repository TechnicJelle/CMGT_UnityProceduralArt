using UnityEditor;
using UnityEngine;

public class Lattice : MonoBehaviour
{
	[SerializeField] private bool editMode = false;
	public bool EditMode => editMode;

	[SerializeField] public Vector3 bottomTopLeft;
	[SerializeField] public Vector3 bottomTopRight;
	[SerializeField] public Vector3 bottomBottomLeft;
	[SerializeField] public Vector3 bottomBottomRight;

	[SerializeField] public Vector3 topTopLeft;
	[SerializeField] public Vector3 topTopRight;
	[SerializeField] public Vector3 topBottomLeft;
	[SerializeField] public Vector3 topBottomRight;

	private MeshFilter _mesh;

	private void OnValidate()
	{
		Selection.selectionChanged += () =>
		{
			if (Selection.Contains(gameObject))
				editMode = false;
		};

		// Deform();
	}

	public void Deform()
	{
		Mesh mesh = _mesh.sharedMesh;
		for (int i = 0; i < mesh.vertices.Length; i++)
		{
			Vector3 vertex = mesh.vertices[i];

			mesh.vertices[i] = vertex - Vector3.down;
		}

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();

		_mesh.sharedMesh = mesh;
	}

	public void ResetHandles()
	{
		_mesh = GetComponent<MeshFilter>();

		Bounds bounds = _mesh.sharedMesh.bounds;

		bottomTopLeft = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
		bottomTopRight = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
		bottomBottomLeft = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
		bottomBottomRight = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);

		topTopLeft = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
		topTopRight = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);
		topBottomLeft = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
		topBottomRight = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
	}
}
