using UnityEngine;
using System.Collections;

public class GazeHelper : MonoBehaviour {

	private GameObject previousObject;
	private Vector3 originalPosition;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity)) {
			if (hit.collider.gameObject.name == "New Game Object") {
				if (previousObject != null) {
					previousObject.transform.position = originalPosition;
				}
				originalPosition = hit.collider.gameObject.transform.position;
				previousObject = hit.collider.gameObject;
				Vector3 changedPosition = transform.position;
				changedPosition.z += 3.0f;
				changedPosition.y -= 2.0f;
				hit.collider.gameObject.transform.position = changedPosition;
				hit.collider.gameObject.transform.Rotate (new Vector3 (0, 60, 0) * Time.deltaTime);
			}
		}
	}
}
