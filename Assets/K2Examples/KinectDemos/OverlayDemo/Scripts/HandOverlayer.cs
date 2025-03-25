using UnityEngine;
using System.Collections;

public class HandOverlayer : MonoBehaviour
{
    [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, etc.")]
    public int playerIndex = 0;

    [Tooltip("Whether the player's left hand should be tracked, or the right hand.")]
    public bool isLeftHanded = false;

    [Tooltip("Smooth factor for cursor movement.")]
    public float smoothFactor = 10f;

    [Tooltip("Particle system overlay for the hand.")]
    public GameObject handParticles;

    private Vector2 cursorPos = Vector2.zero;
    private InteractionManager.HandEventType lastHandEvent = InteractionManager.HandEventType.None;

    /// <summary>
    /// Gets the cursor position.
    /// </summary>
    /// <returns>The cursor position.</returns>
    public Vector2 GetCursorPos()
    {
        return cursorPos;
    }

    /// <summary>
    /// Gets the last hand event of the active hand (right or left).
    /// </summary>
    /// <returns>The last hand event.</returns>
    public InteractionManager.HandEventType GetLastHandEvent()
    {
        return lastHandEvent;
    }

    void Start()
    {
        if (handParticles == null)
        {
            Debug.LogError("Please assign a Particle System to 'handParticles'.");
        }
    }

    void Update()
    {
        KinectManager manager = KinectManager.Instance;
        if (manager && manager.IsInitialized())
        {
            int iJointIndex = isLeftHanded ? (int)KinectInterop.JointType.HandLeft : (int)KinectInterop.JointType.HandRight;

            if (manager.IsUserDetected(playerIndex))
            {
                long userId = manager.GetUserIdByIndex(playerIndex);

                if (manager.IsJointTracked(userId, iJointIndex))
                {
                    Vector3 jointPos = manager.GetJointKinectPosition(userId, iJointIndex);
                    if (jointPos != Vector3.zero)
                    {
                        Vector2 depthPos = manager.MapSpacePointToDepthCoords(jointPos);
                        ushort depthValue = manager.GetDepthForPixel((int)depthPos.x, (int)depthPos.y);

                        if (depthPos != Vector2.zero && depthValue > 0)
                        {
                            Vector2 colorPos = manager.MapDepthPointToColorCoords(depthPos, depthValue);

                            if (!float.IsInfinity(colorPos.x) && !float.IsInfinity(colorPos.y))
                            {
                                float colorWidth = manager.GetColorImageWidth();
                                float colorOfsX = 0f;

                                PortraitBackground portraitBack = PortraitBackground.Instance;
                                if (portraitBack && portraitBack.enabled)
                                {
                                    colorWidth = manager.GetColorImageHeight() * manager.GetColorImageHeight() / manager.GetColorImageWidth();
                                    colorOfsX = (manager.GetColorImageWidth() - colorWidth) / 2f;
                                }

                                float xScaled = (colorPos.x - colorOfsX) / colorWidth;
                                float yScaled = colorPos.y / manager.GetColorImageHeight();

                                cursorPos = Vector2.Lerp(cursorPos, new Vector2(xScaled, 1f - yScaled), smoothFactor * Time.deltaTime);

                                // Update the particle system position
                                Vector3 screenPos = new Vector3(cursorPos.x * Screen.width, (1f - cursorPos.y) * Screen.height, Camera.main.nearClipPlane + 2f);
                                Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

                                handParticles.transform.position = worldPos;
                            }
                        }
                    }
                }
            }
        }
    }
}
