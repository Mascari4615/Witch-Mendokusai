using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI headerField;
    [SerializeField] private TextMeshProUGUI descriptionField;
    [SerializeField] private TextMeshProUGUI weaponDamageText;
    [SerializeField] private TextMeshProUGUI gradeText;

    public void SetToolTip(Sprite sprite, string header, string description)
    {
        image.sprite = sprite;
        headerField.text = header;
        headerField.color = Color.white;
        descriptionField.text = description;
        weaponDamageText.text = "";
        gradeText.text = "";
    }

    public void SetToolTip(SpecialThing specialThing)
    {
        if (image != null)
            image.sprite = specialThing.sprite;

        headerField.text = specialThing.Name;
        // headerField.color = specialThing is HasGrade ? DataManager.Instance.GetGradeColor((specialThing as HasGrade).grade) : Color.white;
        descriptionField.text = specialThing.description;
        // weaponDamageText.text = specialThing is Weapon ? $"{(specialThing as Weapon).minDamage} ~ {(specialThing as Weapon).maxDamage} ??��???" : "";

        //if (gradeText != null)
         //   gradeText.text = specialThing is HasGrade ? $"{(specialThing as HasGrade).grade} ����????" : "";
    }
}