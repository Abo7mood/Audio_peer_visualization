//  Written by Marcel Remmers © for Yuetility 10.06.22
using UnityEngine;
using UnityEditor;
using YuetilitySoftbody;

namespace YuetilitySoftbody
{

    // Editor
    [CustomEditor(typeof(YueSoftbodyPhysics))]

    [ExecuteAlways]
    public class MyEditorClass : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Yue Softbody Physics Editor", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("\n");

            YueSoftbodyPhysics softbodyPhysics = target as YueSoftbodyPhysics;

            EditorGUILayout.LabelField("Performance", EditorStyles.boldLabel);

            if (!Application.isPlaying)
            {
                softbodyPhysics.simplyfy = EditorGUILayout.Toggle("Simplyfy", softbodyPhysics.simplyfy);

                if (softbodyPhysics.simplyfy)
                {
                    softbodyPhysics.radius = EditorGUILayout.FloatField("Radius", softbodyPhysics.radius);

                }
            }
            else
            {
                EditorGUILayout.LabelField("Change Values in Edit Mode");
            }

            EditorGUILayout.LabelField("\n");
            EditorGUILayout.LabelField("Vertex Tuning", EditorStyles.boldLabel);
            softbodyPhysics.tuning.PositionProportional = EditorGUILayout.FloatField("Proportinal Velocity Factor", softbodyPhysics.tuning.PositionProportional);
            softbodyPhysics.tuning.PositionDerivative = EditorGUILayout.FloatField("Derivative Velocity Factor", softbodyPhysics.tuning.PositionDerivative);



            EditorGUILayout.LabelField("\n");
            EditorGUILayout.LabelField("Retraction Force", EditorStyles.boldLabel);

            softbodyPhysics.autoSpring = EditorGUILayout.Toggle("Auto Retrationforce", softbodyPhysics.autoSpring);

            if (!softbodyPhysics.autoSpring)
            {
                softbodyPhysics.springFactor = EditorGUILayout.FloatField("Retraction Force Factor", softbodyPhysics.springFactor);

            }

            EditorGUILayout.LabelField("\n");
            EditorGUILayout.LabelField("Collision", EditorStyles.boldLabel);

            softbodyPhysics.UseMeshCollider = EditorGUILayout.Toggle("Use Mesh Collider", softbodyPhysics.UseMeshCollider);

            if (!Application.isPlaying)
                softbodyPhysics.softbodyLayer = EditorGUILayout.IntField("Softbody Layer", softbodyPhysics.softbodyLayer);

            softbodyPhysics.physicsVertexRadius = EditorGUILayout.FloatField("Physics Vertex Radius", softbodyPhysics.physicsVertexRadius);
            softbodyPhysics.colliderMaterial = (PhysicMaterial)EditorGUILayout.ObjectField("Physics Material", softbodyPhysics.colliderMaterial, typeof(PhysicMaterial), true);

        }
    }
}
