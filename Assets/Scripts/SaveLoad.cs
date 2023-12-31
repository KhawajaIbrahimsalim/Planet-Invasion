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
    public float[] Damage;
    public string CurrentPlanet_name;
    public float TileLength;
    public int count;
    public bool IsNotFill;
    public int NoOfTilesFill;
    public float AutoClick_Delay;
    public float DelayAfterPressingTheButton;
    public int TouchCount;
    public float DelayDamage5x;
    public bool IsAutoClickActive = false;
    public bool Damage5xIsActive = false;
    public bool IsDelayChanged = true;
    public float Coins;
    public float Cost;
    public float MaxHealth;
    public float Health;
    public float HealthSum;
    public float Offline_currencyPerSecond;
    public int Offline_Level;
    public float Offline_UpgradeCost;
    public float Power_DamageRatio;
    public int Power_Level;
    public float Power_UpgradeCost;
    public float Speed_SpeedRatio;
    public int Speed_Level;
    public float Speed_UpgradeCost;
    public long BoostIndex = 0;
    public int ShowRateUs_Index = 0;
    public bool IsFree = true;
    public bool TutorialPanelActive = false;
    public bool IsPurchased = false;
    public bool PurchaseButton_Interactable;
    public bool BuyFromAdsButton_Interactable;
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
        Canvas.ForceUpdateCanvases();

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

            // For Tutorial Properties
            {
                GetComponent<GameController>().TutorialPanelActive = data.TutorialPanelActive;
            }

            // For Planet Load
            {
                if (data.TutorialPanelActive == false)
                {
                    GetComponent<GameController>().TutorialPanel.SetActive(false);

                    // Spawn the last saved Planet
                    foreach (GameObject Planet in GetComponent<GameController>().Planets)
                    {
                        if (data.CurrentPlanet_name == Planet.name + "(Clone)")
                        {
                            GameObject planet = Instantiate(Planet, gameObject.transform.position, Quaternion.identity);

                            // Set Parent
                            planet.transform.SetParent(gameObject.transform);

                            GetComponent<GameController>().CurrentPlanet = planet;

                            // Load MaxHealth
                            planet.GetComponent<PlanetCollisionEventSystem>().MaxHealth = data.MaxHealth;

                            // Load Health
                            planet.GetComponent<PlanetCollisionEventSystem>().Health = data.Health;

                            // Load HealthSum
                            GetComponent<GameController>().HealthSum = data.HealthSum;

                            // Show Health
                            HealthBar.GetComponent<Slider>().value = data.Health / data.MaxHealth;

                            // If Palnet is about to destroy then Enable Particles
                            if (data.Health <= data.MaxHealth / 4)
                            {
                                GetComponent<GameController>().MeteorSpawnPoint_1.SetActive(true);
                                GetComponent<GameController>().MeteorSpawnPoint_2.SetActive(true);

                                GetComponent<GameController>().MeteorSpawnPoint_1.transform.parent = planet.transform;
                                GetComponent<GameController>().MeteorSpawnPoint_2.transform.parent = planet.transform;
                            }

                            // Make it true when a new Planet is Spawned
                            GetComponent<GameController>().BonusCoinsAdded = true;

                            // Load IsAnimating_Indicator
                            GetComponent<GameController>().IsAnimating_Indicator = true;

                            // Load BoostIndex
                            GetComponent<GameController>().BoostIndex = data.BoostIndex;

                            // Same as in "GameController" Script when index is 1 then Disable Damage5x_btn
                            if (data.BoostIndex == 1)
                            {
                                GetComponent<GameController>().Damage5x_btn.SetActive(false);
                            }

                            // Load ShowRateUs_Index
                            GetComponent<GameController>().ShowRateUs_Index = data.ShowRateUs_Index;

                            // Increment PlanetIndex
                            GetComponent<GameController>().PlanetIndex++;
                        }
                    }
                }           
            }

            // For Mergable objects Load
            {
                for (int i = 0; i < data.Tiles.Length; i++)
                {
                    if (data.Tiles[i] != null)
                    {
                        // Spawn mergeable
                        mergable = Instantiate(MergablePrefab);

                        // Set Damage
                        mergable.GetComponent<ProjectileSpawning>().Damage = data.Damage[i];

                        // Set Color
                        mergable.transform.GetChild(1).transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.color = data.MergableColors[i];

                        // Set TimesPower_txt
                        mergable.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = data.TimesPower_txt[i];

                        // Set Parent
                        mergable.transform.SetParent(data.Tiles[i].transform, false);

                        // Set LocalPosition
                        mergable.transform.localPosition = Vector3.zero;

                        // Set ProjectileSpawnPoint Position to zero
                        mergable.GetComponent<ProjectileSpawning>().ProjectileSpawnPoint.transform.localPosition = Vector3.zero;

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
                GetComponent<GameController>().IsFree = data.IsFree;

                if (data.IsFree == false)
                {
                    GetComponent<GameController>().Free_txt.SetActive(false);

                    GetComponent<GameController>().Cost_txt.SetActive(true);
                }

                // Load Coins Properties
                {
                    // Load Coins
                    GetComponent<GameController>().Coins = data.Coins;

                    // Show Coins
                    Coins_txt.text = GetComponent<GameController>().CompressNumber(data.Coins);
                }

                // Load Cost Properties
                {
                    // Load Cost
                    GetComponent<GameController>().Cost = data.Cost;

                    // Show Cost
                    Cost_txt.text = GetComponent<GameController>().CompressNumber(data.Cost);
                }         
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

            // Load Speed Button Properties
            {
                // Load SpeedRatio
                GetComponent<SpeedUpgrade>().SpeedRatio = data.Speed_SpeedRatio;

                if (data.Speed_SpeedRatio <= 0.5f) // If Speed Upgrade limit has reached its limit then Enable Completed Text and Disable Cost
                {
                    GetComponent<SpeedUpgrade>().Coin.SetActive(false);

                    GetComponent<SpeedUpgrade>().Completed_txt.SetActive(true);
                }

                // Show SpeedRatio
                GetComponent<SpeedUpgrade>().SpeedAmount_txt.text = data.Speed_SpeedRatio.ToString(".00");

                // Load Level
                GetComponent<SpeedUpgrade>().Level = data.Speed_Level;

                // Show Level
                GetComponent<SpeedUpgrade>().Level_txt.text = "Lv." + GetComponent<GameController>().CompressNumber(data.Speed_Level);

                // Lead UpgradeCost
                GetComponent<SpeedUpgrade>().UpgradeCost = data.Speed_UpgradeCost;

                // Show UpgradeCost
                GetComponent<SpeedUpgrade>().UpgradeCost_txt.text = GetComponent<GameController>().CompressNumber(data.Speed_UpgradeCost);
            }

            // Load Offline Button Properties
            {
                // Load currencyPerSecond
                GetComponent<OfflineCurrency>().currencyPerSecond = data.Offline_currencyPerSecond;

                // Show Earning_txt
                GetComponent<OfflineCurrency>().Earning_txt.text = "Earning � " + data.Offline_currencyPerSecond;

                // Load Level
                GetComponent<OfflineCurrency>().Level = data.Offline_Level;

                // Show Level Increase
                GetComponent<OfflineCurrency>().Level_txt.text = "Lv." + data.Offline_Level;

                // Load UpgradeCost
                GetComponent<OfflineCurrency>().UpgradeCost = data.Offline_UpgradeCost;

                // Show Upgrade Cost
                GetComponent<OfflineCurrency>().UpgradeCost_txt.text = GetComponent<GameController>().CompressNumber(data.Offline_UpgradeCost);
            }

            // Load InApp-Purchase Properties
            {
                // Load IsPurchased
                GetComponent<GameController>().IsPurchased = data.IsPurchased;

                // Load PurchaseButton_Interactible
                GetComponent<GameController>().Purchase_btn.interactable = data.PurchaseButton_Interactable;

                // Load BuyFromAdsButton_Interactable
                GetComponent<GameController>().BuyFromAds_btn.interactable = data.BuyFromAdsButton_Interactable;
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
        data.Damage = new float[Tiles.Length];

        // IsEmpty is equal to false if tile has a child, means it is fill (not empty)
        foreach (var item in Tiles)
        {
            if (item != null && item.transform.childCount > 0)
            {
                item.GetComponent<TileEmptyStatus>().IsEmpty = false;
            }

            else
            {
                item.GetComponent<TileEmptyStatus>().IsEmpty = true;
            }
        }

        // Save:

        // For Mergeable objects Position and Tiles Save
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

                        // Save Damage
                        data.Damage[i] = Mergable[j].GetComponent<ProjectileSpawning>().Damage;
                    }
                }
            }

            else
            {
                // If condition is not fullfilled then null the index
                data.Tiles[i] = null;
            }
        }

        // For Save Tutorial Properties
        {
            data.TutorialPanelActive = GetComponent<GameController>().TutorialPanelActive;
        }

        // For Save Planet Properties
        if (Planet)
        {
            // Save Current Planet
            data.CurrentPlanet_name = Planet.name;

            // Save Maxhealth
            data.MaxHealth = Planet.GetComponent<PlanetCollisionEventSystem>().MaxHealth;

            // Save Health
            data.Health = Planet.GetComponent<PlanetCollisionEventSystem>().Health;

            // Save HealthSum
            data.HealthSum = GetComponent<GameController>().HealthSum;

            // Save BoostIndex
            data.BoostIndex = GetComponent<GameController>().BoostIndex;

            // Save ShowRateUs_Index
            data.ShowRateUs_Index = GetComponent<GameController>().ShowRateUs_Index;
        }

        else
        {
            // Save Health
            data.Health = 0;
        }

        // Save IsFree
        data.IsFree = GetComponent<GameController>().IsFree;

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

        // Save Speed Button Properties
        {
            // Save SpeedRatio
            data.Speed_SpeedRatio = GetComponent<SpeedUpgrade>().SpeedRatio;

            // Save Level
            data.Speed_Level = GetComponent<SpeedUpgrade>().Level;

            // Save UpgradeCost
            data.Speed_UpgradeCost = GetComponent<SpeedUpgrade>().UpgradeCost;
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

        // Save InApp-Purchase Properties
        {
            // Save IsPurchased
            data.IsPurchased = GetComponent<GameController>().IsPurchased;

            // Save PurchaseButton_Interactible
            data.PurchaseButton_Interactable = GetComponent<GameController>().Purchase_btn.interactable;

            // Save data.BuyFromAdsButton_Interactable
            data.BuyFromAdsButton_Interactable = GetComponent<GameController>().BuyFromAds_btn.interactable;
        }

        // save data to Json
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }
}