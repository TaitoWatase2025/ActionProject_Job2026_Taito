using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class TypewriterUniversal : MonoBehaviour
{
    [TextArea(3, 10)]
    public string fullText =
@"!! FATAL ERROR !!
PLAYER NOT RESPONDING
>>> MEMORY DUMP INITIATED <<<";

    public float delay = 0.03f;

    private TMP_Text tmpText;
    private Text uiText;

    void Awake()
    {
        // TMP Ç© Text ÇÃÇ«Ç¡ÇøÇ™ïtÇ¢ÇƒÇÈÇ©í≤Ç◊ÇÈ
        tmpText = GetComponent<TMP_Text>();
        uiText = GetComponent<Text>();
    }

    void Start()
    {
        StartCoroutine(TypeEffect());
    }

    IEnumerator TypeEffect()
    {
        // èâä˙âª
        if (tmpText) tmpText.text = "";
        if (uiText) uiText.text = "";

        // àÍï∂éöÇ∏Ç¬ï\é¶
        foreach (char c in fullText)
        {
            if (tmpText) tmpText.text += c;
            if (uiText) uiText.text += c;

            yield return new WaitForSeconds(delay);
        }
    }
    public void StartTypewriter(string text)
    {
        fullText = text;
        StartCoroutine(TypeEffect());
    }
}

