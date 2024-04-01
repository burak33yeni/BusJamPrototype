using System;
using System.Collections.Generic;
using _Game._Scripts._ScriptableObjects;
using UnityEngine;

namespace _Game._Scripts._ScriptableObjects
{
    public class ColorScriptableObjectsService : MonoBehaviour
    {
        [SerializeField] private List<ColorScriptableObjectConfig> configs;
    
        public Color GetColor(CommonColor commonColor)
        {
            foreach (var config in configs)
            {
                if (config.colorSo.commonColor != commonColor) continue;
                return config.colorSo.color;
            }
            throw new Exception(commonColor + " Scriptable Object not found!");
        }
        
        public CommonColor GetColor(HumanBusType humanBusType)
        {
            switch (humanBusType)
            {
                case HumanBusType.Blue:
                    return CommonColor.Blue;
                case HumanBusType.Green:
                    return CommonColor.Green;
                case HumanBusType.Red:
                    return CommonColor.Red;
                case HumanBusType.Yellow:
                    return CommonColor.Yellow;
                case HumanBusType.Orange:
                    return CommonColor.Orange;
                case HumanBusType.Magenta:
                    return CommonColor.Magenta;
                default:
                    throw new Exception( humanBusType + "Color Scriptable Object not found!");
            }
        }
    }
}

[Serializable]
public class ColorScriptableObjectConfig
{
    public ColorScriptableObject colorSo;
}