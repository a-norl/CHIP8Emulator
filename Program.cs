using SDL2;

namespace CHIP8Emulator;

class Program
{
    static void Main(string[] args)
    {

        byte[] romBytes = File.ReadAllBytes(args[0]);

        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO | SDL.SDL_INIT_EVENTS) < 0)
        {
            Console.WriteLine($"Error initializing SDL: {SDL.SDL_GetError()}");
        }

        var screen = new Screen();
        var cpu = new CPU(romBytes, screen);

        var _window = SDL.SDL_CreateWindow("CHIP-8", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, 128 * 8, 64 * 8, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

        if (_window == IntPtr.Zero)
        {
            Console.WriteLine($"Error making window: {SDL.SDL_GetError()}");
        }

        var _renderer = SDL.SDL_CreateRenderer(_window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

        if (_renderer == IntPtr.Zero)
        {
            Console.WriteLine($"Error creating the renderer: {SDL.SDL_GetError()}");
        }

        var running = true;
        _ = Task.Run(() => cpu.Tick());
        while (running)
        {
            // cpu.Tick();
            // Check to see if there are any events and continue to do so until the queue is empty.
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        running = false;
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        Console.WriteLine($"{e.key.keysym.sym} DOWN");
                        switch(e.key.keysym.sym)
                        {
                            case SDL.SDL_Keycode.SDLK_1:
                                cpu.PRESSED_KEYS[0x0] = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_2:
                                cpu.PRESSED_KEYS[0x1] = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_3:
                                cpu.PRESSED_KEYS[0x2] = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_4:
                                cpu.PRESSED_KEYS[0x3] = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_q:
                                cpu.PRESSED_KEYS[0x4] = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_w:
                                cpu.PRESSED_KEYS[0x5] = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_e:
                                cpu.PRESSED_KEYS[0x6] = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_r:
                                cpu.PRESSED_KEYS[0x7] = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_a:
                                cpu.PRESSED_KEYS[0x8] = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_s:
                                cpu.PRESSED_KEYS[0x9] = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_d:
                                cpu.PRESSED_KEYS[0xA] = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_f:
                                cpu.PRESSED_KEYS[0xB] = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_z:
                                cpu.PRESSED_KEYS[0xC] = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_x:
                                cpu.PRESSED_KEYS[0xD] = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_c:
                                cpu.PRESSED_KEYS[0xE] = true;
                                break;
                            case SDL.SDL_Keycode.SDLK_v:
                                cpu.PRESSED_KEYS[0xF] = true;
                                break;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_KEYUP:
                    Console.WriteLine($"{e.key.keysym.sym} UP");
                    switch(e.key.keysym.sym)
                        {
                            case SDL.SDL_Keycode.SDLK_1:
                                cpu.PRESSED_KEYS[0x0] = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_2:
                                cpu.PRESSED_KEYS[0x1] = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_3:
                                cpu.PRESSED_KEYS[0x2] = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_4:
                                cpu.PRESSED_KEYS[0x3] = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_q:
                                cpu.PRESSED_KEYS[0x4] = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_w:
                                cpu.PRESSED_KEYS[0x5] = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_e:
                                cpu.PRESSED_KEYS[0x6] = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_r:
                                cpu.PRESSED_KEYS[0x7] = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_a:
                                cpu.PRESSED_KEYS[0x8] = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_s:
                                cpu.PRESSED_KEYS[0x9] = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_d:
                                cpu.PRESSED_KEYS[0xA] = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_f:
                                cpu.PRESSED_KEYS[0xB] = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_z:
                                cpu.PRESSED_KEYS[0xC] = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_x:
                                cpu.PRESSED_KEYS[0xD] = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_c:
                                cpu.PRESSED_KEYS[0xE] = false;
                                break;
                            case SDL.SDL_Keycode.SDLK_v:
                                cpu.PRESSED_KEYS[0xF] = false;
                                break;
                        }
                        break;
                }
            }
            if(cpu.SOUND_TIMER > 0)
            {
                //beep sound
            }
            if (screen.needDraw)
            {


                SDL.SDL_SetRenderDrawColor(_renderer, 155, 188, 15, 255);


                SDL.SDL_RenderClear(_renderer);


                for (int x = 0; x < screen.getWidth(); x++)
                {
                    for (int y = 0; y < screen.getHeight(); y++)
                    {
                        SDL.SDL_Rect pixel;
                        if (screen.hiRes)
                        {
                            pixel = new SDL.SDL_Rect
                            {
                                x = x * 8,
                                y = y * 8,
                                w = 8,
                                h = 8
                            };
                        }
                        else
                        {
                            pixel = new SDL.SDL_Rect
                            {
                                x = x * 16,
                                y = y * 16,
                                w = 16,
                                h = 16
                            };
                        }

                        if (screen.pixelsToDraw[x, y])
                        {
                            SDL.SDL_SetRenderDrawColor(_renderer, 15, 56, 15, 255);
                        }
                        else
                        {
                            SDL.SDL_SetRenderDrawColor(_renderer, 155, 188, 15, 255);
                        }
                        SDL.SDL_RenderFillRect(_renderer, ref pixel);
                        screen.pixelsDrawn[x, y] = screen.pixelsToDraw[x, y];
                    }
                }
                screen.needDraw = false;
                SDL.SDL_RenderPresent(_renderer);

            }

        }


        // Clean up the resources that were created.
        SDL.SDL_DestroyRenderer(_renderer);
        SDL.SDL_DestroyWindow(_window);
        SDL.SDL_Quit();
    }
}
