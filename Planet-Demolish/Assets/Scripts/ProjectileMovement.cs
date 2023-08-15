using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    [SerializeField] private float Speed;

    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Planet");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 Direction = target.transform.position - transform.position;

            Direction.Normalize();

            transform.position += Direction * Time.deltaTime * Speed;
        }
    }
}
