using TMPro;
using UnityEngine;

public class Boosts : MonoBehaviour
{
    [Header("Auto click Properties:")]
    [SerializeField] private float AutoClick_Delay;
    [SerializeField] private GameObject AutoClick_txt;
    [SerializeField] private float DelayAfterPressingTheButton;

    private float MaxAutoClickDelay;
    private float Temp_DelayAfterPressingTheButton;
    private GameObject GameController;
    private bool IsDelayChanged = true;
    private GameObject[] MergeableObjects;

    // Start is called before the first frame update
    void Start()
    {
        GameController = GameObject.Find("GameController");
        AutoClick_txt = GameObject.Find("Auto Click_txt");

        IsDelayChanged = true;
        MaxAutoClickDelay = AutoClick_Delay;
        Temp_DelayAfterPressingTheButton = DelayAfterPressingTheButton;
    }

    // Update is called once per frame
    void Update()
    {
        MergeableObjects = GameObject.FindGameObjectsWithTag("Mergable");

        AutoClick_txt.GetComponent<TextMeshProUGUI>().text = "Auto: " + AutoClick_Delay.ToString("0") + "/" + MaxAutoClickDelay;

        if (AutoClick_Delay > 0)
        {
            GameController.GetComponent<GameController>().IsAutoClickActive = false;
        }

        if (GameController.GetComponent<GameController>().IsAutoClickActive && AutoClick_Delay <= 0)
        {
            foreach (var mergeable in MergeableObjects)
            {
                if (mergeable.GetComponent<ProjectileSpawning>().Temp_SpawnDelay == 2f)
                {
                    mergeable.GetComponent<ProjectileSpawning>().Temp_SpawnDelay = 0.2f;
                }
            }

            IsDelayChanged = false;
        }

        else if (!GameController.GetComponent<GameController>().IsAutoClickActive && AutoClick_Delay > 0)
        {
            AutoClick_Delay -= Time.deltaTime;
        }       

        if (IsDelayChanged == false && DelayAfterPressingTheButton > 0)
        {
            DelayAfterPressingTheButton -= Time.deltaTime;
        }

        else if (IsDelayChanged == false && DelayAfterPressingTheButton <= 0)
        {
            AutoClick_Delay = MaxAutoClickDelay;

            AutoClick_txt.GetComponent<TextMeshProUGUI>().text = "Auto: " + AutoClick_Delay.ToString("0") + "/" + MaxAutoClickDelay;

            DelayAfterPressingTheButton = Temp_DelayAfterPressingTheButton;

            foreach (var mergeable in MergeableObjects)
            {
                mergeable.GetComponent<ProjectileSpawning>().Temp_SpawnDelay = 2f;
            }

            GameController.GetComponent<GameController>().IsAutoClickActive = false;

            IsDelayChanged = true;
        }
    }
}
