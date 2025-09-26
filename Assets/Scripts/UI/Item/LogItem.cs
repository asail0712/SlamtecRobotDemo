using UnityEngine;
using UnityEngine.UI;

public class LogItem : MonoBehaviour
{
    [SerializeField] private Text showName;

    public void SetLog(string str)
    {
        showName.text   = str;

        // 根據字體的 Preferred Size 來調整
        float height    = showName.preferredHeight;

        (transform as RectTransform).sizeDelta = new Vector2((transform as RectTransform).sizeDelta.x, height + 10);
    }
}
