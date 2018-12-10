using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitController : MonoBehaviour {

    // Common
    new public string name;
    public Fraction fraction;
    public bool isSelected = false;
    public float radius = 3f;
    private float stoppingDistance = 0f;

    // Model
    public List<SkinnedMeshRenderer> fractionMeshRenderers = new List<SkinnedMeshRenderer>();

    // Unit stats
    public float maxHealth = 100;
    public float CurrentHealth { get; private set; }
    public Stat damage;
    public Stat attackDistance;

    // Combat
    public float attackSpeed = 1f;
    private float attackCountdown = 0f;
    const float combatCooldown = 5;
    float lastAttackTime;
    public float attackDelay = .6f;
    public bool InCombat { get; private set; }
    public GameObject bullet;
    public float bulletForce = 0f;

    // Following target
    private UnitController targetUnit;

    // Components
    private GameManager gameManager;
    private NavMeshAgent agent;

    void Awake()
    {
        CurrentHealth = maxHealth;
    }

    void Start () {
        name = GetComponent<Transform>().name;
        agent = GetComponent<NavMeshAgent>();
        gameManager = GameManager.instance;
    }

    private void FixedUpdate()
    {
        CheckLight();
        CheckAttackCoolDown();
        CheckTarget();
        CheckFractionColor();
    }

    public void MoveToPoint(Vector3 point)
    {
        if (agent.isActiveAndEnabled)
        {
            agent.SetDestination(point);
        }
        else
        {
            Debug.LogError("NavMeshAgent agent did not initialized!");
        }
    }

    public void FollowTarget(UnitController unit, float globalStoppingDistance)
    {
        if (agent.isActiveAndEnabled)
        {
            agent.SetDestination(unit.transform.position);
            targetUnit = unit;
            stoppingDistance = globalStoppingDistance;
        }
        else
        {
            Debug.LogError("NavMeshAgent agent did not initialized!");
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Damage taked " + damage);

        damage = Mathf.Clamp(damage, 0, int.MaxValue);
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void CheckTarget()
    {
        if (targetUnit)
        {
            FaceTarget();
            float distanceToTarget = Vector3.Distance(transform.position, targetUnit.transform.position);
            if (distanceToTarget <= attackDistance.GetValue())
            {
                if (fraction.enemyFractions.Count != 0 && fraction.enemyFractions.Contains(targetUnit.fraction))
                {
                    agent.stoppingDistance = attackDistance.GetValue();
                    Attack(targetUnit, damage.GetValue());
                }
                else
                    agent.stoppingDistance = targetUnit.radius * stoppingDistance;
            }
        }
    }

    public void Attack(UnitController targetUnit, int damage)
    {
        if (attackCountdown <= 0f)
        {
            StartCoroutine(DoDamage(targetUnit, attackDelay));
            attackCountdown = 1f / attackSpeed;
            InCombat = true;
            lastAttackTime = Time.time;
        }
    }

    IEnumerator DoDamage(UnitController unit, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (bullet)
        {
            GameObject newBullet = Instantiate(bullet, gameObject.transform.position, gameObject.transform.rotation);
            newBullet.AddComponent<Rigidbody>();
            Rigidbody bulletRigidbody = newBullet.GetComponent<Rigidbody>();
            Light light = newBullet.AddComponent<Light>();
            light.color = fraction.color;
            bulletRigidbody.AddForce(newBullet.transform.forward * bulletForce);
        }
        unit.TakeDamage(damage.GetValue());
        if (unit.CurrentHealth <= 0)
        {
            InCombat = false;
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    private void CheckLight()
    {
        if (isSelected)
        {
            if (fraction == gameManager.playerFraction && !gameObject.GetComponent<Light>())
            {
                Light light = gameObject.AddComponent<Light>();
                light.color = fraction.color;
            }
            else if (!gameObject.GetComponent<Light>())
            {
                Light light = gameObject.AddComponent<Light>();
                light.color = fraction.color;
            }
        }
        else if (!isSelected && gameObject.GetComponent<Light>())
        {
            Destroy(gameObject.GetComponent<Light>());
        }
    }

    private void CheckAttackCoolDown()
    {
        attackCountdown -= Time.deltaTime;

        if (Time.time - lastAttackTime > combatCooldown)
        {
            InCombat = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance.GetValue());
    }

    void FaceTarget()
    {
        Vector3 direction = (targetUnit.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void CheckFractionColor()
    {
        if (fractionMeshRenderers.Count > 0)
        {
            foreach(SkinnedMeshRenderer meshRenderer in fractionMeshRenderers)
            {
                if (meshRenderer.material.color != fraction.color)
                {
                    meshRenderer.material.color = fraction.color;
                }
            }
        }
    }
}
