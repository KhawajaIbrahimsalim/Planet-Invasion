using TMPro;
using UnityEngine;

[System.Serializable]
public class ProjectileSpawning : MonoBehaviour
{
    [Header("Spawn Properties:")]
    public float SpawnDelay;
    [SerializeField] private GameObject Projectile;
    [SerializeField] private GameObject ProjectileSpawnPoint;

    [Header("Damage Properties:")]
    public float Damage;
    public TextMeshProUGUI TimesPower_txt;

    [Header("Animation Properties:")]
    [SerializeField] private Animator AttackAnimation;
    [SerializeField] private float AnimDelay;

    [HideInInspector] public float Temp_SpawnDelay;
    [HideInInspector] public bool IsDamageIncreased = false;

    private float Temp_AnimDelay;
    private bool IsTouched;
    private GameObject GameController;

    // Start is called before the first frame update
    void Start()
    {
        GameController = GameObject.Find("GameController");

        Temp_SpawnDelay = SpawnDelay;

        Temp_AnimDelay = AnimDelay;

        Damage = float.Parse(TimesPower_txt.text);
    }

    // Update is called once per frame
    void Update()
    {
        // Auto Click Animation
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                IsTouched = true;
            }

            if (touch.phase == TouchPhase.Moved && GameController.GetComponent<GameController>().IsAutoClickActive == false)
            {
                AttackAnimation.SetBool("AutoClick", false);
            }

            if (touch.phase == TouchPhase.Ended)
            {
                IsTouched = false;
            }

            if (IsTouched)
            {
                AttackAnimation.SetBool("AutoClick", true);

                GameObject projectile = Instantiate(Projectile, ProjectileSpawnPoint.transform.position, Quaternion.identity);

                projectile.GetComponent<ProjectileMovement>().Damage = Damage;

                IsTouched = false;
            }

            // If Player touch or keep touching then the delay will be refilled
            AnimDelay = Temp_AnimDelay;
        }

        // If Auto Click is on the Play the Auto Click Animation
        else if (GameController.GetComponent<GameController>().IsAutoClickActive == true)
        {
            AttackAnimation.SetBool("AutoClick", true);
        }

        else
        {
            //=> Wait for Delay to finish.
            //=> The reason for this delay is that during the frame while touching the screen if in one frame
            // animation has started then in the other frame animation stopped, so the problem arises
            // when player is touching continusely a slight delay in touch stops the animation which does not
            // make it a smooth animation, so a delay is added to make it up for that slight nano second pause
            // in touch.
            if (AnimDelay <= 0)
            {
                AttackAnimation.SetBool("AutoClick", false);

                AnimDelay = Temp_AnimDelay;
            }

            AnimDelay -= Time.deltaTime;
        }

        // Throw Animation
        if (SpawnDelay <= 0)
        {
            // If Auto Click is on then stop this animation
            if (AttackAnimation.GetBool("AutoClick") == false && GameController.GetComponent<GameController>().IsAutoClickActive == false)
            {
                AttackAnimation.SetBool("Throw", true);
            }

            GameObject projectile = Instantiate(Projectile, ProjectileSpawnPoint.transform.position, Quaternion.identity);

            projectile.GetComponent<ProjectileMovement>().Damage = Damage;

            SpawnDelay = Temp_SpawnDelay;
        }

        else
        {
            AttackAnimation.SetBool("Throw", false);

            SpawnDelay -= Time.deltaTime;
        }
    }
}
