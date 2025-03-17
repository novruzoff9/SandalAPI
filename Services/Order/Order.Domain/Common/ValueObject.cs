namespace Order.Domain.Common;

public abstract class ValueObject
{
    public static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        return left.Equals(right);
    }

    public static bool operator ==(ValueObject left, ValueObject right)
    {
        return EqualOperator(left, right);
    }

    public static bool operator !=(ValueObject left, ValueObject right)
    {
        return !EqualOperator(left, right);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        var thisValues = GetEqualityComponents().GetEnumerator();
        var otherValues = other.GetEqualityComponents().GetEnumerator();

        while (thisValues.MoveNext() && otherValues.MoveNext())
        {
            if (thisValues.Current is null && otherValues.Current is not null ||
                thisValues.Current is not null && otherValues.Current is null)
            {
                return false;
            }

            if (thisValues.Current != null && !thisValues.Current.Equals(otherValues.Current))
            {
                return false;
            }
        }

        return !thisValues.MoveNext() && !otherValues.MoveNext();
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;

            foreach (var obj in GetEqualityComponents())
            {
                hash = hash * 23 + (obj?.GetHashCode() ?? 0);
            }

            return hash;
        }
    }

    protected abstract IEnumerable<object> GetEqualityComponents();
}
