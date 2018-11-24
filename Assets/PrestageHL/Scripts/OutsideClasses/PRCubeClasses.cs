using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PRCubeClasses
{
    public class PREdgeHolder
    {
        public Vector3 MidPos;
        public Quaternion MidRot;
        public Vector3 V0;
        public Vector3 V1;
        public int V0Index;
        public int V1Index;
        public GameObject Parent;
        public PREdgeHolder savedEH;
        private PRCube PR_CO
        {
            get { return Parent.GetComponent<PRCube>(); }
        }
        public List<int> SameV0Index;
        public List<int> SameV1Index;

        private float threshold = 0.01f; 

        // Constructor
        public PREdgeHolder(Vector3 firstVertex, Vector3 secondVertex, GameObject parent)
        {
            V0 = firstVertex;
            V1 = secondVertex;
            Parent = parent;
            MidPos = ComputeMidPos(firstVertex, secondVertex);
            MidRot = ComputeMidRot(firstVertex, secondVertex, parent.transform);
            SetupSameVertices();
        }
        public PREdgeHolder(PREdgeHolder eh)
        {
            V0 = eh.V0;
            V1 = eh.V1;
            Parent = eh.Parent;
            V0Index = eh.V0Index;
            V1Index = eh.V1Index;
            MidPos = ComputeMidPos(V0, V1);
            MidRot = ComputeMidRot(V0, V1, Parent.transform);
            SetupSameVertices();
        }
        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// Method used for all inactive edges during Down and Up events to update the vertex position
        /// in every edge after manipulation on the active edge.
        /// </summary>
        /// <param name="mesh">Mesh</param>
        public void UpdateInactiveEdgeInfo(Mesh mesh)
        {
            V0 = mesh.vertices[V0Index];
            V1 = mesh.vertices[V1Index];
            MidPos = ComputeMidPos(V0, V1);
            MidRot = ComputeMidRot(V0, V1, PR_CO.transform);
        }

        public void UpdateEdge(Vector3 move)
        {
            MoveEdge(move);
            MidPos = ComputeMidPos(V0, V1);
            MidRot = ComputeMidRot(V0, V1, Parent.transform);
        }

        public void MoveEdge(Vector3 move)
        {
            if (savedEH != null)
            {
                V0 = savedEH.V0 + move;
                V1 = savedEH.V1 + move;
            }
        }

        #region Setup
        /// <summary>
        /// Setup the same vertex indexes that share the same location for V0 and V1.
        /// </summary>
        public void SetupSameVertices()
        {
            SameV0Index = new List<int>();
            SameV1Index = new List<int>();
            for (int i = 0; i < PR_CO.CubeMesh.vertexCount; i++)
            {
                Vector3 v = PR_CO.CubeMesh.vertices[i];
                if (Mathf.Abs(v.x - V0.x) < threshold &&
                    Mathf.Abs(v.y - V0.y) < threshold &&
                    Mathf.Abs(v.z - V0.z) < threshold)
                {
                    SameV0Index.Add(i);
                }
                else if (Mathf.Abs(v.x - V1.x) < threshold &&
                         Mathf.Abs(v.y - V1.y) < threshold &&
                         Mathf.Abs(v.z - V1.z) < threshold)
                {
                    SameV1Index.Add(i);
                }
            }
        }

        private Vector3 ComputeMidPos(Vector3 v0, Vector3 v1)
        {
            return (v0 + v1) / 2;
        }

        private Quaternion ComputeMidRot(Vector3 v0, Vector3 v1, Transform objTrs)
        {
            return Quaternion.FromToRotation(objTrs.forward, v0 - v1);
        }
        #endregion //Setup
    }

    public class PRFaceHolder
    {
        public Mesh Mesh;
        public int[] VertexIndices; 
        public Vector3 Normal;
        public Vector3 Center;
        public Quaternion FaceRot;
        public Transform ParentTrs;
        private Vector3 _v0;
        private Vector3 _v1;
        private Vector3 _v2;
        private Vector3 _v3;

        public Vector3[] F_VERTICES
        {
            get
            {
                Vector3[] coll = new Vector3[4];
                coll[0] = _v0;
                coll[1] = _v1;
                coll[2] = _v2;
                coll[3] = _v3;
                return coll;
            }
        }

        // Constructor
        public PRFaceHolder(Mesh mesh, int[] vertexIndices, Transform parentTrs)
        {
            Mesh = mesh;
            VertexIndices = vertexIndices;
            ParentTrs = parentTrs;
            SetupVertices(mesh, vertexIndices);
            Normal = SetupNormal(mesh, vertexIndices);
            FaceRot = ComputeFaceRot(parentTrs);
            Center = ComputeCenter(mesh, vertexIndices);
        }

        #region SetUp
        private void SetupVertices(Mesh mesh, int[] vertexIndices)
        {
            _v0 = mesh.vertices[vertexIndices[0]];
            _v1 = mesh.vertices[vertexIndices[1]];
            _v2 = mesh.vertices[vertexIndices[2]];
            _v3 = mesh.vertices[vertexIndices[3]];
        }
        private Vector3 SetupNormal(Mesh mesh, int[] vertexIndices)
        {
            // I get the normal only from the first vector. Can have problems.
            return mesh.normals[vertexIndices[0]];
        }
        #endregion //SetUp

        private Quaternion ComputeFaceRot(Transform objTrs)
        {
            // Use objTrs.right to match normal of FacePrefab
            return Quaternion.FromToRotation(objTrs.right, Normal);
        }
        private Vector3 ComputeCenter(Mesh mesh, int[] vertexIndices)
        {
            Vector3 sum = new Vector3();
            for (int i = 0; i < vertexIndices.Length; i++)
            {
                int vertIndex = vertexIndices[i];
                sum += mesh.vertices[vertIndex];
            }

            return sum / vertexIndices.Length;
        }
    }
}

