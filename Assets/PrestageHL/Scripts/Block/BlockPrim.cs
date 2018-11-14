using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using HoloToolkit.Unity.InputModule;

public class BlockPrim : MonoBehaviour
{

    /// <summary>
    /// Block ID in the list that stores all existing blocks. This is not uniq, because
    /// the list can be dynamicaly updated.
    /// </summary>
    public int BlockId = -1;
    /// <summary>
    /// Bool that is activated when the block is selected. Now works when the block is moved.
    /// </summary>
    public bool Selected = false;
    /// <summary>
    /// Manager of the scene.
    /// </summary>
    public Manager MANAGER
    {
        get { return GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>(); }
    }
    //Vertex Array, use only geting the vertices. This property cannot rewrite them.
    public Vector3[] VERTS_COLL
    {
        get
        {
            return GetComponent<MeshFilter>().mesh.vertices;
        }
    }
    /// <summary>
    /// Block's mesh component.
    /// </summary>
    public Mesh BlockMesh;

    public float SnapDistance;

    #region Vertices and Edges
    // Make them private later.
    public Vector3[] FACE_VERTS_POS_Z
    {
        get
        {
            Vector3[] facePosZ = new Vector3[4];
            facePosZ[0] = VERTS_COLL[0];
            facePosZ[1] = VERTS_COLL[1];
            facePosZ[2] = VERTS_COLL[2];
            facePosZ[3] = VERTS_COLL[3];
            return facePosZ;
        }
    }
    public Vector3[] FACE_VERTS_NEG_Z
    {
        get
        {
            Vector3[] faceNegZ = new Vector3[4];
            faceNegZ[0] = VERTS_COLL[8];
            faceNegZ[1] = VERTS_COLL[9];
            faceNegZ[2] = VERTS_COLL[10];
            faceNegZ[3] = VERTS_COLL[11];
            return faceNegZ;
        }
    }
    public Vector3[] FACE_VERTS_POS_X
    {
        get
        {
            Vector3[] facePosX = new Vector3[4];
            facePosX[0] = VERTS_COLL[12];
            facePosX[1] = VERTS_COLL[13];
            facePosX[2] = VERTS_COLL[14];
            facePosX[3] = VERTS_COLL[15];
            return facePosX;
        }
    }
    public Vector3[] FACE_VERTS_NEG_X
    {
        get
        {
            Vector3[] faceNegX = new Vector3[4];
            faceNegX[0] = VERTS_COLL[4];
            faceNegX[1] = VERTS_COLL[5];
            faceNegX[2] = VERTS_COLL[6];
            faceNegX[3] = VERTS_COLL[7];
            return faceNegX;
        }
    }
    public Vector3[] FACE_VERTS_POS_Y
    {
        get
        {
            Vector3[] facePosY = new Vector3[4];
            facePosY[0] = VERTS_COLL[20];
            facePosY[1] = VERTS_COLL[21];
            facePosY[2] = VERTS_COLL[22];
            facePosY[3] = VERTS_COLL[23];
            return facePosY;
        }
    }
    public Vector3[] FACE_VERTS_NEG_Y
    {
        get
        {
            Vector3[] faceNegY = new Vector3[4];
            faceNegY[0] = VERTS_COLL[16];
            faceNegY[1] = VERTS_COLL[17];
            faceNegY[2] = VERTS_COLL[18];
            faceNegY[3] = VERTS_COLL[19];
            return faceNegY;
        }
    }
    public Vector3[][] BLOCK_FACE_VERTS
    {
        get
        {
            Vector3[][] blockFaceColl = new Vector3[6][];
            blockFaceColl[0] = FACE_VERTS_POS_Z;
            blockFaceColl[1] = FACE_VERTS_NEG_Z;
            blockFaceColl[2] = FACE_VERTS_POS_X;
            blockFaceColl[3] = FACE_VERTS_NEG_X;
            blockFaceColl[4] = FACE_VERTS_POS_Y;
            blockFaceColl[5] = FACE_VERTS_NEG_Y;
            return blockFaceColl;
        }
    }

    // Faces objects.
    public GameObject FACE_POS_Z_OBJ
    {
        get;
        private set;
    }
    public GameObject FACE_NEG_Z_OBJ
    {
        get;
        private set;
    }
    public GameObject FACE_POS_X_OBJ
    {
        get;
        private set;
    }
    public GameObject FACE_NEG_X_OBJ
    {
        get;
        private set;
    }
    public GameObject FACE_POS_Y_OBJ
    {
        get;
        private set;
    }
    public GameObject FACE_NEG_Y_OBJ
    {
        get;
        private set;
    }

    // Face Script components.
    public BlockFace FACE_POS_Z
    {
        get
        {
            if (FACE_POS_Z_OBJ == null)
            {
                throw (new CannotSetUpFaceException("Face Setup failed - FACE_POS_Z"));
            }
            else
            {
                return FACE_POS_Z_OBJ.GetComponent<BlockFace>();
            }

        }
    }
    public BlockFace FACE_NEG_Z
    {
        get
        {
            if (FACE_NEG_Z_OBJ == null)
            {
                throw (new CannotSetUpFaceException("Face Setup failed - FACE_NEG_Z"));
            }
            else
            {
                return FACE_NEG_Z_OBJ.GetComponent<BlockFace>();
            }

        }
    }
    public BlockFace FACE_POS_X
    {
        get
        {
            if (FACE_POS_X_OBJ == null)
            {
                throw (new CannotSetUpFaceException("Face Setup failed - FACE_POS_X"));
            }
            else
            {
                return FACE_POS_X_OBJ.GetComponent<BlockFace>();
            }

        }
    }
    public BlockFace FACE_NEG_X
    {
        get
        {
            if (FACE_NEG_X_OBJ == null)
            {
                throw (new CannotSetUpFaceException("Face Setup failed - FACE_NEG_X"));
            }
            else
            {
                return FACE_NEG_X_OBJ.GetComponent<BlockFace>();
            }

        }
    }
    public BlockFace FACE_POS_Y
    {
        get
        {
            if (FACE_POS_Y_OBJ == null)
            {
                throw (new CannotSetUpFaceException("Face Setup failed - FACE_POS_Y"));
            }
            else
            {
                return FACE_POS_Y_OBJ.GetComponent<BlockFace>();
            }

        }
    }
    public BlockFace FACE_NEG_Y
    {
        get
        {
            if (FACE_NEG_Y_OBJ == null)
            {
                throw (new CannotSetUpFaceException("Face Setup failed - FACE_NEG_Y"));
            }
            else
            {
                return FACE_NEG_Y_OBJ.GetComponent<BlockFace>();
            }

        }
    }
    /// <summary>
    /// Stores all BlockFace components of this Block.
    /// Used to easier update or get data in for loops.
    /// </summary>
    public BlockFace[] FACE_COLL
    {
        get
        {
            BlockFace[] coll = new BlockFace[] {
                FACE_POS_Z, FACE_NEG_Z,
                FACE_POS_X, FACE_NEG_X,
                FACE_POS_Y, FACE_NEG_Y
            };
            return coll;
        }
    }

    // Mid-points on vertical edges.
    public Vector3 EDGE_MID_PT0_WORLD
    {
        get
        {
            return this.transform.TransformPoint((Vertices[0] + Vertices[1]) / 2);
        }
    }
    public Vector3 EDGE_MID_PT1_WORLD
    {
        get
        {
            return this.transform.TransformPoint((Vertices[3] + Vertices[2]) / 2);
        }
    }
    public Vector3 EDGE_MID_PT2_WORLD
    {
        get
        {
            return this.transform.TransformPoint((Vertices[8] + Vertices[9]) / 2);
        }
    }
    public Vector3 EDGE_MID_PT3_WORLD
    {
        get
        {
            return this.transform.TransformPoint((Vertices[11] + Vertices[10]) / 2);
        }
    }
    public Vector3[] EDGE_MID_COLL
    {
        get
        {
            Vector3[] coll = new Vector3[4];
            coll[0] = EDGE_MID_PT0_WORLD;
            coll[1] = EDGE_MID_PT1_WORLD;
            coll[2] = EDGE_MID_PT2_WORLD;
            coll[3] = EDGE_MID_PT3_WORLD;
            return coll;
        }
    }
    /// <summary>
    /// Proximity collider that checks if there are blocks nearby to activate snap check.
    /// </summary>
    public ProximityCollider PROXI_COLLIDER
    {
        get
        {
            return transform.GetComponentInChildren<ProximityCollider>();
        }
    }
    #endregion // Vertices and Edges

    /// <summary>
    /// This represend collection of indexes for vertices that share the same coordinate. Think about them as Block's 8 vertices.
    /// Check Rhino file to see to which container each vertex belongs.
    /// </summary>
    public int[][] VertexIndexCon = new int[][]
    {
        new int[] {0, 7, 21},
        new int[] {1, 6, 19},
        new int[] {2, 13, 18},
        new int[] {3, 12, 22},
        new int[] {8, 15, 23},
        new int[] {9, 14, 17},
        new int[] {10, 5, 16},
        new int[] {11, 4, 20}
    };
    /// <summary>
    /// Geometric center of the block in world space that does not equl to this.transform.position.
    /// </summary>
    public Vector3 GEOMETRIC_CENTER_WORLD
    {
        get
        {
            Vector3 center = new Vector3();
            foreach (Vector3 vert in this.GetComponent<MeshFilter>().mesh.vertices)
            {
                center += vert;
            }
            return this.transform.TransformPoint(center / 24);
        }
    }
    /// <summary>
    /// Geometric center of the block in local space that does not equl to this.transform.position. Is used for to resize collider.
    /// </summary>
    public Vector3 GEOMETRIC_CENTER
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
    /// <summary>
    /// Dimension of the block on Z direction in local space.
    /// </summary>
    public float LENGTH_Z
    {
        get
        {
            return (Vertices[0] - Vertices[11]).magnitude;
        }
    }
    /// <summary>
    /// Dimension of the block on X direction in local space.
    /// </summary>
    public float LENGTH_X
    {
        get
        {
            return (Vertices[4] - Vertices[15]).magnitude;
        }
    }
    /// <summary>
    /// Block mesh vertices, use this to transform vertices location.
    /// </summary>
    public Vector3[] Vertices;
    /// <summary>
    /// Saved block's vertices, used to create the movement vector.
    /// </summary>
    public Vector3[] VerticesSaved;
    /// <summary>
    /// Saved location of the block. It is saved during MouseDown event.
    /// </summary>
    private Vector3 _savedBlockLoc;
    /// <summary>
    /// String that stores the name of the hit collider untill the mouse is released.
    /// </summary>
    public String ColliderName;
    /// <summary>
    /// Plane that is used to move the object, it is equal to block centroid.
    /// </summary>
    private Plane _movePlane;
    /// <summary>
    /// Saved the location of intersection between mouse ray with movePlane. Used for moving the block.
    /// </summary>
    private Vector3 _savedMoveTarget;
    /// <summary>
    /// Intersection position between plane and mouse ray that is used for horizontal movements of block and faces.
    /// </summary>
    public Vector3 TARGET_WORLD
    {
        get
        {
            return SetTarggetPosition();
        }
    }
    /// <summary>
    /// GameObject that holds the menu of the block.
    /// </summary>
    public GameObject BlockMenu;

    #region Unity
    // Use this for initialization
    void Start () {
        //--------------------------------------------
        _savedBlockLoc = this.transform.position;
        BlockMesh = GetComponent<MeshFilter>().mesh;
        Vertices = BlockMesh.vertices;
        VerticesSaved = BlockMesh.vertices;
        if (BlockId == -1) BlockId = Manager.CollBlocksObjects.IndexOf(gameObject);
        _savedMoveTarget = SetTarggetPosition();
        _movePlane = new Plane(Vector3.up, this.transform.position);
        SetUpIndividualFaces();
        UpdateBlockCollider();
        UpdateProximityCollider();
        SnapDistance = 0.04f;
        //--------------------------------------------
        foreach (Vector3 vertex in Vertices)
        {
            //Debug.Log(vertex);
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
	    UpdateBlockMenu();
	}

    private void LateUpdate()
    {
        // By having these function inside LateUpdate - I make sure that first the select boolean is triggered by 
        // Manager and then run these function.
        // Here run methods when the block is selected only.
        if (Selected)
        {
            UpdateFaceVerts();
            // Here run everything that should run on Input down (Airtap Down).
            if (Manager.InputDown)
            {
                MoveBlock();
                RotateBlock();
            }
        }
    }
    #endregion // Unity

    //---------------------------------------------HOLOLENS INPUTS------------------------------------------------------
    public void OnInputDownLocal()
    {
        _savedBlockLoc = this.transform.position;
        _savedMoveTarget = SetTarggetPosition();
        // Update the colliderName when MouseDown.
        ColliderName = MANAGER.GET_COLLIDER_NAME;
        //-------------------------------------------------------
        // Save location for several things inside BlockFace. Like FaceCenter.
        foreach (BlockFace face in FACE_COLL)
        {
            face.UpdateOnMouseDown();
        }
    }

    public void OnInputUpLocal()
    {
        // Reset the collider name to empty.
        ColliderName = "";
        VerticesSaved = BlockMesh.vertices;
        UpdateBlockCollider();
        UpdateProximityCollider();

        // Recomanded by Unity to recalculate this things after mesh is changed.
        BlockMesh.RecalculateBounds();
        BlockMesh.RecalculateNormals();
        BlockMesh.RecalculateTangents();

        //-------------------------------------------------------
        // Update info for face variables when mouse up.
        foreach (BlockFace face in FACE_COLL)
        {
            face.UpdateOnMouseUp();
        }
    }
    //---------------------------------------------HOLOLENS INPUTS------------------------------------------------------

    /// <summary>
    /// Update the proximity collider, that triggers snap functions.
    /// </summary>
    public void UpdateProximityCollider()
    {
        BoxCollider proximityCollider = this.transform.Find("ProximityCollider").GetComponent<BoxCollider>();
        // Create the offset vector that will move the collider up a bit from face_neg_y center.
        Vector3 offsetUp = new Vector3(0, 0.05f, 0);
        // Move the proximity collider lower, this way it will not interfere with other face colliders.
        proximityCollider.center = FACE_NEG_Y.FACE_CENTER + offsetUp;
        // Create hte offset vector that will be added to the xz size of the proximity collider.
        Vector3 offsetSize = new Vector3(0.6f, 0, 0.6f);
        proximityCollider.size = new Vector3(LENGTH_X, 0.05f, LENGTH_Z) + offsetSize;
    }

    /// <summary>
    /// Update the block collider that is a BoxCollider. Used for calculation of closest points for snap.
    /// </summary>
    public void UpdateBlockCollider()
    {
        BoxCollider blockCollider = this.GetComponent<BoxCollider>();
        // Update collider center to be the geometric center of the block, which is not equal to the transform.position.
        blockCollider.center = GEOMETRIC_CENTER;
        blockCollider.size = new Vector3(LENGTH_X, 1, LENGTH_Z);
    }

    //---------------------------------------------------------------------------------------------------
    private void RotateBlock()
    {
        if (ColliderName == "face_pos_y")
        {
            if (Input.GetKey(KeyCode.N))
            {
                this.transform.Rotate(Vector3.up * Time.deltaTime * 50, Space.World);
            }
            else if (Input.GetKey(KeyCode.M))
            {
                this.transform.Rotate(Vector3.up * Time.deltaTime * (-50), Space.World);
            }
        }

    }

    public void UpdateBlockMenu()
    {
        if (Selected)
        {
            BlockMenu.SetActive(true);
        }
        else
        {
            BlockMenu.SetActive(false);
        }
    }

    ////---------------------------------------------------------------------------------------------------
    ///// <summary>
    ///// Method that returns the intersection point between object's middle plane and a ray from mouse position.
    ///// </summary>
    ///// <returns></returns>
    //private Vector3 SetTarggetPosition()
    //{
    //    Ray rayPlane = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    float point = 0f;
    //    if (_movePlane.Raycast(rayPlane, out point))
    //    {
    //        // Return the point in world space that intersects with this plane.
    //        return rayPlane.GetPoint(point);
    //    }
    //    else
    //    {
    //        return new Vector3();
    //    }
    //}

    /// <summary>
    /// Method that returns the intersection point between object's middle plane and a ray from mouse position.
    /// FOR HOLOLENS
    /// </summary>
    /// <returns></returns>
    private Vector3 SetTarggetPosition()
    {
        Ray rayPlane = GazeManager.Instance.Rays[0];
        float point = 0f;
        if (_movePlane.Raycast(rayPlane, out point))
        {
            // Return the point in world space that intersects with this plane.
            return rayPlane.GetPoint(point);
        }
        else
        {
            return new Vector3();
        }
    }

    #region Move&Snap
    //-----------------------------------------------MOVE BLOCK----------------------------------------------------
    /// <summary>
    /// Move the entire block, when click and drag on the top face.
    /// </summary>
    private void MoveBlock()
    {
        if(ColliderName == "face_pos_y")
        {
            //if(Input.GetMouseButton(0)) SetTarggetPosition();
            transform.position = _savedBlockLoc + (SetTarggetPosition() - _savedMoveTarget);
            
            for(int i = 0; i < ((List<GameObject>)MoveSnapBuildup()[0]).Count; i++)
            {
                MoveBlockToSnap(SnapDistance, SnapDistance, i);
            }
        }
    }

    //-----------------------------------------------SNAP BLOCK----------------------------------------------------
    /// <summary>
    /// Method that snaps this block when it is within snap zone.
    /// </summary>
    /// <param name="snapDist">Snap distance for face snap.</param>
    /// <param name="cornerSnap">Snap distance for corner snap.</param>
    /// <param name="proxiIndex">Index of the proxi blocks within snap zone.</param>
    /// <returns></returns>
    private float MoveBlockToSnap(float snapDist, float cornerSnap, int proxiIndex)
    {
        List<GameObject> list = (List<GameObject>)MoveSnapBuildup()[0];
        float closestDist = (float)MoveSnapBuildup(proxiIndex)[4];
        if (list.Count == 0)
        {
            return 0f;
        }
        //--------------------------------------------------------------------------
        // Find the closest edge of this obj and the coresponded closest edge of proxi obj that fits
        // the snapDist comparison. (This part is used in the Corner Snap only.)
        float cornerSnapDist = 1000;
        Vector3 closestEdge = new Vector3();
        Vector3 closestEdgeProxi = new Vector3();
        for (int i = 0; i < list[proxiIndex].GetComponent<BlockPrim>().EDGE_MID_COLL.Length; i++)
        {
            Vector3 edgeMidProxi = list[proxiIndex].GetComponent<BlockPrim>().EDGE_MID_COLL[i];
            for (int j = 0; j < EDGE_MID_COLL.Length; j++)
            {
                Vector3 edgeMidThis = EDGE_MID_COLL[j];
                if ((edgeMidThis - edgeMidProxi).magnitude < cornerSnapDist)
                {
                    cornerSnapDist = (edgeMidThis - edgeMidProxi).magnitude;
                    closestEdge = edgeMidThis;
                    closestEdgeProxi = list[proxiIndex].GetComponent<BlockPrim>().EDGE_MID_COLL[i];
                }
            }
        }
        //--------------------------------------------------------------------------
        // 1. Corner snap has the most priority.
        if (cornerSnapDist < cornerSnap)
        {
            Vector3 move = closestEdgeProxi - closestEdge;
            this.transform.Translate(move, Space.World);
            //print("Zero [" + proxiIndex + "]: " + move.magnitude);
            return move.magnitude;
        }
        // 2. Apply face snap from this as priority.
        else if (closestDist < snapDist)
        {
            // Specify the proxiIndex in order for Snap() to correctly calculate closest distance.
            Vector3 move = (Vector3)MoveSnapBuildup(proxiIndex)[1];
            this.transform.Translate(move, Space.World);
            //print("First [" + proxiIndex + "]: " + move.magnitude);
            return move.magnitude;
        }
        // 3. Apply face snap from other proxi objects as priority.
        // Specify the proxiIndex in order for Snap() to correctly calculate closest distance.
        else if ((float)list[proxiIndex].GetComponent<BlockPrim>().MoveSnapBuildup(proxiIndex)[4] < snapDist)
        {
            Vector3 move = (Vector3)list[proxiIndex].GetComponent<BlockPrim>().MoveSnapBuildup(proxiIndex)[1];
            this.transform.Translate(-move, Space.World);
            //print("Second [" + proxiIndex + "]: " + move.magnitude);
            return move.magnitude;
        }
        else
        {
            return 1000;
        }

    }
    
    /// <summary>
    /// 
    /// 0. Proximity objects list
    /// 1. Move Vector, used to move the object
    /// 2. Closest projected vector
    /// 3. Closest projected vector index, i need it to get vector from this object
    /// 4. Closest distance (float)
    /// 5. Array with projections on the proximity objects.
    /// </summary>
    /// <returns></returns>
    public List<object> MoveSnapBuildup(int proxiIndex = 0)
    {
        List<object> returnObj = new List<object>();
        // 1.Get the collider from the positive proximity distance
        List<GameObject> closeBlocksColl = PROXI_COLLIDER.closeBlocksColl;
        returnObj.Add(closeBlocksColl);
        //print(closeBlocksColl.Count);
        // 2.Find the closest loc for each edge mid pt on each collider.
        Vector3 closestVec = new Vector3();
        float closestDist = 1000;
        int closestIndex = -1;
        Vector3[] collClosest = new Vector3[4];
        if (closeBlocksColl.Count > 0)
        {
            // Check the closest vector from this to the proximity objects.
            for (int i = 0; i < EDGE_MID_COLL.Length; i++)
            {
                collClosest[i] = closeBlocksColl[proxiIndex].GetComponent<BoxCollider>().ClosestPoint(EDGE_MID_COLL[i]);
                if ((collClosest[i] - EDGE_MID_COLL[i]).magnitude < closestDist)
                {
                    closestDist = (collClosest[i] - EDGE_MID_COLL[i]).magnitude;
                    closestVec = EDGE_MID_COLL[i];
                    closestIndex = i;
                }
                //print("Edge PT " + i + " : " + (coll_closest[i] - EDGE_MID_COLL[i]).magnitude);
            }

            // The vector between EdgePt and projection on ProxiObj of this EdgePt.
            Vector3 moveVector = collClosest[closestIndex] - closestVec;

            returnObj.Add(moveVector);
            returnObj.Add(closestVec);
            returnObj.Add(closestIndex);
            returnObj.Add(closestDist);
            returnObj.Add(collClosest);
        }
        return returnObj;
    }
    #endregion //Move&Snap

    #region FaceSetUp
    //---------------------------------------------------------------------------------------------------
    /// <summary>
    /// Method that runs when the block is selected. It updates the faces vertices and their normals.
    /// </summary>
    private void UpdateFaceVerts()
    {
        // Assign Vertices to each face
        FACE_POS_Z.FaceVerts = FACE_VERTS_POS_Z;
        FACE_NEG_Z.FaceVerts = FACE_VERTS_NEG_Z;
        FACE_POS_X.FaceVerts = FACE_VERTS_POS_X;
        FACE_NEG_X.FaceVerts = FACE_VERTS_NEG_X;
        FACE_POS_Y.FaceVerts = FACE_VERTS_POS_Y;
        FACE_NEG_Y.FaceVerts = FACE_VERTS_NEG_Y;

        // Assign FaceNormals (Update)
        FACE_POS_Z.FaceNormal = BlockMesh.normals[0];
        FACE_NEG_Z.FaceNormal = BlockMesh.normals[8];
        FACE_POS_X.FaceNormal = BlockMesh.normals[12];
        FACE_NEG_X.FaceNormal = BlockMesh.normals[4];
        FACE_POS_Y.FaceNormal = BlockMesh.normals[20];
        FACE_NEG_Y.FaceNormal = BlockMesh.normals[16];
    }

    //---------------------------------------------------------------------------------------------------
    /// <summary>
    /// Set up the variables for each face, like what vertice indexes it has.
    /// 1. Find and assign the faces in this object.
    /// 2. Assign vertices to each face class.
    /// 3. Assign face normals to each face class.
    /// 4. Assign vertexIndexContainer.
    /// 5. Assign edge mid points indexes to each face.
    /// </summary>
    private void SetUpIndividualFaces()
    {
        // Find and assign face objects to properties in this class.
        try
        {
            FACE_POS_Z_OBJ = transform.Find("FACE_POS_Z".ToLower()).gameObject;
            FACE_NEG_Z_OBJ = transform.Find("FACE_NEG_Z".ToLower()).gameObject;
            FACE_POS_X_OBJ = transform.Find("FACE_POS_X".ToLower()).gameObject;
            FACE_NEG_X_OBJ = transform.Find("FACE_NEG_X".ToLower()).gameObject;
            FACE_POS_Y_OBJ = transform.Find("FACE_POS_Y".ToLower()).gameObject;
            FACE_NEG_Y_OBJ = transform.Find("FACE_NEG_Y".ToLower()).gameObject;
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Problem with Face setup in BlockPrim/SetUpIndividualFaces()");
        }
        // Assign Vertices to each face (Update)
        FACE_POS_Z.FaceVerts = FACE_VERTS_POS_Z;
        FACE_NEG_Z.FaceVerts = FACE_VERTS_NEG_Z;
        FACE_POS_X.FaceVerts = FACE_VERTS_POS_X;
        FACE_NEG_X.FaceVerts = FACE_VERTS_NEG_X;
        FACE_POS_Y.FaceVerts = FACE_VERTS_POS_Y;
        FACE_NEG_Y.FaceVerts = FACE_VERTS_NEG_Y;

        // Assign FaceNormals (Update)
        FACE_POS_Z.FaceNormal = BlockMesh.normals[0];
        FACE_NEG_Z.FaceNormal = BlockMesh.normals[8];
        FACE_POS_X.FaceNormal = BlockMesh.normals[12];
        FACE_NEG_X.FaceNormal = BlockMesh.normals[4];
        FACE_POS_Y.FaceNormal = BlockMesh.normals[20];
        FACE_NEG_Y.FaceNormal = BlockMesh.normals[16];

        // Assign vertexIndexContainer - these are the arrays indexes that hold vertices of the block. (One Time assignment)
        FACE_POS_Z.VertexIndexCon = new int[] { 0, 1, 2, 3 };
        FACE_NEG_Z.VertexIndexCon = new int[] { 4, 5, 6, 7 };
        FACE_POS_X.VertexIndexCon = new int[] { 2, 3, 4, 5 };
        FACE_NEG_X.VertexIndexCon = new int[] { 0, 1, 6, 7 };
        FACE_POS_Y.VertexIndexCon = new int[] { 0, 3, 4, 7 };
        FACE_NEG_Y.VertexIndexCon = new int[] { 1, 2, 5, 6 };

        // Assign edge mid points to each face.
        FACE_POS_Z.EdgeMidCollIndex = new int[] { 0, 1 };
        FACE_NEG_Z.EdgeMidCollIndex = new int[] { 2, 3 };
        FACE_POS_X.EdgeMidCollIndex = new int[] { 1, 2 };
        FACE_NEG_X.EdgeMidCollIndex = new int[] { 0, 3 };
    }
    #endregion // FaceSetUp

    void OnGUI()
    {
        GUI.color = new Color(1f, 0.1f, 0f, 1f);
        if (Selected) GUI.Label(new Rect(20, 0, 220, 100), ("Selected Block ID: " + this.BlockId));

        GUI.color = new Color(1f, 0.5f, 0f, 1f);
        Vector3 mouseLoc = Manager.CHANGE_IN_MOUSE_LOC;
        GUI.Label(new Rect(20, 20, 220, 100), ("Diff mouse loc - " + "x: " + mouseLoc.x + " y: " + mouseLoc.y + " z: " + mouseLoc.z));
        Vector3 mouseLocWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        //Vector3 mouseLocWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GUI.Label(new Rect(20, 35, 400, 100), ("Diff_m_world - " + "x: " + mouseLocWorld.x + " y: " + mouseLocWorld.y + " z: " + mouseLocWorld.z));

        GUI.color = new Color(0.2f, 0.1f, 0.9f, 1f);
        //GUI.Label(new Rect(20, 35, 400, 100), ("BlockID: " + blockID));
        //Drawing.DrawLabel(FACE_POS_Y.FACE_CENTER_WORLD + new Vector3(0, 0.3f , 0), "BlockID: " + BlockId);
        //Vector2 scre = Camera.main.WorldToScreenPoint(FACE_POS_Y.FACE_CENTER_WORLD);
        //Vector2 scre2 = Camera.main.WorldToScreenPoint(FACE_POS_Y.FACE_CENTER_WORLD + new Vector3(0, 0.3f, 0));
        //Drawing.DrawLine(new Vector2(scre.x, Screen.height - scre.y), new Vector2(scre2.x, Screen.height - scre2.y), Color.red, 2);




        GUI.color = Color.red;
        Drawing.DrawLabel(EDGE_MID_PT0_WORLD, "E_0");
        Drawing.DrawLabel(EDGE_MID_PT1_WORLD, "E_1");
        Drawing.DrawLabel(EDGE_MID_PT2_WORLD, "E_2");
        Drawing.DrawLabel(EDGE_MID_PT3_WORLD, "E_3");

        //Drawing.DrawLabel(vertices[0], "V_0", this.gameObject);
        //Drawing.DrawLabel(vertices[1], "V_1", this.gameObject);
        //Drawing.DrawLabel(vertices[2], "V_2", this.gameObject);
        //Drawing.DrawLabel(vertices[3], "V_3", this.gameObject);

        //Drawing.DrawLabel(vertices[8], "V_4", this.gameObject);
        //Drawing.DrawLabel(vertices[9], "V_5", this.gameObject);
        //Drawing.DrawLabel(vertices[10], "V_6", this.gameObject);
        //Drawing.DrawLabel(vertices[11], "V_7", this.gameObject);

        /*
        GUI.color = Color.green;
        DrawLabel(vertices[4], "V_4");
        DrawLabel(vertices[5], "V_5");
        DrawLabel(vertices[6], "V_6");
        DrawLabel(vertices[7], "V_7");
        
        GUI.color = Color.green;
        DrawLabel(vertices[8], "V_8");
        DrawLabel(vertices[9], "V_9");
        DrawLabel(vertices[10], "V_10");
        DrawLabel(vertices[11], "V_11");
        
        GUI.color = Color.green;
        DrawLabel(vertices[12], "V_12");
        DrawLabel(vertices[13], "V_13");
        DrawLabel(vertices[14], "V_14");
        DrawLabel(vertices[15], "V_15");
        
        GUI.color = Color.green;
        DrawLabel(vertices[16], "V_16");
        DrawLabel(vertices[17], "V_17");
        DrawLabel(vertices[18], "V_18");
        DrawLabel(vertices[19], "V_19");
        
        GUI.color = Color.green;
        DrawLabel(vertices[20], "V_20");
        DrawLabel(vertices[21], "V_21");
        DrawLabel(vertices[22], "V_22");
        DrawLabel(vertices[23], "V_23");
        */

    }

}
