using TMPro;
using UnityEngine;

[System.Serializable]
public class GameController : MonoBehaviour
{
    [SerializeField] private float DelayBeforeSpawn;

    [Header("Planet Spawn Properties:")]
    public GameObject[] Planets;
    public GameObject CurrentPlanet;
    public float HealthSum;

    [Header("Boosts Properties:")]
    public bool IsAutoClickActive = false;
    public bool Damage5xIsActive = false;

    [Header("UI Properties:")]
    public GameObject PauseGame_Panel;
    public GameObject UpgradePanel;
    public GameObject FooterPanel;
    public GameObject StorePanel;

    [Header("Coins Properties:")]
    public float Coins;
    public TextMeshProUGUI Coins_txt;
    [SerializeField] private float BonusCoins;
    [SerializeField] private GameObject FiveX_Coins_txt;

    [Header("Buy Properties:")]
    public float Cost;
    public TextMeshProUGUI Cost_txt;
    [SerializeField] private float IncreaseCost;
    public bool IsWithInCost = false;

    private float Temp_DelayBeforeSpawn;
    private bool IsPaused = true;
    private bool IsUpgradePanelOpened = false;

    private void Awake()
    {
        CurrentPlanet = GameObject.FindGameObjectWithTag("Planet");

        Coins_txt.text = CompressNumber(Coins);

        Temp_DelayBeforeSpawn = DelayBeforeSpawn;
    }

    private void Update()
    {
        // If Player want to Pause the Game
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("q"))
        {
            if (IsPaused)
            {
                // Enable PausedGame_Panel
                PauseGame_Panel.SetActive(true);

                // Disable FooterPanel
                FooterPanel.SetActive(false);

                IsPaused = false;
            }

            else
            {
                // Disable PausedGame_Panel
                PauseGame_Panel.SetActive(false);

                EnablePropertiesWhen_Resumed();

                IsPaused = true;
            }
        }

        // To Spawn a new Planet when destroyed
        if (CurrentPlanet == null && DelayBeforeSpawn <= 0f) 
        {
            // Randomly choose new Planet
            int RandomPlanet = Random.Range(0, Planets.Length);

            // Spawn a new Planet
            CurrentPlanet = Instantiate(Planets[RandomPlanet], gameObject.transform.position, Quaternion.identity);

            // Make the GameController parent of the new Planet
            CurrentPlanet.transform.parent = gameObject.transform;

            // Store the value of the HealthSum
            CurrentPlanet.GetComponent<PlanetCollisionEventSystem>().MaxHealth = CurrentPlanet.GetComponent<PlanetCollisionEventSystem>().Health = HealthSum;

            // Multiply the HealthSum by 2
            HealthSum *= 2;

            // Refill the Delay
            DelayBeforeSpawn = Temp_DelayBeforeSpawn;

            // Diable the FiveX_Coins_txt
            FiveX_Coins_txt.SetActive(false);
        }

        if (CurrentPlanet == null)
        {
            DelayBeforeSpawn -= Time.deltaTime;
        }

        // Bonus Coins After Destroying a Planet
        if (CurrentPlanet != null && CurrentPlanet.GetComponent<PlanetCollisionEventSystem>().Health <= 0)
        {
            Coins *= BonusCoins;

            Coins_txt.text = CompressNumber(Coins);

            // Activate the FiveX_Coins_txt
            FiveX_Coins_txt.SetActive(true);
        }
    }

    private void EnablePropertiesWhen_Resumed()
    {
        // Enable FooterPanel
        FooterPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        PauseGame_Panel.SetActive(false);

        EnablePropertiesWhen_Resumed();
    }

    public void Quit()
    {
        Application.Quit();
    }

    // Connected to the Auto Click button
    public void AutoClick()
    {
        IsAutoClickActive = true;
    }

    // Connected to the Damage 5x button
    public void Damage5x()
    {
        Damage5xIsActive = true;
    }

    // Connected Buy Button
    public void Buy()
    {
        if (Coins >= Cost && GetComponent<SpawnNewMergableObjects>().IsNotFill == true)
        {
            // Substract the Cost from the total Coins
            Coins -= Cost;

            // Show Change
            Coins_txt.text = CompressNumber(Coins);

            // Increase Cost Value
            {
                Cost *= IncreaseCost;

                Cost_txt.text = CompressNumber(Cost);
            }

            IsWithInCost = true;
        }
    }

    public string CompressNumber(float num)
    {
        if (num >= 100000000000000)
            return (num / 100000000000000f).ToString("0.#") + "aa";
        if (num >= 10000000000000)
            return (num / 10000000000000f).ToString("0.#") + "a";
        if (num >= 1000000000000)
            return (num / 1000000000000f).ToString("0.#") + "T";
        if (num >= 1000000000)
            return (num / 1000000000f).ToString("0.#") + "B";
        if (num >= 1000000)
            return (num / 1000000f).ToString("0.#") + "M";
        if (num >= 1000)
            return (num / 1000f).ToString("0.#") + "K";
        return num.ToString("#,0");
    }

    public void OpenUpgradePanel()
    {
        if (IsUpgradePanelOpened == false)
        {
            UpgradePanel.SetActive(true);

            IsUpgradePanelOpened = true;
        }

        else
        {
            UpgradePanel.SetActive(false);

            IsUpgradePanelOpened = false;
        }
    }

    public void CloseUpgradePanel()
    {
        UpgradePanel.SetActive(false);
    }

    public void OpenStore()
    {
        StorePanel.SetActive(true);
    }

    public void CloseStorePanel()
    {
        StorePanel.SetActive(false);
    }
}
