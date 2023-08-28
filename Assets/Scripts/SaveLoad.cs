using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SaveData
{
    public GameObject[] Tiles;
    public Color[] MergableColors;
    public string[] TimesPower_txt;
    public string CurrentPlanet_name;
    public float TileLength;
    public int count;
    public bool IsNotFill;
    public int NoOfTilesFill;
    public bool IfHealthIsHalf;
    public float AutoClick_Delay;
    public float DelayAfterPressingTheButton;
    public int TouchCount;
    public float DelayDamage5x;
    public bool IsAutoClickActive = false;
    public bool Damage5xIsActive = false;
    public bool IsDelayChanged = true;
    public float Coins;
    public float Cost;
    public float Health;
    public float Offline_currencyPerSecond;
    public int Offline_Level;
    public float Offline_UpgradeCost;
    public float Power_DamageRatio;
    public int Power_Level;
    public float Power_UpgradeCost;
}

[System.Serializable]
public class SaveLoad : MonoBehaviour
{
    public GameObject[] Tiles;
    public GameObject MergablePrefab;
    public GameObject[] Mergable;
    public GameObject Planet;
    public GameObject HealthBar;
    public GameObject Damage5x_txt;
    [SerializeField] private TextMeshProUGUI Coins_txt;
    [SerializeField] private TextMeshProUGUI Cost_txt;

    private string savePath;
    GameObject mergable;

    private void Awake()
    {
        Tiles = GameObject.FindGameObjectsWithTag("Tile");
    }

    void Start()
    {
        savePath = Application.persistentDataPath + "/Max93.json";
        File.Delete(savePath);

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

                if (Planet)
                {
                    // Load Health
                    Planet.GetComponent<PlanetCollisionEventSystem>().Health = data.Health;

                    // Show Health
                    HealthBar.GetComponent<Slider>().value = Planet.GetComponent<PlanetCollisionEventSystem>().Health / Planet.GetComponent<PlanetCollisionEventSystem>().MaxHealth;
                }
            }

            // For Mergable objects Load
            {
                for (int i = 0; i < data.Tiles.Length; i++)
                {
                    if (data.Tiles[i] != null)
                    {
                        // Spawn mergable
                        mergable = Instantiate(MergablePrefab);

                        // Set Color
                        mergable.transform.GetChild(1).transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.color = data.MergableColors[i];

                        // Set TimesPower_txt
                        mergable.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = data.TimesPower_txt[i];

                        // Set Parent
                        mergable.transform.SetParent(data.Tiles[i].transform, false);

                        // Set LocalPosition
                        mergable.transform.localPosition = Vector3.zero;

                        //mergable.transform.parent.GetComponent<TileEmptyStatus>().IsEmpty = false;

                        // Load count
                        GetComponent<SpawnNewMergableObjects>().count++;
                    }
                }
            }

            // Load Auto Click Properties
            {
                GetComponent<Boosts>().AutoClick_Delay = data.AutoClick_Delay;

                GetComponent<Boosts>().DelayAfterPressingTheButton = data.DelayAfterPressingTheButton;

                GetComponent<Boosts>().IsDelayChanged = data.IsDelayChanged;

                GetComponent<GameController>().IsAutoClickActive = data.IsAutoClickActive;
            }

            // Load Damage 5x Properties
            {
                GetComponent<Boosts>().TouchCount = data.TouchCount;

                Damage5x_txt.GetComponent<TextMeshProUGUI>().text = "Clicks: " + data.TouchCount + "/" + GetComponent<Boosts>().MaxTouch;

                GetComponent<Boosts>().DelayDamage5x = data.DelayDamage5x;

                GetComponent<GameController>().Damage5xIsActive = data.Damage5xIsActive;
            }

            // Load Buy Button Properties
            {
                // Load Coins
                GetComponent<GameController>().Coins = data.Coins;

                // Show Coins
                Coins_txt.text = GetComponent<GameController>().CompressNumber(data.Coins);

                // Load Cost
                GetComponent<GameController>().Cost = data.Cost;

                // Show Cost
                Cost_txt.text = GetComponent<GameController>().CompressNumber(data.Cost);
            }

            // Load Power Button Properties
            {
                // Load DamageRatio
                GetComponent<PowerUpgrade>().DamageRatio = data.Power_DamageRatio;

                // Show DamageRatio
                GetComponent<PowerUpgrade>().PowerAmount_txt.text = data.Power_DamageRatio.ToString(".00");

                // Load Level
                GetComponent<PowerUpgrade>().Level = data.Power_Level;

                // Show Level Increase
                GetComponent<PowerUpgrade>().Level_txt.text = "Lv." + data.Power_Level;

                // Load UpgradeCost
                GetComponent<PowerUpgrade>().UpgradeCost = data.Power_UpgradeCost;

                // Show Upgrade Cost
                GetComponent<PowerUpgrade>().UpgradeCost_txt.text = GetComponent<GameController>().CompressNumber(data.Power_UpgradeCost);
            }

            // Load Offline Button Properties
            {
                // Load currencyPerSecond
                GetComponent<OfflineCurrency>().currencyPerSecond = data.Offline_currencyPerSecond;

                // Show Earning_txt
                GetComponent<OfflineCurrency>().Earning_txt.text = "Earning × " + data.Offline_currencyPerSecond;

                // Load Level
                GetComponent<OfflineCurrency>().Level = data.Offline_Level;

                // Show Level Increase
                GetComponent<OfflineCurrency>().Level_txt.text = "Lv." + data.Offline_Level;

                // Load UpgradeCost
                GetComponent<OfflineCurrency>().UpgradeCost = data.Offline_UpgradeCost;

                // Show Upgrade Cost
                GetComponent<OfflineCurrency>().UpgradeCost_txt.text = GetComponent<GameController>().CompressNumber(data.Offline_UpgradeCost);
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
        data.MergableColors = new Color[Tiles.Length];
        data.TimesPower_txt = new string[Tiles.Length];

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

                // To find the Mergable for this Tile's current child and store that prefab in the data save file (Json)
                for (int j = 0; j < Mergable.Length; j++)
                {
                    if (Mergable[j] && Mergable[j].name == Tiles[i].transform.GetChild(0).gameObject.name)
                    {
                        // Save color of the mergeable Object
                        data.MergableColors[i] = Mergable[j].transform.GetChild(1).transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.color;

                        // Save TimesPower_txt of the Mergeable Object
                        data.TimesPower_txt[i] = Mergable[j].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                    }
                }
            }

            else
            {
                // If condition is not fullfilled then null the index
                data.Tiles[i] = null;
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

        // Save Auto Click Properties
        {
            data.AutoClick_Delay = GetComponent<Boosts>().AutoClick_Delay;

            data.DelayAfterPressingTheButton = GetComponent<Boosts>().DelayAfterPressingTheButton;

            data.IsDelayChanged = GetComponent<Boosts>().IsDelayChanged;

            data.IsAutoClickActive = GetComponent<GameController>().IsAutoClickActive;
        }

        // Damage 5x Properties
        {
            data.TouchCount = GetComponent<Boosts>().TouchCount;

            data.DelayDamage5x = GetComponent<Boosts>().DelayDamage5x;

            data.Damage5xIsActive = GetComponent<GameController>().Damage5xIsActive;
        }

        // Save Coins and Cost
        {
            // Save Coins
            data.Coins = GetComponent<GameController>().Coins;

            // Save Cost
            data.Cost = GetComponent<GameController>().Cost;
        }

        // Save Power Button Properties
        {
            // Save DamageRatio
            data.Power_DamageRatio = GetComponent<PowerUpgrade>().DamageRatio;

            // Save Level
            data.Power_Level = GetComponent<PowerUpgrade>().Level;

            // Save UpgradeCost
            data.Power_UpgradeCost = GetComponent<PowerUpgrade>().UpgradeCost;
        }

        // Save Offline Button Properties
        {
            // Save currencyPerSecond
            data.Offline_currencyPerSecond = GetComponent<OfflineCurrency>().currencyPerSecond;

            // Save Level
            data.Offline_Level = GetComponent<OfflineCurrency>().Level;

            // Save UpgradeCost
            data.Offline_UpgradeCost = GetComponent<OfflineCurrency>().UpgradeCost;
        }

        // save data to Json
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }
}