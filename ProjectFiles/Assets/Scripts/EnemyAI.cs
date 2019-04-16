using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Wandering = 0,
    Listening,
    Searching,
    Attacking,
    Rage
}

public class EnemyAI : MonoBehaviour
{
    public GameObject player;
    public NavMeshAgent navMesh;
    public EnemyState state = EnemyState.Wandering;
    public float timer = 0;
    public float earSensibility = 50f;

    // Start is called before the first frame update
    void Start()
    {
        player = Camera.main.gameObject;
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.SetDestination(transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)));
    }

    void SetWandering()
    {
        state = EnemyState.Wandering;
        Vector3 direction = new Vector3(Random.Range(0, 400), 0, Random.Range(0, 400));
        navMesh.SetDestination(transform.position + (direction - transform.position) * 0.1f);
    }

    void SetListening()
    {
        state = EnemyState.Listening;
        timer = 0;
        navMesh.SetDestination(transform.position);
    }

    void SetSearching()
    {
        state = EnemyState.Searching;
        timer = 0;
        float quality = Vector3.Distance(transform.position, player.transform.position);
        Vector3 buf = player.transform.position + new Vector3(Random.Range(-quality, quality), 0, Random.Range(-quality, quality));
        navMesh.SetDestination(buf);
    }

    // Update is called once per frame
    void Update()
    {
        bool hearPlayer = false;
        
        // Can I hear the player?
        if(player.GetComponent<SimpleMove>().noisy)
        {
            if(Vector3.Distance(transform.position, player.transform.position) < earSensibility)
            {
                // YES
                hearPlayer = true;
            }
        }

        // Which state am I in?
        if (state == EnemyState.Wandering)
        {
            if (Vector3.Distance(navMesh.destination, transform.position) < 2)
            {
                SetWandering();
            }

            if (hearPlayer)
            {
                SetListening();
            }
        }
        else if(state == EnemyState.Listening)
        {
            timer += Time.deltaTime;
            if (hearPlayer && timer > 1f)
            {
                SetSearching();
            }
            else if (timer > 5f)
            {
                SetWandering();
            }
        }
        else if(state == EnemyState.Searching)
        {
            timer += Time.deltaTime;
            if(Vector3.Distance(transform.position, player.transform.position) < 2)
            {
                SetWandering();
            }
            else if (Vector3.Distance(navMesh.destination, transform.position) < 2)
            {
                SetListening();
            }
            if(hearPlayer && timer > 1f)
            {   
                SetSearching();
            }
        }
    }
}
