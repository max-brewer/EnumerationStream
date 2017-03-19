using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EnumerationStream
{
    public class EnumerationStream : Stream
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly IEnumerator<byte> _byteEnumerator;

        public EnumerationStream(IEnumerable enumerable)
        {
            _byteEnumerator = GetBytes(enumerable).GetEnumerator();
        }

        public override void Flush() { }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var position = offset;
            for (; position < buffer.Length && _byteEnumerator.MoveNext(); position++)
                buffer[position] = _byteEnumerator.Current;

            return position - offset;
        }

        private static IEnumerable<byte> GetBytes(IEnumerable enumerable)
        {
            return HeadBytes
                .Concat(GetContentBytes(enumerable))
                .Concat(TailBytes);
        }

        private static IEnumerable<byte> GetContentBytes(IEnumerable enumerable)
        {
            var contentEnumerator = enumerable.GetEnumerator();

            if (!contentEnumerator.MoveNext())
                yield break;

            foreach (var b in GetBytes(contentEnumerator.Current))
                yield return b;


            while (contentEnumerator.MoveNext())
            {
                foreach (var b in GetBytes(","))
                    yield return b;

                foreach (var b in GetBytes(contentEnumerator.Current))
                    yield return b;
            }
        }


        private static IEnumerable<byte> HeadBytes => GetBytes("[");
        private static IEnumerable<byte> TailBytes => GetBytes("]");

        private static IEnumerable<byte> GetBytes(object item) =>
            GetBytes(JsonConvert.SerializeObject(item, JsonSerializerSettings));

        private static IEnumerable<byte> GetBytes(string characters) =>
            System.Text.Encoding.UTF8.GetBytes(characters);


        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length { get; }
        public override long Position { get; set; }
    }
}
