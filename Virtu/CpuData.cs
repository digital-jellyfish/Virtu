﻿using System;

namespace Jellyfish.Virtu
{
    public partial class Cpu
    {
        private const int OpCodeCount = 256;

        private readonly Action[] ExecuteOpCode65N02;
        private readonly Action[] ExecuteOpCode65C02;

        private const int PC = 0x01;
        private const int PZ = 0x02;
        private const int PI = 0x04;
        private const int PD = 0x08;
        private const int PB = 0x10;
        private const int PR = 0x20;
        private const int PV = 0x40;
        private const int PN = 0x80;

        private const int DataCount = 256;

        private static readonly int[] DataPN = new int[DataCount]
        {
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN, 
            PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN, 
            PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN, 
            PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN, 
            PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN, 
            PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN, 
            PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN, 
            PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN
        };

        private static readonly int[] DataPZ = new int[DataCount]
        {
            PZ,  0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0
        };

        private static readonly int[] DataPNZ = new int[DataCount]
        {
            PZ,  0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN, 
            PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN, 
            PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN, 
            PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN, 
            PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN, 
            PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN, 
            PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN, 
            PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN,  PN
        };
    }
}
