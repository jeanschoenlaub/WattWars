using System;

[Serializable]
public class ConversionAttribute
{
    public TowerType inputType;
    public int conversionRatio;

    public ConversionAttribute(TowerType inputType, int inputQty, int conversionRatio, TowerType outputType)
    {
        this.inputType = inputType;
        this.conversionRatio = conversionRatio;
    }
}
