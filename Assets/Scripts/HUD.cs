using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] Image imgHealthBar_P1;
    [SerializeField] Image imgHealthBar_P2;
    [SerializeField] Image imgSelector_P1;
    [SerializeField] Image imgSelector_P2;

    float maxHealthbarLengthP1;
    float maxHealthbarLengthP2;

    public static HUD Instance = null;


    private void Awake()
    {
        Instance = this;

        maxHealthbarLengthP1 = imgHealthBar_P1.rectTransform.sizeDelta.x;
        maxHealthbarLengthP2 = imgHealthBar_P2.rectTransform.sizeDelta.x;
    }

    public void UpdateHealthbarPlayer(int playerIndex, float health)
    {
        switch (playerIndex)
        {
            case 1:
                imgHealthBar_P1.rectTransform.sizeDelta = new Vector2(maxHealthbarLengthP1 * (health / 100f), imgHealthBar_P1.rectTransform.sizeDelta.y);
                break;

            case 2:
                imgHealthBar_P2.rectTransform.sizeDelta = new Vector2(maxHealthbarLengthP2 * (health / 100f), imgHealthBar_P2.rectTransform.sizeDelta.y);
                break;
        }
    }

    public void MoveSelectorPlayer(int playerIndex, int slotIndex)
    {
        switch (playerIndex)
        {
            case 1:
                imgSelector_P1.rectTransform.eulerAngles = new Vector3(0, 0, 0 + (90 * slotIndex));
                break;

            case 2:
                imgSelector_P2.rectTransform.eulerAngles = new Vector3(0, 0, 0 + (90 * slotIndex));
                break;
        }
    }
}
