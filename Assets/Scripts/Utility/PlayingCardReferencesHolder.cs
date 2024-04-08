using TMPro;
using UnityEngine;

public class PlayingCardReferencesHolder : MonoBehaviour
{
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private TextMeshPro text1;
    [SerializeField] private TextMeshPro text2;

    public SpriteRenderer Icon => icon;
    public TextMeshPro Text1 => text1;
    public TextMeshPro Text2 => text2;
}
