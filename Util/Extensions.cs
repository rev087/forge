using UnityEngine;
using System.Reflection;

namespace Forge.Extensions {

	public static class Extensions {

		public static Vector3 Abs(this Vector3 v) {
			return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
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

		public static System.Type OutletType(this MemberInfo member) {
			switch (member.MemberType) {
				case MemberTypes.Method:
					return ((MethodInfo)member).ReturnType;
				case MemberTypes.Field:
					return ((FieldInfo)member).FieldType;
				case MemberTypes.Property:
					return ((PropertyInfo)member).PropertyType;
        default:
					throw new System.ArgumentException("OutletType can only be used with FieldInfo, MethodInfo, or PropertyInfo");
			}
		}

	}

}