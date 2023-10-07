using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingLine : MonoBehaviour
{
    #region CONST VARS
    const float ThresholdForNewPoint = 0.00001f;
    #endregion

    #region DELEGATES
    public delegate bool RaycastDelegate(out Vector3 hitPosition);

    #endregion

    #region PUBLIC VARIABLES
    public bool isActive = true;
    new public Camera camera = null;
    public Gradient lineGradient;
    public float lineWidth = 0.001f;
    public RaycastDelegate raycastDelegate = null;
    #endregion

    #region PROTECTED
    protected LineRenderer lineRenderer;
    protected List<Vector3> vPoints = new List<Vector3>();
    #endregion

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(raycastDelegate == null)
        {
            raycastDelegate = DefaultRaycastLogic;
        }
        if(lineGradient != null)
        {
            lineRenderer.colorGradient = lineGradient;
        }
    }


    private bool DefaultRaycastLogic(out Vector3 hitPosition)
    {
        var pointToRaycast = Input.mousePosition;


        Ray ray = camera.ScreenPointToRay(pointToRaycast);
        // Debug.DrawRay(point, ray.direction, Color.red, 30); // for debug

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            hitPosition = hit.point;
            return true;
        }
        else
        {
            hitPosition = Vector3.zero;
            return false;
        }
    }

    public void SetLineOrder(int order)
    {
        lineRenderer.sortingOrder = order;
    }


    public void ChangeLineWidth(float newValue)
    {
        lineWidth = newValue;

        if (lineRenderer == null)
        {
            return;
        }
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }
    // Update is called once per frame
    void Update()
    {
        if(isActive == false)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            DrawPoint();
        }

        // preventing drawing when the mouse is up
        if (Input.GetMouseButtonUp(0))
        {
            if (lineRenderer.positionCount == 1)
            {
                DrawPoint(true);             // Add an extra point if the line is not complete
            }
            isActive = false;
        }
    }

    private void DrawPoint(bool forceAdd = true)
    {
        Vector3 hitPos;
        bool hasHit = raycastDelegate(out hitPos);
        if (hasHit)
        {
            AddPointToRender(hitPos.x, hitPos.y, hitPos.z, forceAdd);
        }
    }

    private void AddPointToRender(float x, float y, float z, bool forceAdd)
    {
        Vector3 vec = new Vector3(x, y, z);

        if (forceAdd == false && GetNewPointDelta(vec) < ThresholdForNewPoint)
        {
            return; // skip this new point
        }


        vPoints.Add(vec);


        // Refresh the renderer data 
        lineRenderer.positionCount = vPoints.Count;
        for (int i = 0; i < vPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, vPoints[i]);
        }
    }

    private float GetNewPointDelta(Vector3 newPoint)
    {
        if (vPoints.Count == 0)
        {
            return float.MaxValue;
        }
        return (vPoints[vPoints.Count - 1] - newPoint).sqrMagnitude;
    }
}

