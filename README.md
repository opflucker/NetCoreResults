# Simple Result Monad Implementation

## Introduction

There are some libraries that implement this monad,
but mostly required functionallity is extremely short,
so we can avoid import a library and implement it ourself.

This is a short implementation example. Relevant design decisions:

- `Result` are expected to be created only when returning from a method, so this is the only mechanism implemented.

- `Result` types are implemented with classes.
  Structs are more efficient but their mandatory default constructors can create invalid results.

- Any type can be used to represent errors.

## Result types

There are 3 Result types:

- Static `Result`, holding types and methods for helping create result objects in two corner cases:
  - A success `Result<TError>`.
  - A `Result<TData,TError>` when `TData = TError`.
- `Result<TError>`, representing a result without success data, only error data.
- `Result<TData,TError>`, representing a result with success and error data.

## Working with `Result<TError>`

Following code shows a method that returns a result:

```c#
enum ErrorCodes { InvalidArgument, EntityNotFound }

Result<ErrorCodes> UpdateEntity(string? id)
{
	if (id == null)
		return ErrorCodes.InvalidArgument;

	var entity = GetEntity(id);
	if (entity == null)
		return ErrorCodes.EntityNotFound;

	UpdateEntity(entity);

	return Result.Success();
}
```

Following code shows a method that received a result:

```c#
record Error(ErrorCodes Code, string Message);

Result<Error> SomeBusinessLogic(string? entityId)
{
	var result = UpdateEntity(entityId);
	if (result.IsFailure())
		return new Error(result.Error, "Failed to update entity");

	return Result.Success();
}
```

Use of getter `Error` can produce an exception is called with a success result. A more secure alternative is:

```c#
record Error(ErrorCodes Code, string Message);

Result<Error> SomeBusinessLogic(string? entityId)
{
	var result = UpdateEntity(entityId);
	if (result.IsFailure(out var code))
		return new Error(code, "Failed to update entity");

	return Result.Success();
}
```

Previous logic can be implemented using a continuation:

```c#
record Error(ErrorCodes Code, string Message);

Result<Error> SomeBusinessLogic(string? entityId)
	=> UpdateEntity(entityId)
		.On<Error>(_ => Result.Success(),
			errorCode => new Error(code, "Failed to update entity"));
```

## Working with `Result<TData,TError>`

Following code shows a method that returns a result:

```c#
enum ErrorCodes { InvalidArgument, EntityNotFound }

Result<Entity,ErrorCodes> UpdateEntity(string? id)
{
	if (id == null)
		return ErrorCodes.InvalidArgument;

	var entity = GetEntity(id);
	if (entity == null)
		return ErrorCodes.EntityNotFound;

	UpdateEntity(entity);

	return entity;
}
```

## Groups of methods

There are some groups of methods in types `Result<TError>` and `Result<TData,TError>`:

- Getters: Properties `Data` and `Error`. They introduce a potencial invalid use that can not be avoided at compile time.
This use corresponds to a program bug, so it is signaled at execution time throwing exception `InvalidOperationException`.

- Simple validations: Methods `IsSuccess()` and `IsFailure()`.

- Validations with extractions: Methods `IsSuccess(out TData data)` an `IsFailure(out TError error)`.
  Allow secure access to result internal data.

- Continuations: Methods `OnSuccess`, `OnFailure` and `On`.
