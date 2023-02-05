using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    [SerializeField] Image presentsImage;
    AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1.2f);

        audioSource.Play();
        
        yield return new WaitForSeconds(3.9f);

        presentsImage.enabled = false;

        yield return new WaitForSeconds(8f);

        SceneManager.LoadScene(1);
    }
}
