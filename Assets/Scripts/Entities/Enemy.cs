using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Speed;
    public float Damage;
    public float Health;
    public float AttackRange;
    public float StopRange;
    private EnemyMovment _movment;

    private void Start()
    {
        Enemy enemy = this;
        _movment = new EnemyMovment(enemy);
    }

    private void Update()
    {
        _movment.Move();
    }

    public void SetDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
            Destroy(gameObject);
    }

    public void Attack(GameObject player)
    {

        if(Vector3.Distance(transform.position, player.transform.position) <= AttackRange)
        {
            var pl = player.GetComponent<Player>();
            pl.SetDamage(Damage * Time.deltaTime);
        }
    }
}
