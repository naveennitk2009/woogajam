using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class RegisterSymbol : Attribute
{
    public string Name
    {
        get;
        set;
    }
}