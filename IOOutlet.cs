using UnityEngine;
using System.Reflection;
using Forge.Extensions;

namespace Forge {

	public struct IOOutlet {
		public MemberInfo Member;

		public System.Type Type {
			get { return Member.OutletType(); }
		}
		public string Name {
			get { return Member.Name; }
		}

		public IOOutlet(MemberInfo member) {
			Member = member;
		}

		public static IOOutlet None {
			get { return new IOOutlet() { Member=null }; }
		}

		public bool IsNone() {
			return Member == null;
		}

		public static bool CanConnect(IOOutlet output, IOOutlet input) {
			return output.Type == input.Type;
		}
	}
	
}