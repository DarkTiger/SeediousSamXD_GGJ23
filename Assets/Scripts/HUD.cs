using System;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Image ImgControls;

    [SerializeField] Image imgHealthBar_P1;
    [SerializeField] Image imgHealthBar_P2;
    [SerializeField] Image imgSelector_P1;
    [SerializeField] Image imgSelector_P2;

    [SerializeField] Image[] inventoryImagesP1;
    [SerializeField] Image[] inventoryImagesP2;

    public Image VictoryP1;
    public Image VictoryP2;

    [SerializeField] ItemIcon[] itemIcons;

    [Serializable]
    public struct ItemIcon
    {
        public ItemType Type;
        public Sprite Sprite;
    }

    public static HUD Instance = null;
    
    float maxHealthbarLengthP1;
    float maxHealthbarLengthP2;



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

    public Sprite GetItemSprite(ItemType type)
    {
        for (int i = 0; i < itemIcons.Length; i++)
        {
            if (itemIcons[i].Type == type)
            {
                return itemIcons[i].Sprite;
            }
        }

        return null;
    }

    public void UpdateInventory(Player player)
    {
        for (int i = 0; i < player.InventorySlots.Length; i++)
        {
            if (player.PlayerIndex == 1)
            {
                inventoryImagesP1[i].sprite = GetItemSprite(player.InventorySlots[i]);
            }
            else
            {
                inventoryImagesP2[i].sprite = GetItemSprite(player.InventorySlots[i]);
            }
        }
    }
}
