using UnityEngine;
using System.Collections;
using com.wayfair;
using UnityEngine.EventSystems;

/// <summary>
/// A very naive product manipulation script that lets users rotate products and zoom into them.
/// </summary>
public class ProductRotator : MonoBehaviour {

    public GameObject product;
    private Vector3 originalRotationAngles;
    private Vector2 firstClickPos;
    public Camera cam;
	
	void Update () {
        if (EventSystem.current.IsPointerOverGameObject ()) {
            return;
        }
        if (Input.GetMouseButton (0)) {
            if (this.firstClickPos == Vector2.zero) {
                this.firstClickPos = Input.mousePosition;
                this.originalRotationAngles = this.product.transform.eulerAngles;
            }
            float aroundX = getDegreesFromMouseY (Input.mousePosition.y, firstClickPos.y);
            float aroundY = getDegreesFromMouseX (Input.mousePosition.x, firstClickPos.x);
            this.product.transform.eulerAngles = new Vector3(originalRotationAngles.x, originalRotationAngles.y - aroundY, originalRotationAngles.z);
			this.product.transform.RotateAround (this.product.transform.position, Vector3.right, aroundX);
        } else {
            this.firstClickPos = Vector2.zero;
        }

        if (Input.GetKey (KeyCode.UpArrow) || Input.GetAxis("Mouse ScrollWheel") > 0) {
            this.moveCameraIn ();
        }

        if (Input.GetKey (KeyCode.DownArrow) || Input.GetAxis("Mouse ScrollWheel") < 0) {
            this.moveCameraOut ();
        }
	}

    float getDegreesFromMouseX(float newPos, float oldPos) {
        return ((newPos - oldPos) / Screen.width) * 360;
    }

    float getDegreesFromMouseY(float newPos, float oldPos) {
        return ((newPos - oldPos) / Screen.height) * 360;
    }

    public void moveCameraIn() {
		cam.transform.position = Vector3.MoveTowards (cam.transform.position, Vector3.zero, 0.1f);
    }

    public void moveCameraOut() {
		cam.transform.position = Vector3.MoveTowards (cam.transform.position, new Vector3(0, 1f, -4), 0.1f);
    }

    public void reset() {
        product.transform.position = new Vector3 (0, 0, 0);
        product.transform.eulerAngles = new Vector3 (0, 0, 0);
    }
}
