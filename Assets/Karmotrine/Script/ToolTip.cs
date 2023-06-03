using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI headerField;
    [SerializeField] private TextMeshProUGUI descriptionField;
    [SerializeField] private TextMeshProUGUI gradeText;

    public void SetToolTip(SpecialThing specialThing) =>
        SetToolTip(specialThing.sprite, specialThing.name, specialThing.description);

    public void SetToolTip(Sprite sprite, string header, string description)
    {
        image.sprite = sprite;
        headerField.text = header;
        headerField.color = Color.white;
        // headerField.color = specialThing is HasGrade ? DataManager.Instance.GetGradeColor((specialThing as HasGrade).grade) : Color.white;
        descriptionField.text = description;
        gradeText.text = "";
        // if (gradeText != null)
        //    gradeText.text = specialThing is HasGrade ? $"{(specialThing as HasGrade).grade} ����????" : "";
    }
}