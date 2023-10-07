using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class DrawingUI : MonoBehaviour
{

    #region PUBLIC VARS
    public DrawingPen pen;
    public GameObject surfaceScanPanel;
    public GameObject drawingPanel;
    public ARPlaneManager arPlaneManager;
    public bool ModeAR;
    #endregion

    #region PRIVATES
    [SerializeField] Text unitText;
    const float UIHeight = 60;      // this is the height of the drawingPanel
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        // Define the boundry of the drawing pen prevent conflict UI touches
        SetPenDrawingBound();
        if (ModeAR)
        {
            // 1
            SetdrawingUIVisible(false);
            SetCoachingUIVisible(true);

            // 2
            arPlaneManager.planesChanged += ChangeInPlanes;
        }
        else
        {
            // 3
            SetdrawingUIVisible(true);
            SetCoachingUIVisible(false);
        }
    }
    void SetPenDrawingBound()
    {
        if (pen == null)
        {
            return;
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            return;
        }

        pen.drawingBound = (canvas.scaleFactor * UIHeight);
    }
    public void OnClearClicked()
    {
        if (pen != null)
        {
            pen.ClearLines();
        }
    }
    public void OnColorClicked(int index)
    {
        if (pen != null)
        {
            pen.ChangeColorIndex(index);
        }
    }
    public void SetCoachingUIVisible(bool flag)
    {
        surfaceScanPanel.SetActive(flag);
    }
    public void SetdrawingUIVisible(bool flag)
    {
        drawingPanel.SetActive(flag);
    }
    public void OnLineWidthChange(float value)
    {

        if (pen != null)
        {
            pen.ChangeLineWidth(value * 0.001f);
        }

        if (unitText != null)
        {
            unitText.text = string.Format("{0:0.0} cm", (value * 0.1f));
        }
    }

    //Plane Changed
    private void ChangeInPlanes(ARPlanesChangedEventArgs planeEvent)
    {
        if (planeEvent.added.Count > 0 || planeEvent.updated.Count > 0)
        {
            SetdrawingUIVisible(true);
            SetCoachingUIVisible(false);

            arPlaneManager.planesChanged -= ChangeInPlanes;
        }
    }
}
