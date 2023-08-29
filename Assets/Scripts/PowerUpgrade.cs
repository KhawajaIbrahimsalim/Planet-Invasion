using TMPro;
using UnityEngine;

[System.Serializable]
public class PowerUpgrade : MonoBehaviour
{
    [Header("Power Upgrade Properties:")]
    public float DamageRatio;
    public int Level;
    public float UpgradeCost;
    [SerializeField] private float IncreaseCost;

    [Header("Coins Properties:")]
    [SerializeField] private TextMeshProUGUI Coins_txt;

    [Header("UI Properties:")]
    public TextMeshProUGUI Level_txt;
    public TextMeshProUGUI PowerAmount_txt;
    public TextMeshProUGUI UpgradeCost_txt;

    [Header("Mergeable Prefabs:")]
    [SerializeField] private GameObject[] MergeablePrefab;

    private float damage;

    // Update is called once per frame
    void Update()
    {
        foreach (var mergeable in GetComponent<Boosts>().MergeableObjects)
        {
            if (mergeable.GetComponent<ProjectileSpawning>().IsDamageUpgraded == false)
            {
                // Set The Default value
                mergeable.GetComponent<ProjectileSpawning>().Damage = float.Parse(mergeable.GetComponent<ProjectileSpawning>().TimesPower_txt.text);

                // Then Multiply the DamageRatio
                damage = mergeable.GetComponent<ProjectileSpawning>().Damage * DamageRatio;

                // Then store it in the Damage
                mergeable.GetComponent<ProjectileSpawning>().Damage = damage;

                // Make the IsDamageUpgraded false so the it won't repeat it self every frame
                mergeable.GetComponent<ProjectileSpawning>().IsDamageUpgraded = true;

                // Note: It can only repeat when the Button is pressed to Increase the Damage more.
            }
        }
    }

    public void Power_Upgrade()
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

            // Decrease DamageRatio
            DamageRatio += 0.1f;

            // Show DamageRatio
            PowerAmount_txt.text = DamageRatio.ToString(".00");

            // Set Damage for mergeable object that are in the scene
            foreach (var mergeable in GetComponent<Boosts>().MergeableObjects)
            {
                mergeable.GetComponent<ProjectileSpawning>().IsDamageUpgraded = false;
            }
        }
    }
}