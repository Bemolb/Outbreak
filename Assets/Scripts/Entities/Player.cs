using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public float MoveSpeed;
    public float Health;
    public float Damage;
    public float AttackRange;

    
    public NavMeshAgent Agent { get; private set; }

    private IAnimatorService _animatorService;
    private LineRenderer _line;
    private Collider _collider;
    private float _canelRunDist;
    private bool _isDead;

    private PlayerStorage _playerStorage = new PlayerStorage();

    public bool Current = false;
    
    private void Start()
    {
        _animatorService = new PlayerAnimatorService(this);
        Agent = GetComponent<NavMeshAgent>();
        _line = GetComponent<LineRenderer>();
        _collider = GetComponent<CapsuleCollider>();
        Agent.autoRepath = true;
        Agent.autoBraking = true;
        Agent.speed = MoveSpeed;
        _canelRunDist = MoveSpeed * 0.2f;
    }

    private void Update()
    {
        if (_isDead)
            return;
        MoveAnim();
    }

    public void Move(Vector3 position)
    {
        if (_isDead)
            return;
        NavMeshPath path = new NavMeshPath();
        Agent.CalculatePath(position, path);
        if (path.status != NavMeshPathStatus.PathComplete)
            return;
        Agent.SetPath(path);
    }

    private void MoveAnim()
    {
        if (Agent.remainingDistance >= _canelRunDist)
        {
            _animatorService.PlayMoveAnim();
            DrawPath(Agent.path);
        }
        else
        {
            _animatorService.StopMoveAnim();
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
            _animatorService.PlayDeathAnim();
            Destroy(Agent);
            Destroy(_collider);
            Destroy(_line);
            _isDead = true;
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
