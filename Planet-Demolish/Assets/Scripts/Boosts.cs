using TMPro;
using UnityEngine;

[System.Serializable]
public class Boosts : MonoBehaviour
{
    [Header("Auto click Properties:")]
    public float AutoClick_Delay;
    public float MaxAutoClickDelay;
    [SerializeField] private GameObject AutoClick_txt;
    public float DelayAfterPressingTheButton;
    public float MaxDelayAfterPressingTheButton;
    [SerializeField] private GameObject DelayAfterPressingTheButton_txt;

    [Header("Damage 5x Properties:")]
    public int TouchCount;
    public int MaxTouch;
    [SerializeField] private GameObject Damage5x_txt;
    public float DelayDamage5x;
    public float MaxDelayDamage5x;
    [SerializeField] private GameObject DelayDamage5x_txt;

    [HideInInspector] public bool IsDelayChanged = true;

    private GameObject GameController;
    private GameObject[] MergeableObjects;

    // Start is called before the first frame update
    void Start()
    {
        GameController = GameObject.Find("GameController");
        AutoClick_txt = GameObject.Find("Auto Click_txt");

        IsDelayChanged = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Auto-Click
        {
            MergeableObjects = GameObject.FindGameObjectsWithTag("Mergable");

            // Show time every frame
            AutoClick_txt.GetComponent<TextMeshProUGUI>().text = "Auto: " + AutoClick_Delay.ToString("0") + "/" + MaxAutoClickDelay.ToString("0");

            // IsAutoClickActive should always be false if AutoClick_Delay > 0, I have to write this because IsAutoClickActive
            // is true when we click the button
            if (AutoClick_Delay > 0)
            {
                GameController.GetComponent<GameController>().IsAutoClickActive = false;
            }

            // If IsAutoClickActive is true and AutoClick_Delay <= 0 the shorten the delay of all the Mergeable objects
            if (GameController.GetComponent<GameController>().IsAutoClickActive && AutoClick_Delay <= 0)
            {
                foreach (var mergeable in MergeableObjects)
                {
                    if (mergeable.GetComponent<ProjectileSpawning>().Temp_SpawnDelay == 2f)
                    {
                        mergeable.GetComponent<ProjectileSpawning>().Temp_SpawnDelay = 0.2f;
                    }
                }

                // Now this is IMPORTANT
                IsDelayChanged = false;

                // Disable the AutoClick_txt so that DelayAfterPressingTheButton_txt can be visible
                AutoClick_txt.SetActive(false);
            }

            // else continue to minus the delay
            else if (!GameController.GetComponent<GameController>().IsAutoClickActive && AutoClick_Delay > 0)
            {
                AutoClick_Delay -= Time.deltaTime;
            }

            // Now if AutoClick_Delay is <= 0 so now its time to have a delay for how long the SpawnDelay is shorten
            if (IsDelayChanged == false && DelayAfterPressingTheButton > 0)
            {
                DelayAfterPressingTheButton_txt.SetActive(true);

                DelayAfterPressingTheButton -= Time.deltaTime;

                DelayAfterPressingTheButton_txt.GetComponent<TextMeshProUGUI>().text = DelayAfterPressingTheButton.ToString("0");
            }

            // If DelayAfterPressingTheButton <= 0 Refill all delays and IsDelayChanged and, IsAutoClickActive is true
            else if (IsDelayChanged == false && DelayAfterPressingTheButton <= 0)
            {
                AutoClick_txt.SetActive(true);

                DelayAfterPressingTheButton_txt.SetActive(false);

                AutoClick_Delay = MaxAutoClickDelay;

                AutoClick_txt.GetComponent<TextMeshProUGUI>().text = "Auto: " + AutoClick_Delay.ToString("0") + "/" + MaxAutoClickDelay;

                DelayAfterPressingTheButton = MaxDelayAfterPressingTheButton;

                foreach (var mergeable in MergeableObjects)
                {
                    mergeable.GetComponent<ProjectileSpawning>().Temp_SpawnDelay = 2f;
                }

                GameController.GetComponent<GameController>().IsAutoClickActive = false;

                IsDelayChanged = true;
            }
        }

        // Damage 5x
        {
            if (TouchCount != MaxTouch)
            {
                GameController.GetComponent<GameController>().Damage5xIsActive = false;

                Damage5x_txt.SetActive(true);
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began && TouchCount < MaxTouch)
                {
                    TouchCount++;

                    Damage5x_txt.GetComponent<TextMeshProUGUI>().text = "Clicks: " + TouchCount + "/" + MaxTouch;      
                }
            }

            // If Required touches on the screen is done and button is also clicked then Multiply damage by 5x
            if (GameController.GetComponent<GameController>().Damage5xIsActive && TouchCount == MaxTouch)
            {
                Damage5x_txt.SetActive(false);

                DelayDamage5x_txt.SetActive(true);

                Debug.Log("Damage 5x");
                foreach (var mergeable in MergeableObjects)
                {
                    if (mergeable.GetComponent<ProjectileSpawning>().IsDamageIncreased == false)
                    {
                        mergeable.GetComponent<ProjectileSpawning>().Damage = mergeable.GetComponent<ProjectileSpawning>().Damage * 5f;

                        mergeable.GetComponent<ProjectileSpawning>().IsDamageIncreased = true;
                    }
                }

                if (DelayDamage5x <= 0)
                {
                    DelayDamage5x_txt.SetActive(false);

                    TouchCount = 0;

                    DelayDamage5x = MaxDelayDamage5x;

                    Damage5x_txt.GetComponent<TextMeshProUGUI>().text = "Clicks: " + TouchCount + "/" + MaxTouch;

                    foreach (var mergeable in MergeableObjects)
                    {
                        mergeable.GetComponent<ProjectileSpawning>().IsDamageIncreased = false;

                        mergeable.GetComponent<ProjectileSpawning>().Damage = float.Parse(mergeable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
                    }
                }

                DelayDamage5x_txt.GetComponent<TextMeshProUGUI>().text = DelayDamage5x.ToString("0");

                DelayDamage5x -= Time.deltaTime;
            }
        }
    }
}
