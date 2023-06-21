using UnityEngine;

[RequireComponent(typeof(RoadGenerator))]
public class RoadSetup : MonoBehaviour
{
	private RoadGenerator _roadGenerator;

	[SerializeField] private Material material;

	private void OnValidate()
	{
		_roadGenerator = GetComponent<RoadGenerator>();
		_roadGenerator.onGenerated.AddListener(Generate);
	}

	private void Generate()
	{
		BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
		if(boxCollider == null) boxCollider = gameObject.AddComponent<BoxCollider>();

		boxCollider.size = new Vector3(_roadGenerator.area.x, 0.05f, _roadGenerator.area.y);
		boxCollider.center = new Vector3(0, -0.025f, 0);

		foreach (Transform child in transform)
		{
			child.GetComponent<MeshRenderer>().sharedMaterial = material;
		}
	}

	private void OnDestroy()
	{
		_roadGenerator.onGenerated.RemoveListener(Generate);
	}
}
