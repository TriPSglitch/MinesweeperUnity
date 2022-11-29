using UnityEngine;
using UnityEngine.UI;

public class FlagModeButton : MonoBehaviour
{
    public GameManager gameManager;
    public Sprite flagSprite;
    public Sprite mineSprite;

    public void SettingFlagMode()
    {
        gameManager.IsFlagMode = !gameManager.IsFlagMode;

        this.GetComponent<Image>().sprite = this.GetComponent<Image>().sprite == flagSprite ? mineSprite : flagSprite;
    }
}