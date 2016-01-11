namespace Forge {

	public struct IOConnection {
		public Operator From;
		public IOOutlet Output;
		public Operator To;
		public IOOutlet Input;

		public IOConnection(Operator a, IOOutlet o, Operator b, IOOutlet i) {
			From = a;
			Output = o;
			To = b;
			Input = i;
		}

		public override string ToString() {
			return string.Format("{0}.{1} \n{2}.{3}", From.Metadata.Title, Output, To.Metadata.Title, Input);
		}
	}
	
}