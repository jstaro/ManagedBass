﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ManagedBass
{
    public class StremFileProcedures : FileProcedures
    {
        readonly Stream _stream;

        public StremFileProcedures(Stream InputStream)
        {
            _stream = InputStream;

            if (!_stream.CanRead)
                throw new ArgumentException("Provide a readable stream", nameof(InputStream));

            Close = M => _stream.Dispose();
            Length = M => _stream.Length;
            Read = ReadProc;
            Seek = SeekProc;
        }

        byte[] b;

        int ReadProc(IntPtr Buffer, int Length, IntPtr User)
        {
            try
            {
                if (b == null || b.Length < Length)
                    b = new byte[Length];

                var read = _stream.Read(b, 0, Length);

                Marshal.Copy(b, 0, Buffer, read);

                return read;
            }
            catch { return 0; }
        }

        bool SeekProc(long Offset, IntPtr User)
        {
            if (!_stream.CanSeek)
                return false;

            try
            {
                _stream.Seek(Offset, SeekOrigin.Current);
                return true;
            }
            catch { return false; }
        }
    }
}