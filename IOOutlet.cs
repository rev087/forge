using UnityEngine;

namespace Forge {

	public struct IOOutlet {
		public System.Type Type;
		public string Name;

		public IOOutlet(System.Type type, string name) {
			Type = type;
			Name = name;
		}

		public static IOOutlet None {
			get { return new IOOutlet(typeof(System.Boolean), null); }
		}

		public bool IsNone() {
			return Type == typeof(System.Boolean) && Name == null;
		}

		public static bool CanConnect(IOOutlet output, IOOutlet input) {
			return output.Type == input.Type;
		}
	}
	
}