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

    [HideInInspector] public bool IfHealthIsHalf = false;

    private GameObject MeteorSpawnPoint_1;
    private GameObject MeteorSpawnPoint_2;
    private GameObject HealthBar;
    private bool IsDestroyed = false;
    private bool IsHit = false;
    private GameObject Hit_particleSystem;
    private GameObject explostionParticles;
    private GameObject Fire_particleSystem;
    private GameObject Fire_particleSystem_2;
    private GameObject Fire_particleSystem_2_2;
    private GameObject GameController;
    private GameObject[] Projectile;

    private void Awake()
    {
        MaxHealth = 15000;
    }

    // Start is called before the first frame update
    void Start()
    {
        HealthBar = GameObject.Find("HealthBar");

        MeteorSpawnPoint_1 = GameObject.Find("MeteorspawnPoint 1");
        MeteorSpawnPoint_2 = GameObject.Find("MeteorspawnPoint 2");

        if (Health == 0)
        {
            Health = MaxHealth;
        }

        Hit_particleSystem = null;
        explostionParticles = null;
        Fire_particleSystem = null;
        Fire_particleSystem_2 = null;

        GameController = GameObject.Find("GameController");

        HealthBar.GetComponent<Slider>().value = Health / MaxHealth;
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        Projectile = GameObject.FindGameObjectsWithTag("Projectile");

        // Destroy the Planet and Instantiate the BOOM! Particle effects
        if (IsDestroyed)
        {
            MeteorSpawnPoint_1.transform.parent = GameController.transform;
            MeteorSpawnPoint_2.transform.parent = GameController.transform;

            foreach (GameObject item in Projectile)
            {
                Destroy(item);
            }

            Destroy(Fire_particleSystem_2_2);
            Destroy(Fire_particleSystem_2);
            Destroy(Fire_particleSystem);

            explostionParticles = Instantiate(Explosion_Particles, gameObject.transform.position, Quaternion.identity);

            Destroy(gameObject);
        }

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

        if (IfHealthIsHalf)
        {
            if (Fire_particleSystem == null)
            {
                Fire_particleSystem = Instantiate(Fire_Particles, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 20), Quaternion.identity);
            }

            if (Fire_particleSystem_2 == null && Fire_particleSystem_2_2 == null)
            {
                // Set the Fire_particleSystem_2 as the child of the Spawnpoint and Set the SpawnPoint as the child of the planet
                Fire_particleSystem_2 = Instantiate(Fire_Particles_2, MeteorSpawnPoint_1.transform.position, Quaternion.identity);
                MeteorSpawnPoint_1.transform.parent = gameObject.transform;
                Fire_particleSystem_2.transform.parent = MeteorSpawnPoint_1.transform;

                // Set Position and rotation
                Fire_particleSystem_2.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));

                // Set the Fire_particleSystem_2_2 as the child of the Spawnpoint and Set the SpawnPoint as the child of the planet
                Fire_particleSystem_2_2 = Instantiate(Fire_Particles_2, MeteorSpawnPoint_2.transform.position, Quaternion.identity);
                MeteorSpawnPoint_2.transform.parent = gameObject.transform;
                Fire_particleSystem_2_2.transform.parent = MeteorSpawnPoint_2.transform;

                // Set Position and rotation
                Fire_particleSystem_2_2.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            }
        }
    }

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            // Decrease Planet Health
            Health -= other.gameObject.GetComponent<ProjectileMovement>().Damage;

            // Also Decrease it in Slider
            HealthBar.GetComponent<Slider>().value = Health / MaxHealth;

            // If Health is half
            if (Health <= MaxHealth / 4)
            {
                IfHealthIsHalf = true;
            }

            else
            {
                IfHealthIsHalf = false;
            }

            // If Health is 0 (Destroy the Planet)
            if (Health <= 0)
            {
                IsDestroyed = true;
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
}
