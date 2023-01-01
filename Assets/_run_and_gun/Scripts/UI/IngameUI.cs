using TMPro;
using UnityEngine;

public class IngameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TextNum;
    [SerializeField] private TextMeshProUGUI m_TextNumDestroy;

    private void Awake()
    {
        SetTextNumDestroy(0);
    }

    public void SetTextNum(int num)
    {
        m_TextNum.text = num.ToString();
    }

    public void SetTextNumDestroy(int num)
    {
        m_TextNumDestroy.text = num.ToString();
    }
}
