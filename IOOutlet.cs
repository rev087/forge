using UnityEngine;
using System.Reflection;
using Forge.Extensions;

namespace Forge {

	public enum IOOutletType { Input, Output }

	public struct IOOutlet {
		public MemberInfo Member;
		public System.Type DataType;

		public string Name {
			get { return Member.Name; }
		}

		public IOOutlet(MemberInfo member, bool isInput=false) {
			Member = member;
			DataType = member.OutletType(isInput);
		}

		public static IOOutlet None {
			get { return new IOOutlet() { Member=null, DataType=null }; }
		}

		public bool IsNone() {
			return Member == null && DataType == null;
		}

		public static bool CanConnect(IOOutlet output, IOOutlet input) {

			// Multiple inputs
			if (input.DataType.IsCollection()) {
				return output.DataType == input.DataType.GetGenericArguments()[0];
			}

			// Single inputs
			else {
				return output.DataType == input.DataType;
			}

		}
	}
	
}