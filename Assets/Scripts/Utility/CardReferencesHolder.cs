using TMPro;
using UnityEngine;

public class CardReferencesHolder : MonoBehaviour
{
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private TextMeshPro text;

    public SpriteRenderer Icon => icon;
    public TextMeshPro Text => text;
}
