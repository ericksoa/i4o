using System;

namespace i4o.Tests
{
#if SILVERLIGHT
    public class Stopwatch
    {
        private DateTime _startUtcDateTime;
        private DateTime? _endUtcDateTime;
        private bool _isRunning;

        //public static readonly long Frequency { get { throw new NotImplementedException(); } }
        //public static readonly bool IsHighResolution;

        public TimeSpan Elapsed
        {
            get
            {
                if (_endUtcDateTime.HasValue)
                {
                    return new TimeSpan(_endUtcDateTime.Value.Ticks - _startUtcDateTime.Ticks);
                }

                return new TimeSpan(DateTime.UtcNow.Ticks - _startUtcDateTime.Ticks);
            }
        }

        public long ElapsedMilliseconds { get { return Elapsed.Milliseconds; } }
        public long ElapsedTicks { get { return Elapsed.Ticks; } }
        public bool IsRunning { get { return _isRunning; } }

        public static long GetTimestamp()
        {
            return DateTime.Now.Ticks;
        }

        public void Reset()
        {
            _endUtcDateTime = null;
            _startUtcDateTime = DateTime.UtcNow;
        }

        public void Start()
        {
            _endUtcDateTime = null;
            _isRunning = true;
            _startUtcDateTime = DateTime.UtcNow;
        }

        public static Stopwatch StartNew()
        {
            var w = new Stopwatch();
            w.Start();
            return w;
        }

        public void Stop()
        {
            _endUtcDateTime = DateTime.UtcNow;
            _isRunning = false;
        }
    }
#endif
}