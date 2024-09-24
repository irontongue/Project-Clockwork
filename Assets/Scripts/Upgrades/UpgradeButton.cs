using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Button button;

    public int index;

    public delegate void ButtonDelegate(UpgradeInfoPacket x);
    public ButtonDelegate buttonDelegate;

    public UpgradeInfoPacket packetRef;

    public void ButtonClicked()
    {
        buttonDelegate(packetRef);
    }
}
