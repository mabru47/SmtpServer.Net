using Tireless.Net.Mail.Exceptions;
using System;
using System.IO;

namespace Tireless.Net.Mail.Helper
{
    class StreamLimiter : Stream
    {
        private Stream baseStream;
        private Int64 bytesReadTotal;

        public Int64 MaxReadLength
        {
            get; set;
        }

        public StreamLimiter(Stream stream, Int64 maxReadLength)
        {
            this.baseStream = stream;
            this.MaxReadLength = maxReadLength;
        }

        public override bool CanRead => baseStream.CanRead;

        public override bool CanSeek => baseStream.CanSeek;

        public override bool CanWrite => baseStream.CanWrite;

        public override long Length => baseStream.Length;

        public override long Position
        {
            get
            {
                return baseStream.Position;
            }
            set
            {
                baseStream.Position = value;
            }
        }

        public override void Flush()
        {
            this.baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytesRead = this.baseStream.Read(buffer, offset, count);
            bytesReadTotal += bytesRead;
            if (bytesReadTotal > this.MaxReadLength)
                throw new SecurityException();
            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.baseStream.Write(buffer, offset, count);
        }

        public void ResetLimits()
        {
            this.bytesReadTotal = 0;
        }
    }
}
