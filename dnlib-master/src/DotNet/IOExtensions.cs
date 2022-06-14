// dnlib: See LICENSE.txt for more info

using System.IO;

namespace dnlib.IO {
	public static partial class IOExtensions {
		public static IImageStream Create(this IImageStream self, FileOffset offset) {
			return self.Create(offset, long.MaxValue);
		}

		public static IImageStream Clone(this IImageStream self) {
			return self.Create((FileOffset)0L, long.MaxValue);
		}

		public static IImageStream Create(this IImageStreamCreator self, FileOffset offset) {
			return self.Create(offset, long.MaxValue);
		}
		public static Stream CreateStream(this IBinaryReader reader) {
			return new BinaryReaderStream(reader);
		}

		public static Stream CreateStream(this IBinaryReader reader, bool ownsReader) {
			return new BinaryReaderStream(reader, ownsReader);
		}

	}
}
