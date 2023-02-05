using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(6f);

        SceneManager.LoadScene(1);
    }
}
