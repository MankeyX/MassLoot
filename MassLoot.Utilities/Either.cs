namespace MassLoot.Utilities;

public sealed class Either<T1, T2>
{
    public T1 Left { get; }
    public T2 Right { get; }

    public bool IsRight => Right is not null;

    internal Either(T1 left)
    {
        Left = left;
        Right = default!;
    }

    internal Either(T2 right)
    {
        Left = default!;
        Right = right;
    }

    public static implicit operator Either<T1, T2>(T1 left)
        => new(left);
    public static implicit operator Either<T1, T2>(T2 right)
        => new(right);
}

public static class EitherExtensions
{
    public static Either<T1, T2> Left<T1, T2>(T1 left)
        => new(left);
    public static Either<T1, T2> Right<T1, T2>(T2 right)
        => new(right);

    public static TResult Match<T1, T2, TResult>(
        this Either<T1, T2> either,
        Func<T1, TResult> left,
        Func<T2, TResult> right
    )
        => either.IsRight
            ? right(either.Right)
            : left(either.Left);

    public static TResult Match<T1, T2, TResult>(
        this Either<T1, T2> either,
        Action<T1> left,
        Func<T2, TResult> right
    )
    {
        if (either.IsRight)
        {
            return right(either.Right);
        }

        left(either.Left);
        return default!;
    }

    public static TResult Match<T1, T2, TResult>(
        this Either<T1, T2> either,
        Func<T1, TResult> left,
        Action<T2> right
    )
    {
        if (!either.IsRight)
        {
            return left(either.Left);
        }

        right(either.Right);
        return default!;
    }

    public static void Match<T1, T2>(
        this Either<T1, T2> either,
        Action<T1> left,
        Action<T2> right
    )
    {
        if (!either.IsRight)
        {
            left(either.Left);
        }
        else
        {
            right(either.Right);
        }
    }
}