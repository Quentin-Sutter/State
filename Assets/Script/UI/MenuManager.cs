using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private RectTransform playerMenu;
    [SerializeField] private RectTransform waveMenu;
    [SerializeField] private RectTransform winMenu;
    [SerializeField] private RectTransform startMenu;
    [SerializeField] private RectTransform loseMenu;
    [SerializeField] private RectTransform upgradeMenu;

    [Header("Wave UI")]
    [SerializeField] private TextMeshProUGUI newWaveText;
    [SerializeField] private WaveNumberText waveNumber;

    [Header("Upgrade UI")]
    [SerializeField] private UpgradePanel[] upgradePanels;

    [Header("Effects")]
    [SerializeField] private Image flashImage;
    [SerializeField] private float flashAlpha = 0.75f;
    [SerializeField] private float swingAnimationDuration = 1f;
    [SerializeField] private float swingDisplayDuration = 3f;

    private Coroutine waveSwingRoutine;

    public void Initialize()
    {
        HideMenu(waveMenu, false);
        HideMenu(winMenu, false);
        HideMenu(startMenu, false);
        HideMenu(loseMenu, false);
        HideMenu(playerMenu, false);
        HideMenu(upgradeMenu, false);
    }

    public void SetMenuEventWaveSpawner(WaveSpawner waveSpawner)
    {
        waveSpawner.OnWaveStart.AddListener(OnWaveStarted);
    }

    public void ShowStartMenu()
    {
        ShowMenu(startMenu, false);
    }

    public void HideStartMenu()
    {
        HideMenu(startMenu, true);
    }

    public void ShowGameMenu()
    {
        ShowMenu(playerMenu);
    }

    public void ShowLoseMenu()
    {
        ShowMenu(loseMenu);
    }

    public void ShowWinMenu()
    {
        ShowMenu(winMenu);
    }

    public void ShowUpgradePanel(SO_Upgrade[] upgrades)
    {
        ShowMenu(upgradeMenu);

        for (var i = 0; i < upgradePanels.Length; i++)
        {
            if (i < upgrades.Length && upgrades[i] != null)
            {
                upgradePanels[i].gameObject.SetActive(true);
                upgradePanels[i].Initialize(upgrades[i]);
            }
            else
            {
                upgradePanels[i].gameObject.SetActive(false);
            }
        }
    }

    public void HideUpgradeMenu()
    {
        HideMenu(upgradeMenu);
    }

    private void OnWaveStarted(int current, int max)
    {
        ShowWaveStart();
        waveNumber?.UpdateText(current, max);
    }

    private void ShowWaveStart()
    {
        if (waveSwingRoutine != null)
        {
            StopCoroutine(waveSwingRoutine);
        }

        var isBossWave = GameManager.Instance.WaveSpawner.IsBossWave();
        newWaveText.text = isBossWave ? "Boss Wave" : "Wave Incoming";

        if (isBossWave)
        {
            ScreenFlash(Color.red, 0.5f, 1, 0.5f);
        }

        ShowMenu(waveMenu, true);
        waveSwingRoutine = StartCoroutine(SwingOnScreenAnimation(waveMenu));
    }

    private IEnumerator SwingOnScreenAnimation(RectTransform animated)
    {
        animated.gameObject.SetActive(true);
        animated.anchoredPosition = Vector2.left * Screen.width * 2f;

        var swing = DOTween.Sequence();
        swing.Append(animated.DOAnchorPos(Vector2.zero, swingAnimationDuration).SetEase(Ease.InOutBack));
        swing.Append(animated.DOAnchorPos(Vector2.right * Screen.width * 2f, swingAnimationDuration)
            .SetEase(Ease.InQuint)
            .SetDelay(1f));

        yield return new WaitForSeconds(swingDisplayDuration);
        animated.gameObject.SetActive(false);
        waveSwingRoutine = null;
    }

    private void ShowMenu(RectTransform menu, bool smooth = true)
    {
        if (menu == null)
        {
            return;
        }

        var canvasGroup = smooth ? GetOrCreateCanvasGroup(menu) : null;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 0.5f);
        }

        menu.gameObject.SetActive(true);
    }

    private void HideMenu(RectTransform menu, bool smooth = true)
    {
        if (menu == null)
        {
            return;
        }

        var canvasGroup = smooth ? GetOrCreateCanvasGroup(menu) : null;
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0f, 0.5f).OnComplete(() => menu.gameObject.SetActive(false));
        }
        else
        {
            menu.gameObject.SetActive(false);
        }
    }

    private static CanvasGroup GetOrCreateCanvasGroup(RectTransform menu)
    {
        var canvasGroup = menu.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = menu.gameObject.AddComponent<CanvasGroup>();
        }

        return canvasGroup;
    }

    private void ScreenFlash(Color color, float duration, int flashes, float delay)
    {
        if (flashImage == null)
        {
            return;
        }

        var loops = Mathf.Max(1, flashes) * 2;
        var startColor = color;
        startColor.a = 0f;

        flashImage.color = startColor;
        flashImage.DOFade(flashAlpha, duration / loops).SetLoops(loops, LoopType.Yoyo).SetDelay(delay);
    }
}
