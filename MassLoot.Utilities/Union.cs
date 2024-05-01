namespace MassLoot.Utilities;

public sealed class Union<T1, T2>
{
    public T1 Left { get; }
    public T2 Right { get; }

    public bool IsRight => Right is not null;

    internal Union(T1 left)
    {
        Left = left;
        Right = default!;
    }

    internal Union(T2 right)
    {
        Left = default!;
        Right = right;
    }

    public static implicit operator Union<T1, T2>(T1 left)
        => new(left);
    public static implicit operator Union<T1, T2>(T2 right)
        => new(right);
}

public static class UnionExtensions
{
    public static Union<T1, T2> Left<T1, T2>(T1 left)
        => new(left);
    public static Union<T1, T2> Right<T1, T2>(T2 right)
        => new(right);

    public static TResult Match<T1, T2, TResult>(
        this Union<T1, T2> union,
        Func<T1, TResult> left,
        Func<T2, TResult> right
    )
        => union.IsRight
            ? right(union.Right)
            : left(union.Left);

    public static TResult Match<T1, T2, TResult>(
        this Union<T1, T2> union,
        Action<T1> left,
        Func<T2, TResult> right
    )
    {
        if (union.IsRight)
        {
            return right(union.Right);
        }
        else
        {
            left(union.Left);
        }

        return default!;
    }

    public static TResult Match<T1, T2, TResult>(
        this Union<T1, T2> union,
        Func<T1, TResult> left,
        Action<T2> right
    )
    {
        if (!union.IsRight)
        {
            return left(union.Left);
        }
        else
        {
            right(union.Right);
        }

        return default!;
    }

    public static void Match<T1, T2>(
        this Union<T1, T2> union,
        Action<T1> left,
        Action<T2> right
    )
    {
        if (!union.IsRight)
        {
            left(union.Left);
        }
        else
        {
            right(union.Right);
        }
    }
}