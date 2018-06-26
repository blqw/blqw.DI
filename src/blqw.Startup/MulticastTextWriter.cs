using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary>
    /// 多路文本输出
    /// </summary>
    sealed class MulticastTextWriter : TextWriter
    {
        private readonly TextWriter[] _writers;
        private readonly TextWriter _first;

        private void Each(Action<TextWriter> action)
        {
            for (var i = 0; i < _writers.Length; i++)
            {
                action(_writers[i]);
            }
        }

        private Task EachAsync(Func<TextWriter, Task> task)
        {
            var tasks = new Task[_writers.Length];
            for (var i = 0; i < _writers.Length; i++)
            {
                tasks[i] = task(_writers[i]);
            }
            return Task.WhenAll(tasks);
        }

        public MulticastTextWriter(params TextWriter[] writers)
        {
            _writers = writers ?? throw new ArgumentNullException(nameof(writers));
            _first = _writers.First();
        }

        public override Encoding Encoding => _first.Encoding;

        public override IFormatProvider FormatProvider => _first.FormatProvider;

        public override string NewLine { get => _first.NewLine; set => _first.NewLine = value; }

        public override void Close() => Each(x => x.Close());
        public override void Flush() => Each(x => x.Flush());
        public override Task FlushAsync() => EachAsync(x => x.FlushAsync());
        public override void Write(bool value) => Each(x => x.Write(value));
        public override void Write(char value) => Each(x => x.Write(value));
        public override void Write(char[] buffer) => Each(x => x.Write(buffer));
        public override void Write(char[] buffer, int index, int count) => Each(x => x.Write(buffer, index, count));
        public override void Write(decimal value) => Each(x => x.Write(value));
        public override void Write(double value) => Each(x => x.Write(value));
        public override void Write(int value) => Each(x => x.Write(value));
        public override void Write(long value) => Each(x => x.Write(value));
        public override void Write(object value) => Each(x => x.Write(value));
        public override void Write(float value) => Each(x => x.Write(value));
        public override void Write(string value) => Each(x => x.Write(value));
        public override void Write(string format, object arg0) => Each(x => x.Write(format, arg0));
        public override void Write(string format, object arg0, object arg1) => Each(x => x.Write(format, arg0, arg1));
        public override void Write(string format, object arg0, object arg1, object arg2) => Each(x => x.Write(format, arg0, arg1, arg2));
        public override void Write(string format, params object[] arg) => Each(x => x.Write(format, arg));
        public override void Write(uint value) => Each(x => x.Write(value));
        public override void Write(ulong value) => Each(x => x.Write(value));
        public override Task WriteAsync(char value) => EachAsync(x => x.WriteAsync(value));
        public override Task WriteAsync(char[] buffer, int index, int count) => EachAsync(x => x.WriteAsync(buffer, index, count));
        public override Task WriteAsync(string value) => EachAsync(x => x.WriteAsync(value));
        public override void WriteLine() => Each(x => x.WriteLine());
        public override void WriteLine(bool value) => Each(x => x.WriteLine(value));
        public override void WriteLine(char value) => Each(x => x.WriteLine(value));
        public override void WriteLine(char[] buffer) => Each(x => x.WriteLine(buffer));
        public override void WriteLine(char[] buffer, int index, int count) => Each(x => x.WriteLine(buffer, index, count));
        public override void WriteLine(decimal value) => Each(x => x.WriteLine(value));
        public override void WriteLine(double value) => Each(x => x.WriteLine(value));
        public override void WriteLine(int value) => Each(x => x.WriteLine(value));
        public override void WriteLine(long value) => Each(x => x.WriteLine(value));
        public override void WriteLine(object value) => Each(x => x.WriteLine(value));
        public override void WriteLine(float value) => Each(x => x.WriteLine(value));
        public override void WriteLine(string value) => Each(x => x.WriteLine(value));
        public override void WriteLine(string format, object arg0) => Each(x => x.WriteLine(format, arg0));
        public override void WriteLine(string format, object arg0, object arg1) => Each(x => x.WriteLine(format, arg0, arg1));
        public override void WriteLine(string format, object arg0, object arg1, object arg2) => Each(x => x.WriteLine(format, arg0, arg1, arg2));
        public override void WriteLine(string format, params object[] arg) => Each(x => x.WriteLine(format, arg));
        public override void WriteLine(uint value) => Each(x => x.WriteLine(value));
        public override void WriteLine(ulong value) => Each(x => x.WriteLine(value));
        public override Task WriteLineAsync() => EachAsync(x => x.WriteLineAsync());
        public override Task WriteLineAsync(char value) => EachAsync(x => x.WriteLineAsync(value));
        public override Task WriteLineAsync(char[] buffer, int index, int count) => EachAsync(x => x.WriteLineAsync(buffer, index, count));
        public override Task WriteLineAsync(string value) => EachAsync(x => x.WriteLineAsync(value));
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Each(x => x.Dispose());
            }
        }
    }
}
