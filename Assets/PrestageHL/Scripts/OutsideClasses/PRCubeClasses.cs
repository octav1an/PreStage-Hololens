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
        public Transform ParentTrs;

        // Constructor
        public PREdgeHolder(Vector3 firstVertex, Vector3 secondVertex)
        {
            V0 = firstVertex;
            V1 = secondVertex;
            MidPos = ComputeMidPos(firstVertex, secondVertex);
        }
        public PREdgeHolder(Vector3 firstVertex, Vector3 secondVertex, Transform parentTrs)
        {
            V0 = firstVertex;
            V1 = secondVertex;
            ParentTrs = parentTrs;
            MidPos = ComputeMidPos(firstVertex, secondVertex);
            MidRot = ComputeMidRot(firstVertex, secondVertex, parentTrs);
        }
        //-----------------------------------------------------------------------------------------

        private Vector3 ComputeMidPos(Vector3 v0, Vector3 v1)
        {
            return (v0 + v1) / 2;
        }

        private Quaternion ComputeMidRot(Vector3 v0, Vector3 v1, Transform objTrs)
        {
            return Quaternion.FromToRotation(objTrs.forward, v0 - v1);
        }
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

