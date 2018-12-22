using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PRGeoClasses;
using UnityEngine;

public class PRGeo : MonoBehaviour {

    /// <summary>
    /// Bool that is activated when the cube is Active. Now works when the cube is moved.
    /// </summary>
    public bool Selected = false;
    //Vertex Array, use only geting the vertices. This property cannot rewrite them.
    public Vector3[] VERTS_COLL
    {
        get
        {
            return GetComponent<MeshFilter>().mesh.vertices;
        }
    }
    /// <summary>
    /// Cube's mesh component.
    /// </summary>
    public Mesh CubeMesh;
    //public Vector3 
    // Elements Prefabs.
    public GameObject VertexPref;
    public GameObject EdgePref;
    public GameObject FacePref;

    public GameObject PR_VERTEX_GO
    {
        get { return transform.Find("Vertex").gameObject; }
    }
    private GameObject[] VertexCollGO;
    public GameObject PR_EDGE_GO
    {
        get { return transform.Find("Edge").gameObject; }
    }
    public GameObject PR_FACE_GO
    {
        get { return transform.Find("Face").gameObject; }
    }
    public bool VertexModeActive;
    public bool EdgeModeActive;
    public bool FaceModeActive;
    public bool CubeModeActive;

    // Remove later
    public PREdgeHolder[] PrEdgeHolders;

    public Vector3 CENTER_GEOMETRICAL
    {
        get
        {
            Vector3 center = new Vector3();
            foreach (Vector3 vert in this.GetComponent<MeshFilter>().mesh.vertices)
            {
                center += vert;
            }
            return center / 24;
        }
    }

    #region Unity
    protected virtual void Awake()
    {
        GetComponent<MeshFilter>().mesh = GenerateMesh();
        CubeMesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshCollider>().sharedMesh = CubeMesh;
        // Generate Cue elements.
        GenerateVertexHandles();
        PrEdgeHolders = CreateUniqEdgePrefabs(GenerateEdgeHolders());
        GenerateFacePrefabs();
    }

    protected virtual void Start()
    {
        // Deactivate Cube menu
        //foreach (PREdgeHolder eH in PrEdgeHolders)
        //{
            //print(eH.MidPos);
        //}
    }

    protected virtual void Update()
    {
        DrawCubeAxis(true);
        //objCenter.transform.position = transform.position;
    }

    protected virtual void LateUpdate()
    {
        if (Selected)
        {

        }
    }

    void OnEnable()
    {
        EventManager.AirTapDown += OnInputDownLocal;
        EventManager.AirTapDown += OnInputUpLocal;
    }

    void OnDisable()
    {
        EventManager.AirTapDown -= OnInputDownLocal;
        EventManager.AirTapDown -= OnInputUpLocal;
    }

    void OnDestroy()
    {
        // Remove it from Gizmo if it was there.
        Manager.Instance.GIZMO.ClearTargets();
        Manager.Instance.CollGeoObjects.Remove(gameObject);
    }
    #endregion //Unity

    #region Events
    public void OnInputDownLocal()
    {

    }
    public void OnInputUpLocal()
    {
        UpdateBlockCollider();
    }
    #endregion // Events

    public PRGeo SelectCube(Material selMat)
    {

        Selected = true;
        Material[] selectedMats = new Material[GetComponent<MeshRenderer>().materials.Length];
        for (int i = 0; i < GetComponent<MeshRenderer>().materials.Length; i++)
        {
            selectedMats[i] = selMat;
        }

        GetComponent<MeshRenderer>().materials = selectedMats;
        Manager.Instance.SelectedGeo = this;
        return this;
    }

    public PRGeo DeselectCube(Material unselMat)
    {
        Selected = false;
        Material[] unselectedMats = new Material[GetComponent<MeshRenderer>().materials.Length];
        for (int i = 0; i < GetComponent<MeshRenderer>().materials.Length; i++)
        {
            unselectedMats[i] = unselMat;
        }
        GetComponent<MeshRenderer>().materials = unselectedMats;
        Manager.Instance.SelectedGeo = null;
        // Turn off TransformElements
        ActiveteVertex(false);
        ActivateEdge(false);
        ActivateFace(false);
        return this;
    }

    #region Collider Work
    /// <summary>
    /// Update the block collider that is a BoxCollider. Used for calculation of closest points for snap.
    /// </summary>
    public void UpdateBlockCollider()
    {
        GetComponent<MeshCollider>().sharedMesh = CubeMesh;
    }
    #endregion //Collider Work

    #region MenuMethodsCall
    /// <summary>
    /// Coroutine that activates the geometry selection mode.
    /// </summary>
    /// <returns></returns>
    public IEnumerator TurnOnCube()
    {
        CubeModeActive = true;
        ContexMenu.Instance.GeometryModeActive = true;
        VertexModeActive = false;
        EdgeModeActive = false;
        FaceModeActive = false;
        PR_FACE_GO.SetActive(FaceModeActive);
        PR_EDGE_GO.SetActive(EdgeModeActive);
        PR_VERTEX_GO.SetActive(VertexModeActive);
        // Add the whole geometry to Gizmo array.
        Manager.Instance.GIZMO.ClearAndAddTarget(this.transform);
        // Save the position, used during movement transformation.
        Manager.Instance.GIZMO.SavePrevPosition();

        yield return null;
    }

    public IEnumerator TurnOffAllModes()
    {
        CubeModeActive = false;
        ContexMenu.Instance.GeometryModeActive = false;
        VertexModeActive = false;
        EdgeModeActive = false;
        FaceModeActive = false;
        PR_FACE_GO.SetActive(FaceModeActive);
        PR_EDGE_GO.SetActive(EdgeModeActive);
        PR_VERTEX_GO.SetActive(VertexModeActive);
        // Remove targets.
        Manager.Instance.GIZMO.ClearTargets();

        yield return null;
    }

    // Activate/Deactivate elements.
    private void ActiveteVertex(bool state)
    {
        VertexModeActive = state;
        PR_VERTEX_GO.SetActive(state);
    }
    private void ActivateEdge(bool state)
    {
        EdgeModeActive = state;
        PR_EDGE_GO.SetActive(state);
    }
    private void ActivateFace(bool state)
    {
        FaceModeActive = state;
        PR_FACE_GO.SetActive(state);
    }

    // Update Elements when switching between modes.
    private void UpdateEdges(GameObject parent)
    {
        PREdge[] edgeColl = parent.GetComponentsInChildren<PREdge>();
        foreach (var edge in edgeColl)
        {
            edge.EdgeHolder.UpdateInactiveEdgeInfo(CubeMesh);
            edge.UpdateCollider();
        }
    }

    private void UpdateFace(GameObject paretn)
    {
        PRFace[] faceColl = paretn.GetComponentsInChildren<PRFace>();
        foreach (var face in faceColl)
        {
            face.UpdateCollider();
        }
    }
    #endregion // MenuMethodsCall

    #region Generate
    protected virtual Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.subMeshCount = 6;
        // 2.5 because i scale down the main object to 0.2 scale factor.
        float size = 1f;
        Vector3 v0 = new Vector3(size, size, size);
        Vector3 v1 = new Vector3(size, size, -size);
        Vector3 v2 = new Vector3(size, -size, -size);
        Vector3 v3 = new Vector3(size, -size, size);
        Vector3 v4 = new Vector3(-size, -size, size);
        Vector3 v5 = new Vector3(-size, -size, -size);
        Vector3 v6 = new Vector3(-size, size, -size);
        Vector3 v7 = new Vector3(-size, size, size);
        // Make the faces array vertices.
        Vector3[] face0 = new Vector3[4]
        {
            transform.TransformVector(v0),
            transform.TransformVector(v1),
            transform.TransformVector(v2),
            transform.TransformVector(v3)
        };
        Vector3[] face1 = new Vector3[4]
        {
            transform.TransformVector(v3),
            transform.TransformVector(v2),
            transform.TransformVector(v5),
            transform.TransformVector(v4)
        };
        Vector3[] face2 = new Vector3[4]
        {
            transform.TransformVector(v4),
            transform.TransformVector(v5),
            transform.TransformVector(v6),
            transform.TransformVector(v7)
        };
        Vector3[] face3 = new Vector3[4]
        {
            transform.TransformVector(v7),
            transform.TransformVector(v6),
            transform.TransformVector(v1),
            transform.TransformVector(v0)
        };
        Vector3[] face4 = new Vector3[4]
        {
            transform.TransformVector(v7),
            transform.TransformVector(v0),
            transform.TransformVector(v3),
            transform.TransformVector(v4)
        };
        Vector3[] face5 = new Vector3[4]
        {
            transform.TransformVector(v1),
            transform.TransformVector(v6),
            transform.TransformVector(v5),
            transform.TransformVector(v2)
        };
        // Create Quads
        int[] quadV0 = new int[4] { 3, 2, 1, 0 };
        int[] quadV1 = new int[4] { 7, 6, 5, 4 };
        int[] quadV2 = new int[4] { 11, 10, 9, 8 };
        int[] quadV3 = new int[4] { 15, 14, 13, 12 };
        int[] quadV4 = new int[4] { 19, 18, 17, 16 };
        int[] quadV5 = new int[4] { 23, 22, 21, 20 };

        // Join the array with vertices.
        var list = new List<Vector3>();
        list.AddRange(face0);
        list.AddRange(face1);
        list.AddRange(face2);
        list.AddRange(face3);
        list.AddRange(face4);
        list.AddRange(face5);
        mesh.vertices = list.ToArray();
        // Assign indexes for each quad.
        mesh.SetIndices(quadV0, MeshTopology.Quads, 0);
        mesh.SetIndices(quadV1, MeshTopology.Quads, 1);
        mesh.SetIndices(quadV2, MeshTopology.Quads, 2);
        mesh.SetIndices(quadV3, MeshTopology.Quads, 3);
        mesh.SetIndices(quadV4, MeshTopology.Quads, 4);
        mesh.SetIndices(quadV5, MeshTopology.Quads, 5);
        // Recalculate all
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        //print((mesh.vertices[0] - mesh.vertices[1]).magnitude);
        return mesh;
    }

    protected void GenerateVertexHandles()
    {
        Vector3[] vColl = VERTS_COLL.Distinct().ToArray();
        VertexCollGO = new GameObject[vColl.Length];
        for (int i = 0; i < vColl.Length; i++)
        {
            GameObject obj = GameObject.Instantiate(VertexPref, transform.TransformPoint(vColl[i]),
                Quaternion.identity, PR_VERTEX_GO.transform);
            obj.name = "Vertex" + i;
            obj.SetActive(true);
            VertexCollGO[i] = obj;
        }

    }

    // TODO: Remove if everything is on with Edges
    ///// <summary>
    ///// Generate the EdgeHolders for every Edge in every face. The array has overlaping EdgeHolders.
    ///// </summary>
    ///// <returns>Array with overlaping EdgeHolders.</returns>
    //protected virtual PREdgeHolder[] GenerateEdgeHolders()
    //{
    //    PREdgeHolder[] edgeColl = new PREdgeHolder[CubeMesh.vertexCount];
    //    for (int i = 0; i < CubeMesh.subMeshCount; i++)
    //    {
    //        // Keep track of the actual number of the edge that is being generated,
    //        // If it is the last one - connect the vertex to first one.
    //        int index = -1;
    //        for (uint j = CubeMesh.GetIndexStart(i); j < CubeMesh.GetIndexStart(i) + CubeMesh.GetIndexCount(i); j++)
    //        {
    //            index++;
    //            if (index < 3)
    //            {
    //                Vector3 v0 = CubeMesh.vertices[j];
    //                Vector3 v1 = CubeMesh.vertices[j + 1];
    //                PREdgeHolder edge = new PREdgeHolder(v0, v1, this.gameObject);
    //                edge.V0Index = (int)j;
    //                edge.V1Index = (int)j + 1;
    //                edgeColl[j] = edge;
    //            }
    //            else
    //            {
    //                Vector3 v0 = CubeMesh.vertices[j];
    //                Vector3 v1 = CubeMesh.vertices[j - 3];
    //                PREdgeHolder edge = new PREdgeHolder(v0, v1, this.gameObject);
    //                edge.V0Index = (int)j;
    //                edge.V1Index = (int)j - 3;
    //                edgeColl[j] = edge;
    //            }
    //        }
    //    }
    //    return edgeColl;
    //}

    /// <summary>
    /// Generate the EdgeHolders for every Edge in every face. The array has overlaping EdgeHolders.
    /// </summary>
    /// <returns>Array with overlaping EdgeHolders.</returns>
    protected virtual PREdgeHolder[] GenerateEdgeHolders()
    {
        PREdgeHolder[] edgeColl = new PREdgeHolder[CubeMesh.vertexCount];
        for (int i = 0; i < CubeMesh.subMeshCount; i++)
        {
            // Keep track of the actual number of the edge that is being generated,
            // If it is the last one - connect the vertex to first one.
            if (CubeMesh.GetTopology(i) == MeshTopology.Quads)
            {
                int index = -1;
                for (uint j = CubeMesh.GetIndexStart(i); j < CubeMesh.GetIndexStart(i) + CubeMesh.GetIndexCount(i); j++)
                {
                    index++;
                    if (index < 3)
                    {
                        Vector3 v0 = CubeMesh.vertices[j];
                        Vector3 v1 = CubeMesh.vertices[j + 1];
                        PREdgeHolder edge = new PREdgeHolder(v0, v1, this.gameObject);
                        edge.V0Index = (int)j;
                        edge.V1Index = (int)j + 1;
                        edgeColl[j] = edge;
                    }
                    else
                    {
                        Vector3 v0 = CubeMesh.vertices[j];
                        Vector3 v1 = CubeMesh.vertices[j - 3];
                        PREdgeHolder edge = new PREdgeHolder(v0, v1, this.gameObject);
                        edge.V0Index = (int)j;
                        edge.V1Index = (int)j - 3;
                        edgeColl[j] = edge;
                    }
                }
            }
            else if (CubeMesh.GetTopology(i) == MeshTopology.Triangles)
            {
                int index = -1;
                for (uint j = CubeMesh.GetIndexStart(i); j < CubeMesh.GetIndexStart(i) + CubeMesh.GetIndexCount(i); j++)
                {
                    index++;
                    if (index < 2)
                    {
                        Vector3 v0 = CubeMesh.vertices[j];
                        Vector3 v1 = CubeMesh.vertices[j + 1];
                        PREdgeHolder edge = new PREdgeHolder(v0, v1, this.gameObject);
                        edge.V0Index = (int)j;
                        edge.V1Index = (int)j + 1;
                        edgeColl[j] = edge;
                    }
                    else
                    {
                        Vector3 v0 = CubeMesh.vertices[j];
                        Vector3 v1 = CubeMesh.vertices[j - 2];
                        PREdgeHolder edge = new PREdgeHolder(v0, v1, this.gameObject);
                        edge.V0Index = (int)j;
                        edge.V1Index = (int)j - 2;
                        edgeColl[j] = edge;
                    }
                }
            }
        }
        return edgeColl;
    }

    /// <summary>
    /// Clean up the duplicate edges that share the same MidPos and Instantiate the Edge prefabs.
    /// </summary>
    /// <param name="edgeColl">Dirty array with edges</param>
    /// <returns>Clean array of edges</returns>
    protected PREdgeHolder[] CreateUniqEdgePrefabs(PREdgeHolder[] edgeColl)
    {
        // Group the edges according to the MidPos vector. For the cube I will have groups of 2 edges that overlap.
        var result = edgeColl.GroupBy(edge => edge.MidPos);
        PREdgeHolder[] cleaEdgeColl = new PREdgeHolder[result.Count()];
        for (int i = 0; i < result.Count(); i++)
        {
            // Get only the first elemnt from each group and assign it to the clean array.
            cleaEdgeColl[i] = result.ToArray()[i].ToArray()[0];
            // Create the objects.
            GameObject obj = GameObject.Instantiate(EdgePref, transform.TransformPoint(cleaEdgeColl[i].MidPos),
                cleaEdgeColl[i].MidRot, PR_EDGE_GO.transform);
            obj.name = "Edge" + i;
            obj.SetActive(true);
            // Setup the PREdge file
            PREdge edgeCO = obj.GetComponent<PREdge>();
            edgeCO.EdgeHolder = cleaEdgeColl[i];
        }

        return cleaEdgeColl;
    }

    protected virtual void GenerateFacePrefabs()
    {
        PRFaceHolder[] faceColl = new PRFaceHolder[CubeMesh.subMeshCount];
        for (int i = 0; i < faceColl.Length; i++)
        {
            PRFaceHolder face = new PRFaceHolder(CubeMesh, i, this.gameObject);
            // 1. Position
            // 2. Quaterion
            GameObject obj = GameObject.Instantiate(FacePref, transform.TransformPoint(face.CENTER),
                Quaternion.identity, PR_FACE_GO.transform);
            // Rename the objects.
            obj.name = "Face" + i;
            obj.SetActive(true);
            obj.GetComponent<PRFace>().FaceHolder = face;
            //print(obj.GetComponent<PRFace>().FaceHolder.MeshTopo);
            obj.GetComponent<MeshFilter>().mesh = obj.GetComponent<PRFace>().GenerateMeshCollider();
        }
    }
    #endregion //Generate

    #region Draw Elements
    void OnGUI()
    {
        Vector3[] vColl = VERTS_COLL.Distinct().ToArray();


        //Drawing.DrawLabel(transform.TransformPoint(vColl[0]), "V0");
        //Drawing.DrawLabel(transform.TransformPoint(vColl[1]), "V1");
        //Drawing.DrawLabel(transform.TransformPoint(vColl[2]), "V2");
        //Drawing.DrawLabel(transform.TransformPoint(vColl[3]), "V3");

        //Drawing.DrawLabel(transform.TransformPoint(vColl[4]), "V4");
        //Drawing.DrawLabel(transform.TransformPoint(vColl[5]), "V5");
        //Drawing.DrawLabel(transform.TransformPoint(vColl[6]), "V6");
        //Drawing.DrawLabel(transform.TransformPoint(vColl[7]), "V7");
    }

    void DrawCubeAxis(bool drawOn)
    {
        if (drawOn)
        {
            float lineLength = 0.1f;
            Vector3 center = transform.position;
            //Debug.DrawLine(center, center + (transform.right * lineLength), Color.cyan);
            //Debug.DrawLine(center, center + (transform.up * lineLength), Color.magenta);
            //Debug.DrawLine(center, center + (-transform.forward * lineLength), Color.yellow);
        }
    }
    #endregion //Draw Elements
}
