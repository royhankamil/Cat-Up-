using UnityEngine;
using UnityEngine.SceneManagement;

public class SitSpot : MonoBehaviour
{
    [SerializeField] private GameObject winUI;
    bool isPlayerOnSpot = false;
    Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerOnSpot && player != null)
        {
            if (player.sitTriggered)
            {
                winUI.SetActive(true); // Reset sit trigger after use
                PlayerPrefs.SetInt("Level", SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player p = collision.GetComponent<Player>();
            if (p != null)
            {
                player = p;
                isPlayerOnSpot = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = null;
            isPlayerOnSpot = false;
        }
    }
}
