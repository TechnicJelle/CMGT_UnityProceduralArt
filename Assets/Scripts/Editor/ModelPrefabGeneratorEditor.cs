using System.IO;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(ModelPrefabGenerator))]
	public class ModelPrefabGeneratorEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUILayout.Button("Generate"))
			{
				ModelPrefabGenerator modelPrefabGenerator = (ModelPrefabGenerator) target;

				foreach (GameObject modelFile in modelPrefabGenerator.Models)
				{
					if (PrefabUtility.GetPrefabAssetType(modelFile) != PrefabAssetType.Model)
					{
						Debug.LogWarning($"Prefab {modelFile.name} was not a model! Skipping...");
						continue;
					}

					string modelPath = "Assets/Prefabs/Generated/" + modelFile.name;
					MakeDir(modelPath);

					for (int i = 0; i < modelFile.transform.childCount; i++)
					{
						GameObject model = modelFile.transform.GetChild(i).gameObject;

						string prefabPath = modelPath + "/" + model.name + ".prefab";

						// Unpack the model from the prefab to the scene.
						// This makes sure only the child of the modelFile gets saved.
						GameObject instance = Instantiate(model);

						PrefabUtility.SaveAsPrefabAsset(instance, prefabPath, out bool prefabSuccess);
						if (!prefabSuccess)
							Debug.LogWarning($"Prefab {model.name} failed to save");

						// Remove the model from the scene again
						DestroyImmediate(instance);
					}
				}
			}
		}

		private static void MakeDir(string path)
		{
			string full = "";
			string prev = full;
			foreach (string dir in path.Split('/'))
			{
				full += dir;
				if (!Directory.Exists(full))
				{
					if (string.IsNullOrEmpty(AssetDatabase.CreateFolder(prev, dir)))
						Debug.LogError($"Couldn't create folder {prev}/{dir}");
				}

				prev = full;
				full += "/";
			}
		}
	}
}
