using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject seedBagPrefab;
    bool enabledSpawnBag = true;

    bool gameIsStarted = false;
    float currentTime = 0f;

    bool firstPhase = false;
    bool secondPhase = false;
    bool thirdPhase = false;
    bool lastPhase = false;

    [SerializeField] GameObject root1;
    [SerializeField] GameObject root2;
    [SerializeField] GameObject root3;

    [SerializeField] AudioClip phase1;
    [SerializeField] AudioClip phase2;
    [SerializeField] AudioClip phase3;

    [SerializeField] AudioSource source;

    public static GameManager Instance = null;

    Player[] players;

    bool gameEnded = false;



    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        while (enabledSpawnBag)
        {
            if (gameIsStarted)
            {
                yield return new WaitForSeconds(Random.Range(15f, 25f));

                Instantiate(seedBagPrefab, new Vector3(Random.Range(-22f, 22f), 30f, Random.Range(-15f, 15f)), Quaternion.Euler(-90f, 0f, 0f));
            }

            yield return null;
        }
    }

    private void Update()
    {
        if (gameEnded) return;

        players = FindObjectsByType<Player>(FindObjectsSortMode.None);

        if (!gameIsStarted && players.Length > 0)
        {
            HUD.Instance.ImgControls.enabled = false;
        }

        if (!gameIsStarted && players.Length >= 2)
        {
            source.Play();
            gameIsStarted = true;
        }

        if (gameIsStarted)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= 180 && !firstPhase)
            {
                firstPhase = true;
                root1.SetActive(true);
                root1.GetComponent<AudioSource>().Play();
                source.clip = phase1;
                source.Play();
            }
            else if (currentTime >= 330 && !secondPhase)
            {
                secondPhase = true;
                root2.SetActive(true);
                root2.GetComponent<AudioSource>().Play();
                source.clip = phase2;
                source.Play();
            }
            else if (currentTime >= 450 && !thirdPhase)
            {
                thirdPhase = true;
                root3.SetActive(true);
                root3.GetComponent<AudioSource>().Play();
                source.clip = phase3;
                source.Play();
            }
            else if (currentTime >= 510 && !lastPhase)
            {
                lastPhase = true;
            }

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].Died && players[i].PlayerIndex == 2)
                {
                    if (!HUD.Instance.VictoryP1.gameObject.activeSelf)
                    {
                        HUD.Instance.VictoryP1.gameObject.SetActive(true);
                        source.Stop();

                        StartCoroutine(ReturnToMenu());

                        gameEnded = true;
                    }
                }
                else if (players[i].Died && players[i].PlayerIndex == 1)
                {
                    if (!HUD.Instance.VictoryP2.gameObject.activeSelf)
                    {
                        HUD.Instance.VictoryP2.gameObject.SetActive(true);
                        source.Stop();

                        StartCoroutine(ReturnToMenu());

                        gameEnded = true;
                    }
                }
            }
        }
    }

    public int GetPhase()
    {
        if (phase3)
        {
            return 3;
        }
        else if (phase2)
        {
            return 2;
        }
        else if (phase1)
        {
            return 1;
        }

        return 0;
    }

    IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(10);

        SceneManager.LoadScene(1);
    }
}
