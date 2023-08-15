using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameController : MonoBehaviour
{
    [SerializeField] private float DelayBeforeSpawn;

    public GameObject[] Planets;
    public GameObject CurrentPlanet;

    private float Temp_DelayBeforeSpawn;

    private void Awake()
    {
        CurrentPlanet = GameObject.FindGameObjectWithTag("Planet");

        Temp_DelayBeforeSpawn = DelayBeforeSpawn;
    }

    private void Update()
    {
        if (CurrentPlanet == null && DelayBeforeSpawn <= 0f) 
        {
            int RandomPlanet = Random.Range(0, Planets.Length);

            CurrentPlanet = Instantiate(Planets[RandomPlanet], gameObject.transform.position, Quaternion.identity);

            CurrentPlanet.transform.parent = gameObject.transform;

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
}
