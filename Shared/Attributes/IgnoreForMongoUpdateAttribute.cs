namespace Shared.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class IgnoreForMongoUpdateAttribute: Attribute
{
    public IgnoreForMongoUpdateAttribute()
    {
    }
}
