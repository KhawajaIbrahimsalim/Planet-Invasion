using TMPro;
using UnityEngine;

[System.Serializable]
public class GameController : MonoBehaviour
{
    [SerializeField] private float DelayBeforeSpawn;
    [SerializeField] private float ItweenTime;
    [SerializeField] private GameObject BOOM;

    [Header("Planet Spawn Properties:")]
    public GameObject[] Planets;
    public GameObject CurrentPlanet;
    public float HealthSum;
    public GameObject MeteorSpawnPoint_1;
    public GameObject MeteorSpawnPoint_2;

    [Header("Boosts Properties:")]
    public bool IsAutoClickActive = false;
    public bool Damage5xIsActive = false;

    [Header("UI Properties:")]
    public GameObject PauseGame_Panel;
    public GameObject UpgradePanel;
    public RectTransform Upgrade_btn;
    public GameObject Store_btn;
    public GameObject FooterPanel;
    public GameObject StorePanel;
    public GameObject Damage_Indicator_txt;
    [SerializeField] private TextMeshProUGUI HealthRemain_txt;
    [SerializeField] private GameObject AutoClick_Indicator;
    [SerializeField] private GameObject Damage5x_Indicator;

    [Header("Coins Properties:")]
    public float Coins;
    public TextMeshProUGUI Coins_txt;
    [SerializeField] private float BonusCoins;
    [SerializeField] private GameObject BonusX_Coins_txt;

    [Header("Buy Properties:")]
    public float Cost;
    public TextMeshProUGUI Cost_txt;
    [SerializeField] private float IncreaseCost;
    public bool IsWithInCost = false;

    [Header("Itween Indicators Properties:")]
    public float time;
    public bool IsAnimating_Indicator = false;
    public int BoostIndex = 0;

    private float Temp_DelayBeforeSpawn;
    private bool IsPaused = true;
    private bool IsUpgradePanelOpened = false;
    private Vector3 UpgradePanel_Pos;
    private Vector3 StorePanel_Pos;
    private Vector3 AutoClick_Indicator_Pos;
    private Vector3 Damage5x_Indicator_Pos;

    [HideInInspector] public bool BonusCoinsAdded = false;

    private void Awake()
    {
        CurrentPlanet = GameObject.FindGameObjectWithTag("Planet");

        Coins_txt.text = CompressNumber(Coins);

        Temp_DelayBeforeSpawn = DelayBeforeSpawn;
    }

    private void Start() // When the point comes to call start() all values of the variables are set and also it means screen resolution is set also.
    {
        // For UpgradePanel
        UpgradePanel_Pos = UpgradePanel.transform.position;

        UpgradePanel.transform.localScale = Vector3.zero;

        UpgradePanel.transform.position = Upgrade_btn.position;

        // For StorePanel
        StorePanel_Pos = StorePanel.transform.position;

        StorePanel.transform.localScale = Vector3.zero;

        StorePanel.transform.position = Store_btn.transform.position;

        // For Pause Panel
        PauseGame_Panel.transform.localScale = Vector3.zero;

        // Set Pos for Indicator_Pos
        AutoClick_Indicator_Pos = new Vector3(AutoClick_Indicator.transform.position.x + 50f, AutoClick_Indicator.transform.position.y, AutoClick_Indicator.transform.position.z);
        Damage5x_Indicator_Pos = new Vector3(Damage5x_Indicator.transform.position.x + 50f, Damage5x_Indicator.transform.position.y, Damage5x_Indicator.transform.position.z);
    }

    private void Update()
    {
        // If Player want to Pause the Game
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
        {
            if (IsPaused)
            {
                iTween.ScaleTo(PauseGame_Panel, iTween.Hash("scale", Vector3.one, "time", ItweenTime));

                // Disable FooterPanel
                FooterPanel.SetActive(false);
                // Close UpgradePanel
                CloseUpgradePanel();
                // Close StorePanel
                CloseStorePanel();

                IsPaused = false;
            }
        }

        // To Spawn a new Planet when destroyed
        if (CurrentPlanet == null && DelayBeforeSpawn <= 0f) 
        {
            // Find Boom Explosion and...
            BOOM = GameObject.Find("Boom(Clone)");

            // Destroy it
            Destroy(BOOM);

            // Randomly choose new Planet
            int RandomPlanet = Random.Range(0, Planets.Length);

            // Spawn a new Planet
            CurrentPlanet = Instantiate(Planets[RandomPlanet], gameObject.transform.position, Quaternion.identity);

            // Make the GameController parent of the new Planet
            CurrentPlanet.transform.SetParent(gameObject.transform);

            // Store the value of the HealthSum
            CurrentPlanet.GetComponent<PlanetCollisionEventSystem>().MaxHealth = CurrentPlanet.GetComponent<PlanetCollisionEventSystem>().Health = HealthSum;

            // Multiply the HealthSum by 2
            HealthSum *= 2;

            // Refill the Delay
            DelayBeforeSpawn = Temp_DelayBeforeSpawn;

            // Diable the FiveX_Coins_txt
            BonusX_Coins_txt.SetActive(false);

            BonusCoinsAdded = true;

            BoostIndex++;

            IsAnimating_Indicator = true;
        }

        else if (CurrentPlanet != null)
        {
            HealthRemain_txt.text = CurrentPlanet.GetComponent<PlanetCollisionEventSystem>().Health.ToString("0");
        }

        if (CurrentPlanet == null)
        {
            // Get Bonus Coins when destroying a Planet
            if (BonusCoinsAdded)
            {
                float Sum = Coins * BonusCoins;

                Coins += Sum;

                Coins_txt.text = CompressNumber(Coins);

                // Enable BonusX_Coins_txt
                BonusX_Coins_txt.SetActive(true);

                BonusCoinsAdded = false;
            }

            // Set to zero
            HealthRemain_txt.text = "0";

            DelayBeforeSpawn -= Time.deltaTime;

            // Disable Damage_Indicator_txt
            Damage_Indicator_txt.SetActive(false);
        }

        if (IsAnimating_Indicator && BoostIndex <= 3)
        {
            if (BoostIndex == 1 && GetComponent<Boosts>().AutoClick_Delay <= 0 && GetComponent<Boosts>().IsDelayChanged == true)
            {
                AutoClick_Indicator.SetActive(true);
                Damage5x_Indicator.SetActive(false);
                iTween.MoveTo(AutoClick_Indicator, iTween.Hash("position", AutoClick_Indicator_Pos, "time", time, "loopType", iTween.LoopType.pingPong));

                IsAnimating_Indicator = false; Debug.Log("BoostIndex 1");
            }

            if (BoostIndex == 2 && GetComponent<Boosts>().TouchCount == GetComponent<Boosts>().MaxTouch && GetComponent<Boosts>().DelayDamage5x == GetComponent<Boosts>().MaxDelayDamage5x)
            {
                Damage5x_Indicator.SetActive(true);
                AutoClick_Indicator.SetActive(false);
                iTween.MoveTo(Damage5x_Indicator, iTween.Hash("position", Damage5x_Indicator_Pos, "time", time, "loopType", iTween.LoopType.pingPong));
                
                IsAnimating_Indicator = false;
            }

            if (BoostIndex > 2)
            {
                AutoClick_Indicator.SetActive(false);
                Damage5x_Indicator.SetActive(false);

                IsAnimating_Indicator = false;
            }
        }
    }

    private void EnablePropertiesWhen_Resumed()
    {
        // Enable FooterPanel
        FooterPanel.SetActive(true);
    }

    public void ResumeGame() // Connected to NO button
    {
        iTween.ScaleTo(PauseGame_Panel, iTween.Hash("scale", Vector3.zero, "time", ItweenTime));

        EnablePropertiesWhen_Resumed();

        IsPaused = true;
    }

    public void Quit() // Connected to YES Button
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
        if (num >= 1000000000000000000000000f)
            return (num / 1000000000000000000000000f).ToString("0.#") + "cc";
        if (num >= 10000000000000000000000f)
            return (num / 10000000000000000000000f).ToString("0.#") + "c";
        if (num >= 100000000000000000000f)
            return (num / 100000000000000000000f).ToString("0.#") + "bb";
        if (num >= 1000000000000000000f)
            return (num / 1000000000000000000f).ToString("0.#") + "b";
        if (num >= 10000000000000000f)
            return (num / 10000000000000000f).ToString("0.#") + "aa";
        if (num >= 100000000000000f)
            return (num / 100000000000000f).ToString("0.#") + "a";
        if (num >= 1000000000000f)
            return (num / 1000000000000f).ToString("0.#") + "T";
        if (num >= 1000000000f)
            return (num / 1000000000f).ToString("0.#") + "B";
        if (num >= 1000000f)
            return (num / 1000000f).ToString("0.#") + "M";
        if (num >= 1000f)
            return (num / 1000f).ToString("0.#") + "K";
        return num.ToString("#,0");
    }

    public void OpenUpgradePanel()
    {
        // When UpgradePanel is Opened
        if (IsUpgradePanelOpened == false)
        {
            iTween.ScaleTo(UpgradePanel, iTween.Hash("scale", Vector3.one, "time", ItweenTime));

            iTween.MoveTo(UpgradePanel, iTween.Hash("position", UpgradePanel_Pos, "time", ItweenTime));

            // Close Store
            CloseStorePanel();

            IsUpgradePanelOpened = true;
        }

        else // And when Closed
        {
            iTween.ScaleTo(UpgradePanel, iTween.Hash("scale", Vector3.zero, "time", ItweenTime));

            iTween.MoveTo(UpgradePanel, iTween.Hash("position", Upgrade_btn.position, "time", ItweenTime));

            IsUpgradePanelOpened = false;
        }
    }

    public void CloseUpgradePanel() // This also Closes UpgradePanel
    {
        iTween.ScaleTo(UpgradePanel, iTween.Hash("scale", Vector3.zero, "time", ItweenTime));

        iTween.MoveTo(UpgradePanel, iTween.Hash("position", Upgrade_btn.position, "time", ItweenTime));

        IsUpgradePanelOpened = false;
    }

    public void OpenStore() // When Store Panel is Open
    {
        iTween.ScaleTo(StorePanel, iTween.Hash("scale", Vector3.one, "time", ItweenTime));

        iTween.MoveTo(StorePanel, iTween.Hash("position", StorePanel_Pos, "time", ItweenTime));

        // Close UpgradePanel
        CloseUpgradePanel();
    }

    public void CloseStorePanel() // And When Closed
    {
        iTween.ScaleTo(StorePanel, iTween.Hash("scale", Vector3.zero, "time", ItweenTime));

        iTween.MoveTo(StorePanel, iTween.Hash("position", Store_btn.transform.position, "time", ItweenTime));
    }
}
