using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
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

    public class ObjectWriterTests
    {
        [Fact]
        public void CanWriteAnObject()
        {
            var stream = new MemoryStream();
            var jsonWriter = new JsonTextWriter(new StreamWriter(stream));


            jsonWriter.WriteStartArray();
            jsonWriter.WriteRaw(JsonConvert.SerializeObject(new
            {
                Description = "An Object.",
                Use = "Demonstrating a point."
            }));
            jsonWriter.WriteEnd();
            jsonWriter.Flush();
            stream.Position = 0;
            var result = new StreamReader(stream).ReadToEnd();

            Assert.Equal("[{\"Description\":\"An Object.\",\"Use\":\"Demonstrating a point.\"}]", result);
        }
    }

    public class ObjectReader<TOut> : IDisposable
    {
        private readonly TextReader _textReader;
        private readonly JsonTextReader _jsonReader;
        private readonly JsonSerializer _jsonSerializer;

        public ObjectReader(TextReader textReader, JsonSerializerSettings settings)
        {
            _textReader = textReader;
            _jsonReader = new JsonTextReader(_textReader);
            _jsonSerializer = JsonSerializer.Create(settings);
        }

        public IEnumerable<TOut> ReadToEnd()
        {
            while (_jsonReader.Read())
                if (_jsonReader.TokenType == JsonToken.StartObject)
                    yield return _jsonSerializer.Deserialize<TOut>(_jsonReader);

            _jsonReader.Close();
        }

        public void Dispose()
        {
            _textReader.Dispose();
        }
    }

    public class ObjectWriter : IDisposable
    {
        private readonly JsonTextWriter _jsonWriter;
        private readonly TextWriter _textWriter;
        private readonly JsonSerializerSettings _settings;

        public ObjectWriter(TextWriter textWriter, JsonSerializerSettings settings)
        {
            _textWriter = textWriter;
            _jsonWriter = new JsonTextWriter(_textWriter);
            _settings = settings;
        }

        public void Write(object toWrite)
        {
            if (_jsonWriter.WriteState == WriteState.Start)
                _jsonWriter.WriteStartArray();

            var objecString = JsonConvert.SerializeObject(toWrite, _settings);
            _jsonWriter.WriteRaw(objecString);
        }

        public void Dispose()
        {
            Close();
            _textWriter.Dispose();
        }

        public void Close()
        {
            _jsonWriter.WriteEnd();
            Flush();
            _jsonWriter.Close();
        }

        private void Flush()
        {
            _jsonWriter.Flush();
        }
    }
}
