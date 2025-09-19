using DG.Tweening;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI newWaveText;
    public RectTransform playerUIMenu;
    public RectTransform waveMenu;
    public RectTransform winMenu;
    public RectTransform startMenu;
    public RectTransform loseMenu;
    public RectTransform upgradeMenu;

    public WaveNumberText waveNumber;

    public Image flashImage;

    public UpgradePanel[] upgradePanels;

    public void Initialize()
    {
        HideMenu(waveMenu, false);
        HideMenu(winMenu, false);
        HideMenu(startMenu, false);
        HideMenu(loseMenu, false);
        HideMenu(playerUIMenu, false); 
        HideMenu(upgradeMenu, false);
    }

    public void SetMenuEventWaveSpawner (WaveSpawner waveSpawner)
    {
        waveSpawner.OnWaveStart.AddListener(WaveStart);
    }

    public void ShowWaveStart()
    {
        bool boss = GameManager.Instance.waveSpawner.IsBossWave();
        StartCoroutine(SwingOnScreenAnimation(newWaveText.rectTransform));
        if (boss)
        {
            newWaveText.text = "Boss Wave";
            ScreenFlash(Color.red, 0.5f, 1, 0.5f);
        }
        else 
        {
            newWaveText.text = "Normal Wave";
        }
        ShowMenu(waveMenu, true);
        StartCoroutine(WaitAndHideMenu(waveMenu));
    }

    IEnumerator WaitAndHideMenu (RectTransform menu)
    {
        yield return new WaitForSeconds(3.0f);
        HideMenu(menu);
    }

    public void ShowStartMenu()
    {
        ShowMenu(startMenu, false);
    }

    public void HideStartMenu ()
    {
        HideMenu(startMenu, true);
    }

    public void ShowGameMenu ()
    {
        ShowMenu(playerUIMenu);
    }

    public void ShowLoseMenu()
    {
        ShowMenu(loseMenu);
    }

    public void ShowWinMenu()
    {
        ShowMenu(winMenu);
    } 

    public void ShowUpgradePanel (SO_Upgrade[] upgrades)
    {
        ShowMenu(upgradeMenu);
        
        for (int i = 0; i < upgradePanels.Length; i++)
        {
            upgradePanels[i].gameObject.SetActive(true);
            upgradePanels[i].Initialize(upgrades[i]);
        }
    }

    public void HideUpgradeMenu ()
    {
        HideMenu(upgradeMenu);
    }

    public IEnumerator SwingOnScreenAnimation(RectTransform animated)
    {
        animated.gameObject.SetActive(true);
        animated.anchoredPosition = Vector2.left * Screen.width * 2.0f;
        Sequence swing = DOTween.Sequence();
        swing.Append(animated.DOAnchorPos(Vector2.zero, 1.0f).SetEase(Ease.InOutBack));
        swing.Append(animated.DOAnchorPos(Vector2.right * Screen.width * 2.0f, 1.0f).SetEase(Ease.InQuint).SetDelay(1.0f));
        swing.Play();
        yield return new WaitForSeconds(3.0f);
        animated.gameObject.SetActive(false);
    }

    public void ShowMenu(RectTransform menu, bool appearSmooth = true)
    {
        CanvasGroup cg = null;
        if (appearSmooth)
        {
            cg = menu.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                appearSmooth = false; 
                Debug.LogWarning("No canvas group on the menu");
            }
        }

        if (appearSmooth)
        {
            cg.alpha = 0.0f;
            cg.DOFade(1.0f, 0.5f);
        }

        menu.gameObject.SetActive(true);
    }

    public void HideMenu(RectTransform menu, bool disapearSmooth = true)
    {
        CanvasGroup cg = null;
        if (disapearSmooth)
        {
            cg = menu.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                disapearSmooth = false;
                Debug.LogWarning("No canvas group on the menu");
            }
        }

        if (disapearSmooth)
        { 
            cg.DOFade(0.0f, 0.5f).OnComplete(()=> { menu.gameObject.SetActive(false); } );
        }
        else
        {
            menu.gameObject.SetActive(false);
        }
    }

    void WaveStart (int current, int max)
    {
        ShowWaveStart();
        waveNumber.UpdateText(current, max); 
    }
    
    void ScreenFlash (Color color, float duration, int number, float delay)
    {
        int trueNumber = number * 2;
        Color startColor = color;
        startColor.a = 0.0f;
        flashImage.color = startColor;

        flashImage.DOFade(0.75f, duration / trueNumber).SetLoops(trueNumber, LoopType.Yoyo).SetDelay(delay);
    }
}
