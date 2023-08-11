using UnityEngine;

public class ProjectileSpawning : MonoBehaviour
{
    [SerializeField] private float SpawnDelay;
    [SerializeField] private GameObject Projectile;
    [SerializeField] private GameObject ProjectileSpawnPoint;

    private float Temp_SpawnDelay;
    private bool IsTouched;

    // Start is called before the first frame update
    void Start()
    {
        Temp_SpawnDelay = SpawnDelay;
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
                Instantiate(Projectile, ProjectileSpawnPoint.transform.position, Quaternion.identity);

                IsTouched = false;
            }
        }

        if (SpawnDelay <= 0)
        {
            Instantiate(Projectile, ProjectileSpawnPoint.transform.position, Quaternion.identity);

            SpawnDelay = Temp_SpawnDelay;
        }

        SpawnDelay -= Time.deltaTime;
    }
}
