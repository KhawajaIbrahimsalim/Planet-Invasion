using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SaveData
{
    public GameObject[] Tiles;
    public GameObject[] Mergable;
    public bool[] IsEmpty;
    public float TileLength;
    public int count;
    public bool IsNotFill;
    public int NoOfTilesFill;

    public float Health;
}

[System.Serializable]
public class SaveLoad : MonoBehaviour
{
    public GameObject[] Tiles;
    public GameObject[] Mergable;
    public GameObject[] MergablePrefab;
    public GameObject Planet;
    public GameObject HealthBar;

    private string savePath;
    GameObject mergable;

    private void Awake()
    {
        Tiles = GameObject.FindGameObjectsWithTag("Tile");
    }

    void Start()
    {
        savePath = Application.persistentDataPath + "/Max92.json";

        File.Delete(savePath);

        // Load:

        if (File.Exists(savePath))
        {
            // Load data from Json
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            Debug.Log("Load");

            // Load Health
            Planet.GetComponent<PlanetCollisionEventSystem>().Health = data.Health;

            // Show Health
            HealthBar.GetComponent<Slider>().value = Planet.GetComponent<PlanetCollisionEventSystem>().Health / Planet.GetComponent<PlanetCollisionEventSystem>().MaxHealth;

            // Load IsNotFill
            GetComponent<SpawnNewMergableObjects>().IsNotFill = data.IsNotFill;

            // Load NoOfTilesFill
            GetComponent<SpawnNewMergableObjects>().NoOfTilesFill = data.NoOfTilesFill;

            if (data.Health <= 0)
            {
                Destroy(Planet);
            }

            int count = 0;

            for (int i = 0; i < data.IsEmpty.Length; i++)
            {
                if (data.Tiles[i] != null && data.IsEmpty[i] == false || data.Tiles[i] != null && data.Tiles[i].GetComponent<TileEmptyStatus>().IsEmpty == false)
                {
                    count++;

                    Debug.Log("Data:" + count);

                    Debug.Log("Data Mergable: " + data.Mergable.Length);

                    if (data.Mergable[i])
                    {
                        // Spawn mergable
                        mergable = Instantiate(data.Mergable[i]);

                        // Set Parent
                        mergable.transform.parent = data.Tiles[i].transform;

                        // Set LocalPosition
                        mergable.transform.localPosition = Vector3.zero;

                        mergable.transform.parent.GetComponent<TileEmptyStatus>().IsEmpty = false;

                        // Load count
                        GetComponent<SpawnNewMergableObjects>().count++;
                    }

                    Debug.Log(data.Tiles[i]);       
                }
            }
        }
    }

    void Update()
    {
        Mergable = GameObject.FindGameObjectsWithTag("Mergable");
        Tiles = GameObject.FindGameObjectsWithTag("Tile");

        // Save the data every frame
        SaveData data = new SaveData();

        data.Tiles = new GameObject[Tiles.Length];
        data.Mergable = new GameObject[Tiles.Length];
        data.IsEmpty = new bool[Tiles.Length];

        // IsEmpty is equal to false if tile has a child, means it is fill (not empty)
        foreach (var item in Tiles)
        {
            if (item != null && item.transform.childCount > 0)
            {
                item.GetComponent<TileEmptyStatus>().IsEmpty = false;
            }
        }

        // Save:

        for (int i = 0; i < Tiles.Length; i++)
        {
            if (Tiles[i] != null && Tiles[i].GetComponent<TileEmptyStatus>().IsEmpty == false && Tiles[i].transform.childCount > 0)
            {
                // Save Tiles
                data.Tiles[i] = Tiles[i];
                data.IsEmpty[i] = false;

                // To find the MergablePrefab for this Tile's current child and store that prefab in the data save file (Json)
                for (int j = 0; j < MergablePrefab.Length; j++)
                {
                    if (MergablePrefab[j] && MergablePrefab[j].name + "(Clone)" == Tiles[i].transform.GetChild(0).gameObject.name)
                    {
                        data.Mergable[i] = MergablePrefab[j];
                    }
                }
            }

            else
            {
                // If condition is not fullfilled then null the index
                data.Tiles[i] = null;
                data.IsEmpty[i] = true;
                data.Mergable[i] = null;
            }
        }

        if (Planet)
        {
            // Save Health
            data.Health = Planet.GetComponent<PlanetCollisionEventSystem>().Health;
        }

        else
        {
            // Save Health
            data.Health = 0;
        }

        // Save IsNotFill
        data.IsNotFill = GetComponent<SpawnNewMergableObjects>().IsNotFill;

        // Save NoOfTiles
        data.NoOfTilesFill = GetComponent<SpawnNewMergableObjects>().NoOfTilesFill;

        // save data to Json
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }
}