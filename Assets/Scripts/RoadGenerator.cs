// #define WFC_ENABLED

using System;
using System.Collections.Generic;
using System.Text;
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

	public UnityEvent onGenerated;

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

#if WFC_ENABLED
#region Setup

		Vector2Int amount = Vector2Int.FloorToInt(area / tileSize);
		Tile[] tiles = new Tile[amount.x * amount.y];

		Tile GetTile(int x, int y)
		{
			return tiles[y * amount.x + x];
		}

		// Tile SetTile(int x, int y, Tile tile)
		// {
		// 	return tiles[y * amount.x + x] = tile;
		// }

		void LogTiles()
		{
			Debug.Log("===");
			StringBuilder sb = new();
			for (int y = 0; y < amount.y; y++)
			{
				for (int x = 0; x < amount.y; x++)
				{
					sb.Append(GetTile(x, y));
				}

				sb.Append("\n");
			}

			Debug.Log(sb.ToString());
		}

		for (int y = 0; y < amount.y; y++)
		{
			for (int x = 0; x < amount.x; x++)
			{
				tiles[y * amount.x + x] = new Tile {PossibleRoads = new List<Road>(roadOptions),};
			}
		}

		LogTiles();

#endregion

#region Step

		void Propagate(int x, int y)
		{
			Road road = GetTile(x, y).Collapse();
			Tile rightTile = GetTile(x + 1, y);
			rightTile.PossibleRoads.RemoveAll(rightRoad => rightRoad.leftConnection != road.rightConnection);
			if (x + 1 < amount.x)
				Propagate(x + 1, y);
			LogTiles();
		}

		Propagate(0, 0);

#endregion
#else
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
#endif
		onGenerated.Invoke();
	}

	private class Tile
	{
		public List<Road> PossibleRoads;

		public override string ToString()
		{
			return $"{PossibleRoads.Count} | ";
		}

		public Road Collapse()
		{
			Road road = PossibleRoads[Random.Range(0, PossibleRoads.Count)];
			PossibleRoads = new List<Road> {road,};
			return road;
		}
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
