using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class RoadGenerator : MonoBehaviour
{
	[SerializeField] private bool customSeed = false;
	public bool CustomSeed => customSeed;
	[SerializeField] private int seed = 1;

	[SerializeField] private bool autoTileSize = true;
	public bool AutoTileSize => autoTileSize;
	[SerializeField] private Vector2 tileSize = Vector2.one;
	public Vector2 TileSize => tileSize;

	[SerializeField] public Road[] roadOptions;

	public UnityEvent onGenerate;

	public Vector2 area = Vector2.one;

	private void OnValidate()
	{
		if (autoTileSize)
		{
			try
			{
				Vector3 size = roadOptions[0].prefab.GetComponent<Renderer>().bounds.size;
				tileSize = new Vector2(size.x, size.z);
			}
			catch (Exception e) when (e is NullReferenceException or IndexOutOfRangeException)
			{
				Debug.LogWarning("No road option available to get the tile size from");
			}
		}

		//Round to nearest tile size
		area = new Vector2(
			Utils.RoundTo(area.x, tileSize.x),
			Utils.RoundTo(area.y, tileSize.y)
		);
	}

	public void Generate()
	{
		if (customSeed)
			Random.InitState(seed);

		Transform t = transform;

		//Remove all children
		for (int i = t.childCount - 1; i >= 0; i--)
			DestroyImmediate(t.GetChild(i).gameObject);

		Vector3 origin = -new Vector3(area.x, 0f, area.y) / 2f;
		Vector2 halfTileSize = tileSize / 2f;
		const float f = 0.01f; //In case of floating point errors
		for (float y = halfTileSize.y; y < area.y + halfTileSize.y - f; y += tileSize.y)
		{
			for (float x = halfTileSize.x; x < area.x + halfTileSize.x - f; x += tileSize.x)
			{
				GameObject o = (GameObject) PrefabUtility.InstantiatePrefab(roadOptions[Random.Range(0, roadOptions.Length)].prefab, t);
				o.transform.localPosition = origin + new Vector3(x, 0, y);
				o.transform.rotation = t.rotation;
				o.transform.Rotate(Vector3.up, Random.Range(0, 4) * 90f);
			}
		}

		onGenerate.Invoke();
	}
}

public enum ConnectionType
{
	Road,
	Pavement,
}

[Serializable]
public class Road
{
	public GameObject prefab;
	public ConnectionType upConnection;
	public ConnectionType rightConnection;
	public ConnectionType downConnection;
	public ConnectionType leftConnection;
}
