// <copyright file="EnumLookupExtension.cs" company="Spatial Focus">
// Copyright (c) Spatial Focus. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SpatialFocus.EntityFrameworkCore.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;
	using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

	public static class EnumLookupExtension
	{
		public static void ConfigureEnumLookup(this ModelBuilder modelBuilder, EnumLookupOptions enumOptions)
		{
			foreach (IMutableProperty property in modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetProperties()).ToList())
			{
				IMutableEntityType entityType = property.DeclaringEntityType;

				if (entityType.IsOwned())
				{
					continue;
				}

				ConfigureEnumLookupForProperty(modelBuilder, enumOptions, property,
					(enumLookupEntityType) =>
					{
						modelBuilder.Entity(entityType.Name)
							.HasOne(enumLookupEntityType)
							.WithMany()
							.HasPrincipalKey("Id")
							.HasForeignKey(property.Name)
							.OnDelete(enumOptions.DeleteBehavior);
					}, (valueConverter) => { modelBuilder.Entity(entityType.Name).Property(property.Name).HasConversion(valueConverter); });
			}
		}

		public static void ConfigureOwnedEnumLookup<TEntity, TDependentEntity>(
			this OwnedNavigationBuilder<TEntity, TDependentEntity> ownedNavigationBuilder, EnumLookupOptions enumOptions,
			ModelBuilder modelBuilder) where TEntity : class where TDependentEntity : class
		{
			foreach (IMutableProperty property in ownedNavigationBuilder.OwnedEntityType.GetProperties().ToList())
			{
				ConfigureEnumLookupForProperty(modelBuilder, enumOptions, property,
					(enumLookupEntityType) =>
					{
						ownedNavigationBuilder.HasOne(enumLookupEntityType)
							.WithMany()
							.HasPrincipalKey("Id")
							.HasForeignKey(property.Name)
							.OnDelete(enumOptions.DeleteBehavior);
					}, (valueConverter) => { ownedNavigationBuilder.Property(property.Name).HasConversion(valueConverter); });
			}
		}

		public static string GetEnumDescription(Enum value)
		{
			FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

			DescriptionAttribute attribute = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute), true);

			return attribute?.Description;
		}

		// See https://github.com/aspnet/EntityFrameworkCore/issues/12248#issuecomment-395450990
		private static void ConfigureEnumLookupForProperty(ModelBuilder modelBuilder, EnumLookupOptions enumOptions,
			IMutableProperty property, Action<Type> configureEntityType, Action<ValueConverter> configureEntityTypeConversion)
		{
			Type propertyType = property.ClrType;

			if (ShouldSkip(propertyType, enumOptions))
			{
				return;
			}

			Type enumType = propertyType.GetEnumOrNullableEnumType();

			Dictionary<int, string> enumValueDescriptions = GetEnumValueDescriptions(enumType);

			bool usesDescription = enumValueDescriptions.Values.Any(x => x != null);

			Type enumLookupEntityType = GetEnumLookupEntityType(enumOptions, usesDescription, enumType);
			bool shouldSkipEnumLookupTableConfiguration = modelBuilder.Model.FindEntityType(enumLookupEntityType) != null;

			configureEntityType(enumLookupEntityType);

			ValueConverter valueConverter = GetValueConverter(enumType);

			if (!enumOptions.UseNumberLookup)
			{
				configureEntityTypeConversion(valueConverter);
			}

			if (shouldSkipEnumLookupTableConfiguration)
			{
				return;
			}

			EntityTypeBuilder enumLookupBuilder = modelBuilder.Entity(enumLookupEntityType);
			ConfigureEnumLookupTable(enumLookupBuilder, enumOptions, enumType);

			if (enumOptions.UseNumberLookup)
			{
				modelBuilder.Entity(enumLookupEntityType).HasIndex("Name").IsUnique();
			}
			else
			{
				modelBuilder.Entity(enumLookupEntityType).Property("Id").HasConversion(valueConverter);
			}

			// TODO: Check status of https://github.com/aspnet/EntityFrameworkCore/issues/12194 before using migrations
			enumLookupBuilder.HasData(GetEnumData(enumType, enumLookupEntityType, enumOptions.UseNumberLookup, usesDescription,
				enumValueDescriptions));
		}

		private static void ConfigureEnumLookupTable(EntityTypeBuilder enumLookupBuilder, EnumLookupOptions enumOptions, Type enumType)
		{
			string typeName = enumType.Name;
			string tableName = enumOptions.NamingFunction(typeName);
			enumLookupBuilder.ToTable(tableName);
		}

		private static object[] GetEnumData(Type enumType, Type concreteType, bool useNumberLookup, bool usesDescription,
			Dictionary<int, string> enumValueDescriptions)
		{
			return Enum.GetValues(enumType)
				.OfType<object>()
				.Select(x =>
				{
					object instance = Activator.CreateInstance(concreteType);

					concreteType.GetProperty("Id").SetValue(instance, x);

					if (useNumberLookup)
					{
						concreteType.GetProperty("Name").SetValue(instance, x.ToString());
					}

					if (usesDescription)
					{
						concreteType.GetProperty("Description").SetValue(instance, enumValueDescriptions[(int)x]);
					}

					return instance;
				})
				.ToArray();
		}

		private static Type GetEnumLookupEntityType(EnumLookupOptions enumOptions, bool usesDescription, Type enumType)
		{
			Type concreteType;
			if (usesDescription)
			{
				concreteType = enumOptions.UseNumberLookup
					? typeof(EnumWithNumberLookupAndDescription<>).MakeGenericType(enumType)
					: typeof(EnumWithStringLookupAndDescription<>).MakeGenericType(enumType);
			}
			else
			{
				concreteType = enumOptions.UseNumberLookup
					? typeof(EnumWithNumberLookup<>).MakeGenericType(enumType)
					: typeof(EnumWithStringLookup<>).MakeGenericType(enumType);
			}

			return concreteType;
		}

		private static Type GetEnumOrNullableEnumType(this Type propertyType)
		{
			if (!propertyType.IsEnumOrNullableEnumType())
			{
				return null;
			}

			return propertyType.IsEnum ? propertyType : propertyType.GetGenericArguments()[0];
		}

		private static Dictionary<int, string> GetEnumValueDescriptions(Type enumType)
		{
			return Enum.GetValues(enumType).Cast<Enum>().ToDictionary(Convert.ToInt32, GetEnumDescription);
		}

		private static ValueConverter GetValueConverter(Type enumType)
		{
			Type converterType = typeof(EnumToStringConverter<>).MakeGenericType(enumType);
			ValueConverter valueConverter = (ValueConverter)Activator.CreateInstance(converterType, new object[] { null });
			return valueConverter;
		}

		private static bool HasEnumLookupAttribute(this Type propertyType)
		{
			if (propertyType.GetCustomAttributes(typeof(EnumLookupAttribute), true).Any())
			{
				return true;
			}

			if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				if (propertyType.GetGenericArguments()[0].GetCustomAttributes(typeof(EnumLookupAttribute), true).Any())
				{
					return true;
				}
			}

			return false;
		}

		private static bool IsEnumOrNullableEnumType(this Type propertyType)
		{
			if (propertyType.IsEnum)
			{
				return true;
			}

			if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				if (propertyType.GetGenericArguments()[0].IsEnum)
				{
					return true;
				}
			}

			return false;
		}

		private static bool ShouldSkip(Type propertyType, EnumLookupOptions enumOptions)
		{
			if (!propertyType.IsEnumOrNullableEnumType())
			{
				return true;
			}

			if (enumOptions.UseEnumsWithAttributesOnly && !propertyType.HasEnumLookupAttribute())
			{
				return true;
			}

			return false;
		}
	}
}