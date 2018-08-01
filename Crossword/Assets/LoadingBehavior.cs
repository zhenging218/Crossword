using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingBehavior : MonoBehaviour {

    public float speed = 100.0f;
    public int targetlevel = 0;

    IEnumerator LoadLevelAsync(int target)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(targetlevel);
        while(op.isDone == false)
        {
            yield return null;
        }
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(LoadLevelAsync(targetlevel));
    }

    // Update is called once per frame
    void Update () {
        transform.Rotate(Vector3.one, speed * Time.deltaTime);
	}
}
