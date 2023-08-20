using TMPro;
using UnityEngine;

public class ProjectileSpawning : MonoBehaviour
{
    [Header("Spawn Properties:")]
    public float SpawnDelay;
    [SerializeField] private GameObject Projectile;
    [SerializeField] private GameObject ProjectileSpawnPoint;

    [Header("Damage Properties:")]
    public float Damage;
    public TextMeshProUGUI TimesPower_txt;

    [HideInInspector] public float Temp_SpawnDelay;
    [HideInInspector] public bool IsDamageIncreased = false;

    private bool IsTouched;

    // Start is called before the first frame update
    void Start()
    {
        Temp_SpawnDelay = SpawnDelay;

        Damage = float.Parse(TimesPower_txt.text);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                IsTouched = true;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                IsTouched = false;
            }

            if (IsTouched)
            {
                GameObject projectile = Instantiate(Projectile, ProjectileSpawnPoint.transform.position, Quaternion.identity);

                projectile.GetComponent<ProjectileMovement>().Damage = Damage;

                IsTouched = false;
            }
        }

        if (SpawnDelay <= 0)
        {
            GameObject projectile = Instantiate(Projectile, ProjectileSpawnPoint.transform.position, Quaternion.identity);

            projectile.GetComponent<ProjectileMovement>().Damage = Damage;

            SpawnDelay = Temp_SpawnDelay;
        }

        SpawnDelay -= Time.deltaTime;
    }
}
