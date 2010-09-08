using System;
using System.Collections;

namespace Jellyfish.Virtu
{
    public sealed class NoSlotClock
    {
        public NoSlotClock()
        {
            Reset();
        }

        public void Reset()
        {
            // SmartWatch reset - whether tied to system reset is component specific
            _comparisonRegister.Reset();
            _clockRegisterEnabled = false;
            _writeEnabled = true;
        }

        public int Read(int address, int data)
        {
            // this may read or write the clock
            if ((address & 0x4) != 0)
            {
                return ClockRead(data);
            }

            ClockWrite(address);
            return data;
        }

        public void Write(int address)
        {
            // this may read or write the clock
            if ((address & 0x4) != 0)
            {
                ClockRead(0);
            }
            else
            {
                ClockWrite(address);
            }
        }

        public int ClockRead(int data)
        {
            // for a ROM, A2 high = read, and data out (if any) is on D0
            if (!_clockRegisterEnabled)
            {
                _comparisonRegister.Reset();
                _writeEnabled = true;
                return data;
            }

            data = _clockRegister.ReadBit(data);
            if (_clockRegister.NextBit())
            {
                _clockRegisterEnabled = false;
            }
            return data;
        }

        public void ClockWrite(int address)
        {
            // for a ROM, A2 low = write, and data in is on A0
            if (!_writeEnabled)
            {
                return;
            }

            if (!_clockRegisterEnabled)
            {
                if ((_comparisonRegister.CompareBit(address)))
                {
                    if (_comparisonRegister.NextBit())
                    {
                        _clockRegisterEnabled = true;
                        PopulateClockRegister();
                    }
                }
                else
                {
                    // mismatch ignores further writes
                    _writeEnabled = false;
                }
            }
            else if (_clockRegister.NextBit())
            {
                // simulate writes, but our clock register is read-only
                _clockRegisterEnabled = false;
            }
        }

        private void PopulateClockRegister()
        {
            // all values are in packed BCD format (4 bits per decimal digit)
            var now = DateTime.Now;

            int centisecond = now.Millisecond / 10; // 00-99
            _clockRegister.WriteNibble(centisecond % 10);
            _clockRegister.WriteNibble(centisecond / 10);

            int second = now.Second; // 00-59
            _clockRegister.WriteNibble(second % 10);
            _clockRegister.WriteNibble(second / 10);

            int minute = now.Minute; // 00-59
            _clockRegister.WriteNibble(minute % 10);
            _clockRegister.WriteNibble(minute / 10);

            int hour = now.Hour; // 01-23
            _clockRegister.WriteNibble(hour % 10);
            _clockRegister.WriteNibble(hour / 10);

            int day = (int)now.DayOfWeek + 1; // 01-07 (1 = Sunday)
            _clockRegister.WriteNibble(day % 10);
            _clockRegister.WriteNibble(day / 10);

            int date = now.Day; // 01-31
            _clockRegister.WriteNibble(date % 10);
            _clockRegister.WriteNibble(date / 10);

            int month = now.Month; // 01-12
            _clockRegister.WriteNibble(month % 10);
            _clockRegister.WriteNibble(month / 10);

            int year = now.Year % 100; // 00-99
            _clockRegister.WriteNibble(year % 10);
            _clockRegister.WriteNibble(year / 10);
        }

        private const ulong ClockInitSequence = 0x5CA33AC55CA33AC5;

        private bool _clockRegisterEnabled;
        private bool _writeEnabled;
        private RingRegister _clockRegister = new RingRegister();
        private RingRegister _comparisonRegister = new RingRegister(ClockInitSequence);

        private sealed class RingRegister
        {
            public RingRegister(ulong data = 0)
            {
                _register = data;

                Reset();
            }

            public void Reset()
            {
                _mask = 0x1;
            }

            public void WriteNibble(int data)
            {
                WriteBits(data, 4);
            }

            public void WriteBits(int data, int count)
            {
                for (int i = 1; i <= count; i++)
                {
                    WriteBit(data);
                    NextBit();
                    data >>= 1;
                }
            }

            public void WriteBit(int data)
            {
                _register = ((data & 0x1) != 0) ? (_register | _mask) : (_register & ~_mask);
            }

            public int ReadBit(int data)
            {
                return ((_register & _mask) != 0) ? (data | 0x1) : (data & ~0x1);
            }

            public bool CompareBit(int data)
            {
                return (((_register & _mask) != 0) == ((data & 0x1) != 0));
            }

            public bool NextBit()
            {
                if ((_mask <<= 1) == 0)
                {
                    _mask = 0x1;
                    return true; // wrap
                }
                return false;
            }

            private ulong _mask;
            private ulong _register;
        }
    }
}
