using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem; // player

public class BalloonSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _ballonPrefab;
    private bool _SpawnEnemies;
    private Player player;
    private Vector3 standPos;
    private Vector3 spawnerPos;
    // private GameObject Spawner;
    IEnumerator SpawnRoutine()
    {
        // Debug.Log("spawnerPos = " + spawnerPos);
        // Debug.Log("player.position = " + player.transform.position);
        float dist = Vector3.Distance(player.transform.position, standPos);
        // Debug.Log("distance = " + dist);
        // Debug.Log("player.transform.position == spawnerPos : " + (player.transform.position == spawnerPos));
        
        if (!_SpawnEnemies && dist <= 2f)
        {
            _SpawnEnemies = true;
        }
        while(_SpawnEnemies == true)
        {
            yield return new WaitForSeconds(2f); // 2s wait

            // Spawn Enemy
            Vector3 spawnPos = spawnerPos;
            spawnPos.z += Random.Range(-4.5f, 4.5f);
            // spawnPos.y = 0f;
            Instantiate(_ballonPrefab, spawnPos, Quaternion.identity);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _SpawnEnemies = false;
        // Spawner = GameObject.Find("BallonSpawner");
        spawnerPos = GameObject.Find("BalloonSpawner").transform.position;
        standPos = GameObject.Find("BowStand").transform.position;
       
        player = FindObjectOfType<Player>();
        
        // Begin Spawning Routine
        // StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("spawnerPos = " + spawnerPos);
        // Debug.Log("player.position = " + player.transform.position);
        // Debug.Log("Start!!!!!");
        if(!_SpawnEnemies)
        {
            // Begin Spawning Routine
            StartCoroutine(SpawnRoutine());
        }
    }
}
