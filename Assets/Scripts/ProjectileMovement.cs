using UnityEngine;

[System.Serializable]
public class ProjectileMovement : MonoBehaviour
{
    [SerializeField] private float Speed;
    public float Damage;
    [SerializeField] private float Spread;

    private GameObject target;
    float x;
    float y;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Planet");

        // Add Spread effect to the game
        x = Random.Range(-Spread, Spread);
        y = Random.Range(-Spread, Spread);
    }

    // Update is called once per frame
    private void Update()
    {
        Destroy(gameObject, 10f);
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 Direction = target.transform.position - transform.position;

            Direction.Normalize();

            transform.position += Speed * Time.deltaTime * (Direction + new Vector3(x, y, 0));
        }

        else
        {
            Destroy(gameObject);
        }
    }
}
