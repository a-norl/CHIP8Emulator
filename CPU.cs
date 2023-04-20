using System.Collections;

namespace CHIP8Emulator;

public class CPU
{

    public byte[] RAM;
    public Stack<ushort> STACK;
    public byte DELAY_TIMER;
    public byte SOUND_TIMER;
    public ushort INDEX_REGISTER;
    public ushort PC;
    public Dictionary<byte, byte> VARIABLE_REGISTERS;
    public Dictionary<byte, bool> PRESSED_KEYS;
    private Screen _screen;
    private bool _superChipQuirk;
    private Random random;

    private byte[] _font = new byte[]
    {
        0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
        0x20, 0x60, 0x20, 0x20, 0x70, // 1
        0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
        0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
        0x90, 0x90, 0xF0, 0x10, 0x10, // 4
        0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
        0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
        0xF0, 0x10, 0x20, 0x40, 0x40, // 7
        0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
        0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
        0xF0, 0x90, 0xF0, 0x90, 0x90, // A
        0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
        0xF0, 0x80, 0x80, 0x80, 0xF0, // C
        0xE0, 0x90, 0x90, 0x90, 0xE0, // D
        0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
        0xF0, 0x80, 0xF0, 0x80, 0x80  // F
    };

    public CPU(byte[] rom, Screen screen, bool superChip = false)
    {
        _superChipQuirk = superChip;
        random = new();
        RAM = new Byte[4096];
        STACK = new();
        DELAY_TIMER = 0;
        SOUND_TIMER = 0;
        PC = 0x200;
        VARIABLE_REGISTERS = new()
        {
            {0x0, 0},
            {0x1, 0},
            {0x2, 0},
            {0x3, 0},
            {0x4, 0},
            {0x5, 0},
            {0x6, 0},
            {0x7, 0},
            {0x8, 0},
            {0x9, 0},
            {0xA, 0},
            {0xB, 0},
            {0xC, 0},
            {0xD, 0},
            {0xE, 0},
            {0xF, 0},
        };

        PRESSED_KEYS = new()
        {
            {0x0, false},
            {0x1, false},
            {0x2, false},
            {0x3, false},
            {0x4, false},
            {0x5, false},
            {0x6, false},
            {0x7, false},
            {0x8, false},
            {0x9, false},
            {0xA, false},
            {0xB, false},
            {0xC, false},
            {0xD, false},
            {0xE, false},
            {0xF, false},
        };
        _screen = screen;

        //Load font into RAM
        int fontLoadAddr = 0x050;
        foreach (byte fontByte in _font)
        {
            RAM[fontLoadAddr++] = fontByte;
        }

        int romLoadAddr = 0x200;
        foreach (byte romByte in rom)
        {
            RAM[romLoadAddr++] = romByte;
        }

        Console.WriteLine(BitConverter.ToString(RAM));
    }

    public void Tick()
    {
        while (true)
        {
            //Fetch

            var instrByteOne = RAM[PC++];
            var instrByteTwo = RAM[PC++]; //NN
            var firstNibble = instrByteOne >> 4;
            var secondNibble = instrByteOne & 0x0F; //X
            var thirdNibble = instrByteTwo >> 4; //Y
            var fourthNibble = instrByteTwo & 0x0F; //N
            var nibbleNNN = 0x000 | (secondNibble << 8) | (thirdNibble << 4) | fourthNibble;

            //Decode and Execute
            switch (firstNibble)
            {
                case 0x0:
                    switch (fourthNibble)
                    {
                        case 0x0: //00E0 - Clear screen
                            _screen.ClearScreen();
                            break;
                        case 0xE: //00EE - Subroutine return
                            PC = STACK.Pop();
                            break;
                    }
                    break;
                case 0x1: //1NNN - Jump
                    PC = (ushort)nibbleNNN;
                    break;
                case 0x2: //2NNN - Subroutine call
                    STACK.Push(PC);
                    PC = (ushort)nibbleNNN;
                    break;
                case 0x3: //3XNN - Skip if equal immediate
                    if (VARIABLE_REGISTERS[(byte)secondNibble] == instrByteTwo)
                    {
                        PC += 2;
                    }
                    break;
                case 0x4: //4XNN - Skip if not equal immediate
                    if (VARIABLE_REGISTERS[(byte)secondNibble] != instrByteTwo)
                    {
                        PC += 2;
                    }
                    break;
                case 0x5: //5XY0 - Skip if equal registers
                    if (VARIABLE_REGISTERS[(byte)secondNibble] == VARIABLE_REGISTERS[(byte)thirdNibble])
                    {
                        PC += 2;
                    }
                    break;
                case 0x6: //6XNN - Set immediate
                    VARIABLE_REGISTERS[(byte)secondNibble] = instrByteTwo;
                    break;
                case 0x7: //7XNN - Add immediate
                    VARIABLE_REGISTERS[(byte)secondNibble] += instrByteTwo;
                    break;
                case 0x8: //Arithmetic or Logical instructions
                    switch (fourthNibble)
                    {
                        case 0x0: //8XY0 - Set register
                            VARIABLE_REGISTERS[(byte)secondNibble] = VARIABLE_REGISTERS[(byte)thirdNibble];
                            break;
                        case 0x1: //8XY1 - Logical OR register
                            VARIABLE_REGISTERS[(byte)secondNibble] = (byte)(VARIABLE_REGISTERS[(byte)secondNibble] | VARIABLE_REGISTERS[(byte)thirdNibble]);
                            break;
                        case 0x2: //8XY2 - Logical AND register
                            VARIABLE_REGISTERS[(byte)secondNibble] = (byte)(VARIABLE_REGISTERS[(byte)secondNibble] & VARIABLE_REGISTERS[(byte)thirdNibble]);
                            break;
                        case 0x3: //8XY3 - Logical XOR register
                            VARIABLE_REGISTERS[(byte)secondNibble] = (byte)(VARIABLE_REGISTERS[(byte)secondNibble] ^ VARIABLE_REGISTERS[(byte)thirdNibble]);
                            break;
                        case 0x4: //8XY4 - Add register
                            int product = VARIABLE_REGISTERS[(byte)secondNibble] + VARIABLE_REGISTERS[(byte)thirdNibble];
                            VARIABLE_REGISTERS[(byte)secondNibble] = (byte)product;
                            if (product > 255)
                            {
                                VARIABLE_REGISTERS[0xF] = 1;
                            }
                            else
                            {
                                VARIABLE_REGISTERS[0xF] = 0;
                            }
                            break;
                        case 0x5: //8XY5 - Subtract register X-Y
                            var minuend5 = VARIABLE_REGISTERS[(byte)secondNibble];
                            var subtrahend5 = VARIABLE_REGISTERS[(byte)thirdNibble];
                            VARIABLE_REGISTERS[(byte)secondNibble] = (byte)(minuend5 - subtrahend5);
                            if (minuend5 > subtrahend5)
                            {
                                VARIABLE_REGISTERS[0xF] = 1;
                            }
                            else
                            {
                                VARIABLE_REGISTERS[0xF] = 0;
                            }
                            break;
                        case 0x6: //8XY6 - Shift right register
                            var leastSignifigant = (byte)(VARIABLE_REGISTERS[(byte)secondNibble] & 1);
                            VARIABLE_REGISTERS[(byte)secondNibble] = (byte)(VARIABLE_REGISTERS[(byte)secondNibble] >> 1);
                            VARIABLE_REGISTERS[0xF] = leastSignifigant;
                            break;
                        case 0x7: //8XY7 - Subtract register Y-X
                            var minuend7 = VARIABLE_REGISTERS[(byte)thirdNibble];
                            var subtrahend7 = VARIABLE_REGISTERS[(byte)secondNibble];
                            VARIABLE_REGISTERS[(byte)secondNibble] = (byte)(minuend7 - subtrahend7);
                            if (minuend7 > subtrahend7)
                            {
                                VARIABLE_REGISTERS[0xF] = 1;
                            }
                            else
                            {
                                VARIABLE_REGISTERS[0xF] = 0;
                            }
                            break;
                        case 0xE: //8XYE - Shift left register
                            var mostSignifigant = (byte)((VARIABLE_REGISTERS[(byte)secondNibble] >> 7) & 1);
                            VARIABLE_REGISTERS[(byte)secondNibble] = (byte)(VARIABLE_REGISTERS[(byte)secondNibble] << 1);
                            VARIABLE_REGISTERS[0xF] = mostSignifigant;
                            break;

                    }
                    break;
                case 0x9: //9XY0 - Skip if not equal registers
                    if (VARIABLE_REGISTERS[(byte)secondNibble] != VARIABLE_REGISTERS[(byte)thirdNibble])
                    {
                        PC += 2;
                    }
                    break;
                case 0xA: //ANNN - Set index
                    INDEX_REGISTER = (ushort)nibbleNNN;
                    break;
                case 0xB: //BNNN or BXNN - Jump with offset
                    if (_superChipQuirk)
                    {
                        PC = (ushort)(nibbleNNN + VARIABLE_REGISTERS[(byte)secondNibble]);
                    }
                    else
                    {
                        PC = (ushort)(nibbleNNN + VARIABLE_REGISTERS[0x0]);
                    }
                    break;
                case 0xC: //CXNN - Random
                    var randArray = new byte[1];
                    random.NextBytes(randArray);
                    VARIABLE_REGISTERS[(byte)secondNibble] = (byte)(randArray[0] & instrByteTwo);
                    break;
                case 0xD: //DXYN - Display
                    byte xcoord = (byte)(VARIABLE_REGISTERS[(byte)secondNibble] % _screen.getWidth());
                    byte ycoord = (byte)(VARIABLE_REGISTERS[(byte)thirdNibble] % _screen.getHeight());
                    VARIABLE_REGISTERS[0xF] = 0;
                    for (int i = 0; i < fourthNibble; i++)
                    {
                        byte spriteRow = RAM[INDEX_REGISTER + i];
                        byte countingXcoord = xcoord;
                        var spriteRowArray = new BitArray(new byte[] { spriteRow });
                        for (int j = 8; j > 0; j--)
                        {
                            bool bit = spriteRowArray[j - 1];
                            if (_screen.XORPixel(bit, countingXcoord, ycoord))
                            {
                                VARIABLE_REGISTERS[0xF] = 1;
                            }

                            countingXcoord++;

                            if (countingXcoord == _screen.getWidth())
                            {
                                countingXcoord = xcoord;
                                break;
                            }
                        }
                        ycoord++;
                        if (ycoord == _screen.getHeight())
                        {
                            break;
                        }
                    }
                    break;
                case 0xE: //Skip if key instructions
                    switch (instrByteTwo)
                    {
                        case 0x9E: //EX9E - skip if pressed
                            if (PRESSED_KEYS[VARIABLE_REGISTERS[(byte)secondNibble]])
                            {
                                PC += 2;
                            }
                            break;
                        case 0xA1: //EXA1 - skip if not pressed
                            if (!PRESSED_KEYS[VARIABLE_REGISTERS[(byte)secondNibble]])
                            {
                                PC += 2;
                            }
                            break;
                    }
                    break;
                case 0xF:
                    switch (instrByteTwo)
                    {
                        case 0x07: //FX07 - Set register to delay timer
                            VARIABLE_REGISTERS[(byte)secondNibble] = DELAY_TIMER;
                            break;
                        case 0x15: //FX15 - Set delay timer to register
                            _ = setDelayTimer(VARIABLE_REGISTERS[(byte)secondNibble]);
                            break;
                        case 0x18: //FX18 - Set sound timer to register
                            _ = setSoundTimer(VARIABLE_REGISTERS[(byte)secondNibble]);
                            break;
                        case 0x1E: //FX1E - Add to index
                            INDEX_REGISTER += VARIABLE_REGISTERS[(byte)secondNibble];
                            break;
                        case 0x0A: //FX0A - Get key
                            PC -= 2;
                            foreach(var keypair in PRESSED_KEYS)
                            {
                                if(keypair.Value == true)
                                {
                                    PC += 2;
                                    VARIABLE_REGISTERS[(byte)secondNibble] = keypair.Key;
                                    break;
                                }
                            }
                            break;
                        case 0x29: //FX29 - Font character
                            var lastVXNibble = VARIABLE_REGISTERS[(byte)secondNibble] & 0x0F;
                            INDEX_REGISTER = (ushort)(0x050 + (5*lastVXNibble));
                            break;
                        case 0x33: //FX33 - Binary-coded decimal conversion
                            var toConvert = VARIABLE_REGISTERS[(byte)secondNibble];
                            if(toConvert == 0)
                            {
                                RAM[INDEX_REGISTER] = 0;
                                break;
                            }
                            var digits = new byte[3];
                            int index = 2;
                            while(toConvert > 0)
                            {
                                var digit = toConvert % 10;
                                toConvert /= 10;
                                digits[index--] = (byte)digit;
                            }
                            for(int i = 0; i < digits.Length; i++)
                            {
                                RAM[INDEX_REGISTER+i] = digits[i];
                            }
                            break;
                        case 0x55: //FX55 - Store registers to memory
                            for(byte i = 0; i < secondNibble+1; i++)
                            {
                                RAM[INDEX_REGISTER+i] = VARIABLE_REGISTERS[i];
                            }
                            break;
                        case 0x65: //5X65 - Load registers from memory
                            for(byte i = 0; i < secondNibble+1; i++)
                            {
                                VARIABLE_REGISTERS[i] = RAM[INDEX_REGISTER+i];
                            }
                            break;
                    }
                    break;
            }
            Thread.Sleep(1);
        }

    }

    public async Task setDelayTimer(byte set)
    {
        DELAY_TIMER = set;
        var autoEvent = new AutoResetEvent(false);
        Timer timer = new(decrementDelayTimer, autoEvent, 0, 17);

        autoEvent.WaitOne();
        await timer.DisposeAsync();

    }

    private void decrementDelayTimer(object? state)
    {
        if (--DELAY_TIMER == 0 && state != null)
        {
            var autoEvent = (AutoResetEvent)state;
            autoEvent.Set();
        }
    }

    public async Task setSoundTimer(byte set)
    {
        SOUND_TIMER = set;
        var autoEvent = new AutoResetEvent(false);
        Timer timer = new(decrementSoundTimer, autoEvent, 0, 17);

        autoEvent.WaitOne();
        await timer.DisposeAsync();

    }

    private void decrementSoundTimer(object? state)
    {
        if (--SOUND_TIMER == 0 && state != null)
        {
            var autoEvent = (AutoResetEvent)state;
            autoEvent.Set();
        }
    }

}