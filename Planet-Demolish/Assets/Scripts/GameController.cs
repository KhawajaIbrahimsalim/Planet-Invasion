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

    private float Temp_DelayBeforeSpawn;
    private bool IsPaused = true;

    private void Awake()
    {
        CurrentPlanet = GameObject.FindGameObjectWithTag("Planet");

        Temp_DelayBeforeSpawn = DelayBeforeSpawn;
    }

    private void Update()
    {
        // If Player want to Pause the Game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                // Enable PausedGame_Panel
                PauseGame_Panel.SetActive(true);

                IsPaused = false;
            }

            else
            {
                // Disable PausedGame_Panel
                PauseGame_Panel.SetActive(false);

                IsPaused = true;
            }
        }

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
        }

        if (CurrentPlanet == null)
        {
            DelayBeforeSpawn -= Time.deltaTime;
        }
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
}
