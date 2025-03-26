using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LinePainter : MonoBehaviour
{
    [Tooltip("Line renderer used for the line drawing.")]
    public LineRenderer linePrefab;

    [Tooltip("UI-Text to display information messages.")]
    public UnityEngine.UI.Text infoText;

    [Tooltip("Hand particle system for the drawing effect.")]
    public ParticleSystem handParticles;

    private HandOverlayer handOverlayer = null;
    private List<GameObject> linesDrawn = new List<GameObject>();
    private LineRenderer currentLine;
    private int lineVertexIndex = 2;

    void Start()
    {
        handOverlayer = GetComponent<HandOverlayer>();

        if (handParticles == null)
        {
            Debug.LogError("Please assign a Particle System to 'handParticles'.");
        }
        else
        {
            handParticles.Stop();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            // U-key means Undo
            DeleteLastLine();
        }

        // Display info message when a user is detected
        KinectManager manager = KinectManager.Instance;
        if (manager && manager.IsInitialized() && manager.IsUserDetected())
        {
            if (infoText)
            {
                infoText.text = "Grip hand to start drawing. Press [U] to undo the last line.";
            }
        }

        if (currentLine == null)
        {
            StartNewLine();
        }

        if (currentLine != null &&
            (handOverlayer != null && (handOverlayer.GetLastHandEvent() == InteractionManager.HandEventType.Release)))
        {
            // End drawing lines
            StopDrawing();
        }

        UpdateParticlePosition();
    }

    // Start a new line
    private void StartNewLine()
    {
        currentLine = Instantiate(linePrefab).GetComponent<LineRenderer>();
        currentLine.name = "Line" + linesDrawn.Count;
        currentLine.transform.parent = transform;

        Vector3 cursorPos = handOverlayer.GetCursorPos();
        cursorPos.z = Camera.main.nearClipPlane;
        Vector3 cursorSpacePos = Camera.main.ViewportToWorldPoint(cursorPos);

        currentLine.SetPosition(0, cursorSpacePos);
        currentLine.SetPosition(1, cursorSpacePos);

        lineVertexIndex = 2;
        linesDrawn.Add(currentLine.gameObject);

        StartCoroutine(DrawLine());
        handParticles.Play(); // Start the hand particles when drawing starts
    }

    // Stop drawing lines
    private void StopDrawing()
    {
        currentLine = null;
        handParticles.Stop(); // Stop the hand particles when drawing stops
    }

    // Update the position of the particle system
    private void UpdateParticlePosition()
    {
        if (handParticles.isPlaying && handOverlayer != null)
        {
            Vector3 cursorPos = handOverlayer.GetCursorPos();
            cursorPos.z = Camera.main.nearClipPlane;
            Vector3 cursorSpacePos = Camera.main.ViewportToWorldPoint(cursorPos);

            handParticles.transform.position = cursorSpacePos;
        }
    }

    // Undo the last drawn line
    public void DeleteLastLine()
    {
        if (linesDrawn.Count > 0)
        {
            GameObject lastLine = linesDrawn[linesDrawn.Count - 1];
            linesDrawn.RemoveAt(linesDrawn.Count - 1);
            Destroy(lastLine);
        }
    }

    public void DeleteAllLines()
    {
        for (int i = linesDrawn.Count - 1; i >= 0; i--)
        {
            GameObject line = linesDrawn[i];
            Destroy(line);
        }

        linesDrawn.Clear(); // Clear the list after deleting all lines
    }

    // Continue drawing line
    IEnumerator DrawLine()
    {
        while (handOverlayer != null && (handOverlayer.GetLastHandEvent() != InteractionManager.HandEventType.Release))
        {
            yield return new WaitForEndOfFrame();

            if (currentLine != null)
            {
                lineVertexIndex++;
                currentLine.positionCount = lineVertexIndex;

                Vector3 cursorPos = handOverlayer.GetCursorPos();
                cursorPos.z = Camera.main.nearClipPlane;

                Vector3 cursorSpacePos = Camera.main.ViewportToWorldPoint(cursorPos);
                currentLine.SetPosition(lineVertexIndex - 1, cursorSpacePos);
            }
        }
    }
}
