using System;
using TMPro;
using UnityEngine;

[System.Serializable]
public class OfflineCurrency : MonoBehaviour
{
    [Header("Coins Properties:")]
    [SerializeField] private TextMeshProUGUI Coins_txt;
    [SerializeField] private TextMeshProUGUI MadeCoins_txt;

    [Header("Offline Currency Properties:")]
    public float currencyPerSecond; // The amount of currency earned per second while offline
    private float maxOfflineTime = TimeSpan.TicksPerDay; // The maximum amount of time the player can be offline and still earn currency (in seconds)

    [Header("UI Properties:")]
    [SerializeField] private GameObject OfflineCoinsPanel;
    public TextMeshProUGUI Level_txt;
    public TextMeshProUGUI UpgradeCost_txt;
    public TextMeshProUGUI Earning_txt;

    [Header("Upgrade Properties:")]
    public float IncreamentCurrency;
    public int Level;
    public float UpgradeCost;
    [SerializeField] private float IncreaseCost;

    private bool IsHeldDown = false;
    private float MadeCoins = 0.0f;

    void Start()
    {
        if (Level != 1)
        {
            OfflineCoinsPanel.SetActive(true);

            // Load the last login time from PlayerPrefs
            string lastLoginTimeStr = PlayerPrefs.GetString("LastLoginTime", "");
            if (!string.IsNullOrEmpty(lastLoginTimeStr))
            {
                DateTime lastLoginTime = DateTime.Parse(lastLoginTimeStr);
                TimeSpan offlineTime = DateTime.Now - lastLoginTime;

                // Add OfflineTime in Seconds in Coins with a maxOfflineTime limit multiply by currencyPerSecond
                MadeCoins = Mathf.Min((float)offlineTime.TotalSeconds, maxOfflineTime) * currencyPerSecond;

                GetComponent<GameController>().Coins += MadeCoins;

                // Show on Coins_txt
                Coins_txt.text = GetComponent<GameController>().CompressNumber(GetComponent<GameController>().Coins);

                MadeCoins_txt.text = GetComponent<GameController>().CompressNumber(MadeCoins);

                Debug.Log("Last Login Data and Time: " + lastLoginTime);

                // Use offlineTime to calculate the offline time in seconds, minutes, hours, etc.
                Debug.Log("Player was offline for " + offlineTime.TotalSeconds + " seconds");

                Debug.Log("Offline Time Multiply by (" + currencyPerSecond + "): " + GetComponent<GameController>().CompressNumber(GetComponent<GameController>().Coins));
            }
        }
    }

    private void OnApplicationQuit()
    {
        // The game has been paused (e.g. the player closed the app)
        Debug.Log("Data and Time on Application Close: " + PlayerPrefs.GetString("LastLoginTime", ""));
    }

    private void Update()
    {
        if (IsHeldDown)
        {
            OfflineUpgrade();
        }

        // Save the current time to PlayerPrefs
        PlayerPrefs.SetString("LastLoginTime", DateTime.Now.ToString());
    }

    public void OfflineUpgrade()
    {
        if (GetComponent<GameController>().Coins >= UpgradeCost)
        {
            // Substract the Cost from the total Coins
            GetComponent<GameController>().Coins -= UpgradeCost;

            // Show Changes for Coins
            Coins_txt.text = GetComponent<GameController>().CompressNumber(GetComponent<GameController>().Coins);

            // Increase Level
            Level++;

            // Show Level Increase
            Level_txt.text = "Lv." + Level;

            // Increase UpgradeCost
            UpgradeCost *= IncreaseCost;

            // Show Upgrade Cost
            UpgradeCost_txt.text = GetComponent<GameController>().CompressNumber(UpgradeCost);

            if (currencyPerSecond == 1)
            {
                currencyPerSecond = 0;
            }

            // Increase currencyPerSecond
            currencyPerSecond += IncreamentCurrency;

            // Show Earning_txt
            Earning_txt.text = "Earning × " + GetComponent<GameController>().CompressNumber(currencyPerSecond);
        }
    }

    public void WatchAds_DoubleCoins()
    {
        // Double the coins
        MadeCoins *= 2;
        GetComponent<GameController>().Coins += MadeCoins;

        GetComponent<GameController>().Coins_txt.text = GetComponent<GameController>().CompressNumber(GetComponent<GameController>().Coins);

        // Disable OfflineCoinsPanel
        OfflineCoinsPanel.SetActive(false);

        Debug.Log(MadeCoins);
    }

    public void Close_btn()
    {
        OfflineCoinsPanel.SetActive(false);
    }

    public void OnPointerDown()
    {
        IsHeldDown = true;
    }

    public void OnPointerUp()
    {
        IsHeldDown = false;
    }
}