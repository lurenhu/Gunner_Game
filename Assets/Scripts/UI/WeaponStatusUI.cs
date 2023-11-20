using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class WeaponStatusUI : MonoBehaviour
{
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    [SerializeField] private Image weaponImage;
    [SerializeField] private Transform ammoHolderTransform;
    [SerializeField] private TextMeshProUGUI reloadText;
    [SerializeField] private TextMeshProUGUI ammoRemaingText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private Transform reloadBar;
    [SerializeField] private Image barImage;

    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

    private void Awake()
    {
        player = GameManager.Instance.GetPlayer();
    }

    private void OnEnable()
    {
        player.setWeaponActiveEvent.OnSetWeaponActive += SetWeaponActiveEvent_OnSetWeaponActive;

        player.weaponFireEvent.OnWeaponFire += WeaponFireEvent_OnWeaponFire;

        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;

        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void OnDisable()
    {
        player.setWeaponActiveEvent.OnSetWeaponActive -= SetWeaponActiveEvent_OnSetWeaponActive;

        player.weaponFireEvent.OnWeaponFire -= WeaponFireEvent_OnWeaponFire;

        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;

        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void Start()
    {
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
    }

    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateActiveWeaponImage(weapon.weaponDetails);
        UpdateActiveWeaponName(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);

        if (weapon.isWeaponReloading)
        {
            UpdateWeaponReloadBar(weapon);
        }
        else
        {
            ResetWeaponReloadBar();
        }
        UpdateReloadText(weapon);
    }

    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent, WeaponReloadedEventArgs weaponReloadedEventArgs)
    {
        WeaponReloaded(weaponReloadedEventArgs.weapon);
    }

    private void WeaponReloaded(Weapon weapon)
    {
        if (player.activeWeapon.GetCurrentWeapon() == weapon)
        {
            UpdateReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateAmmoLoadedIcons(weapon);
            ResetWeaponReloadBar();
        }

    }

    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
    }

    private void WeaponFireEvent_OnWeaponFire(WeaponFireEvent weaponFireEvent, WeaponFireEventArgs weaponFireEventArgs)
    {
        WeaponFire(weaponFireEventArgs.weapon);
    }

    private void WeaponFire(Weapon weapon)
    {
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        UpdateReloadText(weapon);
    }

    private void SetWeaponActiveEvent_OnSetWeaponActive(SetWeaponActiveEvent setWeaponActiveEvent, SetWeaponActiveEventArgs setWeaponActiveEventArgs)
    {
        SetActiveWeapon(setWeaponActiveEventArgs.weapon);
    }

    private void UpdateActiveWeaponImage(WeaponDtailsSO weaponDtails)
    {
        weaponImage.sprite = weaponDtails.weaponSprite;
    }

    private void UpdateActiveWeaponName(Weapon weapon)
    {
        weaponNameText.text = "(" + weapon.weaponListPosition + ")" + weapon.weaponDetails.weaponName.ToString();
    }

    private void UpdateAmmoText(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            ammoRemaingText.text = " 999/999 ";
        }else
        {
            ammoRemaingText.text = weapon.weaponRemainingAmmo.ToString() + " / " + weapon.weaponDetails.weaponAmmoCapacity.ToString();
        }
    }


    /// <summary>
    /// 将更新弹夹中的子弹呈现在武器UI上，并实时更新
    /// </summary>
    /// <param name="weapon"></param>
    private void UpdateAmmoLoadedIcons(Weapon weapon)
    {
        ClearAmmoLoadedIcons();

        for (int i = 0; i < weapon.weaponClipRemainingAmmo; i++)
        {
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);

            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,Settings.uiAmmoIconSpacing * i);

            ammoIconList.Add(ammoIcon);
        }
    }

    /// <summary>
    /// 清理子弹弹夹
    /// </summary>
    private void ClearAmmoLoadedIcons()
    {
        foreach (GameObject ammoIcon in ammoIconList)
        {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }

    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteClipCapacity)
        {
            return;
        }

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);

        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }

    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon weapon)
    {
        barImage.color = Color.red;

        while (weapon.isWeaponReloading)
        {
            float barFill = weapon.weaponReloadTimer / weapon.weaponDetails.weaponReloadTime;

            reloadBar.transform.localScale = new Vector3(barFill, 1f, 1f);

            yield return null;
        }
    }

    private void ResetWeaponReloadBar()
    {
        StopReloadWeaponCoroutine();

        barImage.color = Color.green;

        reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void StopReloadWeaponCoroutine()
    {
        if (reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }
    }

    private void UpdateReloadText(Weapon weapon)
    {
        if ((!weapon.weaponDetails.hasInfiniteClipCapacity) && (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading))
        {
            barImage.color = Color.red;

            StopBlinkingReloadTextCoroutine();

            blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextRoutine());
        }
        else
        {
            StopBlinkingReloadText();
        }
    }

    private IEnumerator StartBlinkingReloadTextRoutine()
    {
        while (true)
        {
            reloadText.text = "RELOAD";
            yield return new WaitForSeconds(.3f);
            reloadText.text = "";
            yield return new WaitForSeconds(.3f);
        }
    }

    private void StopBlinkingReloadText()
    {
        StopBlinkingReloadTextCoroutine();

        reloadText.text = "";
    }

    private void StopBlinkingReloadTextCoroutine()
    {
        if (blinkingReloadTextCoroutine != null)
        {
            StopCoroutine(blinkingReloadTextCoroutine);
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelperUtility.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtility.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelperUtility.ValidateCheckNullValue(this, nameof(ammoRemaingText), ammoRemaingText);
        HelperUtility.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelperUtility.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);
        HelperUtility.ValidateCheckNullValue(this, nameof(barImage), barImage);
    }

#endif
    #endregion
}
