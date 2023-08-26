using UnityEngine;

[System.Serializable]
public class SpawnNewMergableObjects : MonoBehaviour
{
    [SerializeField] private GameObject ObjToSpawn_ByButton;

    public int count = 0;
    public int Random_Index;
    public bool IsNotFill = true;
    public int NoOfTilesFill;

    private GameObject[] MergableObjects;
    private GameObject[] Tiles;

    // Start is called before the first frame update
    void Start()
    {
        Tiles = GameObject.FindGameObjectsWithTag("Tile");
    }

    // Update is called once per frame
    void Update()
    {
        MergableObjects = GameObject.FindGameObjectsWithTag("Mergable");

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                foreach (var obj in MergableObjects)
                {
                    obj.transform.localPosition = new Vector3(0, 0, 0);
                }
            }
        }

        // Picks a random tile to spawn the object on
        Random_Index = Random.Range(0, Tiles.Length);

        foreach (var tile in Tiles)
        {
            // how many tiles are filled
            if (!tile.GetComponent<TileEmptyStatus>().IsEmpty)
            {
                NoOfTilesFill++;
            }
        }

        // If NoOfTilesFill == Tiles.Length then "while loop" won't start when clicking the
        // button
        if (NoOfTilesFill == Tiles.Length)
        {
            IsNotFill = false;

            NoOfTilesFill = 0;
        }

        else
        {
            IsNotFill = true;

            NoOfTilesFill = 0;
        }
    }

    public void SpawnObject()
    {
        if (GetComponent<GameController>().IsWithInCost == true)
        {
            if (count < Tiles.Length)
            {
                while (IsNotFill)
                {
                    // First check if that index Tile is Empty if is then Spawn if not....
                    if (Tiles[Random_Index].GetComponent<TileEmptyStatus>().IsEmpty == true)
                    {
                        GameObject newObj = Instantiate(ObjToSpawn_ByButton, Tiles[Random_Index].GetComponent<RectTransform>().position, Quaternion.identity);

                        newObj.transform.SetParent(Tiles[Random_Index].transform, false);

                        newObj.transform.localPosition = new Vector3(0, 0, 0);

                        count++;

                        Tiles[Random_Index].GetComponent<TileEmptyStatus>().IsEmpty = false;

                        break; // Break the loop after spawning
                    }


                    // Then it will check else, where it will find the index of a tile where tile is Empty
                    else
                    {
                        for (int i = 0; i < Tiles.Length; i++)
                        {
                            if (Tiles[i].GetComponent<TileEmptyStatus>().IsEmpty == true)
                            {
                                Random_Index = i;
                            }
                        }
                    }
                }
            }

            // Make it false
            GetComponent<GameController>().IsWithInCost = false;
        }    
    }
}
