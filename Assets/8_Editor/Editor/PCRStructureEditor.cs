using UnityEngine;
using UnityEditor;
namespace LUP.PCR
{
    [CustomEditor(typeof(StructureBase), true)]
    public class PCRStructureEditor : Editor
    {
        protected virtual void OnSceneGUI()
        {
            StructureBase structure = (StructureBase)target;

            if (structure.entranceAnchor == null)
            {
                return;
            }


            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < structure.localWaypoints.Count; i++)
            {

                Vector3 worldPos = structure.transform.TransformPoint(structure.localWaypoints[i]);


                Vector3 newWorldPos = Handles.PositionHandle(worldPos, Quaternion.identity);


                Handles.Label(newWorldPos + Vector3.up * 0.5f, $"{i + 1}");

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(structure, "Move Waypoint");

                    structure.localWaypoints[i] = structure.transform.InverseTransformPoint(newWorldPos);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            StructureBase structure = (StructureBase)target;

            GUILayout.Space(10);
            GUILayout.Label("경로 편집 도구", EditorStyles.boldLabel);

            if (GUILayout.Button("Way Point 추가"))
            {
                Undo.RecordObject(structure, "Add Waypoint");


                Vector3 lastPos = structure.localWaypoints.Count > 0
                    ? structure.localWaypoints[structure.localWaypoints.Count - 1]
                    : Vector3.zero;

                structure.localWaypoints.Add(lastPos + new Vector3(1, 0, 0));
            }

            if (GUILayout.Button("마지막 포인트 삭제"))
            {
                if (structure.localWaypoints.Count > 0)
                {
                    Undo.RecordObject(structure, "Remove Waypoint");
                    structure.localWaypoints.RemoveAt(structure.localWaypoints.Count - 1);
                }
            }
        }
    }
}
