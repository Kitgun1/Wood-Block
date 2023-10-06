using System;

namespace WoodBlock
{
    [Flags]
    public enum CategoryProductType
    {
        None = 0,
        Quantitative = 1 << 0,
        Disposable = 1 << 1
    }
}