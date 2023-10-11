using TMPro;
using UnityEngine;

[System.Serializable]
public class SpeedUpgrade : MonoBehaviour
{
    [Header("Speed Upgrade Properties:")]
    public float SpeedRatio;
    public int Level;
    public float UpgradeCost;
    [SerializeField] private float IncreaseCost;

    [Header("Coins Properties:")]
    [SerializeField] private TextMeshProUGUI Coins_txt;

    [Header("UI Properties:")]
    public TextMeshProUGUI Level_txt;
    public TextMeshProUGUI SpeedAmount_txt;
    public TextMeshProUGUI UpgradeCost_txt;
    public GameObject Coin;
    public GameObject Completed_txt;

    [Header("Mergeable Prefabs:")]
    [SerializeField] private GameObject[] MergeablePrefab;

    private bool IsHeldDown = false;

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Boosts>().IsDelayChanged == true)
        {
            // Set Delay for the mergeable that are present in the scene
            foreach (var mergeable in GetComponent<Boosts>().MergeableObjects)
            {
                if (mergeable.GetComponent<ProjectileSpawning>().Temp_SpawnDelay != SpeedRatio)
                {
                    mergeable.GetComponent<ProjectileSpawning>().Temp_SpawnDelay = SpeedRatio;
                }
            }
        }

        if (IsHeldDown && GetComponent<GameController>().IsPurchased == false)
        {
            Speed_Upgrade();
        }
    }

    public void Speed_Upgrade()
    {
        if (GetComponent<GameController>().Coins >= UpgradeCost && SpeedRatio > 0.50f)
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

            // Decrease SpeedRatio
            SpeedRatio -= 0.01f;

            // Show SpeedRatio
            SpeedAmount_txt.text = SpeedRatio.ToString(".00");
        }

        if (SpeedRatio <= 0.5f) // If Speed Upgrade limit has reached its limit then Enable Completed Text and Disable Cost
        {
            Coin.SetActive(false);

            Completed_txt.SetActive(true);
        }
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
