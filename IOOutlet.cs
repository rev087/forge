using UnityEngine;
using System.Reflection;
using Forge.Extensions;

namespace Forge {

	public enum IOOutletType { None, Input, Output }

	public struct IOOutlet {
		public MemberInfo Member;
		public System.Type DataType;
		public IOOutletType Type;

		public string Name {
			get { return Member.Name; }
		}

		public IOOutlet(MemberInfo member, IOOutletType outletType) {
			Member = member;
			DataType = member.OutletDataType(outletType);
			Type = outletType;
		}

		public override bool Equals(object obj) {
			try {
				return (bool)(this == (IOOutlet)obj);
			} catch {
				return false;
			}
		}
		public static bool operator ==(IOOutlet a, IOOutlet b) {
			return a.Member == b.Member && a.DataType == b.DataType && a.Type == b.Type;
		}
		public static bool operator !=(IOOutlet a, IOOutlet b) {
			return a.Member != b.Member || a.DataType != b.DataType || a.Type == b.Type;
		}
		public override int GetHashCode() {
			return Member.GetHashCode() * 17 + DataType.GetHashCode() + Type.GetHashCode();
		}


		public static IOOutlet None {
			get { return new IOOutlet() { Member=null, DataType=null, Type=IOOutletType.None }; }
		}

		public bool IsInput {
			get { return Type == IOOutletType.Input; }
		}

		public bool IsOutput {
			get { return Type == IOOutletType.Output; }
		}

		public bool IsNone() {
			return Member == null && DataType == null && Type == IOOutletType.None;
		}

		public static bool CanConnect(IOOutlet output, IOOutlet input) {

			if (!output.IsOutput || !input.IsInput) {
				return false;
			}

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