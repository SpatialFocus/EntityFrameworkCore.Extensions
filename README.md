# SpatialFocus.EntityFrameworkCore.Extensions

A set of useful extensions for EntityFrameworkCore (Enum Lookup Tables, Naming of tables / properties / keys, Pluralize).

[![Build Status](https://spatial-focus.visualstudio.com/EntityFrameworkCore.Extensions/_apis/build/status/EntityFrameworkCore.Extensions-CI)](https://spatial-focus.visualstudio.com/EntityFrameworkCore.Extensions/_build/latest?definitionId=3)
[![NuGet](https://img.shields.io/nuget/v/SpatialFocus.EntityFrameworkCore.Extensions.svg)](https://www.nuget.org/packages/SpatialFocus.EntityFrameworkCore.Extensions/)

## Installation

```console
Install-Package SpatialFocus.EntityFrameworkCore.Extensions
```

## Usage

This extension provides two extension methods to modify the `modelBuilder` in the `OnModelCreating` method in the `DbContext` class.

```csharp
using SpatialFocus.EntityFrameworkCore.Extensions;

public partial class MyContext : DbContext
{
   // ...

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      modelBuilder.ConfigureEnumLookup(EnumLookupOptions.Default.UseStringAsIdentifier());

      modelBuilder.ConfigureNames(NamingOptions.Default.SkipTableNamingForGenericEntityTypes());

      // ...
   }
}
```

## Configuration Options

The two extension methods in the `OnModelCreating` method in the `DbContext` class:

- **ConfigureEnumLookup(...)** allows you to define in which form lookup tables for *Enums* will be constructed and named:

  - **Default** defines the naming scheme for the table to use *snake_case* and to use the number lookup format.
  - **UseNumberAsIdentifier()** or **UseStringAsIdentifier()** defines whether the lookup table will be based on the string enum values or the numeric enum value as primary key in the resulting table and as foreign key in the relation.
  - **Singularize()** or **Pluralize()** defines whether the table names will be the singular or plural versions of the enum type.
  - **SetNamingScheme(...)** allows you to override the naming using one of the predefined schemes (see below) or a custom function.
  - **UseEnumsWithAttributeOnly()** to generate the enum lookup tables only for enums marked with the `[EnumLookup]` attribute
  - **SetDeleteBehavior(...)** to configure the delete behavior for the generated FKs, using the `Microsoft.EntityFrameworkCore.DeleteBehavior` enum (defaults to _Cascade_)

- **ConfigureNames(...)** allows you to define in which form tables, properties and constraints will be named:

  - **Default** defines the naming scheme for the elemens to use *snake_case* for naming and the *DbSet name* to name the tables.
  - **SetTableNamingSource(...)** defines which naming source to use (see below). It means whether to use the ClrType name or the DbSet name to name the tables.
  - **Singularize()** or **Pluralize()** defines whether the table names will be the singular or plural versions.
  - **SetNamingScheme(...)** allows you to override the naming using one of the predefined schemes (see below) or a custom function.
  - **OverrideTableNaming(...)**, **OverrideColumnNaming(...)**, **OverrideConstraintNaming(...)** to deviate from the general naming scheme.
  - **SkipEntireEntities(...)** and **SkipTableNamingForEntities(...)** to skip the naming for either the whole entity or just the table name for certain entities by using a `Func<IMutableEntityType, bool>` skip function, e.g. `SkipEntireEntities(entity => entity.Name == "MyEntity")`.
  - **SkipTableNamingForGenericEntityTypes()** should be used to avoid naming the Enum lookup tables which can lead to unwanted results.

### Naming Schemes

- NamingScheme.SnakeCase
- NamingScheme.ScreamingSnakeCase
- NamingScheme.KebabCase
- Any custom `Func<string, string>`, e.g. `SetNamingScheme(name => name.ToLower())`

#### Naming Source

- NamingSource.ClrType
- NamingSource.DbSet

## Examples

For an exemplary usage, see the `DemoContext` in the [sample project](https://github.com/SpatialFocus/SpatialFocus.EntityFrameworkCore.Extensions/tree/master/samples/SpatialFocus.EntityFrameworkCore.Extensions.SQLiteDemo).

----

Made with :heart: by [Spatial Focus](https://spatial-focus.net/)
