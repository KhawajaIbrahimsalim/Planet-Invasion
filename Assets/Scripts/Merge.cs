using UnityEngine;

[System.Serializable]
public class Merge : MonoBehaviour
{
    public GameObject NewObj;
    public int LeverageMeter;

    public GameObject GameController;   
    public int Priority;

    // Start is called before the first frame update
    void Start()
    {
        GameController = GameObject.Find("GameController");

        Priority = Random.Range(-LeverageMeter, LeverageMeter);
    }

    private void OnTriggerEnter(Collider other)
    {
        // First both gameobject and other should have same name
        if (gameObject.name == other.name && Input.touchCount > 0)
        {
            // If both are equal then we will again search for a random number
            if (Priority == other.gameObject.GetComponent<Merge>().Priority)
            {
                Priority = Random.Range(other.gameObject.GetComponent<Merge>().Priority + 1, LeverageMeter + 2);
            }

            // If this gameobject has the greater priority than give this gameobject the TAG
            if (Priority > other.gameObject.GetComponent<Merge>().Priority && Priority != other.gameObject.GetComponent<Merge>().Priority)
            {
                gameObject.tag = "Secondary";
            }

            if (gameObject.tag == "Secondary" && other.tag != "Secondary") // Next: Also if not equal to the same color or number or anything that is different from each other
            {
                GameObject newObj = Instantiate(NewObj);

                if (gameObject.transform.localPosition == new Vector3(0, 0, 0))
                {
                    // Make the new GameObject child of the (this) Gameobject's parent
                    newObj.transform.parent = gameObject.transform.parent;

                    // Set newObj position
                    newObj.transform.localPosition = Vector3.zero;

                    // "other" is empty because it has move towards "gameobject", so that means
                    // the Tile that "other" was on is empty now
                    GameObject Parent = other.transform.parent.gameObject;
                    Parent.GetComponent<TileEmptyStatus>().IsEmpty = true;
                }

                else if (other.transform.localPosition == new Vector3(0, 0, 0))
                {
                    // Make the new GameObject child of the (this) Gameobject's parent
                    newObj.transform.parent = other.gameObject.transform.parent;

                    // Set newObj position
                    newObj.transform.localPosition = Vector3.zero;

                    // "gameobject" is empty because it has move towards "other", so that means
                    // the Tile that "gameobject" was on is empty now
                    GameObject Parent = gameObject.transform.parent.gameObject;
                    Parent.GetComponent<TileEmptyStatus>().IsEmpty = true;
                }

                if (GameController != null)
                {
                    // Decrease the count as one slot is now empty
                    GameController.GetComponent<SpawnNewMergableObjects>().count--;
                }
            }

            Destroy(other.gameObject);

            Destroy(gameObject);
        }
    }
}
