using TMPro;
using UnityEngine;

[System.Serializable]
public class Merge : MonoBehaviour
{
    public GameObject NewObj;
    public Color[] colors;
    public int LeverageMeter;

    public GameObject GameController;
    public int Priority;
    public int ColorIndex;

    // Start is called before the first frame update
    void Start()
    {
        GameController = GameObject.Find("GameController");

        Priority = Random.Range(-LeverageMeter, LeverageMeter);

        for (int i = 0; i < colors.Length; i++)
        {
            // Set that material with new color to newObj material
            Transform Child = gameObject.transform.GetChild(1);
            Transform GrandChild = Child.transform.GetChild(1);

            if (GrandChild.gameObject.GetComponent<SkinnedMeshRenderer>().material.color == colors[i])
            {
                ColorIndex = i;
            }
        }
    }

    //[System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mergable")) 
        {
            // First both gameobject and other should have same ColorIndex
            if (gameObject.name == other.gameObject.name && Input.touchCount > 0)
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
                    // Spawn New Object
                    SpawnObject(other);
                }

                Destroy(other.gameObject);

                Destroy(gameObject);
            }
        }
    }

    private void SpawnObject(Collider other)
    {
        GameObject newObj = Instantiate(NewObj);

        // Set Tag
        newObj.tag = "Mergable";

        // Change Name
        newObj.name = ColorIndex.ToString();

        // Set Damage
        SetDamage(newObj);

        // Set Material
        SetMaterial(newObj);

        if (gameObject.transform.localPosition == new Vector3(0, 0, 0))
        {
            // Make the new GameObject child of the (this) Gameobject's parent
            newObj.transform.SetParent(gameObject.transform.parent, false);

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
            newObj.transform.SetParent(other.gameObject.transform.parent, false);

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

    private void SetDamage(GameObject newObj)
    {
        Transform ThisChild = gameObject.transform.GetChild(0);
        Transform NewChild = newObj.transform.GetChild(0);

        NewChild.GetComponent<TextMeshProUGUI>().text = (float.Parse(ThisChild.GetComponent<TextMeshProUGUI>().text) * 2).ToString("0");
    }

    private void SetMaterial(GameObject newObj)
    {
        // Create a new Material
        Material material = new Material(Shader.Find("Standard"));

        // Set color to the material
        material.color = colors[++ColorIndex];

        // Set that material with new color to newObj material
        Transform Child = newObj.transform.GetChild(1);
        Transform GrandChild = Child.transform.GetChild(1);

        // Attach the material
        GrandChild.gameObject.GetComponent<SkinnedMeshRenderer>().material = material;
    }
}