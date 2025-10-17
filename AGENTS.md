# Project Guidelines

## Architecture
- Use Clean Architecture.
- Organize layers as **Domain**, **Application**, **Infrastructure**, and **API**.
- Use Entity Framework Core for data access.
- Store configuration in `appsettings.json`.
- Implement RESTful controllers with validation and DTOs.
- Configure dependency injection in `Program.cs`.
- Enable structured logging with Serilog.
- Provide Swagger/OpenAPI documentation.
- Write unit tests with xUnit and FluentAssertions.

## Code Style
- Use clear type definitions; do not use `var`.
- Prefer good, descriptive variable names.
- Use camelCase for local variables.
- Use PascalCase for methods and class-level variables.
- Use UPPER_SNAKE_CASE for TA properties.
- Endpoints must not return TA elements.
- LINQ and Entity Framework transformations are allowed with simple, reasonable use.
- Use 4 spaces instead of tabs.
- Use CRLF for new lines.
- Place braces (`{}`) on new lines.
- Insert a blank line between methods.
- Apply common sense for blank lines inside methods; group related code together.
- Add a space before and after any operator (except for `++`).
- Use the `++` operator only after the variable, never before it.
- Prefer `double` over `float`.
- Prefer `List<T>` over arrays.
- Avoid using `out` and optional parameters.

## Build
- After completing any request, always attempt to run `dotnet build`.
