using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(RoadGenerator))]
	public class RoadGeneratorEditor : UnityEditor.Editor
	{
		private RoadGenerator _target;
		private BoxBoundsHandle _boundsHandle;

		private void OnEnable()
		{
			_boundsHandle = new BoxBoundsHandle();
			_boundsHandle.SetColor(Color.green);
		}

		private void OnSceneGUI()
		{
			_target = (RoadGenerator) target;

			if (!Selection.Contains(_target.gameObject)) return; //Only draw when selected

			Transform transform = _target.transform;
			Matrix4x4 handleMatrix = Matrix4x4.TRS(
				transform.position,
				transform.rotation,
				transform.lossyScale
			);

			_boundsHandle.center = Vector3.zero;
			_boundsHandle.size = new Vector3(_target.area.x, 0, _target.area.y);

			EditorGUI.BeginChangeCheck();
			using (new Handles.DrawingScope(handleMatrix))
			{
				_boundsHandle.DrawHandle();
			}

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(_target, "Changed Bounds");
				_target.area = new Vector2(
					Utils.RoundTo(_boundsHandle.size.x, _target.TileSize.x),
					Utils.RoundTo(_boundsHandle.size.z, _target.TileSize.y)
				);
			}
		}

		public override void OnInspectorGUI()
		{
			_target = (RoadGenerator) target;

			if (GUILayout.Button("Generate"))
				_target.Generate();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("customSeed"));
			if (_target.CustomSeed)
				EditorGUILayout.PropertyField(serializedObject.FindProperty("seed"));

			EditorGUILayout.PropertyField(serializedObject.FindProperty("autoTileSize"));
			if (!_target.AutoTileSize)
				EditorGUILayout.PropertyField(serializedObject.FindProperty("tileSize"));

			EditorGUILayout.PropertyField(serializedObject.FindProperty("roadOptions"), true);

			EditorGUILayout.PropertyField(serializedObject.FindProperty("area"));

			serializedObject.ApplyModifiedProperties();
		}
	}

	[CustomPropertyDrawer(typeof(Road))]
	public class IngredientDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			// Don't make child fields be indented
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			float prefabWidth = position.width / 4;
			float restWidth = position.width - prefabWidth;
			float perWidth = restWidth / 4;
			float labelWidth = position.height; //square
			float propWidth = perWidth - labelWidth;
			Rect prefabRect = new(position.x, position.y, prefabWidth, position.height);
			Rect upLabel    = new(position.x + prefabWidth + perWidth * 0,              position.y, labelWidth, position.height);
			Rect upProp     = new(position.x + prefabWidth + perWidth * 0 + labelWidth, position.y, propWidth,  position.height);
			Rect rightLabel = new(position.x + prefabWidth + perWidth * 1,              position.y, labelWidth, position.height);
			Rect rightProp  = new(position.x + prefabWidth + perWidth * 1 + labelWidth, position.y, propWidth,  position.height);
			Rect downLabel  = new(position.x + prefabWidth + perWidth * 2,              position.y, labelWidth, position.height);
			Rect downProp   = new(position.x + prefabWidth + perWidth * 2 + labelWidth, position.y, propWidth,  position.height);
			Rect leftLabel  = new(position.x + prefabWidth + perWidth * 3,              position.y, labelWidth, position.height);
			Rect leftProp   = new(position.x + prefabWidth + perWidth * 3 + labelWidth, position.y, propWidth,  position.height);

			EditorGUI.PropertyField(prefabRect, property.FindPropertyRelative("prefab"), GUIContent.none);

			GUI.DrawTexture(upLabel, Resources.Load<Texture>("up"));
			EditorGUI.PropertyField(upProp, property.FindPropertyRelative("upConnection"), GUIContent.none);
			GUI.DrawTexture(rightLabel, Resources.Load<Texture>("right"));
			EditorGUI.PropertyField(rightProp, property.FindPropertyRelative("rightConnection"), GUIContent.none);
			GUI.DrawTexture(downLabel, Resources.Load<Texture>("down"));
			EditorGUI.PropertyField(downProp, property.FindPropertyRelative("downConnection"), GUIContent.none);
			GUI.DrawTexture(leftLabel, Resources.Load<Texture>("left"));
			EditorGUI.PropertyField(leftProp, property.FindPropertyRelative("leftConnection"), GUIContent.none);

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}
}
