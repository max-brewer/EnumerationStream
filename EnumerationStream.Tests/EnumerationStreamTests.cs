using System.IO;
using Xunit;

namespace EnumerationStream.Tests
{
    public class EnumerationStreamTests
    {
        [Fact]
        public void It_is_a_Stream()
        {
            using (var target = GetTarget())
                Assert.IsAssignableFrom<Stream>(target);
        }

        [Fact]
        public void It_is_not_Writeable()
        {
            using (var target = GetTarget())
                Assert.False(target.CanWrite);
        }

        [Fact]
        public void It_is_not_Seekable()
        {
            using (var target = GetTarget())
                Assert.False(target.CanSeek);
        }

        [Fact]
        public void It_is_Readable()
        {
            using (var target = GetTarget())
                Assert.True(target.CanRead);
        }

        [Fact]
        public void An_empty_enumeration_streams_an_empty_Json_array()
        {
            Assert.Equal("[]", ReadTarget());
        }

        [Fact]
        public void An_enumeration_of_one_object_streams_its_Json_representation_in_an_array()
        {
            Assert.Equal("[{}]", ReadTarget(new object()));
        }

        [Fact]
        public void An_enumeration_of_two_object_streams_their_Json_representations_in_an_array()
        {
            Assert.Equal("[{},{}]", ReadTarget(new object(),new object()));
        }

        [Fact]
        public void Correctly_serialises_simple_object()
        {
            Assert.Equal("[{\"number\":1,\"text\":\"Some Text\"}]", 
                ReadTarget(new {Number=1, Text = "Some Text"}));
        }

        [Fact]
        public void Correctly_handles_non_ascii_characters()
        {
            Assert.Equal("[{\"turkishEye\":\"Iıİi\"},{\"kanji\":\"漢字\"}]",
                ReadTarget(new {TurkishEye = "Iıİi"}, new { Kanji = "漢字"}));
        }

        private static string ReadTarget(params object[] contents)
        {
            using (var target = GetTarget(contents))
            using (var reader = new StreamReader(target))
                return reader.ReadToEnd();
        }

        private static EnumerationStream GetTarget(params object[] contents)
        {
            return new EnumerationStream(contents);
        }
    }
}
