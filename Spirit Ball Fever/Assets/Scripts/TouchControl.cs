using UnityEngine;

[System.Serializable]
public class TouchControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Variables to keep track of the dragging state and the selected object
    bool dragging = false;
    GameObject selectedObject;

    void Update()
    {
        // Check if there is at least one touch on the screen
        if (Input.touchCount > 0)
        {
            // Get the first touch
            Touch touch = Input.GetTouch(0);
            // Check if the touch has just begun
            if (touch.phase == TouchPhase.Began)
            {
                // Cast a ray from the camera to the touch position
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                // Check if the ray hits an object with a collider
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag == "Mergable")
                    {
                        // Set the selected object to the hit object and set dragging to true
                        selectedObject = hit.collider.gameObject;
                        dragging = true;
                    }
                }
            }
            // Check if the touch has moved and an object is being dragged
            else if (touch.phase == TouchPhase.Moved && dragging)
            {
                if (selectedObject)
                {
                    // Get the touch position in world coordinates
                    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    // Update the position of the selected object to follow the touch position
                    selectedObject.transform.position = new Vector3(touchPosition.x, touchPosition.y, selectedObject.transform.position.z);
                }
            }
            // Check if the touch has ended
            else if (touch.phase == TouchPhase.Ended)
            {
                // Set dragging to false to stop dragging the object
                dragging = false;
            }
        }
    }
}
