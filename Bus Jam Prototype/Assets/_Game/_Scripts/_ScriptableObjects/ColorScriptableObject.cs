using UnityEngine;

namespace _Game._Scripts._ScriptableObjects
{
    [CreateAssetMenu(fileName = "ColorSO", menuName = "SO/New ColorSO", order = 0)]
    public class ColorScriptableObject : ScriptableObject
    {
        public CommonColor commonColor;
        public Color color; 
    }
}