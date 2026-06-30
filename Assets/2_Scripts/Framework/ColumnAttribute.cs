using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ColumnAttribute : Attribute
{
    public string HeaderName { get; private set; }

    public bool Required { get; set; }

    public ColumnAttribute(string headerName)
    {
        HeaderName = headerName;
        Required = true;
    }
}
