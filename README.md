# Simple Result Monad Implementation

There are some libraries that implement this monad,
but most used functionality requires an extremely short code,
so we can avoid import a library and implement it ourself.

This is a short implementation example. Relevant design decisions:

- Use of result monad as a return type must be efficient, so structs are used.
As a consequence, `Result<T>` can not derive from `Result`.

- Result an error concepts are separated.
Error information is specific to each project.
A real implementation could use just one Error type
or many derived error types, one per library, per layer, etc.
If only one error type is needed, it could be defined as struct
but it must be nullable (an boxed, loosing efficiency)
or add an additional flag in result types (loosing efficiency in all cases, not only on error results).

- Because structs are used, default constructors can not be hidden.
In this implementation, default results are successful.

In addition to main implementation, some common helper methods are added.
There are two groups of helpers:

- Errors conversions: Methods `ToError()` an `ToError<T>()` allows convert an error result to a
different error result. These allow methods that receive an error, return it to the caller.
Conversion examples:

``` C#
    Result<int> failureInt = Result<int>.Failure();
    Result failure = failureInt.ToError(); // Result<A> => Result
    Result<SomeEntity> = failureInt.ToError<SomeEntity>(); // Result<A> => Result<B>
    Result<double> failureDouble = failure.ToError<double>(); // Result => Result<A>
```

- Implicit casts: Methods `operator Result` and `operator Result<T>` allow shorter and more readable codes.
For example, a division method:

``` C#
    // without implicit casts
    Result<int> Divide(int a, int b)
    {
        if (b == 0)
            return Result<int>.Failure();
        return Result.Success(a / b);
    }

    // with implicit casts
    Result<int> Divide(int a, int b)
    {
        if (b == 0)
            return Error.Unit;
        return a / b;
    }
```

Methods `ToError()` an `ToError<T>()` introduce a potencial invalid use 
that can not be enforced at compile time,
so it is enforced at execution time with exception `InvalidResultUseException`.
