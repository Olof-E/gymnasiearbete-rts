using System;

//Fixed point data structure
struct FixedInt
{
    public long rawValue;
    public const int shiftAmt = 32;
    public const long ONE = 1L << shiftAmt;

    public FixedInt(long value)
    {
        rawValue = value;
    }

    public double ToFloat()
    {
        return (double)rawValue / (double)ONE;
    }

    #region operators

    //Overrides for conversions with fixed ints
    #region conversions
    public static implicit operator FixedInt(int value)
    {
        return new FixedInt((long)value * ONE);
    }

    public static implicit operator FixedInt(long value)
    {
        return new FixedInt(value * ONE);
    }

    public static implicit operator FixedInt(float value)
    {
        return new FixedInt((long)(value * ONE));
    }

    public static implicit operator FixedInt(double value)
    {
        return new FixedInt((long)(value * ONE));
    }

    public static implicit operator int(FixedInt value)
    {
        return (int)(value.rawValue >> shiftAmt);
    }

    public static implicit operator long(FixedInt value)
    {
        return value.rawValue >> shiftAmt;
    }

    public static implicit operator float(FixedInt value)
    {
        return (float)value.rawValue / ONE;
    }

    public static implicit operator double(FixedInt value)
    {
        return (double)value.rawValue / ONE;
    }
    #endregion

    //Overrides for adding with fixed ints
    #region arithmetics
    public static FixedInt operator +(FixedInt a, FixedInt b)
    {
        return new FixedInt(a.rawValue + b.rawValue);
    }

    public static FixedInt operator -(FixedInt a, FixedInt b)
    {
        return new FixedInt(a.rawValue - b.rawValue);
    }

    public static FixedInt operator *(FixedInt a, FixedInt b)
    {
        long result = (((a.rawValue >> shiftAmt) * (b.rawValue >> shiftAmt)) << shiftAmt) +
        ((a.rawValue >> shiftAmt) * (b.rawValue & 0xffffffff)) +
        ((b.rawValue >> shiftAmt) * (a.rawValue & 0xffffffff)) +
        ((a.rawValue & 0xffffffff) * (b.rawValue & 0xffffffff)) >> shiftAmt;

        return new FixedInt(result);
        // var xl = a.rawValue;
        // var yl = b.rawValue;

        // var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
        // var xhi = xl >> shiftAmt;
        // var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
        // var yhi = yl >> shiftAmt;

        // var lolo = xlo * ylo;
        // var lohi = (long)xlo * yhi;
        // var hilo = xhi * (long)ylo;
        // var hihi = xhi * yhi;

        // var loResult = lolo >> shiftAmt;
        // var midResult1 = lohi;
        // var midResult2 = hilo;
        // var hiResult = hihi << shiftAmt;

        // var sum = (long)loResult + midResult1 + midResult2 + hiResult;
        // return new FixedInt(sum);
    }

    public static FixedInt operator /(FixedInt a, FixedInt b)
    {
        return new FixedInt((a.rawValue << shiftAmt) / b.rawValue);
    }
    #endregion

    #endregion
}