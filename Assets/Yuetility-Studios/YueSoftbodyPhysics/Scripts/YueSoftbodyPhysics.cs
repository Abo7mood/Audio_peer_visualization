//  Written by Marcel Remmers © for Yuetility 10.06.22

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuetilitySoftbody;

namespace YuetilitySoftbody
{
    // Bone Class
    [System.Serializable]

    public class Bone
    {
        public Vector3 position;

        public int originVertex = 0;

        public List<int> verticies;
        public List<float> weights;

        public Bone()
        {
            verticies = new List<int>();
            weights = new List<float>();
        }
    }

    // Softbody Class
    [RequireComponent(typeof(Rigidbody))]

    public class YueSoftbodyPhysics : MonoBehaviour
    {
        [HideInInspector]
        public bool simplyfy = true;
        [HideInInspector]
        public float radius = 0.1f;

        [HideInInspector]
        public bool autoSpring = true;
        [HideInInspector]
        public float springFactor = 0.001f;

        [HideInInspector]
        public bool UseMeshCollider = false;
        [HideInInspector]
        public int softbodyLayer = 30;
        [HideInInspector]
        public float physicsVertexRadius = 0.05f;

        [HideInInspector]
        public PhysicMaterial colliderMaterial;

        [Header("Physics Vertex Tuning")]

        [HideInInspector]
        public PDTuning tuning;


        // private
        private Mesh mesh;
        private Rigidbody rigid;
        private Transform Offset;

        [HideInInspector]
        public List<Transform> physicsVerticiesOnVertex;
        [HideInInspector]
        public List<Transform> physicsVerticiesOnBone;

        [HideInInspector]
        public List<Bone> bones;

        private Vector3[] vertices;
        private Vector3[] startVertices;
        private Vector2[] uv;

        private int[] triangles;

        private MeshCollider meshCollider;
        private MeshFilter meshFilter;
        private PDTuning lastTuning;


        void Start()
        {
            // Create MeshColldier
            if (GetComponent<MeshCollider>())
                meshCollider = GetComponent<MeshCollider>();
            else
                meshCollider = this.gameObject.AddComponent<MeshCollider>();

            Physics.IgnoreLayerCollision(softbodyLayer, softbodyLayer, true);

            meshCollider.convex = true;
            meshCollider.enabled = false;
            meshCollider.sharedMesh = null;

            // Create OffsetTransform
            Offset = new GameObject("SoftbodyPhysics").transform;

            Offset.parent = transform;
            Offset.position = transform.position;
            Offset.rotation = Quaternion.Euler(0, 0, 0);

            // Position Tracker
            tuning.TransTraget = transform;
            lastTuning = new PDTuning();

            // Find References
            rigid = GetComponent<Rigidbody>();
            meshFilter = GetComponent<MeshFilter>();

            // Setup Softbody
            CreateMeshdata();

            if (simplyfy)
            {
                GenerateBones();
                GenerateWeights();
            }

            AddPhysicsTargets();

        }

        private void CreateMeshdata()
        {
            vertices = GetComponent<MeshFilter>().mesh.vertices;
            triangles = GetComponent<MeshFilter>().mesh.triangles;
            uv = GetComponent<MeshFilter>().mesh.uv;

            startVertices = GetComponent<MeshFilter>().mesh.vertices;

            mesh = new Mesh();

            meshFilter.mesh = mesh;
        }
        private void UpdateMesh()
        {
            if (autoSpring && !simplyfy)
                springFactor = (1 / (float)physicsVerticiesOnVertex.Count);
            else if (autoSpring && simplyfy)
                springFactor = (1 / (float)physicsVerticiesOnBone.Count);

            if (simplyfy)
            {
                int e = 0;

                if (bones.Count > 0)
                {
                    foreach (Bone bone in bones)
                    {
                        int f = 0;

                        if (bone.verticies.Count > 0)
                        {
                            foreach (int i in bone.verticies)
                            {
                                Vector3 localPhysicsVertexPos = transform.InverseTransformPoint(physicsVerticiesOnBone[e].position);
                                Vector3 localBonePos = transform.InverseTransformPoint(bone.position);

                                if (bone.weights.Count > 0)
                                    vertices[i] = startVertices[i] + localPhysicsVertexPos - startVertices[bone.originVertex];

                                f++;
                            }
                            e++;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = transform.InverseTransformPoint(physicsVerticiesOnVertex[i].position);
                }
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;

            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();

        }
        private void UpdateMeshCollider()
        {
            if (UseMeshCollider)
            {
                meshCollider.material = colliderMaterial;

                meshCollider.enabled = true;
                meshCollider.sharedMesh = mesh;
                meshCollider.convex = true;
            }
            else
            {
                meshCollider.convex = true;
                meshCollider.enabled = false;
                meshCollider.sharedMesh = null;
            }
        }
        private void GenerateBones()
        {

            int i = 0;
            foreach (Vector3 vertex in vertices)
            {
                Vector3 WorldSpaceVetexPosition = transform.TransformPoint(vertex);

                if (bones.Count == 0)
                {
                    Bone bone = new Bone();

                    bone.position = WorldSpaceVetexPosition; ;
                    bones.Add(bone);
                }
                else
                {
                    bool notTaken = true;

                    foreach (Bone bone in bones)
                    {
                        if ((bone.position - WorldSpaceVetexPosition).magnitude < radius)
                        {
                            notTaken = false;
                        }
                    }

                    if (notTaken)
                    {
                        Bone bone = new Bone();

                        bone.originVertex = i;
                        bone.position = WorldSpaceVetexPosition;
                        bones.Add(bone);
                    }
                }
                i++;
            }
        }
        private void GenerateWeights()
        {
            if (bones.Count > 0)
            {
                for (int e = 0; e < bones.Count; e++)
                {
                    for (int i = 0; i < startVertices.Length; i++)
                    {

                        Vector3 WorldSpaceVetexPosition = transform.TransformPoint(startVertices[i]);

                        if ((WorldSpaceVetexPosition - bones[e].position).magnitude < radius * 1.05f)
                        {
                            if ((WorldSpaceVetexPosition - bones[e].position).magnitude != 0)
                            {
                                bones[e].verticies.Add(i);
                                bones[e].weights.Add((-(WorldSpaceVetexPosition - bones[e].position).magnitude + radius * 1.05f) / radius * 1.05f);
                            }
                            else
                            {
                                bones[e].verticies.Add(i);
                                bones[e].weights.Add(1);
                            }
                        }
                    }
                }
            }
        }
        private void AddPhysicsTargets()
        {
            if (simplyfy)
            {
                foreach (Bone bone in bones)
                {
                    CreatePhysicsVertex(bone.position, false);
                }
            }
            else
            {
                foreach (Vector3 vertex in vertices)
                {
                    Vector3 WorldSpaceVetexPosition = transform.TransformPoint(vertex);
                    CreatePhysicsVertex(WorldSpaceVetexPosition, true);
                }
            }
        }
        private void CreatePhysicsVertex(Vector3 Position, bool OnVertex)
        {
            GameObject target = new GameObject("Traget");

            target.transform.position = Position;
            target.transform.rotation = Quaternion.Euler(0, 0, 0);
            target.transform.parent = Offset;

            GameObject phyVertex = new GameObject("PhysicsVertex");

            // Position
            phyVertex.transform.position = Position;
            phyVertex.transform.rotation = Quaternion.Euler(0, 0, 0);
            phyVertex.transform.parent = Offset;

            // Rigidbody
            Rigidbody rigidBody = phyVertex.AddComponent<Rigidbody>();
            rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

            // Tracker
            phyVertex.AddComponent<PositionTracker>();
            phyVertex.GetComponent<PositionTracker>().TrackRotation = false;
            phyVertex.GetComponent<PositionTracker>().InItTuning(tuning, target.transform);


            // Collider
            SphereCollider sphereCollider = phyVertex.AddComponent<SphereCollider>();
            Physics.IgnoreCollision(sphereCollider, meshCollider);
            sphereCollider.radius = physicsVertexRadius;
            phyVertex.layer = softbodyLayer;

            if (colliderMaterial)
                sphereCollider.material = colliderMaterial;

            // Save PhysicsVertex
            if (OnVertex)
                physicsVerticiesOnVertex.Add(phyVertex.transform);
            else
                physicsVerticiesOnBone.Add(phyVertex.transform);
        }

        private void CheckTuningChange()
        {
            if (tuning.PositionProportional != lastTuning.PositionProportional)
                UpdateTuning();

            if (tuning.PositionDerivative != lastTuning.PositionDerivative)
                UpdateTuning();

            if (tuning.Tensor.magnitude != lastTuning.Tensor.magnitude)
                UpdateTuning();

            if (tuning.maxDepenetrationVelocity != lastTuning.maxDepenetrationVelocity)
                UpdateTuning();

        }
        private void UpdateTuning()
        {
            if (physicsVerticiesOnVertex.Count > 0)
            {
                foreach (Transform tracker in physicsVerticiesOnVertex)
                {
                    tracker.GetComponent<PositionTracker>().InItTuning(tuning, tracker.GetComponent<PositionTracker>().TransTarget);
                }
            }
            if (physicsVerticiesOnBone.Count > 0)
            {
                foreach (Transform tracker in physicsVerticiesOnBone)
                {
                    tracker.GetComponent<PositionTracker>().InItTuning(tuning, tracker.GetComponent<PositionTracker>().TransTarget);
                }
            }

            lastTuning.PositionProportional = tuning.PositionProportional;
            lastTuning.PositionDerivative = tuning.PositionDerivative;

            lastTuning.Tensor = tuning.Tensor;
            lastTuning.maxDepenetrationVelocity = tuning.maxDepenetrationVelocity;

        }

        private void Update()
        {
            UpdateMesh();
            CheckTuningChange();
        }

        void FixedUpdate()
        {
            if (simplyfy)
            {
                if (bones.Count > 0)
                {
                    for (int i = 0; i < bones.Count; i++)
                    {
                        rigid.AddForceAtPosition(-physicsVerticiesOnBone[i].GetComponent<PositionTracker>().Force * springFactor * rigid.mass, physicsVerticiesOnBone[i].position);
                    }
                }
            }
            else
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    rigid.AddForceAtPosition(-physicsVerticiesOnVertex[i].GetComponent<PositionTracker>().Force * springFactor * rigid.mass, physicsVerticiesOnVertex[i].position);
                }
            }

            UpdateMeshCollider();
        }
    }
}
