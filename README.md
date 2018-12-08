# Vostok.Logging.Hercules

[![Build status](https://ci.appveyor.com/api/projects/status/github/vostok/logging.hercules?svg=true&branch=master)](https://ci.appveyor.com/project/vostok/logging.hercules/branch/master)
[![NuGet](https://img.shields.io/nuget/v/Vostok.Logging.Hercules.svg)](https://www.nuget.org/packages/Vostok.Logging.Hercules)

An implementation of Vostok ILog that send events to Hercules. Also provides mapping from Hercules events back to Log Events.

Here's how [LogEvent](https://github.com/vostok/logging.abstractions/blob/master/Vostok.Logging.Abstractions/LogEvent.cs) instances are mapped into Hercules events:

- `Timestamp` (mandatory) corresponds to:
  - Hercules event build-timestamp - a `UtcDateTime` of `Timestamp`.
  - `UtcOffset` tag - a `long` tag with offset from UTC expressed in 100-ns ticks.

- `MessageTemplate` ---> `MessageTemplate` tag of `string` type.
   
- `RenderedMessage` ---> `MessageTemplate` rendered to log string with templates replaced to corresponding values from `Properties`.

- `Properties` dictionary corresponds to a container with the same name. This container contains a tag for each pair. Keys are translated as-is, and the values are handled according to following conventions:
  - If the value is a primitive scalar or a vector of primitive scalars natively supported by Hercules (such as `int`, `long`, `guid`, `string`, etc), it's mapped as-is. 
  - Otherwise the value gets converted to `string`: either stringified directly (if it properly overrides `ToString()`) or serialized to JSON. No further container-like structure is allowed, all values end up being 'flat'.
  
- `Exception` object corresponds to a container with the same name and following tags:
  - Exception runtime type (e.g. `System.NullReferenceException`) ---> `Type` tag of type `string`.
  - Exception message ---> `Message` tag of type `string`.
  - Nested exceptions (e.g. `InnerException` and `InnerExceptions` for `AggregateException`) ---> `InnerExceptions` tag of type `Vector` which contains other exceptions in the same format.
  - Stacktrace of exception ---> `StackTrace` tag of type `Vector` of `StackFrame`. `StackFrame` is `Container` of the following tags which describe a point of code which executed when exception occured:
    - `Function` - a name of function (method).
    - `Type` - a type when `Function` is declared.
    - `File` - file name.
    - `Line` - line number.
    - `Column` - column number.
 