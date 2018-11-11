using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class BlockFace : MonoBehaviour, IInputHandler
{

    public int BLOCK_ID
    {
        get
        {
            return BLOCK_COMP.BlockId;
        }
    }
    public BlockPrim BLOCK_COMP
    {
        get
        {
            return transform.GetComponentInParent<BlockPrim>();
        }
    }
    public bool FaceActive = false;
    public Vector3[] FaceVerts;
    /// <summary>
    /// Face center location for the local center. To get the global position call transform.position
    /// </summary>
    public Vector3 FACE_CENTER
    {
        get
        {
            if (FaceVerts.Length == 4)
            {
                Vector3 faceCenter = (FaceVerts[0] + FaceVerts[1] + FaceVerts[2] + FaceVerts[3]) / 4;
                return faceCenter;
            }
            else
            {
                return new Vector3();
            }
        }
    }
    /// <summary>
    /// Face center location for world space.
    /// </summary>
    public Vector3 FACE_CENTER_WORLD
    {
        get
        {
            return BLOCK_COMP.transform.TransformPoint(FACE_CENTER);
        }
    }
    private Vector3 _savedFaceCenter;
    /// <summary>
    /// Saved center location when the face was clicked. Used for moving face verts.
    /// </summary>
    private Vector3 SAVED_FACE_CENTER_WORLD
    {
        get
        {
            return BLOCK_COMP.transform.TransformPoint(_savedFaceCenter);
        }
    }
    /// <summary>
    /// Indexes of the mid edge points of the face, setup in the BlockPrim.
    /// </summary>
    public int[] EdgeMidCollIndex;
    /// <summary>
    /// Edge mid points of this face, referenced from parent BlockPrim.
    /// </summary>
    public Vector3[] FACE_EDGE_MID_COLL
    {
        get
        {
            Vector3[] coll = new Vector3[2];
            coll[0] = BLOCK_COMP.EDGE_MID_COLL[EdgeMidCollIndex[0]];
            coll[1] = BLOCK_COMP.EDGE_MID_COLL[EdgeMidCollIndex[1]];
            return coll;
        }
    }
    /// <summary>
    /// These are the 4 container indexes that hold face's vertices and the adjacent face's verts that
    /// share the same location.
    /// </summary>
    public int[] VertexIndexCon;
    public Vector3 FaceNormal;
    /// <summary>
    /// Get the face normal direction in world space.
    /// </summary>
    public Vector3 FACE_NORMAL_WORLD
    {
        get
        {
            return BLOCK_COMP.gameObject.transform.TransformDirection(FaceNormal);
        }
    }
    /// <summary>
    /// Return vector that is the projection of the target vector, for move porpuses.
    /// </summary>
    private Vector3 PROJECTED_TARGET
    {
        get
        {
            Vector3 prj = Vector3.Project(BLOCK_COMP.TARGET_WORLD, FACE_NORMAL_WORLD);
            return BLOCK_COMP.transform.position + prj;
        }
    }
    /// <summary>
    /// Save the PROJECTED_TARGET in order to create the offset necessary when starting to move the face.
    /// Getting the difference between PROJECTED_TARGET and savedProjectedTarget with cancel the jumping effect.
    /// </summary>
    private Vector3 _savedProjectedTarget;
    /// <summary>
    /// Vector between savedProjectedTarget and actualProjectedTarget, this is done in
    /// order to avoid jumping the face location when the mouse is clicked.
    /// </summary>
    private Vector3 DYNAMIC_DIFF
    {
        get
        {
            return PROJECTED_TARGET - _savedProjectedTarget;
        }
    }

    private bool _snapZoneOn;
    // Exact Status
    private bool _exactCornerSnap;
    private bool _exactFaceSnap;
    // Influence Zone
    private bool _cornerSnapZone;
    private bool _faceSnapZone;
    // Drag fields. Vector is used to store DYNAMIC_DIFF when snapping.
    private Vector3 _diffMove;
    private bool _dragThrough;

    //---------------------------------------------------------------------------------------------------
    // Use this for initialization
    void Start()
    {
        _savedFaceCenter = FACE_CENTER;
        _savedProjectedTarget = PROJECTED_TARGET;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFaceLoc(BLOCK_COMP.Selected);
        if (BLOCK_COMP.Selected && FaceActive)
        {
            if (this.name != "face_pos_y" && this.name != "face_neg_y")
            {
                _snapZoneOn = ActivateSnapZone(0.1f, 0.1f);
            }
        }
    }

    private void LateUpdate()
    {
        // Runs after void Update.
        if (BLOCK_COMP.Selected && FaceActive)
        {
            if (this.name != "face_pos_y" && this.name != "face_neg_y")
            {
                if (!_snapZoneOn)
                {
                    _diffMove = DYNAMIC_DIFF;
                    MoveFace(BLOCK_COMP.ColliderName);
                    _dragThrough = false;
                    //print("01");
                }
                else if (_snapZoneOn && !_dragThrough)
                {
                    if (!_exactCornerSnap && _cornerSnapZone && !_exactFaceSnap)
                    {
                        MoveSnapFace(0.1f, 0.1f);
                        //print("02");
                    }
                    else if (!_exactFaceSnap && _faceSnapZone)
                    {
                        MoveSnapFace(0.1f, 0.1f);
                        //print("03");
                    }
                    else if (_exactFaceSnap && _cornerSnapZone)
                    {
                        MoveSnapFace(0.1f, 0.1f);
                        //print("04");
                    }
                }
                else if (_snapZoneOn && _dragThrough)
                {
                    if (_exactCornerSnap)
                    {
                        _diffMove = DYNAMIC_DIFF;
                        MoveFace(BLOCK_COMP.ColliderName);
                    }
                    // If I am in cornerSnapZone, snap to corner and deactivate the drag bool.
                    else if (_cornerSnapZone && _exactFaceSnap)
                    {
                        MoveSnapFace(0.1f, 0.1f);
                        _dragThrough = false;
                        //print("05");
                    }
                    else
                    {
                        _diffMove = DYNAMIC_DIFF;
                        MoveFace(BLOCK_COMP.ColliderName);
                        //print("06");
                    }
                }
                // If the mouse is moved after snap, turn on drag bool.
                if (Mathf.Abs(DYNAMIC_DIFF.magnitude - _diffMove.magnitude) > 0.1f)
                {
                    _dragThrough = true;
                }
                //if (BLOCK_COMP.name == "Block" && this.name == "face_pos_z") print("snap: " + MoveSnapFace(0.1f, 0.1f, 0));
                if (this.name == "face_pos_z")
                {
                    //print("snapMag: " + snapDiffMag);
                }
            }
        }
    }

    //---------------------------------------------------------------------------------------------------
    /// <summary>
    /// Method that is called in BlockPrim when MouseDown event was triggered.
    /// </summary>
    public void UpdateOnMouseDown()
    {
        _savedFaceCenter = FACE_CENTER;
        _savedProjectedTarget = PROJECTED_TARGET;
        if(BLOCK_COMP.ColliderName == this.name) FaceActive = true;
    }

    public void UpdateOnMouseUp()
    {
        _diffMove = new Vector3();
         FaceActive = false;
    }

    //---------------------------------------------HOLOLENS INPUTS------------------------------------------------------
    public void OnInputDown(InputEventData eventData)
    {
        //UpdateOnMouseDown();
    }

    public void OnInputUp(InputEventData eventData)
    {
        //UpdateOnMouseUp();
    }
    //---------------------------------------------HOLOLENS INPUTS------------------------------------------------------

    //---------------------------------------------------------------------------------------------------
    /// <summary>
    /// Method to move the face. Works only of the vertical faces.
    /// </summary>
    /// <param name="colliderName">Collider that is hit, if it is the same as this obj then run the function.</param>
    public void MoveFace(string colliderName)
    {
        if (this.name == colliderName)
        {
            //if (BLOCK_COMP.name == "Block" && this.name == "face_pos_z") print("diff: " + diff.magnitude);
            if(Input.GetKeyDown(KeyCode.Z)){
                //Vector3 diffMod = diff;
                //diffMod.Normalize();
                //print("diffMod: " + (diff + (diffMod * 10f)));
                //diffMove += diffMod * 0.2f;
            }
            for (int i = 0; i < VertexIndexCon.Length; i++)
            {
                // Get the container index (Check Rhino file for these indexes).
                int contIndex = VertexIndexCon[i];
                // Get the actual container that stores vertix index with the same coordinates.
                int[] vertexIndex = BLOCK_COMP.VertexIndexCon[contIndex];
                for (int j = 0; j < vertexIndex.Length; j++)
                {
                    // Get the index of vertex from BLOCK_COMP.vertex_index_con - this represent 3 vertices 
                    // per container that share the same location
                    int index = vertexIndex[j];
                    // Get the actual vertex from mesh.
                    // When moving a face, in addition to its 4 vertices, this will move adjacent face's verts.

                    // Before adding the "diff" vector convert it to local space, otherwise it won't work when the block is rotated.
                    BLOCK_COMP.Vertices[index] = BLOCK_COMP.VerticesSaved[index] + BLOCK_COMP.transform.InverseTransformVector(DYNAMIC_DIFF);
                    // If snap face is active move it accordingly
                }
            }
            // Update block vertices with freshly moved ones.
            BLOCK_COMP.BlockMesh.vertices = BLOCK_COMP.Vertices;
            BLOCK_COMP.UpdateProximityCollider();
        }
    }

    //---------------------------------------------------------------------------------------------------
    /// <summary>
    /// Method to move the face with adding the 'move' vector, used for faceSnap. Works only of the vertical faces.
    /// </summary>
    /// <param name="colliderName">Collider that is hit, if it is the same as this obj then run the function.</param>
    /// <param name="move">Vector to add to the move vector, used for snap jump.</param>
    public void MoveFace(string colliderName, Vector3 move)
    {
        if (this.name == colliderName)
        {
            // First get the vector between savedProjectedTarget and actualProjectedTarget, this is done in
            // order to avoid jumping the face location when the mouse is clicked.
            //Vector3 diff = (PROJECTED_TARGET + move) - savedProjectedTarget;
            Vector3 diff = _diffMove + move;
            for (int i = 0; i < VertexIndexCon.Length; i++)
            {
                // Get the container index (Check Rhino file for these indexes).
                int contIndex = VertexIndexCon[i];
                // Get the actual container that stores vertix index with the same coordinates.
                int[] vertexIndex = BLOCK_COMP.VertexIndexCon[contIndex];
                for (int j = 0; j < vertexIndex.Length; j++)
                {
                    // Get the index of vertex from BLOCK_COMP.vertex_index_con - this represent 3 vertices 
                    // per container that share the same location
                    int index = vertexIndex[j];
                    // Get the actual vertex from mesh.
                    // When moving a face, in addition to its 4 vertices, this will move adjacent face's verts.

                    //BLOCK_COMP.vertices[index] += moveDir * 0.05f;
                    
                    //if (BLOCK_COMP.name == "Block" && this.name == "face_pos_z") print(move.magnitude);
                    
                    // Before adding the "diff" vector convert it to local sapace, otherwise it won't work when the block is rotated.
                    BLOCK_COMP.Vertices[index] = BLOCK_COMP.VerticesSaved[index] + BLOCK_COMP.transform.InverseTransformVector(diff);
                    // If snap face is active move it accordingly
                }
            }
            // Update block vertices with freshly moved ones.
            BLOCK_COMP.BlockMesh.vertices = BLOCK_COMP.Vertices;
            BLOCK_COMP.UpdateProximityCollider();
        }
    }

    //---------------------------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// 0. Proximity objects list
    /// 1. Move Vector, used to move the object
    /// 2. Closest projected vector
    /// 3. Closest projected vector index, i need it to get vector from this object
    /// 4. Closest distance (float)
    /// 5. Array with projections on the proximity objects.
    /// </summary>
    /// <param name="proxiIndex"></param>
    /// <returns></returns>
    private List<object> Snap(int proxiIndex = 0)
    {
        List<object> returnObj = new List<object>();
        // 1.Get the collider from the positive proximity distance
        List<GameObject> closeBlocksColl = BLOCK_COMP.PROXI_COLLIDER.closeBlocksColl;
        returnObj.Add(closeBlocksColl);
        // 2.Find the closest loc for each edge mid pt on each collider.
        Vector3 closestVec = new Vector3();
        float closestDist = 1000;
        int closestIndex = -1;
        Vector3[] collClosest = new Vector3[2];
        if (closeBlocksColl.Count > 0)
        {
            // Check the closest vector from this to the proximity objects.
            for (int i = 0; i < FACE_EDGE_MID_COLL.Length; i++)
            {
                collClosest[i] = closeBlocksColl[proxiIndex].GetComponent<BoxCollider>().ClosestPoint(FACE_EDGE_MID_COLL[i]);
                if ((collClosest[i] - FACE_EDGE_MID_COLL[i]).magnitude < closestDist)
                {
                    closestDist = (collClosest[i] - FACE_EDGE_MID_COLL[i]).magnitude;
                    closestVec = FACE_EDGE_MID_COLL[i];
                    closestIndex = i;
                }
            }
        }
        // The vector between EdgePt and projection on ProxiObj of this EdgePt.
        Vector3 moveVector = new Vector3();
        if (closeBlocksColl.Count > 0)
        {
            moveVector = collClosest[closestIndex] - closestVec;
        }
        returnObj.Add(moveVector);
        returnObj.Add(closestVec);
        returnObj.Add(closestIndex);
        returnObj.Add(closestDist);
        returnObj.Add(collClosest);

        return returnObj;
    }

    //---------------------------------------------------------------------------------------------------
    private void MoveSnapFace(float snapDist, float cornerSnap)
    {
        List<GameObject> list = BLOCK_COMP.PROXI_COLLIDER.closeBlocksColl;
        Vector3 moveVec2 = new Vector3();
        if (list.Count > 0)
        {
            // Find shortest vector.
            float minDist = 1000;
            float cornerSnapDist = 1000;
            Vector3 closestEdge = new Vector3();
            Vector3 closestEdgeProxi = new Vector3();
            for (int i = 0; i < list.Count; i++)
            {
                List<object> snapList = Snap(i);
                Vector3 closeVec = (Vector3)snapList[1];
                if(closeVec.magnitude < minDist)
                {
                    minDist = closeVec.magnitude;
                    moveVec2 = closeVec;
                }
                //--------------------------------------------------------------------------
                // Find the closest edge of this obj and the coresponded closest edge of proxi obj that fits
                // the snapDist comparison. (This part is used in the Corner Snap only.)
                for (int j = 0; j < FACE_EDGE_MID_COLL.Length; j++)
                {
                    Vector3 edgeMidThis = FACE_EDGE_MID_COLL[j];
                    //list[i].GetComponent<BlockPrim>().EDGE_MID_COLL.Length
                    for (int h = 0; h < 4; h++)
                    {
                        Vector3 edgeMidProxi = list[i].GetComponent<BlockPrim>().EDGE_MID_COLL[h];
                        if ((edgeMidThis - edgeMidProxi).magnitude < cornerSnapDist)
                        {
                            cornerSnapDist = (edgeMidThis - edgeMidProxi).magnitude;
                            closestEdge = edgeMidThis;
                            closestEdgeProxi = list[i].GetComponent<BlockPrim>().EDGE_MID_COLL[h];
                        }
                    }
                }
            }
            //print("mindist: " + Vector3.Project(moveVec2, FACE_NORMAL_WORLD).magnitude);
            //--------------------------------------------------------------------------
            // 1. Corner snap has the most priority.
            if (cornerSnapDist < cornerSnap)
            {
                // Project the move vector on the face normal, to avoid shift and break the block right angles.
                Vector3 move = Vector3.Project(closestEdgeProxi - closestEdge, FACE_NORMAL_WORLD);
                //exactCornerSnap = true;
                MoveFace(BLOCK_COMP.ColliderName, move);
                //if (BLOCK_COMP.name == "Block" && this.name == "face_pos_z") print("Zero [" + "]: " + move.magnitude);
                return;
            }
            // 2. Apply face snap from this as priority.
            // Doesn't properly work.
            else if (Vector3.Project(moveVec2, FACE_NORMAL_WORLD).magnitude < snapDist)
            {
                // Specify the proxiIndex in order for Snap() to correctly calculate closest distance.
                Vector3 move = Vector3.Project(moveVec2, FACE_NORMAL_WORLD);
                //exactFaceSnap = true;
                MoveFace(BLOCK_COMP.ColliderName, move);
                //if (BLOCK_COMP.name == "Block" && this.name == "face_pos_z") print("First [" + "]: " + move.magnitude);
                return;
            }
            else
            {
                print("No snap -2- !");
                MoveFace(BLOCK_COMP.ColliderName);
                return;
            }
        }

        MoveFace(BLOCK_COMP.ColliderName);
        print("No snap!");
    }

    //---------------------------------------------------------------------------------------------------
    /// <summary>
    /// Method that updates the Snap status on each face when it is active. It updates the exactSnap staus for both
    /// corner and face, and also the snapZone status for corner and face.
    /// </summary>
    /// <param name="snapDist">Snap distance used for face snap.</param>
    /// <param name="cornerSnap">Snap distance used for corner snap.</param>
    /// <returns></returns>
    private bool ActivateSnapZone(float snapDist, float cornerSnap)
    {
        List<GameObject> list = BLOCK_COMP.PROXI_COLLIDER.closeBlocksColl;
        Vector3 moveVec2 = new Vector3();
        if (list.Count > 0)
        {
            // Find shortest vector.
            float minDist = 1000;
            float cornerSnapDist = 1000;
            //Vector3 closestEdge = new Vector3();
            //Vector3 closestEdgeProxi = new Vector3();
            for (int i = 0; i < list.Count; i++)
            {
                List<object> snapList = Snap(i);
                Vector3 closeVec = (Vector3)snapList[1];
                if (closeVec.magnitude < minDist)
                {
                    minDist = closeVec.magnitude;
                    moveVec2 = closeVec;
                }
                //--------------------------------------------------------------------------
                // Find the closest edge of this obj and the coresponded closest edge of proxi obj that fits
                // the snapDist comparison. (This part is used in the Corner Snap only.)
                for (int j = 0; j < FACE_EDGE_MID_COLL.Length; j++)
                {
                    Vector3 edgeMidThis = FACE_EDGE_MID_COLL[j];
                    //list[i].GetComponent<BlockPrim>().EDGE_MID_COLL.Length
                    for (int h = 0; h < 4; h++)
                    {
                        Vector3 edgeMidProxi = list[i].GetComponent<BlockPrim>().EDGE_MID_COLL[h];
                        if ((edgeMidThis - edgeMidProxi).magnitude < cornerSnapDist)
                        {
                            cornerSnapDist = (edgeMidThis - edgeMidProxi).magnitude;
                            //closestEdge = edgeMidThis;
                            //closestEdgeProxi = list[i].GetComponent<BlockPrim>().EDGE_MID_COLL[h];
                        }
                    }
                }
            }
            //--------------------------------------------------------------------------
            // 1. Corner snap has the most priority.
            if (cornerSnapDist < cornerSnap)
            {
                _cornerSnapZone = true;
                _faceSnapZone = false;
                if (cornerSnapDist < 0.0001f)
                {
                    _exactCornerSnap = true;
                    _exactFaceSnap = false;
                }
                    //if (BLOCK_COMP.name == "Block" && this.name == "face_pos_z") print("Zero [" + "]: " + cornerSnapDist);
                    return true;
            }
            // 2. Apply face snap from this as priority.
            else if (Vector3.Project(moveVec2, FACE_NORMAL_WORLD).magnitude < snapDist)
            {
                _cornerSnapZone = false;
                _faceSnapZone = true;
                if (Vector3.Project(moveVec2, FACE_NORMAL_WORLD).magnitude < 0.0001f)
                {
                    _exactCornerSnap = false;
                    _exactFaceSnap = true;
                }
                //if (BLOCK_COMP.name == "Block" && this.name == "face_pos_z") print("First [" + "]: " + minDist);
                return true;
            }
            else
            {
                // Influence Snap Zone
                _cornerSnapZone = false;
                _faceSnapZone = false;
                // Exact snaps.
                _exactCornerSnap = false;
                _exactFaceSnap = false;
                //print("No snap -2- !");
                return false;
            }
        }
        else
        {
            // Influence Snap Zone
            _cornerSnapZone = false;
            _faceSnapZone = false;
            // Exact snaps.
            _exactCornerSnap = false;
            _exactFaceSnap = false;
            //print("No snap!");
            return false;
        }
    }

    //---------------------------------------------------------------------------------------------------
    /// <summary>
    /// When the cube is selected constanly update the this.transform to match the FACE_CENTER.
    /// </summary>
    /// <param name="selected">Boolean from BlockPrim class, is true if the block is selected.</param>
    public void UpdateFaceLoc(bool selected)
    {
        if (selected)
        {
            this.transform.localPosition = FACE_CENTER;
        }
    }

    //---------------------------------------------------------------------------------------------------
    private void OnGUI()
    {
        GUI.color = Color.blue;
        //Drawing.DrawLabel(transform.position, gameObject.name, 80);
    }


}
