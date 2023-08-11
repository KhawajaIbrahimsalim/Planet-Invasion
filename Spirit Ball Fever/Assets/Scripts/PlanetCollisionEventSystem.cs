using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlanetCollisionEventSystem : MonoBehaviour
{
    public float MaxHealth;
    public float Health;
    public GameObject HealthBar;
    public GameObject Panel;
    public GameObject Explosion_Particles;
    public GameObject Hit_Particles;
    public GameObject Planet_Fire_Particles;

    [Header("Half Health Particle Systems:")]
    public GameObject Fire_Particles;
    public GameObject Fire_Particles_2;
    public GameObject Fire_Particles_3;

    private bool IsDestroyed = false;
    private bool IsHit = false;
    private bool IfHealthIsHalf = false;
    private GameObject Hit_particleSystem;
    private GameObject explostionParticles;
    private GameObject Fire_particleSystem;

    // Start is called before the first frame update
    void Start()
    {
        if (Health == 0)
        {
            Health = MaxHealth;
        }

        Hit_particleSystem = null;
        explostionParticles = null;
        Fire_particleSystem = null;
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        // Destroy the Planet and Instantiate the BOOM! Particle effects
        if (IsDestroyed)
        {
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
                Fire_particleSystem = Instantiate(Fire_Particles, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 70), Quaternion.identity);
            }

            else
            {
                //Fire_particleSystem.transform.position = new Vector3(Fire_particleSystem.transform.position.x, Fire_particleSystem.transform.position.y, Fire_particleSystem.transform.position.z - 7);
                Fire_particleSystem.GetComponent<ParticleSystem>().startSize += 40;
            }

            Fire_Particles_2.SetActive(true);
            Fire_Particles_3.SetActive(true);

            IfHealthIsHalf = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Projectile")
        {
            // Destroy the projectile
            Destroy(other.gameObject);

            // Decrease Planet Health
            Health--;

            // Also Decrease it in Slider
            HealthBar.GetComponent<Slider>().value = Health / MaxHealth;

            // If Health is half
            if (Health <= MaxHealth / 2)
            {
                IfHealthIsHalf = true;
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
        }
    }
}
