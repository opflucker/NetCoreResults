# Simple Result Monad Implementation

## Introduction

There are some libraries that implement this monad,
but common used functionallity is extremely short,
so we can avoid import a library and implement it ourself.
This is an short implementation example.

Relevant design decisions:

- `Result` objects are expected to be created when returning from a method, so this is implemented using implicit cast operators.

- `Result` types are implemented with classes.
  Structs are more efficient but their mandatory default constructors can create invalid results.

- Any type can be used to represent errors.

## Result types

There are 3 Result types:

- `Result<TError>`: Represents a result without success data, only error data.
- `Result<TData,TError>`: Represents a result with success and error data.
- `Result`: Hold helper types and methods for creating result objects in two corner cases:
  - A success `Result<TError>`.
  - A `Result<TData,TError>` when `TData = TError`.

## Working with `Result<TError>`

Following code shows a method that returns a `Result` object:

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

Following code shows a method that receive a `Result` object:

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

Use of getter `Error` can produce an exception if it is called with a success `Result`. A more secure alternative is:

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
		.MapFailure(errorCode => new Error(code, "Failed to update entity"));
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

- Getters: Properties `Data` and `Error`. They introduce a potencial invalid use case that can not be avoided at compile time.
This use case corresponds to a program bug, so it is signaled at execution time throwing exception `InvalidOperationException`.

- Simple validations: Methods `IsSuccess()` and `IsFailure()`.

- Validations with extractions: Methods `IsSuccess(out TData data)` an `IsFailure(out TError error)`.
Allow secure access to result internal data.

- Continuations: Methods `OnSuccess`, `OnFailure`, `On` and `Narrow`.
