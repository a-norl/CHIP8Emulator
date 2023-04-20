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

    public CPU(byte[] rom, Screen screen)
    {
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

    public void Loop()
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
            var nibbleNNN = secondNibble << 8 & thirdNibble << 4 & fourthNibble; //NNN

            switch (firstNibble)
            {
                case 0x0:
                    break;
                case 0x1:
                    break;
                case 0x2:
                    break;
                case 0x3:
                    break;
                case 0x4:
                    break;
                case 0x5:
                    break;
                case 0x6:
                    break;
                case 0x7:
                    break;
                case 0x8:
                    break;
                case 0x9:
                    break;
                case 0xA:
                    break;
                case 0xB:
                    break;
                case 0xC:
                    break;
                case 0xD:
                    break;
                case 0xE:
                    break;
                case 0xF:
                    break;
            }

            //Decode

            //Execute
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