using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class DrawingPen : MonoBehaviour
{
    #region PUBLIC VARS
    public const float UIHeight = 60;        // the screen height of the UI 
    new public Camera camera;
    public DrawingLine linePrefab = null;
    public Gradient[] colorTheme = null;
    public ARRaycastManager raycastManager;
    public bool modeAR = false;
    public float drawingBound = 0;      // The lower bound where touch is valid
    #endregion

    #region PRIVATES
    private int mySelectedColorIndex = 0;
    private float myLineWidth = 0.005f;
    private int mySortingOrder = 1;
    #endregion
    
    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            // skip the touches fall on the UI
            if (Input.mousePosition.y < drawingBound)
            {
                return;
            }

            SpawnNewLine();
        }
    }

    Gradient getLineGradient()
    {
        return colorTheme[mySelectedColorIndex];
    }

    public void ChangeColorIndex(int index)
    {
        mySelectedColorIndex = index;
    }

    public void ChangeLineWidth(float lineWidth)
    {
        myLineWidth = lineWidth;
    }

    public void SpawnNewLine()
    {
        if (linePrefab == null)
        {
            return;
        }

        var newLine = Instantiate(linePrefab);
        SetupRaycastLogic(newLine);

        newLine.lineGradient = getLineGradient();
        newLine.SetLineOrder(mySortingOrder);
        newLine.ChangeLineWidth(myLineWidth);

        Transform t = newLine.transform;
        t.parent = transform;

        mySortingOrder++;
    }
    public void ClearLines()
    {
        DrawingLine[] lines = GetComponentsInChildren<DrawingLine>();
        // Debug.Log("ClearLines: lines.count=" + lines.Length);
        foreach (DrawingLine line in lines)
        {
            //  Debug.Log("line: " + line);
            Destroy(line.gameObject);
        }
    }
    void SetupRaycastLogic(DrawingLine drawingLine)
    {
        if (modeAR)
        {
            drawingLine.raycastDelegate = ARRaycastLogic;
        }
        else
        {
            drawingLine.raycastDelegate = NonArRaycastLogic;
        }
        drawingLine.gameObject.SetActive(true);
    }
    bool NonArRaycastLogic(out Vector3 hitPosition)
    {
        var point = Input.mousePosition;


        Ray ray = camera.ScreenPointToRay(point);

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
    //AR Raycast Logic
    bool ARRaycastLogic(out Vector3 hitPosition)
    {
        var hits = new List<ARRaycastHit>();
        bool hasHit = raycastManager.Raycast(Input.mousePosition, hits, TrackableType.PlaneWithinInfinity);

        if (hasHit == false || hits.Count == 0)
        {
            hitPosition = Vector3.zero;
            return false;
        }
        else
        {
            hitPosition = hits[0].pose.position;
            return true;
        }
    }

}
