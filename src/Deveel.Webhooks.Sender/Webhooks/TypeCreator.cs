// Copyright 2022-2023 Deveel
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Reflection;
using System.Reflection.Emit;

namespace Deveel.Webhooks {
	static class TypeCreator {
		private static int anonymousCounter = -1;

		private static void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType) {
			var fieldBuilder = typeBuilder.DefineField($"_{propertyName}", propertyType, FieldAttributes.Private);

			var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);

			var getMethodBuilder = typeBuilder.DefineMethod($"get_{propertyName}",
				MethodAttributes.Public |
				MethodAttributes.SpecialName |
				MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);

			var ilGen = getMethodBuilder.GetILGenerator();
			ilGen.Emit(OpCodes.Ldarg_0);
			ilGen.Emit(OpCodes.Ldfld, fieldBuilder);
			ilGen.Emit(OpCodes.Ret);

			var setMethodBuilder = typeBuilder.DefineMethod($"set_{propertyName}",
				MethodAttributes.Public |
				MethodAttributes.SpecialName |
				MethodAttributes.HideBySig, null, new[] { propertyType });
			ilGen = setMethodBuilder.GetILGenerator();
			ilGen.Emit(OpCodes.Ldarg_0);
			ilGen.Emit(OpCodes.Ldarg_1);
			ilGen.Emit(OpCodes.Stfld, fieldBuilder);
			ilGen.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(getMethodBuilder);
			propertyBuilder.SetSetMethod(setMethodBuilder);
		}

		private static Type? CreateType(Dictionary<string, Type> propertyTypes) {
			var assemblyName = new AssemblyName("Deveel.Webhooks.AnonymousTypes");
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule("Deveel.Webhooks.Types");
			var typeAtts = TypeAttributes.Public |
				TypeAttributes.AnsiClass |
				TypeAttributes.AutoClass |
				TypeAttributes.AutoLayout |
				TypeAttributes.BeforeFieldInit;

			var typeBuilder = moduleBuilder.DefineType($"_Anonynous_{anonymousCounter++}", typeAtts);

			foreach (var propertyType in propertyTypes) {
				string propertyName = propertyType.Key;
				var propertyDataType = propertyType.Value;

				CreateProperty(typeBuilder, propertyName, propertyDataType);
			}

			return typeBuilder.CreateType();
		}

		public static object? CreateAnonymousType(Dictionary<string, object?> dictionary) {
			var propertyTypes = dictionary.ToDictionary(x => x.Key, y => y.Value?.GetType() ?? typeof(object));
			var anonymousType = CreateType(propertyTypes);

			if (anonymousType == null)
				throw new InvalidOperationException("Unable to create the anonymous type");

			var anonymousObject = Activator.CreateInstance(anonymousType);
			if (anonymousObject != null) {
				foreach (var kvp in dictionary) {
					anonymousType.GetProperty(kvp.Key)?.SetValue(anonymousObject, kvp.Value);
				}
			}

			return anonymousObject;
		}
	}
}
