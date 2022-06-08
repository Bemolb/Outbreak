using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public float MoveSpeed;
    public float Health;
    public float Damage;
    public float AttackRange;


    private Animator _animator;
    private NavMeshAgent _agent;
    private LineRenderer _line;
    private float _canelRunDist;

    private PlayerStorage _playerStorage = new PlayerStorage();

    public bool Current = false;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _line = GetComponent<LineRenderer>();
        _agent.autoRepath = true;
        _agent.autoBraking = true;
        _agent.speed = MoveSpeed;
        _canelRunDist = MoveSpeed * 0.2f;
    }

    private void Update()
    {
        MoveAnim();
    }

    public void Move(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(position, path);
        if (path.status != NavMeshPathStatus.PathComplete)
            return;
        _agent.SetPath(path);
        DrawPath(_agent.path);
    }

    private void MoveAnim()
    {
        if (_agent.remainingDistance >= _canelRunDist)
        {
            _animator.SetBool("isMove", true);
            _animator.speed = _agent.velocity.magnitude / _agent.speed;
            DrawPath(_agent.path);
        }
        else
        {
            _animator.SetBool("isMove", false);
            _line.positionCount = 0;
        }
    }

    public void Attack(GameObject enemy)
    {

        if (Vector3.Distance(transform.position, enemy.transform.position) <= AttackRange)
        {
            var pl = enemy.GetComponent<Enemy>();
            pl.SetDamage(Damage * Time.deltaTime);
        }
    }

    public void SetDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            _animator.SetTrigger("death");
            Destroy(gameObject);
            _playerStorage.RemovePlayer(this);
        }
    }

    private void DrawPath(NavMeshPath path)
    {
        if (path.corners.Length < 2)
        {
            if (path.corners.Length != 0)
                _line.positionCount = 0;
            return;
        }
        _line.positionCount = path.corners.Length;
        _line.SetPosition(0, new Vector3(transform.position.x, 0, transform.position.z));
        for (var i = 1; i < path.corners.Length; i++)
        {
            _line.SetPosition(i, path.corners[i]);
        }
    }
}
