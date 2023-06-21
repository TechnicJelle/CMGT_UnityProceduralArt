using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(Lattice))]
	public class LatticeEditor : UnityEditor.Editor
	{
		private Lattice _target;

		public override void OnInspectorGUI()
		{
			_target = (Lattice) target;

			if (GUILayout.Button("Reset"))
			{
				_target.ResetHandles();
				SceneView.RepaintAll();
			}

			base.OnInspectorGUI();
		}

		private void OnSceneGUI()
		{
			_target = (Lattice) target;

			Transform t = _target.transform;
			Matrix4x4 handleMatrix = Matrix4x4.TRS(
				t.position,
				t.rotation,
				t.lossyScale
			);

			using (new Handles.DrawingScope(handleMatrix))
			{
				DrawWireCube(Color.yellow,
					_target.bottomTopLeft, _target.bottomTopRight, _target.bottomBottomLeft, _target.bottomBottomRight,
					_target.topTopLeft, _target.topTopRight, _target.topBottomLeft, _target.topBottomRight
				);

				if (_target.EditMode)
				{
					Handles.Label(_target.bottomTopLeft, nameof(_target.bottomTopLeft));
					Handles.Label(_target.bottomTopRight, nameof(_target.bottomTopRight));
					Handles.Label(_target.bottomBottomLeft, nameof(_target.bottomBottomLeft));
					Handles.Label(_target.bottomBottomRight, nameof(_target.bottomBottomRight));

					Handles.Label(_target.topTopLeft, nameof(_target.topTopLeft));
					Handles.Label(_target.topTopRight, nameof(_target.topTopRight));
					Handles.Label(_target.topBottomLeft, nameof(_target.topBottomLeft));
					Handles.Label(_target.topBottomRight, nameof(_target.topBottomRight));

					EditorGUI.BeginChangeCheck();
					Vector3 newBottomTopLeft = Handles.PositionHandle(_target.bottomTopLeft, Quaternion.identity);
					Vector3 newBottomTopRight = Handles.PositionHandle(_target.bottomTopRight, Quaternion.identity);
					Vector3 newBottomBottomLeft = Handles.PositionHandle(_target.bottomBottomLeft, Quaternion.identity);
					Vector3 newBottomBottomRight = Handles.PositionHandle(_target.bottomBottomRight, Quaternion.identity);

					Vector3 newTopTopLeft = Handles.PositionHandle(_target.topTopLeft, Quaternion.identity);
					Vector3 newTopTopRight = Handles.PositionHandle(_target.topTopRight, Quaternion.identity);
					Vector3 newTopBottomLeft = Handles.PositionHandle(_target.topBottomLeft, Quaternion.identity);
					Vector3 newTopBottomRight = Handles.PositionHandle(_target.topBottomRight, Quaternion.identity);

					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(_target, "Changed Lattice");
						_target.bottomTopLeft = newBottomTopLeft;
						_target.bottomTopRight = newBottomTopRight;
						_target.bottomBottomLeft = newBottomBottomLeft;
						_target.bottomBottomRight = newBottomBottomRight;

						_target.topTopLeft = newTopTopLeft;
						_target.topTopRight = newTopTopRight;
						_target.topBottomLeft = newTopBottomLeft;
						_target.topBottomRight = newTopBottomRight;

						_target.Deform();
					}
				}
			}
		}

		private static void DrawWireCube(Color colour,
			Vector3 bottomTopLeft, Vector3 bottomTopRight, Vector3 bottomBottomLeft, Vector3 bottomBottomRight,
			Vector3 topTopLeft, Vector3 topTopRight, Vector3 topBottomLeft, Vector3 topBottomRight)
		{
			Handles.DrawSolidRectangleWithOutline(
				new[] {bottomTopLeft, bottomTopRight, bottomBottomRight, bottomBottomLeft,},
				Color.clear, colour);

			Handles.DrawSolidRectangleWithOutline(
				new[] {topTopLeft, topTopRight, topBottomRight, topBottomLeft,},
				Color.clear, colour);

			Handles.color = colour;
			Handles.DrawLine(bottomTopLeft, topTopLeft);
			Handles.DrawLine(bottomTopRight, topTopRight);
			Handles.DrawLine(bottomBottomLeft, topBottomLeft);
			Handles.DrawLine(bottomBottomRight, topBottomRight);
		}
	}
}
