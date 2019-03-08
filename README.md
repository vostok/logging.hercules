# Vostok.Logging.Hercules

[![Build status](https://ci.appveyor.com/api/projects/status/github/vostok/logging.hercules?svg=true&branch=master)](https://ci.appveyor.com/project/vostok/logging.hercules/branch/master)
[![NuGet](https://img.shields.io/nuget/v/Vostok.Logging.Hercules.svg)](https://www.nuget.org/packages/Vostok.Logging.Hercules)

An implementation of Vostok ILog that send events to Hercules. Also provides mapping from Hercules events back to Log Events.

**Build guide**: https://github.com/vostok/devtools/blob/master/library-dev-conventions/how-to-build-a-library.md

**User documentation**: https://vostok.gitbook.io/logging/

-----

Here's how [LogEvent](https://github.com/vostok/logging.abstractions/blob/master/Vostok.Logging.Abstractions/LogEvent.cs) instances are mapped into Hercules events (according to [schema](https://github.com/vostok/hercules/blob/master/doc/event-schema/log-event-schema.md)):

- `Timestamp` (mandatory) corresponds to:
  - Hercules event built-in timestamp — `UtcDateTime` of `Timestamp`.
  - `utcOffset` tag — a `long` tag with offset from UTC expressed in 100-ns ticks.
  
- `Level` ---> `level` tag of `string` type.

- `MessageTemplate` ---> `messageTemplate` tag of `string` type.
   
- `RenderedMessage` ---> `message` tag containing rendered text.

- `Properties` dictionary corresponds to a container with name `properties`. This container contains a tag for each pair. Keys are translated as-is, and the values are handled according to following conventions:
  - If the value is a primitive scalar or a vector of primitive scalars natively supported by Hercules (such as `int`, `long`, `guid`, `string`, etc), it's mapped as-is. 
  - Otherwise the value gets converted to `string`: either stringified directly (if it properly overrides `ToString()`) or serialized to JSON. No further container-like structure is allowed, all values end up being 'flat'.
  
- `Exception` object corresponds to a container with name `exception` and following tags:
  - Exception runtime type (e.g. `System.NullReferenceException`) ---> `type` tag of type `string`.
  - Exception message ---> `message` tag of type `string`.
  - Nested exceptions (e.g. `InnerException` and `InnerExceptions` for `AggregateException`) ---> `innerExceptions` tag of type `Vector` which contains other exceptions in the same format.
  - Stacktrace of exception ---> `stackTrace` tag of type `Vector<StackFrame>`. `StackFrame` is a container of following tags which describe a point of code which executed when the exception occured:
    - `function` - a name of function (method).
    - `type` - a type when `function` is declared.
    - `file` - file name.
    - `line` - line number.
    - `column` - column number.
 
