using UnityEngine;
using System.Reflection;

namespace Forge.Extensions {

	public static class Extensions {

		public static bool Contains(this int[] array, int value) {
			for (int i = 0; i < array.Length; i++) {
				if (array[i] == value) return true;
			}
			return false;
		}

		public static Vector3 Abs(this Vector3 v) {
			return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
		}

		public static float Remap(this float original, float fromMin, float fromMax, float toMin, float toMax) {
			if (toMin == toMax) return toMin;
			
			if (fromMin == fromMax) {
				return (toMax - toMin) / 2;
			}

			return (((original - fromMin) * (toMax - toMin)) / (fromMax - fromMin)) + toMin;
		}

		public static float Clamp(this float value, float min, float max) {
			if (value > max) return max;
			else if (value < min) return min;
			return value;
		}

		public static bool In(this int val, params int[] list) {
			for (int i = 0; i < list.Length; i++) {
				if (list[i] == val) return true;
			}
			return false;
		}

		public static int[] ToRange(this int val, bool inclusive = false) {
			int count = inclusive ? val + 1 : val;
			int[] range = new int[count];
			for (var i = 0; i < count; i++) {
				range[i] = i;
			}
			return range;
		}

		public static System.Type OutletDataType(this MemberInfo member, IOOutletType outletType) {
			switch (member.MemberType) {
				case MemberTypes.Method:
					if (outletType == IOOutletType.Input) {
						var paramsInfo = ((MethodInfo)member).GetParameters();
						if (paramsInfo.Length != 1) {
							throw new System.ArgumentException(
								System.String.Format("{0}.{1} must accept exactly one parameter to be a valid operator input",
									((MethodInfo)member).DeclaringType.Name,
									((MethodInfo)member).Name)
							);
						}
						return paramsInfo[0].ParameterType;
					} else if (outletType == IOOutletType.Output) {
						return ((MethodInfo)member).ReturnType;
					} else {
						throw new System.ArgumentException("Invalid outlet type");
					}
				case MemberTypes.Field:
					return ((FieldInfo)member).FieldType;
				case MemberTypes.Property:
					return ((PropertyInfo)member).PropertyType;
        default:
					throw new System.ArgumentException("OutletDataType can only be used with FieldInfo, MethodInfo, or PropertyInfo");
			}
		}

		public static void Add(this JSONObject json, params float[] values) {
			foreach (float val in values) {
				json.Add(val);
			}
		}

		public static bool IsCollection(this System.Type type) {
			return System.Array.Exists(type.GetInterfaces(), t => {
				return t == typeof(System.Collections.ICollection);
			});
		}


		public static string TypeAlias(this System.Type type) {

			// See https://msdn.microsoft.com/en-us/library/ya5y69ds
			if (type == typeof(System.Boolean)) return "bool";
			else if (type == typeof(System.String)) return "string";
			else if (type == typeof(System.Int32)) return "int";			
			else if (type == typeof(System.Single)) return "float";
			else if (type == typeof(System.Double)) return "double";
			else if (type == typeof(System.Byte)) return "byte";
			else if (type == typeof(System.SByte)) return "sbyte";
			else if (type == typeof(System.Char)) return "char";
			else if (type == typeof(System.Decimal)) return "decimal";
			else if (type == typeof(System.UInt32)) return "uint";
			else if (type == typeof(System.Int64)) return "long";
			else if (type == typeof(System.UInt64)) return "ulong";
			else if (type == typeof(System.Int16)) return "short";
			else if (type == typeof(System.UInt16)) return "ushort";

			// Collections
			else if (type.IsCollection()) {
				return System.String.Format("{0} ({1})", type.Name, type.GetGenericArguments().TypeAlias());
			}

			return type.Name;
		}

		public static string TypeAlias(this System.Type[] types) {
			string alias = "";
			for (int i = 0; i < types.Length; i++) {
				if (i > 0) alias += ", ";
				alias += types[i].TypeAlias();
			}
			return alias;
		}

	}

}