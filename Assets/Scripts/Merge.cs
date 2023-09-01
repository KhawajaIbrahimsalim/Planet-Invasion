using TMPro;
using UnityEngine;

[System.Serializable]
public class Merge : MonoBehaviour
{
    public GameObject NewObj;
    public Color[] colors;
    public int LeverageMeter;
    public TextMeshProUGUI TimesPower_txt;

    public GameObject GameController;
    public int Priority;
    public int ColorIndex = 0;

    private float goldenRatio = 0.618033988749895f;
    private float h = 0.0f;
    private int NumColor = 20;

    // Start is called before the first frame update
    void Start()
    {
        colors = new Color[NumColor];

        GameController = GameObject.Find("GameController");

        Priority = Random.Range(-LeverageMeter, LeverageMeter);

        // This algorith is will give a sequence of colors in a pattern (Note: They are still no randomly generating
        // colors they have a pattern)
        for (int i = 1; i < colors.Length; i++)
        {
            // Set Colors in the colors array
            h += goldenRatio;
            h %= 1;
            colors[i] = Color.HSVToRGB(h, 1f, 1f);

            // Set that material with new color to newObj material
            Transform Child = gameObject.transform.GetChild(1);
            Transform GrandChild = Child.transform.GetChild(1);

            if (GrandChild.gameObject.GetComponent<SkinnedMeshRenderer>().material.color == colors[i])
            {
                ColorIndex = i;       
            }
        }

        // Change Name
        gameObject.name = ColorIndex.ToString();
    }

    //[System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tile") && other.gameObject.GetComponent<TileEmptyStatus>().IsEmpty == true)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    gameObject.transform.SetParent(other.transform, false);

                    gameObject.transform.localPosition = Vector3.zero;

                    other.gameObject.GetComponent<TileEmptyStatus>().IsEmpty = false;
                }
            }
        }

        if (other.CompareTag("Mergable"))
        {
            // First both gameobject and other should have same name and ColorIndex
            if (gameObject.name == other.gameObject.name && TimesPower_txt.text == other.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text && Input.touchCount > 0)
            {
                // Reset the values
                if (ColorIndex + 1 >= colors.Length)
                {
                    ColorIndex = 0;
                }

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

        // Set Damage
        SetTimesPower_txt(newObj);

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

    private void SetTimesPower_txt(GameObject newObj)
    {
        Transform ThisChild = gameObject.transform.GetChild(0);
        Transform NewChild = newObj.transform.GetChild(0);

        NewChild.GetComponent<TextMeshProUGUI>().text = GameController.GetComponent<GameController>().CompressNumber(float.Parse(ThisChild.GetComponent<TextMeshProUGUI>().text) * 2);
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

//Vector3 newPosition = new Vector3(
//                    Mathf.Round((touchPosition.x + offset.x) / gridSize) * gridSize,
//                    Mathf.Round((touchPosition.y + offset.y) / gridSize) * gridSize,
//                    0f
//                );
//transform.position = newPosition;