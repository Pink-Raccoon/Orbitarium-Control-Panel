using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class TabHandler : MonoBehaviour
{
    public List<Button> TabButtons;
    public List<GameObject> Tabs;

    public Color ActiveTabColor;
    public Color DisabledTabColor;

    private void Start()
    {
        TabButtons.ForEach((Button btn) =>
        {
            btn.onClick.AddListener(() => TabButtonClicked(btn));
        });

        TabButtonClicked(TabButtons[0]);
    }

    public void TabButtonClicked(Button btn)
    {
        // disable all tabs
        Tabs.ForEach((tab) =>
        {
            tab.SetActive(false);
        });

        TabButtons.ForEach((tabBtn) =>
        {
            tabBtn.gameObject.GetComponent<Image>().color = DisabledTabColor;
        });

        // Enable tab
        int index = TabButtons.FindIndex((b) => b == btn);

        Tabs[index].SetActive(true);
        btn.gameObject.GetComponent<Image>().color = ActiveTabColor;
    }
}
