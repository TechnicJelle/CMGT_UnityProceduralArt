using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "ModelPrefabGen", menuName = "ScriptableObjects/ModelPrefabGen", order = 1)]
	public class ModelPrefabGenerator : ScriptableObject
	{
		[SerializeField] private GameObject[] models;
		public IEnumerable<GameObject> Models => models;
	}
}
