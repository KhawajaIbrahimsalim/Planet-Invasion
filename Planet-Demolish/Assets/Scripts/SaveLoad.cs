using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SaveData
{
    public GameObject[] Tiles;
    public GameObject[] Mergable;
    public string CurrentPlanet_name;
    public float TileLength;
    public int count;
    public bool IsNotFill;
    public int NoOfTilesFill;
    public bool IfHealthIsHalf;

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
        savePath = Application.persistentDataPath + "/Max93.json";
        //File.Delete(savePath);

        // Load:

        if (File.Exists(savePath))
        {
            // Load data from Json
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            Debug.Log("Load");

            // For Tiles Properties Load
            {
                // Load IsNotFill
                GetComponent<SpawnNewMergableObjects>().IsNotFill = data.IsNotFill;

                // Load NoOfTilesFill
                GetComponent<SpawnNewMergableObjects>().NoOfTilesFill = data.NoOfTilesFill;
            }

            // For Planet Load
            {
                // Just for safety
                if (data.Health <= 0)
                {
                    Planet = GameObject.FindGameObjectWithTag("Planet");
                    Destroy(Planet);
                }

                // Spawn the last saved Planet
                foreach (GameObject Planet in GetComponent<GameController>().Planets)
                {
                    if (data.CurrentPlanet_name == Planet.name + "(Clone)")
                    {
                        GameObject planet = Instantiate(Planet, gameObject.transform.position, Quaternion.identity);

                        GetComponent<GameController>().CurrentPlanet = planet;

                        // Activate the Particle system If it waas Active
                        planet.GetComponent<PlanetCollisionEventSystem>().IfHealthIsHalf = data.IfHealthIsHalf;

                        Debug.Log(data.IfHealthIsHalf);
                    }
                }

                // Find the new spawned Planet
                Planet = GameObject.FindGameObjectWithTag("Planet");

                // Load Health
                Planet.GetComponent<PlanetCollisionEventSystem>().Health = data.Health;

                // Show Health
                HealthBar.GetComponent<Slider>().value = Planet.GetComponent<PlanetCollisionEventSystem>().Health / Planet.GetComponent<PlanetCollisionEventSystem>().MaxHealth;
            }

            // For Mergable objects Load
            {
                for (int i = 0; i < data.Tiles.Length; i++)
                {
                    if (data.Tiles[i] != null)
                    {
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
                    }
                }
            }
        }
    }

    void Update()
    {
        Mergable = GameObject.FindGameObjectsWithTag("Mergable");
        Tiles = GameObject.FindGameObjectsWithTag("Tile");
        Planet = GameObject.FindGameObjectWithTag("Planet");

        // Save the data every frame
        SaveData data = new SaveData();

        data.Tiles = new GameObject[Tiles.Length];
        data.Mergable = new GameObject[Tiles.Length];

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
                data.Mergable[i] = null;
            }
        }

        if (Planet)
        {
            // Save Current Planet
            data.CurrentPlanet_name = Planet.name;

            // Save Health
            data.Health = Planet.GetComponent<PlanetCollisionEventSystem>().Health;

            // Save IfHealthIsHalf to Activate particle effects (StartParticlesFor_HalfHealth is a checking variable to check if it is Half Health of the Planet)
            data.IfHealthIsHalf = Planet.GetComponent<PlanetCollisionEventSystem>().IfHealthIsHalf;

            Debug.Log(Planet.GetComponent<PlanetCollisionEventSystem>().IfHealthIsHalf);
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