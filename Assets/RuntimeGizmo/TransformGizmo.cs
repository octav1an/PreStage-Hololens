using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using CommandUndoRedo;
using HoloToolkit.Unity.InputModule;

namespace RuntimeGizmos
{
	//To be safe, if you are changing any transforms hierarchy, such as parenting an object to something,
	//you should call ClearTargets before doing so just to be sure nothing unexpected happens... as well as call UndoRedoManager.Clear()
	//For example, if you select an object that has children, move the children elsewhere, deselect the original object, then try to add those old children to the selection, I think it wont work.

	[RequireComponent(typeof(Camera))]
	public class TransformGizmo : MonoBehaviour
    {
		public TransformSpace space = TransformSpace.Global;
        // TODO: make the type to be 2 different types. Move/Rotate and Scale.
		public TransformType type = TransformType.Move;
		public TransformPivot pivot = TransformPivot.Pivot;
		public CenterType centerType = CenterType.All;
		public ScaleType scaleType = ScaleType.FromPoint;

		//These are the same as the unity editor hotkeys
        // TODO: remove later
		//public KeyCode SetMoveType = KeyCode.W;
		//public KeyCode SetRotateType = KeyCode.E;
		//public KeyCode SetScaleType = KeyCode.R;
        //public KeyCode SetSpaceToggle = KeyCode.X;
        public KeyCode SetPivotModeToggle = KeyCode.Z;
        public KeyCode SetCenterTypeToggle = KeyCode.C;
        public KeyCode SetScaleTypeToggle = KeyCode.S;
        public KeyCode AddSelection = KeyCode.LeftShift;
		public KeyCode RemoveSelection = KeyCode.LeftControl;
        public KeyCode ActionKey = KeyCode.LeftShift; //Its set to shift instead of control so that while in the editor we dont accidentally undo editor changes =/
        public KeyCode UndoAction = KeyCode.Z;
        public KeyCode RedoAction = KeyCode.Y;

		public float moveSpeedMultiplier = 1f;
		public float scaleSpeedMultiplier = 1f;
        // TODO: Make a rotate multiplier.
		public float rotateSpeedMultiplier = 200f;

		public bool useFirstSelectedAsMain = true;

		//Mainly for if you want the pivot point to update correctly if Active objects are moving outside the transformgizmo.
		//Might be poor on performance if lots of objects are Active...
		public bool forceUpdatePivotPointOnChange = true;
        public bool InputDown = false;
        public bool InputUp = true;
        public bool DisableGizmo = false;
        public bool FirstTime = false;

        public int maxUndoStored = 100;

        public bool _isTransforming;
        float totalScaleAmount;
		Quaternion totalRotationAmount;
		Axis nearAxis = Axis.None;
        public Axis NEAR_AXIS
        {
            get { return nearAxis;}
        }
		AxisInfo axisInfo;

		Vector3 pivotPoint;
        private Vector3 pivotPointSaeved;
		Vector3 totalCenterPivotPoint;
        private Vector3 totalCenterPivotPointSaved;
        private Vector3 manipulationVec;

		//We use a HashSet and a List for targetRoots so that we get fast lookup with the hashset while also keeping track of the order with the list.
		Transform mainTargetRoot {get {return (targetRootsOrdered.Count > 0) ? (useFirstSelectedAsMain) ? targetRootsOrdered[0] : targetRootsOrdered[targetRootsOrdered.Count - 1] : null;}}
		public List<Transform> targetRootsOrdered = new List<Transform>();
        private Vector3[] targetRootsOrderedSavedPos;
		Dictionary<Transform, TargetInfo> targetRoots = new Dictionary<Transform, TargetInfo>();
		HashSet<Transform> children = new HashSet<Transform>();
		List<Transform> childrenBuffer = new List<Transform>();

		WaitForEndOfFrame waitForEndOFFrame = new WaitForEndOfFrame();
		Coroutine forceUpdatePivotCoroutine;

        public GameObject GizmoGo;
        //======================================
        private Vector3 _savedHitLoc;
        private bool _startedManipulation = false;
        private Vector3 _savedStartedManipulation;
        private Plane _scale3DPlane;
        public string TransformationHandleName;


        #region Unity
        void Awake()
		{

        }

		void OnEnable()
		{
            EventManager.ManipulationUpdated += OnManipulationUpdatedLocal;
		    EventManager.ManipulationCompleted += OnManipulationCompletedLocal;
		    EventManager.NavigationCanceled += OnNavigationCanceledLocal;
            forceUpdatePivotCoroutine = StartCoroutine(ForceUpdatePivotPointAtEndOfFrame());
		}

		void OnDisable()
		{
            EventManager.ManipulationUpdated -= OnManipulationUpdatedLocal;
		    EventManager.ManipulationCompleted -= OnManipulationCompletedLocal;
		    EventManager.NavigationCanceled -= OnNavigationCanceledLocal;
            ClearTargets(); //Just so things gets cleaned up, such as removing any materials we placed on objects.

			StopCoroutine(forceUpdatePivotCoroutine);
		}

		void OnDestroy()
		{
		}

		void Update()
		{
			HandleUndoRedo();
	        UpdateGizmoStatus();

            if (mainTargetRoot == null) return;
			
		    TransformSelected();
		    SetManipulatioPlane();
		}

        void LateUpdate()
        {
            if (mainTargetRoot == null) return;
            // If the ContexMenu is on disable the handles.
            // DisableGizmo - way of controling the gizmo display when Grab mode is active.
            if (!ContexMenu.Instance.IsActive && !DisableGizmo)
            {
                //We run this in lateupdate since coroutines run after update and we want our gizmos to have the updated target transform position after TransformSelected()
                UpdateGizmoGameObject();
            }
        }

        public void OnInputDownLocal()
        {
            InputDown = true;
            InputUp = false;
            _startedManipulation = false;
            // Reset manipulation Vector that is responisble for move transformation.
            manipulationVec = Vector3.zero;
            GetTarget("PREdge", "PRFace");
            // Will run only if this Airtap is not the first one that adds the target.
            if (targetRootsOrdered.Count > 0)
            {
                _savedHitLoc = Manager.Instance.GET_HIT_LOCATION;
                SaveTargetPrevPosition();
            }
            pivotPointSaeved = pivotPoint;
            totalCenterPivotPointSaved = totalCenterPivotPoint;
        }

        public void OnInputUpLocal()
        {
            InputUp = true;
            InputDown = false;
            TransformationHandleName = null;
        }

        private void OnManipulationUpdatedLocal()
        {
            manipulationVec = Manager.Instance.EVENT_MANAGER.EventDataManipulation.CumulativeDelta;
        }

        private void OnManipulationCompletedLocal()
        {

        }

        private void OnNavigationCanceledLocal()
        {
            Debug.Log("Navigation Canceled!!!");
            // Reset highlighted handle.
            // TODO: make sure that the cancel work ok on Hololens.
            nearAxis = Axis.None;
            OnInputUpLocal();
        }

        public void OnClickLocal()
        {
            if (FirstTime)
            {
                FirstTime = false;
            }
        }
        #endregion //Unity

        private void UpdateGizmoStatus()
        {
            if (mainTargetRoot == null)
            {
                GizmoGo.SetActive(false);
                return;
            }

            if (DisableGizmo || ContexMenu.Instance.IsActive)
            {
                GizmoGo.SetActive(false);
            }
            else
            {
                GizmoGo.SetActive(true);
            }
        }

        /// <summary>
        /// Save position of the traget, used to get the difference for maniputations.
        /// </summary>
        public void SaveTargetPrevPosition()
        {
            targetRootsOrderedSavedPos = new Vector3[targetRootsOrdered.Count];

            for (int i = 0; i < targetRootsOrdered.Count; i++)
            {
                targetRootsOrderedSavedPos[i] = new Vector3(targetRootsOrdered[i].position.x,
                    targetRootsOrdered[i].position.y,
                    targetRootsOrdered[i].position.z);
            }
        }

        void HandleUndoRedo()
        {
            if (maxUndoStored != UndoRedoManager.maxUndoStored) { UndoRedoManager.maxUndoStored = maxUndoStored; }

            if (Input.GetKey(ActionKey))
            {
                if (Input.GetKeyDown(UndoAction))
                {
                    UndoRedoManager.Undo();
                }
                else if (Input.GetKeyDown(RedoAction))
                {
                    UndoRedoManager.Redo();
                }
            }
        }

        // TODO: remove later
        //void SetSpaceAndType()
        //{
        //    if (Input.GetKey(ActionKey)) return;

        //    if (Input.GetKeyDown(SetMoveType)) type = TransformType.Move;
        //    else if (Input.GetKeyDown(SetRotateType)) type = TransformType.Rotate;
        //    else if (Input.GetKeyDown(SetScaleType)) type = TransformType.Scale;

        //    if (Input.GetKeyDown(SetPivotModeToggle))
        //    {
        //        if (pivot == TransformPivot.Pivot) pivot = TransformPivot.Center;
        //        else if (pivot == TransformPivot.Center) pivot = TransformPivot.Pivot;

        //        SetPivotPoint();
        //    }

        //    if (Input.GetKeyDown(SetCenterTypeToggle))
        //    {
        //        if (centerType == CenterType.All) centerType = CenterType.Solo;
        //        else if (centerType == CenterType.Solo) centerType = CenterType.All;

        //        SetPivotPoint();
        //    }

        //    if (Input.GetKeyDown(SetSpaceToggle))
        //    {
        //        if (space == TransformSpace.Global) space = TransformSpace.Local;
        //        else if (space == TransformSpace.Local) space = TransformSpace.Global;
        //    }

        //    if (Input.GetKeyDown(SetScaleTypeToggle))
        //    {
        //        if (scaleType == ScaleType.FromPoint) scaleType = ScaleType.FromPointOffset;
        //        else if (scaleType == ScaleType.FromPointOffset) scaleType = ScaleType.FromPoint;
        //    }

        //    if (type == TransformType.Scale)
        //    {
        //        //space = TransformSpace.Local; //Only support local scale
        //        if (pivot == TransformPivot.Pivot) scaleType = ScaleType.FromPoint; //FromPointOffset can be inaccurate and should only really be used in Center mode if desired.
        //    }
        //}

        #region TransformMethods

        void TransformSelected()
        {
            if (mainTargetRoot != null)
            {
                if (Manager.Instance.GET_COLLIDER_GO && Manager.Instance.GET_COLLIDER_GO.layer == 8 && InputDown)
                {
                    StartCoroutine(PRTransform());
                    InputDown = false;
                }
            }
        }

        IEnumerator PRTransform()
        {
            _isTransforming = true;
            TransformationHandleName = Manager.Instance.GET_COLLIDER_NAME;
            // Define what axis to project on
            string transformType = Manager.Instance.GET_COLLIDER_TAG;
            Vector3 translateAxis = GetTranslateAxis();
            Plane translatePlane = GetTranslatePlane();
            Vector3 savedProjectedOnPlane = translatePlane.ClosestPointOnPlane(_savedHitLoc);
            Quaternion quaSavedRotation = mainTargetRoot.localRotation;
            // Scale save
            Vector3 savedScale = mainTargetRoot.localScale;
            Vector3 scaleAxis = GetScaleAxis();
            Vector3 scaleAxisLocal = GetScaleAxisLocal();
            GameObject scaleCube = null;
            Vector3 scaleCubeLoc = Vector3.zero;
            Vector3 scaleCubeScale = Vector3.zero;

            if (Manager.Instance.GET_COLLIDER_TAG == "GizmoScale" ||
                Manager.Instance.GET_COLLIDER_TAG == "GizmoScaleCenter")
            {
                scaleCube = Manager.Instance.GET_COLLIDER_GO;
                scaleCubeLoc = scaleCube.transform.position;
                scaleCubeScale = scaleCube.transform.localScale;
            }
            while (!InputUp)
            {
                if (transformType == "GizmoMove")
                {
                    //Debug.Log("Move");
                    // Project
                    float moveAmount = ExtVector3.MagnitudeInDirection(manipulationVec, translateAxis) *
                                       moveSpeedMultiplier;
                    for (int i = 0; i < targetRootsOrdered.Count; i++)
                    {
                        Transform target = targetRootsOrdered[i];
                        // Save target position, in worder to add the movement Vector. Translate gives a continuos transformation.
                        Vector3 targetSavedPos = targetRootsOrderedSavedPos[i];
                        target.position = targetSavedPos + translateAxis * moveAmount;
                    }
                }
                else if (transformType == "GizmoPlaneMove")
                {
                    //Debug.Log("PlaneMove");
                    // Project the Manipulation vector onto the plane.
                    Vector3 projectedOnPlane = Vector3.ProjectOnPlane(manipulationVec, translatePlane.normal);
                    // Apply the projected manipulation vector for movement.
                    for (int i = 0; i < targetRootsOrdered.Count; i++)
                    {
                        Transform target = targetRootsOrdered[i];
                        // Save target position, in worder to add the movement Vector. Translate gives a continuos transformation.
                        Vector3 targetSavedPos = targetRootsOrderedSavedPos[i];
                        target.position = targetSavedPos + projectedOnPlane * moveSpeedMultiplier;
                    }
                }
                else if (transformType == "GizmoRotate")
                {
                    //Debug.Log("Rotate");
                    // Get the world location of the manipolation in respect to the savedHit location.
                    Vector3 worldManip = savedProjectedOnPlane + manipulationVec;
                    // Project the worlManip to the translatePlane which has Gizmo coordinates.
                    Vector3 projectedOnPlane = translatePlane.ClosestPointOnPlane(worldManip);
                    // Get the quaterion with rotation between the savedHit and the actualHit that have the origin as the 0,0,0 coordinate, and not the gizmo.
                    Quaternion quaRotation = Quaternion.FromToRotation(savedProjectedOnPlane - GizmoGo.transform.position, projectedOnPlane - GizmoGo.transform.position);
                    // Apply quaternion.
                    for (int i = 0; i < targetRootsOrdered.Count; i++)
                    {
                        Transform target = targetRootsOrdered[i];
                        if (space == TransformSpace.Global)
                        {
                            target.rotation = quaRotation * quaSavedRotation;
                            // Update the Gizmo rotation while doing the global rotation.
                            GizmoGo.transform.rotation = quaRotation;
                        }
                        else if (space == TransformSpace.Local)
                        {
                            target.localRotation = quaRotation * quaSavedRotation;
                        }  
                    }
                }
                else if (transformType == "GizmoScale")
                {
                    //Debug.Log("Scale");
                    float scaleAmount = ExtVector3.MagnitudeInDirection(manipulationVec, scaleAxis) *
                                       scaleSpeedMultiplier;
                    for (int i = 0; i < targetRootsOrdered.Count; i++)
                    {
                        Transform target = targetRootsOrdered[i];
                        target.localScale = savedScale + scaleAxis * scaleAmount;
                        // Update the Gizmo position for scale cubes and scale rects.
                        scaleCube.transform.position = scaleCubeLoc + (scaleAxisLocal * scaleAmount);
                    }
                }
                else if (transformType == "GizmoScaleCenter")
                {
                    // Get the porjection of the manipulation vector on the predifined plane during manipulation start.
                    Vector3 closest = _scale3DPlane.ClosestPointOnPlane(GizmoGo.transform.position + manipulationVec);
                    float magScale = 0;
                    // To have the scale go both ways (larger and smaller) check the angle between manipulation vector and plane normal.
                    if (Vector3.Angle(_scale3DPlane.normal, manipulationVec) < 90)
                    {
                        magScale = (closest - (GizmoGo.transform.position + manipulationVec)).magnitude;
                    }
                    else
                    {
                        magScale = -(closest - (GizmoGo.transform.position + manipulationVec)).magnitude;
                    }
                    for (int i = 0; i < targetRootsOrdered.Count; i++)
                    {
                        Transform target = targetRootsOrdered[i];
                        target.localScale = savedScale + new Vector3(magScale, magScale, magScale);
                        // Update the Gizmo position for scale cubes and scale rects.
                        scaleCube.transform.localScale = scaleCubeScale + new Vector3(magScale, magScale, magScale) * 2;
                    }
                }
                // Update the Gizmo location while doing the global rotation.
                GizmoGo.transform.position = mainTargetRoot.transform.position;
                yield return null;
            }
            // Reset scale elements.
            if (scaleCube)
            {
                scaleCube.transform.position = scaleCubeLoc;
                scaleCube.transform.localScale = scaleCubeScale;
            }

            _isTransforming = false;
        }

        Plane GetTranslatePlane()
        {
            if (Manager.Instance.GET_COLLIDER_NAME == "plane_translate_xy" ||
                Manager.Instance.GET_COLLIDER_NAME == "rotate_z")
            {
                return new Plane(GizmoGo.transform.forward, GizmoGo.transform.position);
            }
            else if (Manager.Instance.GET_COLLIDER_NAME == "plane_translate_yz" ||
                     Manager.Instance.GET_COLLIDER_NAME == "rotate_x")
            {
                return new Plane(GizmoGo.transform.right, GizmoGo.transform.position);
            }
            else if (Manager.Instance.GET_COLLIDER_NAME == "plane_translate_xz" ||
                     Manager.Instance.GET_COLLIDER_NAME == "rotate_y")
            {
                return new Plane(GizmoGo.transform.up, GizmoGo.transform.position);
            }
            else
            {
                return new Plane();
            }
        }

        Vector3 GetTranslateAxis()
        {
            if (Manager.Instance.GET_COLLIDER_NAME == "translate_x")
            {
                return GizmoGo.transform.right;
            }
            else if (Manager.Instance.GET_COLLIDER_NAME == "translate_z")
            {
                return GizmoGo.transform.forward;
            }
            else if (Manager.Instance.GET_COLLIDER_NAME == "translate_y")
            {
                return GizmoGo.transform.up;
            }
            else
            {
                return Vector3.zero;
            }
            
        }

        Vector3 GetScaleAxis()
        {
            if (space == TransformSpace.Local || space == TransformSpace.Global)
            {
                if (Manager.Instance.GET_COLLIDER_NAME == "scale_x")
                {
                    return new Vector3(1, 0, 0);
                }
                else if (Manager.Instance.GET_COLLIDER_NAME == "scale_z")
                {
                    return new Vector3(0, 0, 1);
                }
                else if (Manager.Instance.GET_COLLIDER_NAME == "scale_y")
                {
                    return new Vector3(0, 1, 0);
                }
                else
                {
                    return Vector3.zero;
                }
            }
            else
            {
                return Vector3.zero;
            }
        }

        Vector3 GetScaleAxisLocal()
        {
            if (Manager.Instance.GET_COLLIDER_NAME == "scale_x")
            {
                return GizmoGo.transform.right;
            }
            else if (Manager.Instance.GET_COLLIDER_NAME == "scale_z")
            {
                return GizmoGo.transform.forward;
            }
            else if (Manager.Instance.GET_COLLIDER_NAME == "scale_y")
            {
                return GizmoGo.transform.up;
            }
            else
            {
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Save a plane with the normal of manipulation vector, when the manipulation starts. I use it to scale both ways in 3D scale.
        /// </summary>
        private void SetManipulatioPlane()
        {
            if (manipulationVec != Vector3.zero && !_startedManipulation)
            {
                _startedManipulation = true;
                _savedStartedManipulation = manipulationVec;
                _scale3DPlane = new Plane(_savedStartedManipulation, GizmoGo.transform.position);
            }
        }
        #endregion // TransformMethods

        private void UpdateGizmoGameObject()
        {
            if (mainTargetRoot != null)
            {
                if (space == TransformSpace.Global && type == TransformType.Scale)
                {
                    GizmoGo.transform.position = mainTargetRoot.transform.position;
                    GizmoGo.transform.rotation = mainTargetRoot.transform.localRotation;
                }
                else if (space == TransformSpace.Global && !_isTransforming)
                {
                    GizmoGo.transform.position = mainTargetRoot.transform.position;
                    GizmoGo.transform.rotation = Quaternion.identity;
                }
                else if (space == TransformSpace.Local)
                {
                    GizmoGo.transform.position = mainTargetRoot.transform.position;
                    GizmoGo.transform.rotation = mainTargetRoot.transform.localRotation;
                }
            }
        }

        /// <summary>
        /// Modified vertion of GetTarget to work with specific tag1 or object.
        /// </summary>
        /// <param name="tag1">Tag name of the targeted object.</param>
        void GetTarget(string tag1, string tag2)
        {
            //if (nearAxis == Axis.None)
            if (Manager.Instance.GET_COLLIDER_LAYER != "Gizmo")
            {
                RaycastHit hit;
                Ray ray = GazeManager.Instance.Rays[0];

                if (Manager.Instance.IS_HIT)
                {
                    Transform target = Manager.Instance.GET_COLLIDER_GO.transform;
                    if (Manager.Instance.GET_COLLIDER_TAG == tag1 || Manager.Instance.GET_COLLIDER_TAG == tag2)
                    {
                        ClearAndAddTarget(target);
                        return;
                    }
                }
            }
        }

        void GetTarget(Ray ray)
		{
			if(nearAxis == Axis.None)
			{
				bool isAdding = Input.GetKey(AddSelection);
				bool isRemoving = Input.GetKey(RemoveSelection);

				RaycastHit hitInfo; 
				if(Physics.Raycast(ray, out hitInfo))
				{
					Transform target = hitInfo.transform;

					if(isAdding)
					{
						AddTarget(target);
					}
					else if(isRemoving)
					{
						RemoveTarget(target);
					}
					else if(!isAdding && !isRemoving)
					{
						ClearAndAddTarget(target);
					}
				}else{
					if(!isAdding && !isRemoving)
					{
						ClearTargets();
					}
				}
			}
		}

		public void AddTarget(Transform target, bool addCommand = true)
		{
			if(target != null)
			{
				if(targetRoots.ContainsKey(target)) return;
				if(children.Contains(target)) return;

				if(addCommand) UndoRedoManager.Insert(new AddTargetCommand(this, target, targetRootsOrdered));

				AddTargetRoot(target);
			    //AddTargetHighlightedRenderers(target);

				SetPivotPoint();
			}
		}

		public void RemoveTarget(Transform target, bool addCommand = true)
		{
			if(target != null)
			{
				if(!targetRoots.ContainsKey(target)) return;

				if(addCommand) UndoRedoManager.Insert(new RemoveTargetCommand(this, target));

				RemoveTargetRoot(target);

				SetPivotPoint();
			}
		}

		public void ClearTargets(bool addCommand = true)
		{
			if(addCommand) UndoRedoManager.Insert(new ClearTargetsCommand(this, targetRootsOrdered));

			targetRoots.Clear();
			targetRootsOrdered.Clear();
			children.Clear();
		}

		public void ClearAndAddTarget(Transform target)
		{
			UndoRedoManager.Insert(new ClearAndAddTargetCommand(this, target, targetRootsOrdered));
            // Trigger the first time boolean, to ignore any manipulations.
		    FirstTime = true;
            print("Add");
            ClearTargets(false);
			AddTarget(target, false);
		}

		void AddTargetRoot(Transform targetRoot)
		{
			targetRoots.Add(targetRoot, new TargetInfo());
			targetRootsOrdered.Add(targetRoot);

			AddAllChildren(targetRoot);
		}
		void RemoveTargetRoot(Transform targetRoot)
		{
			if(targetRoots.Remove(targetRoot))
			{
				targetRootsOrdered.Remove(targetRoot);

				RemoveAllChildren(targetRoot);
			}
		}

		void AddAllChildren(Transform target)
		{
			childrenBuffer.Clear();
			target.GetComponentsInChildren<Transform>(true, childrenBuffer);
			childrenBuffer.Remove(target);

			for(int i = 0; i < childrenBuffer.Count; i++)
			{
				Transform child = childrenBuffer[i];
				children.Add(child);
				RemoveTargetRoot(child); //We do this in case we Active child first and then the parent.
			}

			childrenBuffer.Clear();
		}
		void RemoveAllChildren(Transform target)
		{
			childrenBuffer.Clear();
			target.GetComponentsInChildren<Transform>(true, childrenBuffer);
			childrenBuffer.Remove(target);

			for(int i = 0; i < childrenBuffer.Count; i++)
			{
				children.Remove(childrenBuffer[i]);
			}

			childrenBuffer.Clear();
		}

		public void SetPivotPoint()
		{
			if(mainTargetRoot != null)
			{
				if(pivot == TransformPivot.Pivot)
				{
					pivotPoint = mainTargetRoot.position;
				}
				else if(pivot == TransformPivot.Center)
				{
					totalCenterPivotPoint = Vector3.zero;

					Dictionary<Transform, TargetInfo>.Enumerator targetsEnumerator = targetRoots.GetEnumerator(); //We avoid foreach to avoid garbage.
					while(targetsEnumerator.MoveNext())
					{
						Transform target = targetsEnumerator.Current.Key;
						TargetInfo info = targetsEnumerator.Current.Value;
						info.centerPivotPoint = target.GetCenter(centerType);

						totalCenterPivotPoint += info.centerPivotPoint;
					}

					totalCenterPivotPoint /= targetRoots.Count;

					if(centerType == CenterType.Solo)
					{
						pivotPoint = targetRoots[mainTargetRoot].centerPivotPoint;
					}
					else if(centerType == CenterType.All)
					{
						pivotPoint = totalCenterPivotPoint;
					}
				}
			}
		}
		void SetPivotPointOffset(Vector3 offset)
		{
			pivotPoint += offset;
			totalCenterPivotPoint += offset;
		}

        void UpdatePivotPositionHL(Vector3 movement)
        {
            pivotPoint = pivotPointSaeved + movement;
            totalCenterPivotPoint = totalCenterPivotPointSaved + movement;
        }

		IEnumerator ForceUpdatePivotPointAtEndOfFrame()
		{
			while(this.enabled)
			{
				ForceUpdatePivotPointOnChange();
				yield return waitForEndOFFrame;
			}
		}

		void ForceUpdatePivotPointOnChange()
		{
			if(forceUpdatePivotPointOnChange)
			{
				if(mainTargetRoot != null && !_isTransforming)
				{
					bool hasSet = false;
					Dictionary<Transform, TargetInfo>.Enumerator targets = targetRoots.GetEnumerator();
					while(targets.MoveNext())
					{
						if(!hasSet)
						{
							if(targets.Current.Value.previousPosition != Vector3.zero && targets.Current.Key.position != targets.Current.Value.previousPosition)
							{
								SetPivotPoint();
								hasSet = true;
							}
						}

						targets.Current.Value.previousPosition = targets.Current.Key.position;
					}
				}
			}
		}

    }
}
