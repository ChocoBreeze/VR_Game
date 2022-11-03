using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherManager : MonoBehaviour
{
    [SerializeField]
    private Transform launchPosition;
    [SerializeField]
    private GameObject[] launchObjects;

    public float launchPower = 10.0f;
    private bool launcherOn = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetLauncherOn(bool newLauncherOn) {
        launcherOn = newLauncherOn;
    }

    public void Fire()
    {
        if (launcherOn)
        {
            int randIdx = Random.Range(0, launchObjects.Length);
            GameObject obj = Instantiate(launchObjects[randIdx]);

            obj.transform.position = launchPosition.position;
            obj.GetComponent<Rigidbody>().velocity = launchPosition.forward * launchPower;

            // Destroy(obj, 3f); // 3초 후 삭제

            // Invoke("obj.GetComponent<FruitManager>().destroyFruit()" , 3.0f);
            // obj.GetComponent<FruitManager>().DestroyFruit(obj);
        }
    }

}
