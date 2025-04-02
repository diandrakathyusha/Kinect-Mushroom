using UnityEngine;
using System.Collections;

public class JumpTrigger : MonoBehaviour 
{
	[Tooltip("Spore Prefab to Spawn")]
	public GameObject sporePrefab;

	[Tooltip("Spawn Area Size")]
	public Vector3 spawnArea = new Vector3(1f, 0.5f, 1f);

	void OnTriggerEnter()
	{
		//Debug.Log ("Jump trigger activated");

		// start the animation clip
		Animation animation = gameObject.GetComponent<Animation>();
		if(animation != null && !animation.isPlaying)
		{
			animation.Play();
		}

		// play the audio clip
		AudioSource audioSrc = gameObject.GetComponent<AudioSource>();
		if(audioSrc != null && !audioSrc.isPlaying)
		{
			audioSrc.Play();
		}

		SpawnSpore();
	}

	void SpawnSpore()
	{
		Vector3 spawnPos = transform.position + new Vector3(
			Random.Range(-spawnArea.x, spawnArea.x),
			Random.Range(0, spawnArea.y),
			Random.Range(-spawnArea.z, spawnArea.z)
		);

		Instantiate(sporePrefab, spawnPos, Quaternion.identity);
	}
}
