using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class navtogveaway : MonoBehaviour
{
    public NavMeshAgent agent;
    public float ChaseDistance = 20;
    public string tagString = "chasetag";
    public bool hasReached;
    public GameObject[] Players;
    public GameObject target;

    void Start()
    {
        // MADE BY SHREK
        // EDITED BY PERI
        // DO NOT DELETE THIS IF YOU MAKE A TUTORIAL FROM THIS SCRIPT, GIVE ME CREDIT PLZ

        hasReached = true;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            agent.enabled = true;
            GameObject[] players = GameObject.FindGameObjectsWithTag(tagString);
            Players = players;

            // Set the target to the closest player
            if (players.Length > 0)
            {
                float minDistance = ChaseDistance;
                foreach (GameObject player in players)
                {
                    // Calculate the distance in 2D space instead of 3D
                    Vector3 playerPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                    float distance = Vector3.Distance(transform.position, playerPos);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        target = player;
                    }
                }
            }

            // If we have a target, set the NavMeshAgent's destination to the target's position
            if (target != null)
            {
                NavMeshHit hit;
                Vector3 targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                if (NavMesh.SamplePosition(targetPos, out hit, 1.0f, NavMesh.AllAreas))
                {
                    agent.destination = hit.position;
                }
                else
                {
                    target = null;
                }
            }
            if (hasReached && target == null)
            {
                Vector3 randomDestination = RandomNavmeshLocation();
                agent.destination = randomDestination;
                hasReached = false;
            }
        }
        else
        {
            agent.enabled = false;
        }

        if (PhotonNetwork.InRoom)
        {
            if (agent.remainingDistance < 0.5)
            {
                hasReached = true;
            }
        }
    }

    Vector3 RandomNavmeshLocation()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 50; // Adjust the multiplier as needed
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 1000, NavMesh.AllAreas);
        return hit.position;
    }
}