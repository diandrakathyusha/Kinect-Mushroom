using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
//using Windows.Kinect;


public class JointOverlayer : MonoBehaviour
{
	//	[Tooltip("GUI-texture used to display the color camera feed on the scene background.")]
	//	public GUITexture backgroundImage;

	[Tooltip("Camera that will be used to overlay the 3D-objects over the background.")]
	public Camera foregroundCamera;

	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;

	[Tooltip("Kinect joint that is going to be overlayed.")]
	public KinectInterop.JointType trackedJoint = KinectInterop.JointType.HandRight;

	[Tooltip("Game object used to overlay the joint.")]
	public Transform overlayObject;

	[Tooltip("Smoothing factor used for joint rotation.")]
	public float smoothFactor = 10f;

	// hand states
	private KinectInterop.HandState leftHandState = KinectInterop.HandState.Unknown;
	private KinectInterop.HandState rightHandState = KinectInterop.HandState.Unknown;

	public float moveScaler;
	//public UnityEngine.UI.Text debugText;

	[NonSerialized]
	public Quaternion initialRotation = Quaternion.identity;
	private bool objFlipped = false;


	public void Start()
	{
		//bottleImage = GetComponent<Image>();
		if (!foregroundCamera)
		{
			// by default - the main camera
			foregroundCamera = Camera.main;
		}

		if (overlayObject)
		{
			// always mirrored
			initialRotation = overlayObject.rotation; // Quaternion.Euler(new Vector3(0f, 180f, 0f));

			Vector3 vForward = foregroundCamera ? foregroundCamera.transform.forward : Vector3.forward;
			objFlipped = (Vector3.Dot(overlayObject.forward, vForward) < 0);

			overlayObject.rotation = Quaternion.identity;
		}
	}

	void Update()
	{
		KinectManager manager = KinectManager.Instance;

		if (manager && manager.IsInitialized() && foregroundCamera)
		{
			// overlay the joint
			long userId = manager.GetUserIdByIndex(playerIndex);

			// get the left hand state
			leftHandState = manager.GetLeftHandState(userId);

			// get the right hand state
			rightHandState = manager.GetRightHandState(userId);

			//Debug.Log(leftHandState + " " + rightHandState);

			//if right hand is tracked (whether open or gripped hand), and left hand is not tracked; then the tracked join will be assigned to right hand
			if (leftHandState == KinectInterop.HandState.NotTracked && rightHandState != KinectInterop.HandState.NotTracked)
			{
				trackedJoint = KinectInterop.JointType.HandRight;
			}

			//if left hand is tracked (whether open or gripped hand), and right hand is not tracked; then the tracked join will be assigned to left hand
			else if (rightHandState == KinectInterop.HandState.NotTracked && leftHandState != KinectInterop.HandState.NotTracked)
			{
				trackedJoint = KinectInterop.JointType.HandLeft;
			}

			//else if ()

			//			//backgroundImage.renderer.material.mainTexture = manager.GetUsersClrTex();
			//			if(backgroundImage && (backgroundImage.texture == null))
			//			{
			//				backgroundImage.texture = manager.GetUsersClrTex();
			//			}

			// get the background rectangle (use the portrait background, if available)
			Rect backgroundRect = foregroundCamera.pixelRect;
			PortraitBackground portraitBack = PortraitBackground.Instance;

			if (portraitBack && portraitBack.enabled)
			{
				backgroundRect = portraitBack.GetBackgroundRect();
			}



			int iJointIndex = (int)trackedJoint;
			if (manager.IsJointTracked(userId, iJointIndex))
			{
				Vector3 posJoint = manager.GetJointPosColorOverlay(userId, iJointIndex, foregroundCamera, backgroundRect);
				posJoint.x *= moveScaler;
				posJoint.z = 10f;
				if (posJoint != Vector3.zero)
				{
					//					if(debugText)
					//					{
					//						debugText.text = string.Format("{0} - {1}", trackedJoint, posJoint);
					//					}

					if (overlayObject)
					{
						overlayObject.position = posJoint;

						//Quaternion rotJoint = manager.GetJointOrientation (userId, iJointIndex, !objFlipped);
						//rotJoint = initialRotation * rotJoint;
						Quaternion rotJoint = new Quaternion(0, 0, 0, 0);

						overlayObject.rotation = Quaternion.Slerp(overlayObject.rotation, rotJoint, smoothFactor * Time.deltaTime);

					}
				}
			}
			else
			{
				// make the overlay object invisible
				if (overlayObject && overlayObject.position.z > 0f)
				{
					Vector3 posJoint = overlayObject.position;
					posJoint.z = 1f;
					overlayObject.position = posJoint;


				}
			}

		}
	}
}
