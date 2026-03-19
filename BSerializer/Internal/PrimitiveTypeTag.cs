namespace BSerializer.Internal;

internal enum PrimitiveTypeTag : uint
{
    Bool    = 1,
    Byte    = 2,
    SByte   = 3,
    Int16   = 4,
    UInt16  = 5,
    Int32   = 6,
    UInt32  = 7,
    Int64   = 8,
    UInt64  = 9,
    Float   = 10,
    Double  = 11,
    Decimal = 12,
    Char    = 13,
    String  = 14,
    ByteArray = 15,
}