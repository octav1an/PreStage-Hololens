using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PRGeoClasses
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
        private PRGeo PR_CO
        {
            get { return Parent.GetComponent<PRGeo>(); }
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
        /// in every edge after manipulation on the Active edge.
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
            Vector3 dir = Parent.GetComponent<PRGeo>().EdgePref.transform.up;
            return Quaternion.FromToRotation(dir, v0 - v1);
        }
        #endregion //Setup
    }

    public class PRFaceHolder
    {
        public Mesh Mesh;
        public int Index;
        public int[] VertexIndices; 
        // Not updated
        public Vector3 Normal;
        public Vector3[] NORMALS
        {
            get
            {
                Vector3[] normColl = new Vector3[F_VERTICES.Length];
                for (int i = 0; i < normColl.Length; i++)
                {
                    normColl[i] = Mesh.normals[VertexIndices[i]];
                }

                return normColl;
            }
        }
        public Vector3 CENTER
        {
            get { return ComputeCenter(Mesh, VertexIndices); }
        }
        public Quaternion FaceRot;
        public Quaternion FACE_ROT
        {
            get { return ComputeFaceRot(Parent.transform); }
        }
        public GameObject Parent;
        private PRGeo PR_CO
        {
            get { return Parent.GetComponent<PRGeo>(); }
        }
        public Vector3 V0;
        public Vector3 V1;
        public Vector3 V2;
        /// <summary>
        /// This is the 4th Vertex, only used for Quad topology.
        /// </summary>
        public Vector3 V3;
        public MeshTopology MeshTopo;
        public PRFaceHolder savedFH;
        public List<int> SameV0Index;
        public List<int> SameV1Index;
        public List<int> SameV2Index;
        /// <summary>
        /// This is the 4th Vertex Index, only used for Quad topology.
        /// </summary>
        public List<int> SameV3Index;

        public Vector3[] F_VERTICES
        {
            get
            {
                if (MeshTopo == MeshTopology.Quads)
                {
                    Vector3[] coll = new Vector3[4];
                    coll[0] = V0;
                    coll[1] = V1;
                    coll[2] = V2;
                    coll[3] = V3;
                    return coll;
                }
                else if (MeshTopo == MeshTopology.Triangles)
                {
                    Vector3[] coll = new Vector3[3];
                    coll[0] = V0;
                    coll[1] = V1;
                    coll[2] = V2;
                    return coll;
                }
                else
                {
                    return null;
                }

            }
        }
        private float threshold = 0.01f;

        // Constructor

        public PRFaceHolder(Mesh mesh, int index, GameObject parent)
        {
            Mesh = mesh;
            Index = index;
            MeshTopo = mesh.GetTopology(Index);
            VertexIndices = mesh.GetIndices(index);
            Parent = parent;
            SetupVertices(mesh, VertexIndices);
            Normal = SetupNormal(Mesh, VertexIndices);
            FaceRot = ComputeFaceRot(parent.transform);
            SetupSameVertices();
        }

        public PRFaceHolder(PRFaceHolder fH)
        {
            Mesh = fH.Mesh;
            VertexIndices = fH.VertexIndices;
            Parent = fH.Parent;
            Index = fH.Index;
            MeshTopo = fH.MeshTopo;
            SetupVertices(Mesh, VertexIndices);
            FaceRot = ComputeFaceRot(Parent.transform);
            SetupSameVertices();
        }

        //-------------------------------------------------
        public void UpdateInactiveFaceInfo(Mesh mesh)
        {
            V0 = mesh.vertices[VertexIndices[0]];
            V1 = mesh.vertices[VertexIndices[1]];
            V2 = mesh.vertices[VertexIndices[2]];
            if (MeshTopo == MeshTopology.Quads)
            {
                V3 = mesh.vertices[VertexIndices[3]];
            }
            FaceRot = ComputeFaceRot(Parent.transform);
        }

        public void UpdateFace(Vector3 move)
        {
            MoveFace(move);
            Normal = SetupNormal(Mesh, VertexIndices);
            FaceRot = ComputeFaceRot(Parent.transform);
        }

        /// <summary>
        /// Moves each vertex of this face with a vector.
        /// </summary>
        /// <param name="move">Input vector.</param>
        private void MoveFace(Vector3 move)
        {
            if (savedFH != null)
            {
                V0 = savedFH.F_VERTICES[0] + move;
                V1 = savedFH.F_VERTICES[1] + move;
                V2 = savedFH.F_VERTICES[2] + move;
                if (MeshTopo == MeshTopology.Quads)
                {
                    V3 = savedFH.F_VERTICES[3] + move;
                }
            }
        }

        #region SetUp
                /// <summary>
        /// Setup the same vertex indexes that share the same location for V0 and V1.
        /// </summary>
        public void SetupSameVertices()
        {
            if (MeshTopo == MeshTopology.Quads)
            {
                SameV0Index = new List<int>();
                SameV1Index = new List<int>();
                SameV2Index = new List<int>();
                SameV3Index = new List<int>();
                for (int i = 0; i < Mesh.vertexCount; i++)
                {
                    Vector3 v = Mesh.vertices[i];
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
                    else if (Mathf.Abs(v.x - V2.x) < threshold &&
                             Mathf.Abs(v.y - V2.y) < threshold &&
                             Mathf.Abs(v.z - V2.z) < threshold)
                    {
                        SameV2Index.Add(i);
                    }
                    else if (Mathf.Abs(v.x - V3.x) < threshold &&
                             Mathf.Abs(v.y - V3.y) < threshold &&
                             Mathf.Abs(v.z - V3.z) < threshold)
                    {
                        SameV3Index.Add(i);
                    }
                }
            }
            else if (MeshTopo == MeshTopology.Triangles)
            {
                SameV0Index = new List<int>();
                SameV1Index = new List<int>();
                SameV2Index = new List<int>();
                for (int i = 0; i < Mesh.vertexCount; i++)
                {
                    Vector3 v = Mesh.vertices[i];
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
                    else if (Mathf.Abs(v.x - V2.x) < threshold &&
                             Mathf.Abs(v.y - V2.y) < threshold &&
                             Mathf.Abs(v.z - V2.z) < threshold)
                    {
                        SameV2Index.Add(i);
                    }
                }
            }
        }

        private void SetupVertices(Mesh mesh, int[] vertexIndices)
        {
            if (MeshTopo == MeshTopology.Quads)
            {
                V0 = mesh.vertices[vertexIndices[0]];
                V1 = mesh.vertices[vertexIndices[1]];
                V2 = mesh.vertices[vertexIndices[2]];
                V3 = mesh.vertices[vertexIndices[3]];
            }
            else if (MeshTopo == MeshTopology.Triangles)
            {
                V0 = mesh.vertices[vertexIndices[0]];
                V1 = mesh.vertices[vertexIndices[1]];
                V2 = mesh.vertices[vertexIndices[2]];
            }
        }

        // TODO: Make an average normal calculation.
        private Vector3 SetupNormal(Mesh mesh, int[] vertexIndices)
        {
            // I get the normal only from the first vector. Can have problems.
            return mesh.normals[vertexIndices[0]];
        }
        
        private Quaternion ComputeFaceRot(Transform objTrs)
        {
            // Use objTrs.right to match normal of FacePrefab
            return Quaternion.FromToRotation(objTrs.right, SetupNormal(Mesh, VertexIndices));
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
        #endregion //SetUp
    }
}

