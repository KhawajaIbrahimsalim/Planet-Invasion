using UnityEngine;

[System.Serializable]
public class ProjectileMovement : MonoBehaviour
{
    [SerializeField] private float Speed;
    public float Damage;

    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Planet");
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

            transform.position += Speed * Time.deltaTime * Direction;
        }

        else
        {
            Destroy(gameObject);
        }
    }
}
