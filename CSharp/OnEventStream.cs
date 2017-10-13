using System;
using System.IO;

namespace DM2BD.DTK.DAL.Lib
{
    /// <summary>
    /// Wraps another stream and provides reporting for when bytes are read or written to the stream.
    /// </summary>
    public class OnEventStream : Stream
    {
        #region Private Data Members
        private Stream innerStream;
        private Action<OnEventStream> _onclosed_action;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Stream supplying the stream for it to report on.
        /// </summary>
        /// <param name="streamToReportOn">The underlying stream that will be reported on when bytes are read or written.</param>
        public OnEventStream(Stream streamToReportOn, Action<OnEventStream> OnClosed =null)
        {
            if (streamToReportOn != null)
            {
                this.innerStream = streamToReportOn;
            }
            else
            {
                throw new ArgumentNullException("streamToReportOn");
            }
            _onclosed_action = OnClosed;

        }
        #endregion



        #region Stream Members

        public override bool CanRead
        {
            get { return innerStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return innerStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return innerStream.CanWrite; }
        }

        public override void Flush()
        {
            innerStream.Flush();
        }

        public override long Length
        {
            get { return innerStream.Length; }
        }

        public override long Position
        {
            get
            {
                return innerStream.Position;
            }
            set
            {
                innerStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytesRead = innerStream.Read(buffer, offset, count);
            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            innerStream.Write(buffer, offset, count);
        }

        public override void Close()
        {
            innerStream.Close();
            base.Close();

            if (_onclosed_action != null)
            {
                _onclosed_action.Invoke(this);
            }
        }
        #endregion
    }








}

