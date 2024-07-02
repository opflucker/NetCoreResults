# Simple Result Monad Implementation

There are some libraries that implement this monad,
but most used functionality requires an extremely short code,
so we can avoid import a library and implement it ourself.

This is a short implementation example. Relevant design decisions:

- `Result` types are implemented with classes. Structs are more efficient but their mandatory default constructors not always can create a valid result.

- `Result` are expected to be created only when returning from a method, so this is the only implemented mechanism.

There are some groups of methods in `Result` types:

- Getters: Properties `Data` an `Error`. Because it is not secure client code use them only after proper validation, 
they introduce a potencial invalid use that can not be avoided at compile time.
This use corresponds to a program bug, so it is signaled at execution time throwing exception `InvalidOperationException`.

- Simple validations: Methods `IsSuccess()` an `IsFailure()`.

- Validations with extractions: Methods `IsSuccess(out TData data)` an `IsFailure(out TError error)`. Allow secure access to result data.

- Continuations: Methods `OnSuccess`, `OnFailure` and `On`.
