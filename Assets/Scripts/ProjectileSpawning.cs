using TMPro;
using UnityEngine;

[System.Serializable]
public class ProjectileSpawning : MonoBehaviour
{
    [Header("Spawn Properties:")]
    public float SpawnDelay;
    public float Temp_SpawnDelay;
    public GameObject ProjectileSpawnPoint;
    [SerializeField] private GameObject Projectile;
    [SerializeField] private float HoldTime;
    [SerializeField] private float TouchSpawnDelay;

    [Header("Damage Properties:")]
    public float Damage;
    public TextMeshProUGUI TimesPower_txt;

    [Header("Animation Properties:")]
    [SerializeField] private Animator AttackAnimation;
    [SerializeField] private float AnimDelay;

    [HideInInspector] public bool IsDamageIncreased = false;
    [HideInInspector] public bool IsDamageUpgraded = false;

    private float Temp_AnimDelay;
    private float Temp_TouchSpawnDelay;
    private bool IsTouched;
    private GameObject GameController;
    private float Timer = 0f;
    private GameObject Audio_Source;
    private bool FirstFireDeleted = false;

    // Start is called before the first frame update
    void Start()
    {
        GameController = GameObject.Find("GameController");

        Audio_Source = GameObject.Find("Audio Source");

        Temp_TouchSpawnDelay = TouchSpawnDelay;

        Temp_AnimDelay = AnimDelay;

        Temp_SpawnDelay = GameController.GetComponent<SpeedUpgrade>().SpeedRatio;

        // Default Damage / Starting Damage
        if (Damage != float.Parse(TimesPower_txt.text))
        {
            IsDamageUpgraded = false;

            Damage = float.Parse(TimesPower_txt.text);
        }

        else // After Damage is Upgraded and also it means the Damage value is been set in SavaLoad Script
        {
            IsDamageUpgraded = true;
        }

        ProjectileSpawnPoint.transform.localPosition = Vector3.zero;

        FirstFireDeleted = false;
    }

    // Update is called once per frame
    void Update()
    {
        // If Auto Click is on, Play the Auto Click Animation
        if (GameController.GetComponent<GameController>().IsAutoClickActive == true)
        {
            AttackAnimation.SetBool("AutoClick", true);

            if (SpawnDelay <= 0)
            {
                SpawnProjectile();

                SpawnDelay = Temp_SpawnDelay;
            }

            else
            {
                SpawnDelay -= Time.deltaTime;
            }
        }

        // Auto Click Animation
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Cast a ray from the camera to the touch position
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            // Check if the ray hits an object with a collider
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.tag != "Untouchable" && hit.collider.gameObject.tag != "Mergable")
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        // Reset the Timer
                        Timer = 0f;

                        IsTouched = true;
                    }

                    // If Player is holding Touch on the Screen then after a delay Turnoff the animation 
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        Timer += Time.deltaTime;

                        if (Timer >= HoldTime)
                        {
                            AttackAnimation.SetBool("AutoClick", false);

                            IsTouched = false;
                        }
                    }

                    if (touch.phase == TouchPhase.Ended)
                    {
                        IsTouched = false;
                    }

                    if (IsTouched)
                    {
                        TouchSpawnDelay -= Time.deltaTime;

                        if (TouchSpawnDelay <= 0f)
                        {
                            AttackAnimation.SetBool("AutoClick", true);

                            SpawnProjectile();

                            IsTouched = false;

                            TouchSpawnDelay = Temp_TouchSpawnDelay;
                        }  
                    }
                }

                else
                {
                    AttackAnimation.SetBool("AutoClick", false);

                    IsTouched = false;
                }
            }

            // If Player touch or keep touching then the delay will be refilled
            AnimDelay = Temp_AnimDelay;
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

        // Auto Click animation
        // At a certain Speed animation changes
        if (GameController.GetComponent<SpeedUpgrade>().SpeedRatio <= 0.5f)
        {
            if (SpawnDelay <= 0 && AnimDelay != Temp_AnimDelay && GameController.GetComponent<GameController>().IsAutoClickActive == false)
            {
                FirstFireDeleted = true;

                AttackAnimation.SetBool("AutoClick", true);

                SpawnProjectile();

                SpawnDelay = Temp_SpawnDelay;
            }

            else
            {
                AttackAnimation.SetBool("AutoClick", true);

                SpawnDelay -= Time.deltaTime;
            }
        }

        else // Throw Animation
        {          
            // If Auto Click is off and also there is not touch then stop this animation and Spawn Projectile
            if (SpawnDelay <= 0 && AnimDelay != Temp_AnimDelay && AttackAnimation.GetBool("AutoClick") == false
                && GameController.GetComponent<GameController>().IsAutoClickActive == false)
            {
                FirstFireDeleted = true;

                AttackAnimation.SetBool("Throw", true);

                SpawnProjectile();

                SpawnDelay = Temp_SpawnDelay;
            }

            else
            {
                AttackAnimation.SetBool("Throw", false);

                SpawnDelay -= Time.deltaTime;
            }
        }

        // Note: If AnimDelay != Temp_AnimDelay then the player is NOT touching the screen and vice versa
    }

    private void SpawnProjectile()
    {
        if (GameController.GetComponent<GameController>().CurrentPlanet && FirstFireDeleted)
        {
            // Spawn Projectile
            GameObject projectile = Instantiate(Projectile, ProjectileSpawnPoint.transform.position, Quaternion.identity);

            // Set Damage
            projectile.GetComponent<ProjectileMovement>().Damage = Damage;

            // Play Audio for this action
            Audio_Source.GetComponent<AudioController>().PlayAudioSource(Audio_Source: Audio_Source.GetComponent<AudioController>().Shoot_Audio);
        }

        else
        {
            FirstFireDeleted = true;
        }
    }
}
