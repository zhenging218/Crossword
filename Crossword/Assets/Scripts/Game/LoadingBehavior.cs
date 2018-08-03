using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingBehavior : MonoBehaviour {

    public float speed = 100.0f;
    public int targetlevel = 0;
	public float WaitSeconds = 0.0f;
    IEnumerator WaitThenLoad()
    {
		// sometimes board gen too fast, so slow down and enjoy load screen.
		yield return new WaitForSeconds(WaitSeconds);
		SceneManager.LoadScene(targetlevel);
	}

	// Use this for initialization
	void Start () {
		BoardGen.Instance.DoDoGenBoard();
    }

    // Update is called once per frame
    void Update () {
        transform.Rotate(Vector3.one, speed * Time.deltaTime);
		if(BoardGen.Instance.OK)
		{
			StartCoroutine(WaitThenLoad());
		}
	}
}
