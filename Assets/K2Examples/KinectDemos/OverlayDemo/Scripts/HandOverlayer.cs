
using UnityEngine;
using System.Collections;
//using Windows.Kinect;


public class HandOverlayer : MonoBehaviour
{
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;

	[Tooltip("Whether the player's left hand should be tracked, or the right hand.")]
	public bool isLeftHanded = false;

	//	[Tooltip("GUI-texture used to display the color camera feed on the scene background.")]
	//	public GUITexture backgroundImage;

	[Tooltip("Hand-cursor texture for the hand-grip state.")]
	public Texture gripHandTexture;
	[Tooltip("Hand-cursor texture for the hand-release state.")]
	public Texture releaseHandTexture;
	[Tooltip("Hand-cursor texture for the non-tracked state.")]
	public Texture normalHandTexture;

	[Tooltip("Smooth factor for cursor movement.")]
	public float smoothFactor = 10f;

	// current cursor position
	private Vector2 cursorPos = Vector2.zero;

	// last hand event (grip or release)
	private InteractionManager.HandEventType lastHandEvent = InteractionManager.HandEventType.None;
	private int collectedItems = 0;
	public LevelManager levelManager;

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


	// ----- end of public functions -----


	void Update()
	{
		KinectManager manager = KinectManager.Instance;
		if (manager && manager.IsInitialized())
		{
			//			//backgroundImage.renderer.material.mainTexture = manager.GetUsersClrTex();
			//			if(backgroundImage && (backgroundImage.texture == null))
			//			{
			//				backgroundImage.texture = manager.GetUsersClrTex();
			//			}

			// overlay the joint
			int iJointIndex = !isLeftHanded ? (int)KinectInterop.JointType.HandRight : (int)KinectInterop.JointType.HandLeft;

			if (manager.IsUserDetected(playerIndex))
			{
				long userId = manager.GetUserIdByIndex(playerIndex);

				if (manager.IsJointTracked(userId, iJointIndex))
				{
					Vector3 posJointRaw = manager.GetJointKinectPosition(userId, iJointIndex);
					//Vector3 posJoint = manager.GetJointPosColorOverlay(userId, iJointIndex, Camera.main, Camera.main.pixelRect);

					if (posJointRaw != Vector3.zero)
					{
						Vector2 posDepth = manager.MapSpacePointToDepthCoords(posJointRaw);
						ushort depthValue = manager.GetDepthForPixel((int)posDepth.x, (int)posDepth.y);

						if (posDepth != Vector2.zero && depthValue > 0)
						{
							// depth pos to color pos
							Vector2 posColor = manager.MapDepthPointToColorCoords(posDepth, depthValue);

							if (!float.IsInfinity(posColor.x) && !float.IsInfinity(posColor.y))
							{
								// get the color image x-offset and width (use the portrait background, if available)
								float colorWidth = manager.GetColorImageWidth();
								float colorOfsX = 0f;

								PortraitBackground portraitBack = PortraitBackground.Instance;
								if (portraitBack && portraitBack.enabled)
								{
									colorWidth = manager.GetColorImageHeight() * manager.GetColorImageHeight() / manager.GetColorImageWidth();
									colorOfsX = (manager.GetColorImageWidth() - colorWidth) / 2f;
								}

								float xScaled = (posColor.x - colorOfsX) / colorWidth;
								float yScaled = posColor.y / manager.GetColorImageHeight();

								cursorPos = Vector2.Lerp(cursorPos, new Vector2(xScaled, 1f - yScaled), smoothFactor * Time.deltaTime);
								Vector3 worldCursorPos = Camera.main.ViewportToWorldPoint(new Vector3(cursorPos.x, cursorPos.y, Camera.main.nearClipPlane));

								LayerMask collectibleLayer = LayerMask.GetMask("Collectibles");
								Collider2D hitCollider = Physics2D.OverlapPoint(new Vector2(worldCursorPos.x, worldCursorPos.y), collectibleLayer);

								if (hitCollider && hitCollider.CompareTag("Collectible"))
								{
									CollectItem(hitCollider.gameObject);								}

							}
						}
					}
				}

			}

		}
	}


	private void CollectItem(GameObject item)
	{
		Debug.Log("Item Collected: " + item.name);
		Destroy(item);  // Remove the item
		collectedItems++;

		// Notify LevelManager for completion check
		levelManager.CheckCollectionCompletion(collectedItems);
	}
	void OnGUI()
	{
		InteractionManager intManager = InteractionManager.Instance;
		Texture texture = null;

		if (intManager && intManager.IsInteractionInited())
		{
			if (isLeftHanded)
			{
				lastHandEvent = intManager.GetLastLeftHandEvent();

				if (lastHandEvent == InteractionManager.HandEventType.Grip)
					texture = gripHandTexture;
				else if (lastHandEvent == InteractionManager.HandEventType.Release)
					texture = releaseHandTexture;
			}
			else
			{
				lastHandEvent = intManager.GetLastRightHandEvent();

				if (lastHandEvent == InteractionManager.HandEventType.Grip)
					texture = gripHandTexture;
				else if (lastHandEvent == InteractionManager.HandEventType.Release)
					texture = releaseHandTexture;
			}
		}

		if (texture == null)
		{
			texture = normalHandTexture;
		}

		if ((cursorPos != Vector2.zero) && (texture != null))
		{
			//handCursor.transform.position = cursorScreenPos; // Vector3.Lerp(handCursor.transform.position, cursorScreenPos, 3 * Time.deltaTime);
			Rect rectTexture = new Rect(cursorPos.x * Screen.width - texture.width / 2, (1f - cursorPos.y) * Screen.height - texture.height / 2,
										texture.width, texture.height);
			GUI.DrawTexture(rectTexture, texture);
		}
	}


}