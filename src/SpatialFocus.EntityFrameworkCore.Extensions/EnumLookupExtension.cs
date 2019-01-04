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
		private static List<Type> ConcreteTypeSeededList { get; set; } = new List<Type>();

		// See https://github.com/aspnet/EntityFrameworkCore/issues/12248#issuecomment-395450990
		public static void ConfigureEnumLookup(this ModelBuilder modelBuilder, EnumLookupOptions enumOptions)
		{
			foreach (IMutableProperty property in modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetProperties()).ToList())
			{
				Type propertyType = property.ClrType;

				if (!propertyType.IsEnumOrNullableEnumType())
				{
					continue;
				}

				if (enumOptions.UseEnumsWithAttributesOnly && !propertyType.HasEnumLookupAttribute())
				{
					continue;
				}

				IMutableEntityType entityType = property.DeclaringEntityType;

				Dictionary<int, string> enumValueDescriptions = Enum.GetValues(propertyType.GetEnumOrNullableEnumType())
					.Cast<Enum>()
					.ToDictionary(Convert.ToInt32, GetEnumDescription);

				bool usesDescription = enumValueDescriptions.Values.Any(x => x != null);

				Type concreteType;
				if (usesDescription)
				{
					concreteType = enumOptions.UseNumberLookup
						? typeof(EnumWithNumberLookupAndDescription<>).MakeGenericType(propertyType.GetEnumOrNullableEnumType())
						: typeof(EnumWithStringLookupAndDescription<>).MakeGenericType(propertyType.GetEnumOrNullableEnumType());
				}
				else
				{
					concreteType = enumOptions.UseNumberLookup
						? typeof(EnumWithNumberLookup<>).MakeGenericType(propertyType.GetEnumOrNullableEnumType())
						: typeof(EnumWithStringLookup<>).MakeGenericType(propertyType.GetEnumOrNullableEnumType());
				}

				EntityTypeBuilder enumLookupBuilder = modelBuilder.Entity(concreteType);

				string typeName = propertyType.GetEnumOrNullableEnumType().Name;
				string tableName = enumOptions.NamingFunction(typeName);
				enumLookupBuilder.ToTable(tableName);

				string keyName = enumOptions.UseNumberLookup
					? nameof(EnumWithNumberLookup<Enum>.Id)
					: nameof(EnumWithStringLookup<Enum>.Id);

				modelBuilder.Entity(entityType.Name)
					.HasOne(concreteType)
					.WithMany()
					.HasPrincipalKey(keyName)
					.HasForeignKey(property.Name)
					.OnDelete(enumOptions.DeleteBehavior);

				if (enumOptions.UseNumberLookup)
				{
					modelBuilder.Entity(concreteType).HasIndex(nameof(EnumWithNumberLookup<Enum>.Name)).IsUnique();
				}
				else
				{
					Type converterType = typeof(EnumToStringConverter<>).MakeGenericType(propertyType.GetEnumOrNullableEnumType());
					ValueConverter valueConverter = (ValueConverter)Activator.CreateInstance(converterType, new object[] { null });

					modelBuilder.Entity(entityType.Name).Property(property.Name).HasConversion(valueConverter);
					modelBuilder.Entity(concreteType).Property(keyName).HasConversion(valueConverter);
				}

				if (ConcreteTypeSeededList.Contains(concreteType))
				{
					continue;
				}

				ConcreteTypeSeededList.Add(concreteType);

				// TODO: Check status of https://github.com/aspnet/EntityFrameworkCore/issues/12194 before using migrations
				object[] data = Enum.GetValues(propertyType.GetEnumOrNullableEnumType())
					.OfType<object>()
					.Select(x =>
					{
						object instance = Activator.CreateInstance(concreteType);

						if (enumOptions.UseNumberLookup)
						{
							concreteType.GetProperty(nameof(EnumWithNumberLookup<object>.Id)).SetValue(instance, x);
							concreteType.GetProperty(nameof(EnumWithNumberLookup<object>.Name)).SetValue(instance, x.ToString());

							if (usesDescription)
							{
								concreteType.GetProperty(nameof(EnumWithNumberLookupAndDescription<object>.Description))
									.SetValue(instance, enumValueDescriptions[(int)x]);
							}
						}
						else
						{
							concreteType.GetProperty(nameof(EnumWithStringLookup<object>.Id)).SetValue(instance, x);

							if (usesDescription)
							{
								concreteType.GetProperty(nameof(EnumWithNumberLookupAndDescription<object>.Description))
									.SetValue(instance, enumValueDescriptions[(int)x]);
							}
						}

						return instance;
					})
					.ToArray();

				enumLookupBuilder.HasData(data);
			}
		}

		public static string GetEnumDescription(Enum value)
		{
			FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

			DescriptionAttribute attribute = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute), true);

			return attribute?.Description;
		}

		private static Type GetEnumOrNullableEnumType(this Type propertyType)
		{
			if (!propertyType.IsEnumOrNullableEnumType())
			{
				return null;
			}

			return propertyType.IsEnum ? propertyType : propertyType.GetGenericArguments()[0];
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
	}
}