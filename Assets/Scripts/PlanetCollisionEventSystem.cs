using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlanetCollisionEventSystem : MonoBehaviour
{
    [Header("Health Properties:")]
    public float MaxHealth;
    public float Health;

    [Header("Particle Systems:")]
    public GameObject Explosion_Particles;
    public GameObject Hit_Particles;
    public GameObject Planet_Fire_Particles;
    public GameObject Impact_Particles;

    [Header("Half Health Particle Systems:")]
    public GameObject Fire_Particles;
    public GameObject Fire_Particles_2;

    private GameObject HealthBar;
    private bool IsHit = false;
    private bool IsHalfHealth = true;
    private bool IsDestroyed = true;
    private GameObject Hit_particleSystem;
    private GameObject explostionParticles;
    private GameObject Fire_particleSystem;
    private GameObject GameController;
    private GameObject[] Projectile;
    private Vector3 Damage_Indicator_txt_pos;
    private GameObject Audio_Source;

    private void Awake()
    {
        GameController = GameObject.Find("GameController");

        MaxHealth = 15000;
    }

    // Start is called before the first frame update
    void Start()
    {
        Audio_Source = GameObject.Find("Audio Source");

        HealthBar = GameObject.Find("HealthBar");

        if (Health == 0)
        {
            Health = MaxHealth;
        }

        Hit_particleSystem = null;
        explostionParticles = null;

        HealthBar.GetComponent<Slider>().value = Health / MaxHealth;
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        Projectile = GameObject.FindGameObjectsWithTag("Projectile");

        // Destroy the BOOM! Particle effect
        if (explostionParticles != null)
        {
            Destroy(explostionParticles, 1.2f);
        }

        // Instantiate the Hit Particle effect when Planet is hit with a projectile
        if (IsHit)
        {
            Hit_particleSystem = Instantiate(Hit_Particles, gameObject.transform.position, Quaternion.identity);

            IsHit = false;
        }

        // Destroy Hit Particle effect
        if (Hit_particleSystem != null)
        {
            Destroy(Hit_particleSystem, 0.8f);
        }

        else
        {
            // Disable Damage_txt
            GameController.GetComponent<GameController>().Damage_Indicator_txt.SetActive(false);
        }
    }

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            // Enable Damage_Indicator_txt
            GameController.GetComponent<GameController>().Damage_Indicator_txt.SetActive(true);

            // Show Damage_Indicator_txt
            GameController.GetComponent<GameController>().Damage_Indicator_txt.GetComponent<TextMeshProUGUI>().text = "-" + GameController.GetComponent<GameController>().CompressNumber(other.gameObject.GetComponent<ProjectileMovement>().Damage);

            // Add Coins
            float Coins = (GameController.GetComponent<GameController>().Coins += other.gameObject.GetComponent<ProjectileMovement>().Damage);

            // Show Coins
            GameController.GetComponent<GameController>().Coins_txt.GetComponent<TextMeshProUGUI>().text = GameController.GetComponent<GameController>().CompressNumber(Coins);

            // Decrease Planet Health
            Health -= other.gameObject.GetComponent<ProjectileMovement>().Damage;

            // Also Decrease it in Slider
            HealthBar.GetComponent<Slider>().value = Health / MaxHealth;

            // If Health is half
            if (Health <= MaxHealth / 4)
            {
                HalfHealth();
            }

            // If Health is 0 (Destroy the Planet)
            if (Health <= 0)
            {
                DestroyPlanet();
            }
            // If Health is not Zero but got hit
            else
            {
                IsHit = true;
            }

            // Destroy the projectile
            Destroy(other.gameObject);
        }
    }

    private void HalfHealth()
    {
        if (IsHalfHealth)
        {
            GameController.GetComponent<GameController>().MeteorSpawnPoint_1.SetActive(true);
            GameController.GetComponent<GameController>().MeteorSpawnPoint_2.SetActive(true);

            GameController.GetComponent<GameController>().MeteorSpawnPoint_1.transform.parent = gameObject.transform;
            GameController.GetComponent<GameController>().MeteorSpawnPoint_2.transform.parent = gameObject.transform;

            if (Fire_particleSystem == null)
            {
                Fire_particleSystem = Instantiate(Fire_Particles, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), Quaternion.identity);
            }

            IsHalfHealth = false;
        }
    }

    private void DestroyPlanet()
    {
        if (IsDestroyed)
        {
            // Play Audio for this action
            Audio_Source.GetComponent<AudioController>().PlayAudioSource(Audio_Source: Audio_Source.GetComponent<AudioController>().BOOM_Audio);

            // Destry Every Projectile
            foreach (GameObject item in Projectile)
            {
                Destroy(item);
            }

            GameController.GetComponent<GameController>().MeteorSpawnPoint_1.transform.parent = GameController.transform;
            GameController.GetComponent<GameController>().MeteorSpawnPoint_2.transform.parent = GameController.transform;

            // Disable Half Health Particle Parent Objects
            GameController.GetComponent<GameController>().MeteorSpawnPoint_1.SetActive(false);
            GameController.GetComponent<GameController>().MeteorSpawnPoint_2.SetActive(false);

            // Destroy BackFire Particle, Activated when Half Health
            Destroy(Fire_particleSystem);

            // And then Spawn the explosion
            explostionParticles = Instantiate(Explosion_Particles, gameObject.transform.position, Quaternion.identity);

            IsDestroyed = false;

            // Destroy this GameObject (Planet)
            Destroy(gameObject);
        }
    }
}
