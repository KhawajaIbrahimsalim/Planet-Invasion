using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameController : MonoBehaviour
{
    public GameObject[] Planets;

    private GameObject CurrentPlanet;
    private GameObject HealthBar;

    private void Awake()
    {
        CurrentPlanet = GameObject.FindGameObjectWithTag("Planet");
        HealthBar = GameObject.Find("HealthBar");
    }

    private void Update()
    {
        if (CurrentPlanet == null) 
        {
            int RandomPlanet = Random.Range(0, Planets.Length);

            CurrentPlanet = Instantiate(Planets[RandomPlanet], gameObject.transform.position, Quaternion.identity);

            CurrentPlanet.transform.parent = gameObject.transform;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
