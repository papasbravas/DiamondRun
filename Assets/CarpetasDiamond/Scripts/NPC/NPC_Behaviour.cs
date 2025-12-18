using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Behaviour : MonoBehaviour
{
    [SerializeField] private Vector3 destination;
    [SerializeField] private Vector3 min, max;
    [SerializeField] private GameObject player;

    [SerializeField] private int childrenIndex;
    [SerializeField] private Transform path;
    [SerializeField] private float playerDetectionDistance;
    [SerializeField] private bool playerDetected;
    private Coroutine runningPatroll;

    public void Start()
    {
        //destination = RandomDestination();
        //GetComponent<NavMeshAgent>().SetDestination(destination);

        //StartCoroutine("Follow");
        //destination = path.GetChild(0).position;
        //GetComponent<NavMeshAgent>().SetDestination(path.GetChild(0).position);
        runningPatroll = StartCoroutine("Patroll");
        //StartCoroutine("DistanceDetection");
    }

    public void Update()
    {
        if (playerDetected)
        {
            StopCoroutine("Patroll");
        }
        else if(runningPatroll == null)
        {
            StartCoroutine("Patroll");
        }

        //if (Input.GetButtonDown("Fire1"))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit = new RaycastHit();

        //    if (Physics.Raycast(ray, out hit, 1000))
        //    {
        //        GetComponent<NavMeshAgent>().SetDestination(hit.point);
        //    }
        //}


        //if (Vector3.Distance(transform.position, destination) < 0.5f)
        //{
        //    destination = RandomDestination();
        //    GetComponent<NavMeshAgent>().SetDestination(destination);
        //}

    }

    #region Patroll movement
    IEnumerator Patroll()
    {
        while (true)
        {
            if(Vector3.Distance(transform.position, destination) < 1.5f)
            {
                childrenIndex++;
                childrenIndex = childrenIndex % path.childCount;

                destination = path.GetChild(childrenIndex).position;
                GetComponent<NavMeshAgent>().SetDestination(destination);
            }
        }
    }

    #endregion

    private Vector3 RandomDestination()
    {
        return new Vector3(Random.Range(min.x, max.x), 0, Random.Range(min.z, max.z));
    }

    #region Distance Detection

    IEnumerator DistanceDetection()
    {
        while (true) // Bucle infinito para la detección por distancia
        {
            // Si el jugador está dentro de la distancia de detección
            if (Vector3.Distance(transform.position, player.transform.position) < playerDetectionDistance) 
            {
                if(runningPatroll != null) // Detiene la patrulla si está en curso
                {
                    StopCoroutine("Patroll");
                    runningPatroll = null;
                }
                playerDetected = true;
                destination = player.transform.position;
                GetComponent<NavMeshAgent>().SetDestination(destination);
            }
            else
            {
                playerDetected = false; // El jugador está fuera de la distancia de detección
                if (runningPatroll == null) // Reanuda la patrulla si no está en curso
                {
                    runningPatroll = StartCoroutine("Patroll");
                }
            }
            yield return new WaitForSeconds(1);
        }
    }
    #endregion


    #region Collider Detection

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) // Si el NPC detecta al jugador
        {
            if (runningPatroll != null) // Detiene la patrulla si está en curso
            {
                StopCoroutine("Patroll");
                runningPatroll = null;
            }
            StartCoroutine("Follow");
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerDetected = false;
            if (runningPatroll == null)
            {
                runningPatroll = StartCoroutine("Patroll");
            }
        }
    }

    #endregion


    #region Always Detect

    IEnumerator Follow()
    {
        while (true)
        {
            destination = player.transform.position;
            GetComponent<NavMeshAgent>().SetDestination(destination);
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(1);
        }
    }

    #endregion



}


