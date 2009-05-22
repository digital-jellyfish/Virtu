using System;
using Jellyfish.Virtu.Properties;
using Jellyfish.Virtu.Services;
using Jellyfish.Virtu.Settings;

namespace Jellyfish.Virtu
{
    public sealed partial class Video : MachineComponent
    {
        public Video(Machine machine) : 
            base(machine)
        {
            _flushRowEvent = FlushRowEvent; // cache delegates; avoids garbage
            _inverseTextEvent = InverseTextEvent;
            _leaveVBlankEvent = LeaveVBlankEvent;
            _resetVSyncEvent = ResetVSyncEvent;

            FlushRowMode = new Action<int>[ModeCount]
            {
                FlushRowMode0, FlushRowMode1, FlushRowMode2, FlushRowMode3, FlushRowMode4, FlushRowMode5, FlushRowMode6, FlushRowMode7, 
                FlushRowMode8, FlushRowMode9, FlushRowModeA, FlushRowModeB, FlushRowModeC, FlushRowModeD, FlushRowModeE, FlushRowModeF
            };
        }

        public override void Initialize()
        {
            _memory = Machine.Memory;
            _videoService = Machine.Services.GetService<VideoService>();

            UpdateSettings();
            IsVBlank = true;

            Machine.Events.AddEvent((CyclesPerVBlank / 2), _leaveVBlankEvent);
            Machine.Events.AddEvent(CyclesPerVSync, _resetVSyncEvent);
            Machine.Events.AddEvent(CyclesPerFlash, _inverseTextEvent);
        }

        public override void Reset()
        {
            SetCharSet();
            DirtyScreen();
            FlushScreen();
        }

        public void DirtyCell(int addressOffset)
        {
            _isCellDirty[CellIndex[addressOffset]] = true;
        }

        public void DirtyCellMixed(int addressOffset)
        {
            int cellIndex = CellIndex[addressOffset];
            if (cellIndex < MixedCellIndex)
            {
                _isCellDirty[cellIndex] = true;
            }
        }

        public void DirtyCellMixedText(int addressOffset)
        {
            int cellIndex = CellIndex[addressOffset];
            if (cellIndex >= MixedCellIndex)
            {
                _isCellDirty[cellIndex] = true;
            }
        }

        public void DirtyScreen()
        {
            for (int i = 0; i < Height * CellColumns; i++)
            {
                _isCellDirty[i] = true;
            }
        }

        public void DirtyScreenText()
        {
            if (_memory.IsText)
            {
                for (int i = 0; i < MixedHeight * CellColumns; i++)
                {
                    _isCellDirty[i] = true;
                }
            }
            if (_memory.IsText || _memory.IsMixed)
            {
                for (int i = MixedHeight * CellColumns; i < Height * CellColumns; i++)
                {
                    _isCellDirty[i] = true;
                }
            }
        }

        public int ReadFloatingBus()
        {
            // TODO
            return 0x00;
        }

        public void SetCharSet()
        {
            _charSet = !_memory.IsCharSetAlternate ? CharSetPrimary : (_memory.Monitor == MonitorType.Standard) ? CharSetSecondaryStandard : CharSetSecondaryEnhanced;
            DirtyScreenText();
        }

        public void ToggleFullScreen()
        {
            Machine.Settings.Video.IsFullScreen ^= true;
            _videoService.ToggleFullScreen();
            DirtyScreen();
            FlushScreen();
        }

        public void ToggleMonochrome()
        {
            Machine.Settings.Video.IsMonochrome ^= true;
            DirtyScreen();
            FlushScreen();
        }

        #region Draw Methods
        private void DrawText40(int data, int x, int y)
        {
            int color = Machine.Settings.Video.IsMonochrome ? ColorMono00 : ColorWhite00;
            int index = _charSet[data] * CharBitmapBytes;
            int inverseMask = (_isTextInversed && !_memory.IsCharSetAlternate && (0x40 <= data) && (data <= 0x7F)) ? 0x7F : 0x00;
            for (int i = 0; i < TextHeight; i++, y++)
            {
                data = CharBitmap[index + i] ^ inverseMask;
                SetPixel(x + 0, y, color | (data & 0x01));
                SetPixel(x + 1, y, color | (data & 0x01));
                SetPixel(x + 2, y, color | (data & 0x02));
                SetPixel(x + 3, y, color | (data & 0x02));
                SetPixel(x + 4, y, color | (data & 0x04));
                SetPixel(x + 5, y, color | (data & 0x04));
                SetPixel(x + 6, y, color | (data & 0x08));
                SetPixel(x + 7, y, color | (data & 0x08));
                SetPixel(x + 8, y, color | (data & 0x10));
                SetPixel(x + 9, y, color | (data & 0x10));
                SetPixel(x + 10, y, color | (data & 0x20));
                SetPixel(x + 11, y, color | (data & 0x20));
                SetPixel(x + 12, y, color | (data & 0x40));
                SetPixel(x + 13, y, color | (data & 0x40));
            }
        }

        private void DrawText80(int data, int x, int y)
        {
            int color = Machine.Settings.Video.IsMonochrome ? ColorMono00 : ColorWhite00;
            int index = _charSet[data] * CharBitmapBytes;
            int mask = (_isTextInversed && !_memory.IsCharSetAlternate && (0x40 <= data) && (data <= 0x7F)) ? 0x7F : 0x00;
            for (int i = 0; i < TextHeight; i++, y++)
            {
                data = CharBitmap[index + i] ^ mask;
                SetPixel(x + 0, y, color | (data & 0x01));
                SetPixel(x + 1, y, color | (data & 0x02));
                SetPixel(x + 2, y, color | (data & 0x04));
                SetPixel(x + 3, y, color | (data & 0x08));
                SetPixel(x + 4, y, color | (data & 0x10));
                SetPixel(x + 5, y, color | (data & 0x20));
                SetPixel(x + 6, y, color | (data & 0x40));
            }
        }

        private void DrawLores(int data, int x, int y)
        {
            if (Machine.Settings.Video.IsMonochrome)
            {
                if ((x & 0x02) == 0x02) // odd cell
                {
                    data = ((data << 2) & 0xCC) | ((data >> 2) & 0x33);
                }
                for (int i = 0; i < LoresHeight; i++, y++)
                {
                    SetPixel(x + 0, y, data & 0x01);
                    SetPixel(x + 1, y, data & 0x02);
                    SetPixel(x + 2, y, data & 0x04);
                    SetPixel(x + 3, y, data & 0x08);
                    SetPixel(x + 4, y, data & 0x01);
                    SetPixel(x + 5, y, data & 0x02);
                    SetPixel(x + 6, y, data & 0x04);
                    SetPixel(x + 7, y, data & 0x08);
                    SetPixel(x + 8, y, data & 0x01);
                    SetPixel(x + 9, y, data & 0x02);
                    SetPixel(x + 10, y, data & 0x04);
                    SetPixel(x + 11, y, data & 0x08);
                    SetPixel(x + 12, y, data & 0x01);
                    SetPixel(x + 13, y, data & 0x02);
                }
                for (int i = 0; i < LoresHeight; i++, y++)
                {
                    SetPixel(x + 0, y, data & 0x10);
                    SetPixel(x + 1, y, data & 0x20);
                    SetPixel(x + 2, y, data & 0x40);
                    SetPixel(x + 3, y, data & 0x80);
                    SetPixel(x + 4, y, data & 0x10);
                    SetPixel(x + 5, y, data & 0x20);
                    SetPixel(x + 6, y, data & 0x40);
                    SetPixel(x + 7, y, data & 0x80);
                    SetPixel(x + 8, y, data & 0x10);
                    SetPixel(x + 9, y, data & 0x20);
                    SetPixel(x + 10, y, data & 0x40);
                    SetPixel(x + 11, y, data & 0x80);
                    SetPixel(x + 12, y, data & 0x10);
                    SetPixel(x + 13, y, data & 0x20);
                }
            }
            else
            {
                int color = ColorLores[data & 0x0F];
                for (int i = 0; i < LoresHeight; i++, y++)
                {
                    SetPixel(x + 0, y, color);
                    SetPixel(x + 1, y, color);
                    SetPixel(x + 2, y, color);
                    SetPixel(x + 3, y, color);
                    SetPixel(x + 4, y, color);
                    SetPixel(x + 5, y, color);
                    SetPixel(x + 6, y, color);
                    SetPixel(x + 7, y, color);
                    SetPixel(x + 8, y, color);
                    SetPixel(x + 9, y, color);
                    SetPixel(x + 10, y, color);
                    SetPixel(x + 11, y, color);
                    SetPixel(x + 12, y, color);
                    SetPixel(x + 13, y, color);
                }
                color = ColorLores[data >> 4];
                for (int i = 0; i < LoresHeight; i++, y++)
                {
                    SetPixel(x + 0, y, color);
                    SetPixel(x + 1, y, color);
                    SetPixel(x + 2, y, color);
                    SetPixel(x + 3, y, color);
                    SetPixel(x + 4, y, color);
                    SetPixel(x + 5, y, color);
                    SetPixel(x + 6, y, color);
                    SetPixel(x + 7, y, color);
                    SetPixel(x + 8, y, color);
                    SetPixel(x + 9, y, color);
                    SetPixel(x + 10, y, color);
                    SetPixel(x + 11, y, color);
                    SetPixel(x + 12, y, color);
                    SetPixel(x + 13, y, color);
                }
            }
        }

        private void Draw7MLores(int data, int x, int y)
        {
            if (Machine.Settings.Video.IsMonochrome)
            {
                if ((x & 0x02) == 0x02) // odd cell
                {
                    data = ((data << 2) & 0xCC) | ((data >> 2) & 0x33);
                }
                for (int i = 0; i < LoresHeight; i++, y++)
                {
                    SetPixel(x + 0, y, data & 0x01);
                    SetPixel(x + 1, y, data & 0x01);
                    SetPixel(x + 2, y, data & 0x02);
                    SetPixel(x + 3, y, data & 0x02);
                    SetPixel(x + 4, y, data & 0x04);
                    SetPixel(x + 5, y, data & 0x04);
                    SetPixel(x + 6, y, data & 0x08);
                    SetPixel(x + 7, y, data & 0x08);
                    SetPixel(x + 8, y, data & 0x01);
                    SetPixel(x + 9, y, data & 0x01);
                    SetPixel(x + 10, y, data & 0x02);
                    SetPixel(x + 11, y, data & 0x02);
                    SetPixel(x + 12, y, data & 0x04);
                    SetPixel(x + 13, y, data & 0x04);
                }
                for (int i = 0; i < LoresHeight; i++, y++)
                {
                    SetPixel(x + 0, y, data & 0x10);
                    SetPixel(x + 1, y, data & 0x10);
                    SetPixel(x + 2, y, data & 0x20);
                    SetPixel(x + 3, y, data & 0x20);
                    SetPixel(x + 4, y, data & 0x40);
                    SetPixel(x + 5, y, data & 0x40);
                    SetPixel(x + 6, y, data & 0x80);
                    SetPixel(x + 7, y, data & 0x80);
                    SetPixel(x + 8, y, data & 0x10);
                    SetPixel(x + 9, y, data & 0x10);
                    SetPixel(x + 10, y, data & 0x20);
                    SetPixel(x + 11, y, data & 0x20);
                    SetPixel(x + 12, y, data & 0x40);
                    SetPixel(x + 13, y, data & 0x40);
                }
            }
            else
            {
                int color = Color7MLores[((x & 0x02) << 3) | (data & 0x0F)];
                for (int i = 0; i < LoresHeight; i++, y++)
                {
                    SetPixel(x + 0, y, color);
                    SetPixel(x + 1, y, color);
                    SetPixel(x + 2, y, color);
                    SetPixel(x + 3, y, color);
                    SetPixel(x + 4, y, color);
                    SetPixel(x + 5, y, color);
                    SetPixel(x + 6, y, color);
                    SetPixel(x + 7, y, color);
                    SetPixel(x + 8, y, color);
                    SetPixel(x + 9, y, color);
                    SetPixel(x + 10, y, color);
                    SetPixel(x + 11, y, color);
                    SetPixel(x + 12, y, color);
                    SetPixel(x + 13, y, color);
                }
                color = Color7MLores[((x & 0x02) << 3) | (data >> 4)];
                for (int i = 0; i < LoresHeight; i++, y++)
                {
                    SetPixel(x + 0, y, color);
                    SetPixel(x + 1, y, color);
                    SetPixel(x + 2, y, color);
                    SetPixel(x + 3, y, color);
                    SetPixel(x + 4, y, color);
                    SetPixel(x + 5, y, color);
                    SetPixel(x + 6, y, color);
                    SetPixel(x + 7, y, color);
                    SetPixel(x + 8, y, color);
                    SetPixel(x + 9, y, color);
                    SetPixel(x + 10, y, color);
                    SetPixel(x + 11, y, color);
                    SetPixel(x + 12, y, color);
                    SetPixel(x + 13, y, color);
                }
            }
        }

        private void DrawDLores(int data, int x, int y)
        {
            if (Machine.Settings.Video.IsMonochrome)
            {
                if ((x & 0x01) == 0x00) // even half cell
                {
                    data = ((data << 1) & 0xEE) | ((data >> 3) & 0x11);
                }
                for (int i = 0; i < LoresHeight; i++, y++)
                {
                    SetPixel(x + 0, y, data & 0x01);
                    SetPixel(x + 1, y, data & 0x02);
                    SetPixel(x + 2, y, data & 0x04);
                    SetPixel(x + 3, y, data & 0x08);
                    SetPixel(x + 4, y, data & 0x01);
                    SetPixel(x + 5, y, data & 0x02);
                    SetPixel(x + 6, y, data & 0x04);
                }
                for (int i = 0; i < LoresHeight; i++, y++)
                {
                    SetPixel(x + 0, y, data & 0x10);
                    SetPixel(x + 1, y, data & 0x20);
                    SetPixel(x + 2, y, data & 0x40);
                    SetPixel(x + 3, y, data & 0x80);
                    SetPixel(x + 4, y, data & 0x10);
                    SetPixel(x + 5, y, data & 0x20);
                    SetPixel(x + 6, y, data & 0x40);
                }
            }
            else
            {
                int color = ColorDLores[((x & 0x01) << 4) | (data & 0x0F)];
                for (int i = 0; i < LoresHeight; i++, y++)
                {
                    SetPixel(x + 0, y, color);
                    SetPixel(x + 1, y, color);
                    SetPixel(x + 2, y, color);
                    SetPixel(x + 3, y, color);
                    SetPixel(x + 4, y, color);
                    SetPixel(x + 5, y, color);
                    SetPixel(x + 6, y, color);
                }
                color = ColorDLores[((x & 0x01) << 4) | (data >> 4)];
                for (int i = 0; i < LoresHeight; i++, y++)
                {
                    SetPixel(x + 0, y, color);
                    SetPixel(x + 1, y, color);
                    SetPixel(x + 2, y, color);
                    SetPixel(x + 3, y, color);
                    SetPixel(x + 4, y, color);
                    SetPixel(x + 5, y, color);
                    SetPixel(x + 6, y, color);
                }
            }
        }

        private void DrawHires(int address, int x, int y)
        {
            if (Machine.Settings.Video.IsMonochrome)
            {
                int data = _memory.ReadRamMainRegion02BF(address);
                SetPixel(x + 0, y, data & 0x01);
                SetPixel(x + 1, y, data & 0x01);
                SetPixel(x + 2, y, data & 0x02);
                SetPixel(x + 3, y, data & 0x02);
                SetPixel(x + 4, y, data & 0x04);
                SetPixel(x + 5, y, data & 0x04);
                SetPixel(x + 6, y, data & 0x08);
                SetPixel(x + 7, y, data & 0x08);
                SetPixel(x + 8, y, data & 0x10);
                SetPixel(x + 9, y, data & 0x10);
                SetPixel(x + 10, y, data & 0x20);
                SetPixel(x + 11, y, data & 0x20);
                SetPixel(x + 12, y, data & 0x40);
                SetPixel(x + 13, y, data & 0x40);
            }
            else
            {
                //   3                   2                   1                   0
                // 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 
                //
                //                 - - - - - - - - 0 0 0 0 0 0 0 0 + + + + + + + +
                //                 H           1 0 H 6 5 4 3 2 1 0 H           1 0

                int data = _memory.ReadRamMainRegion02BF(address) << 8;
                if (x < Width - CellWidth)
                {
                    data |= _memory.ReadRamMainRegion02BF(address + 1);
                    SetPixel(x + 14, y, ColorHires[((~x & 0x02) << 3) | ((data >> 4) & 0x08) | ((data << 1) & 0x06) | ((data >> 14) & 0x01)]);
                    SetPixel(x + 15, y, ColorHires[((~x & 0x02) << 3) | ((data >> 4) & 0x08) | ((data << 1) & 0x06) | ((data >> 14) & 0x01)]);
                }
                if (x > 0)
                {
                    data |= _memory.ReadRamMainRegion02BF(address - 1) << 16;
                    SetPixel(x - 2, y, ColorHires[((~x & 0x02) << 3) | ((data >> 20) & 0x08) | ((data >> 6) & 0x04) | ((data >> 21) & 0x03)]);
                    SetPixel(x - 1, y, ColorHires[((~x & 0x02) << 3) | ((data >> 20) & 0x08) | ((data >> 6) & 0x04) | ((data >> 21) & 0x03)]);
                }
                SetPixel(x + 0, y, ColorHires[((x & 0x02) << 3) | ((data >> 12) & 0x08) | ((data >> 7) & 0x06) | ((data >> 22) & 0x01)]);
                SetPixel(x + 1, y, ColorHires[((x & 0x02) << 3) | ((data >> 12) & 0x08) | ((data >> 7) & 0x06) | ((data >> 22) & 0x01)]);
                SetPixel(x + 2, y, ColorHires[((~x & 0x02) << 3) | ((data >> 12) & 0x08) | ((data >> 8) & 0x07)]);
                SetPixel(x + 3, y, ColorHires[((~x & 0x02) << 3) | ((data >> 12) & 0x08) | ((data >> 8) & 0x07)]);
                SetPixel(x + 4, y, ColorHires[((x & 0x02) << 3) | ((data >> 12) & 0x08) | ((data >> 9) & 0x07)]);
                SetPixel(x + 5, y, ColorHires[((x & 0x02) << 3) | ((data >> 12) & 0x08) | ((data >> 9) & 0x07)]);
                SetPixel(x + 6, y, ColorHires[((~x & 0x02) << 3) | ((data >> 12) & 0x08) | ((data >> 10) & 0x07)]);
                SetPixel(x + 7, y, ColorHires[((~x & 0x02) << 3) | ((data >> 12) & 0x08) | ((data >> 10) & 0x07)]);
                SetPixel(x + 8, y, ColorHires[((x & 0x02) << 3) | ((data >> 12) & 0x08) | ((data >> 11) & 0x07)]);
                SetPixel(x + 9, y, ColorHires[((x & 0x02) << 3) | ((data >> 12) & 0x08) | ((data >> 11) & 0x07)]);
                SetPixel(x + 10, y, ColorHires[((~x & 0x02) << 3) | ((data >> 12) & 0x0F)]);
                SetPixel(x + 11, y, ColorHires[((~x & 0x02) << 3) | ((data >> 12) & 0x0F)]);
                SetPixel(x + 12, y, ColorHires[((x & 0x02) << 3) | ((data >> 12) & 0x08) | ((data << 2) & 0x04) | ((data >> 13) & 0x03)]);
                SetPixel(x + 13, y, ColorHires[((x & 0x02) << 3) | ((data >> 12) & 0x08) | ((data << 2) & 0x04) | ((data >> 13) & 0x03)]);
            }
        }

        private void DrawNDHires(int address, int x, int y)
        {
            if (Machine.Settings.Video.IsMonochrome)
            {
                int data = _memory.ReadRamMainRegion02BF(address);
                SetPixel(x + 0, y, data & 0x01);
                SetPixel(x + 1, y, data & 0x01);
                SetPixel(x + 2, y, data & 0x02);
                SetPixel(x + 3, y, data & 0x02);
                SetPixel(x + 4, y, data & 0x04);
                SetPixel(x + 5, y, data & 0x04);
                SetPixel(x + 6, y, data & 0x08);
                SetPixel(x + 7, y, data & 0x08);
                SetPixel(x + 8, y, data & 0x10);
                SetPixel(x + 9, y, data & 0x10);
                SetPixel(x + 10, y, data & 0x20);
                SetPixel(x + 11, y, data & 0x20);
                SetPixel(x + 12, y, data & 0x40);
                SetPixel(x + 13, y, data & 0x40);
            }
            else
            {
                //   3                   2                   1                   0
                // 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 
                //
                //                 - - - - - - - - 0 0 0 0 0 0 0 0 + + + + + + + +
                //                 X           1 0 X 6 5 4 3 2 1 0 X           1 0

                int data = _memory.ReadRamMainRegion02BF(address) << 8;
                if (x < Width - CellWidth)
                {
                    data |= _memory.ReadRamMainRegion02BF(address + 1);
                    SetPixel(x + 14, y, ColorHires[((~x & 0x02) << 3) | ((data << 1) & 0x06) | ((data >> 14) & 0x01)]);
                    SetPixel(x + 15, y, ColorHires[((~x & 0x02) << 3) | ((data << 1) & 0x06) | ((data >> 14) & 0x01)]);
                }
                if (x > 0)
                {
                    data |= _memory.ReadRamMainRegion02BF(address - 1) << 16;
                    SetPixel(x - 2, y, ColorHires[((~x & 0x02) << 3) | ((data >> 6) & 0x04) | ((data >> 21) & 0x03)]);
                    SetPixel(x - 1, y, ColorHires[((~x & 0x02) << 3) | ((data >> 6) & 0x04) | ((data >> 21) & 0x03)]);
                }
                SetPixel(x + 0, y, ColorHires[((x & 0x02) << 3) | ((data >> 7) & 0x06) | ((data >> 22) & 0x01)]);
                SetPixel(x + 1, y, ColorHires[((x & 0x02) << 3) | ((data >> 7) & 0x06) | ((data >> 22) & 0x01)]);
                SetPixel(x + 2, y, ColorHires[((~x & 0x02) << 3) | ((data >> 8) & 0x07)]);
                SetPixel(x + 3, y, ColorHires[((~x & 0x02) << 3) | ((data >> 8) & 0x07)]);
                SetPixel(x + 4, y, ColorHires[((x & 0x02) << 3) | ((data >> 9) & 0x07)]);
                SetPixel(x + 5, y, ColorHires[((x & 0x02) << 3) | ((data >> 9) & 0x07)]);
                SetPixel(x + 6, y, ColorHires[((~x & 0x02) << 3) | ((data >> 10) & 0x07)]);
                SetPixel(x + 7, y, ColorHires[((~x & 0x02) << 3) | ((data >> 10) & 0x07)]);
                SetPixel(x + 8, y, ColorHires[((x & 0x02) << 3) | ((data >> 11) & 0x07)]);
                SetPixel(x + 9, y, ColorHires[((x & 0x02) << 3) | ((data >> 11) & 0x07)]);
                SetPixel(x + 10, y, ColorHires[((~x & 0x02) << 3) | ((data >> 12) & 0x07)]);
                SetPixel(x + 11, y, ColorHires[((~x & 0x02) << 3) | ((data >> 12) & 0x07)]);
                SetPixel(x + 12, y, ColorHires[((x & 0x02) << 3) | ((data << 2) & 0x04) | ((data >> 13) & 0x03)]);
                SetPixel(x + 13, y, ColorHires[((x & 0x02) << 3) | ((data << 2) & 0x04) | ((data >> 13) & 0x03)]);
            }
        }

        private void DrawDHiresA(int address, int x, int y)
        {
            if (Machine.Settings.Video.IsMonochrome)
            {
                if ((x & 0x2) == 0x00) // even cell
                {
                    int data = ((_memory.ReadRamMainRegion02BF(address) << 7) & 0x80) | (_memory.ReadRamAuxRegion02BF(address) & 0x7F);
                    SetPixel(x + 0, y, data & 0x01);
                    SetPixel(x + 1, y, data & 0x02);
                    SetPixel(x + 2, y, data & 0x04);
                    SetPixel(x + 3, y, data & 0x08);
                    SetPixel(x + 4, y, data & 0x10);
                    SetPixel(x + 5, y, data & 0x20);
                    SetPixel(x + 6, y, data & 0x40);
                    SetPixel(x + 7, y, data & 0x80);
                }
                else
                {
                    int data = ((_memory.ReadRamMainRegion02BF(address) << 9) & 0xE00) | ((_memory.ReadRamAuxRegion02BF(address) << 2) & 0x1FC) |
                        ((_memory.ReadRamMainRegion02BF(address - 1) >> 5) & 0x003);
                    SetPixel(x - 2, y, data & 0x01);
                    SetPixel(x - 1, y, data & 0x02);
                    SetPixel(x + 0, y, data & 0x04);
                    SetPixel(x + 1, y, data & 0x08);
                    SetPixel(x + 2, y, data & 0x10);
                    SetPixel(x + 3, y, data & 0x20);
                    SetPixel(x + 4, y, data & 0x40);
                    SetPixel(x + 5, y, data & 0x80);
                    SetPixel(x + 6, y, (data >> 8) & 0x01);
                    SetPixel(x + 7, y, (data >> 8) & 0x02);
                    SetPixel(x + 8, y, (data >> 8) & 0x04);
                    SetPixel(x + 9, y, (data >> 8) & 0x08);
                }
            }
            else
            {
                if ((x & 0x2) == 0x00) // even cell
                {
                    int data = ((_memory.ReadRamMainRegion02BF(address) << 7) & 0x80) | (_memory.ReadRamAuxRegion02BF(address) & 0x7F);
                    SetPixel(x + 0, y, ColorDHires0 | (data & 0x0F));
                    SetPixel(x + 1, y, ColorDHires0 | (data & 0x0F));
                    SetPixel(x + 2, y, ColorDHires0 | (data & 0x0F));
                    SetPixel(x + 3, y, ColorDHires0 | (data & 0x0F));
                    SetPixel(x + 4, y, ColorDHires0 | (data >> 4));
                    SetPixel(x + 5, y, ColorDHires0 | (data >> 4));
                    SetPixel(x + 6, y, ColorDHires0 | (data >> 4));
                    SetPixel(x + 7, y, ColorDHires0 | (data >> 4));
                }
                else
                {
                    int data = ((_memory.ReadRamMainRegion02BF(address) << 9) & 0xE00) | ((_memory.ReadRamAuxRegion02BF(address) << 2) & 0x1FC) | 
                        ((_memory.ReadRamMainRegion02BF(address - 1) >> 5) & 0x003);
                    SetPixel(x - 2, y, ColorDHires0 | (data & 0x0F));
                    SetPixel(x - 1, y, ColorDHires0 | (data & 0x0F));
                    SetPixel(x + 0, y, ColorDHires0 | (data & 0x0F));
                    SetPixel(x + 1, y, ColorDHires0 | (data & 0x0F));
                    SetPixel(x + 2, y, ColorDHires0 | ((data >> 4) & 0x0F));
                    SetPixel(x + 3, y, ColorDHires0 | ((data >> 4) & 0x0F));
                    SetPixel(x + 4, y, ColorDHires0 | ((data >> 4) & 0x0F));
                    SetPixel(x + 5, y, ColorDHires0 | ((data >> 4) & 0x0F));
                    SetPixel(x + 6, y, ColorDHires0 | (data >> 8));
                    SetPixel(x + 7, y, ColorDHires0 | (data >> 8));
                    SetPixel(x + 8, y, ColorDHires0 | (data >> 8));
                    SetPixel(x + 9, y, ColorDHires0 | (data >> 8));
                }
            }
        }

        private void DrawDHiresM(int address, int x, int y)
        {
            if (Machine.Settings.Video.IsMonochrome)
            {
                if ((x & 0x2) == 0x02) // odd cell
                {
                    int data = ((_memory.ReadRamMainRegion02BF(address) << 1) & 0xFE) | ((_memory.ReadRamAuxRegion02BF(address) >> 6) & 0x01);
                    SetPixel(x + 6, y, data & 0x01);
                    SetPixel(x + 7, y, data & 0x02);
                    SetPixel(x + 8, y, data & 0x04);
                    SetPixel(x + 9, y, data & 0x08);
                    SetPixel(x + 10, y, data & 0x10);
                    SetPixel(x + 11, y, data & 0x20);
                    SetPixel(x + 12, y, data & 0x40);
                    SetPixel(x + 13, y, data & 0x80);
                }
                else
                {
                    int data = ((_memory.ReadRamAuxRegion02BF(address + 1) << 10) & 0xC00) | ((_memory.ReadRamMainRegion02BF(address) << 3) & 0x3F8) |
                        ((_memory.ReadRamAuxRegion02BF(address) >> 4) & 0x007);
                    SetPixel(x + 4, y, data & 0x01);
                    SetPixel(x + 5, y, data & 0x02);
                    SetPixel(x + 6, y, data & 0x04);
                    SetPixel(x + 7, y, data & 0x08);
                    SetPixel(x + 8, y, data & 0x10);
                    SetPixel(x + 9, y, data & 0x20);
                    SetPixel(x + 10, y, data & 0x40);
                    SetPixel(x + 11, y, data & 0x80);
                    SetPixel(x + 12, y, (data >> 8) & 0x01);
                    SetPixel(x + 13, y, (data >> 8) & 0x02);
                    SetPixel(x + 14, y, (data >> 8) & 0x04);
                    SetPixel(x + 15, y, (data >> 8) & 0x08);
                }
            }
            else
            {
                if ((x & 0x2) == 0x02) // odd cell
                {
                    int data = ((_memory.ReadRamMainRegion02BF(address) << 1) & 0xFE) | ((_memory.ReadRamAuxRegion02BF(address) >> 6) & 0x01);
                    SetPixel(x + 6, y, ColorDHires0 | (data & 0x0F));
                    SetPixel(x + 7, y, ColorDHires0 | (data & 0x0F));
                    SetPixel(x + 8, y, ColorDHires0 | (data & 0x0F));
                    SetPixel(x + 9, y, ColorDHires0 | (data & 0x0F));
                    SetPixel(x + 10, y, ColorDHires0 | (data >> 4));
                    SetPixel(x + 11, y, ColorDHires0 | (data >> 4));
                    SetPixel(x + 12, y, ColorDHires0 | (data >> 4));
                    SetPixel(x + 13, y, ColorDHires0 | (data >> 4));
                }
                else
                {
                    int data = ((_memory.ReadRamAuxRegion02BF(address + 1) << 10) & 0xC00) | ((_memory.ReadRamMainRegion02BF(address) << 3) & 0x3F8) |
                        ((_memory.ReadRamAuxRegion02BF(address) >> 4) & 0x007);
                    SetPixel(x + 4, y, ColorDHires0 | (data & 0x0F));
                    SetPixel(x + 5, y, ColorDHires0 | (data & 0x0F));
                    SetPixel(x + 6, y, ColorDHires0 | (data & 0x0F));
                    SetPixel(x + 7, y, ColorDHires0 | (data & 0x0F));
                    SetPixel(x + 8, y, ColorDHires0 | ((data >> 4) & 0x0F));
                    SetPixel(x + 9, y, ColorDHires0 | ((data >> 4) & 0x0F));
                    SetPixel(x + 10, y, ColorDHires0 | ((data >> 4) & 0x0F));
                    SetPixel(x + 11, y, ColorDHires0 | ((data >> 4) & 0x0F));
                    SetPixel(x + 12, y, ColorDHires0 | (data >> 8));
                    SetPixel(x + 13, y, ColorDHires0 | (data >> 8));
                    SetPixel(x + 14, y, ColorDHires0 | (data >> 8));
                    SetPixel(x + 15, y, ColorDHires0 | (data >> 8));
                }
            }
        }
        #endregion

        #region Flush Methods
        private void FlushRowMode0(int y)
        {
            int address = (!_memory.IsVideoPage2 ? 0x0400 : 0x0800) + AddressOffset[y];
            for (int x = 0; x < CellColumns; x++)
            {
                if (_isCellDirty[CellColumns * y + x])
                {
                    _isCellDirty[CellColumns * y + x] = false;
                    DrawLores(_memory.ReadRamMainRegion02BF(address + x), CellWidth * x, y); // lores
                }
            }
        }

        private void FlushRowMode1(int y)
        {
            int address = (!_memory.IsVideoPage2 ? 0x0400 : 0x0800) + AddressOffset[y];
            for (int x = 0; x < CellColumns; x++)
            {
                if (_isCellDirty[CellColumns * y + x])
                {
                    _isCellDirty[CellColumns * y + x] = false;
                    DrawText40(_memory.ReadRamMainRegion02BF(address + x), CellWidth * x, y); // text40
                }
            }
        }

        private void FlushRowMode2(int y)
        {
            int address = (!_memory.IsVideoPage2 ? 0x0400 : 0x0800) + AddressOffset[y];
            for (int x = 0; x < 2 * CellColumns; x += 2)
            {
                if (_isCellDirty[CellColumns * y + x / 2])
                {
                    _isCellDirty[CellColumns * y + x / 2] = false;
                    DrawText80(_memory.ReadRamAuxRegion02BF(address + x / 2), CellWidth / 2 * (x + 0), y); // text80
                    DrawText80(_memory.ReadRamMainRegion02BF(address + x / 2), CellWidth / 2 * (x + 1), y);
                }
            }
        }

        private void FlushRowMode3(int y)
        {
            if (y < MixedHeight)
            {
                FlushRowMode0(y); // lores
            }
            else
            {
                FlushRowMode1(y); // text40
            }
        }

        private void FlushRowMode4(int y)
        {
            if (y < MixedHeight)
            {
                FlushRowMode0(y); // lores
            }
            else
            {
                FlushRowMode2(y); // text80
            }
        }

        private void FlushRowMode5(int y)
        {
            int address = !_memory.IsVideoPage2 ? 0x2000 : 0x4000;
            for (int i = 0; i < CellHeight; i++, y++)
            {
                for (int x = 0; x < CellColumns; x++)
                {
                    if (_isCellDirty[CellColumns * y + x])
                    {
                        _isCellDirty[CellColumns * y + x] = false;
                        DrawHires(address + AddressOffset[y] + x, CellWidth * x, y); // hires
                    }
                }
            }
        }

        private void FlushRowMode6(int y)
        {
            if (y < MixedHeight)
            {
                FlushRowMode5(y); // hires
            }
            else
            {
                FlushRowMode1(y); // text40
            }
        }

        private void FlushRowMode7(int y)
        {
            if (y < MixedHeight)
            {
                FlushRowMode5(y); // hires
            }
            else
            {
                FlushRowMode2(y); // text80
            }
        }

        private void FlushRowMode8(int y)
        {
            int address = (!_memory.IsVideoPage2 ? 0x0400 : 0x0800) + AddressOffset[y];
            for (int x = 0; x < CellColumns; x++)
            {
                if (_isCellDirty[CellColumns * y + x])
                {
                    _isCellDirty[CellColumns * y + x] = false;
                    Draw7MLores(_memory.ReadRamMainRegion02BF(address + x), CellWidth * x, y); // 7mlores
                }
            }
        }

        private void FlushRowMode9(int y)
        {
            int address = (!_memory.IsVideoPage2 ? 0x0400 : 0x0800) + AddressOffset[y];
            for (int x = 0; x < 2 * CellColumns; x += 2)
            {
                if (_isCellDirty[CellColumns * y + x / 2])
                {
                    _isCellDirty[CellColumns * y + x / 2] = false;
                    DrawDLores(_memory.ReadRamAuxRegion02BF(address + x / 2), CellWidth / 2 * (x + 0), y); // dlores
                    DrawDLores(_memory.ReadRamMainRegion02BF(address + x / 2), CellWidth / 2 * (x + 1), y);
                }
            }
        }

        private void FlushRowModeA(int y)
        {
            if (y < MixedHeight)
            {
                FlushRowMode8(y); // 7mlores
            }
            else
            {
                FlushRowMode1(y); // text40
            }
        }

        private void FlushRowModeB(int y)
        {
            if (y < MixedHeight)
            {
                FlushRowMode9(y); // dlores
            }
            else
            {
                FlushRowMode2(y); // text80
            }
        }

        private void FlushRowModeC(int y)
        {
            int address = !_memory.IsVideoPage2 ? 0x2000 : 0x4000;
            for (int i = 0; i < CellHeight; i++, y++)
            {
                for (int x = 0; x < CellColumns; x++)
                {
                    if (_isCellDirty[CellColumns * y + x])
                    {
                        _isCellDirty[CellColumns * y + x] = false;
                        DrawNDHires(address + AddressOffset[y] + x, CellWidth * x, y); // ndhires
                    }
                }
            }
        }

        private void FlushRowModeD(int y)
        {
            int address = !_memory.IsVideoPage2 ? 0x2000 : 0x4000;
            for (int i = 0; i < CellHeight; i++, y++)
            {
                for (int x = 0; x < CellColumns; x++)
                {
                    if (_isCellDirty[CellColumns * y + x])
                    {
                        _isCellDirty[CellColumns * y + x] = false;
                        DrawDHiresA(address + AddressOffset[y] + x, CellWidth * x, y); // dhires
                        DrawDHiresM(address + AddressOffset[y] + x, CellWidth * x, y);
                    }
                }
            }
        }

        private void FlushRowModeE(int y)
        {
            if (y < MixedHeight)
            {
                FlushRowModeC(y); // ndhires
            }
            else
            {
                FlushRowMode1(y); // text40
            }
        }

        private void FlushRowModeF(int y)
        {
            if (y < MixedHeight)
            {
                FlushRowModeD(y); // dhires
            }
            else
            {
                FlushRowMode2(y); // text80
            }
        }
        #endregion

        private void FlushRowEvent()
        {
            int y = (CyclesPerVSync - (CyclesPerVBlank / 2) - Machine.Events.FindEvent(_resetVSyncEvent)) / CyclesPerHSync;

            FlushRowMode[_memory.VideoMode](y - CellHeight); // in arrears

            if (y < Height)
            {
                Machine.Events.AddEvent(CyclesPerFlush, _flushRowEvent);
            }
            else
            {
                IsVBlank = true;

                Machine.Events.AddEvent(CyclesPerVBlank, _leaveVBlankEvent);
            }
        }

        private void FlushScreen()
        {
            Action<int> flushRowMode = FlushRowMode[_memory.VideoMode];

            for (int y = 0; y < Height; y += CellHeight)
            {
                flushRowMode(y);
            }
        }

        private void InverseTextEvent()
        {
            _isTextInversed = !_isTextInversed;

            DirtyScreenText();

            Machine.Events.AddEvent(CyclesPerFlash, _inverseTextEvent);
        }

        private void LeaveVBlankEvent()
        {
            IsVBlank = false;

            Machine.Events.AddEvent(CyclesPerFlush, _flushRowEvent);
        }

        private void ResetVSyncEvent()
        {
            UpdateSettings();

            Machine.Events.AddEvent(CyclesPerVSync, _resetVSyncEvent);
        }

        private void SetPixel(int x, int y, int color)
        {
            _videoService.SetPixel(x, 2 * y, _colorPalette[color]);
        }

        private void UpdateSettings()
        {
            VideoSettings settings = Machine.Settings.Video;

            _colorPalette[ColorMono00] = settings.Color.Black;
            _colorPalette[ColorMono01] = settings.Color.Monochrome;
            _colorPalette[ColorMono02] = settings.Color.Monochrome;
            _colorPalette[ColorMono04] = settings.Color.Monochrome;
            _colorPalette[ColorMono08] = settings.Color.Monochrome;
            _colorPalette[ColorMono10] = settings.Color.Monochrome;
            _colorPalette[ColorMono20] = settings.Color.Monochrome;
            _colorPalette[ColorMono40] = settings.Color.Monochrome;
            _colorPalette[ColorMono80] = settings.Color.Monochrome;

            _colorPalette[ColorWhite00] = settings.Color.Black;
            _colorPalette[ColorWhite01] = settings.Color.White;
            _colorPalette[ColorWhite02] = settings.Color.White;
            _colorPalette[ColorWhite04] = settings.Color.White;
            _colorPalette[ColorWhite08] = settings.Color.White;
            _colorPalette[ColorWhite10] = settings.Color.White;
            _colorPalette[ColorWhite20] = settings.Color.White;
            _colorPalette[ColorWhite40] = settings.Color.White;
            _colorPalette[ColorWhite80] = settings.Color.White;

            _colorPalette[ColorDHires0] = settings.Color.Black;
            _colorPalette[ColorDHires1] = settings.Color.DarkBlue;
            _colorPalette[ColorDHires2] = settings.Color.DarkGreen;
            _colorPalette[ColorDHires3] = settings.Color.MediumBlue;
            _colorPalette[ColorDHires4] = settings.Color.Brown;
            _colorPalette[ColorDHires5] = settings.Color.LightGrey;
            _colorPalette[ColorDHires6] = settings.Color.Green;
            _colorPalette[ColorDHires7] = settings.Color.Aquamarine;
            _colorPalette[ColorDHires8] = settings.Color.DeepRed;
            _colorPalette[ColorDHires9] = settings.Color.Purple;
            _colorPalette[ColorDHiresA] = settings.Color.DarkGrey;
            _colorPalette[ColorDHiresB] = settings.Color.LightBlue;
            _colorPalette[ColorDHiresC] = settings.Color.Orange;
            _colorPalette[ColorDHiresD] = settings.Color.Pink;
            _colorPalette[ColorDHiresE] = settings.Color.Yellow;
            _colorPalette[ColorDHiresF] = settings.Color.White;

            if (_videoService.IsFullScreen != settings.IsFullScreen)
            {
                _videoService.ToggleFullScreen();
            }
        }

        public bool IsVBlank { get; private set; }

        private Action _flushRowEvent;
        private Action _inverseTextEvent;
        private Action _leaveVBlankEvent;
        private Action _resetVSyncEvent;

        private Memory _memory;
        private VideoService _videoService;

        private ushort[] _charSet;
        private uint[] _colorPalette = new uint[ColorPaletteCount];
        private bool[] _isCellDirty = new bool[Height * CellColumns + 1]; // includes sentinel
        private bool _isTextInversed;
    }
}
