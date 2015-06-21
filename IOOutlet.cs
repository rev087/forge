using UnityEngine;
using System.Reflection;
using Forge.Extensions;

namespace Forge {

	public struct IOOutlet {
		public MemberInfo Member;
		public System.Type Type;

		public string Name {
			get { return Member.Name; }
		}

		public IOOutlet(MemberInfo member, bool isInput=false) {
			Member = member;
			Type = member.OutletType(isInput);
		}

		public static IOOutlet None {
			get { return new IOOutlet() { Member=null, Type=null }; }
		}

		public bool IsNone() {
			return Member == null && Type == null;
		}

		public static bool CanConnect(IOOutlet output, IOOutlet input) {

			// Multiple inputs
			if (input.Type.IsCollection()) {
				return output.Type == input.Type.GetGenericArguments()[0];
			}

			// Single inputs
			else {
				return output.Type == input.Type;
			}

		}
	}
	
}