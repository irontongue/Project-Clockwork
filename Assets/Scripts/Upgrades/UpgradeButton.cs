using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Button button;
    public TextMeshProUGUI[] statTexts;

    public int index;

    public delegate void ButtonDelegate(UpgradeLine x);
    public ButtonDelegate buttonDelegate;

    public UpgradeLine packetRef;

    public AudioClip hoverOverSound, clickSound;

    public void ButtonClicked()
    {
        buttonDelegate(packetRef);
        PlayerMovement.playerAudioSource.PlayOneShot(clickSound, GlobalSettings.audioVolume);
    }

   public void OnPointerEnter(PointerEventData pointerEventData)
    {
        PlayerMovement.playerAudioSource.PlayOneShot(hoverOverSound, GlobalSettings.audioVolume);
    }
}
