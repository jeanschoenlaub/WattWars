using System;

[Serializable]
public class ConversionAttribute
{
    public BulletType inputType;
    public int conversionRatio;

    public ConversionAttribute(BulletType inputType, int inputQty, int conversionRatio, BulletType outputType)
    {
        this.inputType = inputType;
        this.conversionRatio = conversionRatio;
    }
}
