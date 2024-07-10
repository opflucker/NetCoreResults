# Minimal Result Monad Implementation

## Introduction

Minimal implementation of result monad, it is:

- Minimal implementation code: Short but effective, holding the minimal required data, without using any conditional, easy to read and understand.
- Minimal client boilerplate code: Less verbose and noisy, easy to read and understand.

Relevant design decisions:

- Result types are implemented with classes:
Chosen minimalism over efficiency. Structs are more efficient but introduce important limitations:
  - Structs have no inheritance, so the current design (classes Result, Success and Failure) can not be used.
Use of inheritance allows a cleaner and shorter code, without using any conditional, and enable the use of 
cast operator overloads that reduce client boilerplate code.
  - Inheritance problem can not be fixed with interfaces (implemented by structs) because they introduce boxing, 
losing structs efficiency. Additionally, interfaces do not support cast operation overloads.

- Some use cases are not implemented because they do not add significant advantages. In particular:
  - A result without success or failure data: This use case can be well covered returning a simple boolean.
  - A result with only success data: This use case can be well covered returning a nullable value.
  - A result with a typed set of errors (like Result<TData, TError1, TError2>): It overcomplicates implementation,
it forces to support different error set sizes (for 3, 4 and more errors), it introduce more client boilerplate code 
(because this large type declaration should be propagated along call stack) and it can be well covered using 
a more appropiate error type.
  - A result that holds internally a collection of errors (like ReadOnlyCollection<E> in type Result<E>): 
It overcomplicates implementation and it can be well covered using Result<ReadOnlyCollection<E>> when really needed.
  - A result with a default error type (like string): Any default type potentially assumes behaviours not needed 
by client code. Remember, this is a minimal implementation.

- Any type can be used to represent errors: Results not only do not need to assume any behaviour about error types 
(the only justification for introducing a base error class) but also errors are concepts that belongs to client code 
domain, not result library domain.

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

- Continuations:
  - Methods `On`, `OnSuccess` and `OnFailure`: Allow perform and action when result is success or failure without modifying the result.
  - Methods `Map`, `MapSuccess` and `MapFailure`: Allow perform a function producing a new result, potentially of a new type.
  - Method `TrimSuccess`: Creates an equivalent `Result<TError>` from a `Result<TData,TResult>`.
