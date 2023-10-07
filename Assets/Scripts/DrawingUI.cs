using UnityEngine;
using UnityEngine.UI;

public class DrawingUI : MonoBehaviour
{

    #region PUBLIC VARS
    public DrawingPen pen;
    public GameObject surfaceScanPanel;
    public GameObject drawingPanel;
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

        // Setup for Non AR. Directly show the drawing UI 
        SetdrawingUIVisible(true);
        SetCoachingUIVisible(false);
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

        pen.drawingBound = canvas.scaleFactor * UIHeight;
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
}
