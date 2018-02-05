using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ChipSkate
{
    class chip8
    {
        private ushort opcode;
        private byte[] memory = new byte[4096];
        private byte[] V = new byte[16];
        private ushort I;
        private ushort pc;
        public byte[] gfx = new byte[32 * 64];
        private byte delay_timer;
        private byte sound_timer;

        private ushort[] stack = new ushort[16];
        private ushort sp;
        public byte[] key = new byte[16];
        public byte whichkey;
        public bool drawflag;
        public bool keywaitflag;
        public byte Voffset;

        private Random random;

        /// <summary>
        /// Creates and initializes a new emulator
        /// </summary>
        public chip8()
        {
            random = new Random();
        }

        /// <summary>
        /// Initializes the emulator and loads the ROM in the filepath variable.
        /// </summary>
        /// <param name="FilePath">the file path to load</param>
        public void LoadGame(string FilePath)
        {
            pc = 0x200;  // Program counter starts at 0x200
            opcode = 0;      // Reset current opcode	
            I = 0;      // Reset index register
            sp = 0;      // Reset stack pointer
            keywaitflag = false;    // reset any wait flags

            // Clear display
            gfx = new byte[32 * 64];
            int z = gfx.Length;
            for (int i = 0; i < gfx.Length; i++)
                gfx[i] = 0;
            // Clear stack
            for (int h = 0; h < stack.Length; h++)
                stack[h] = 0;
            // Clear registers V0-VF
            for (int b = 0; b < V.Length; b++)
                V[b] = 0;
            // Clear memory
            for (int j = 0; j < memory.Length; j++)
                memory[j] = 0;
            // Load fontset
            for (int i = 0; i < 80; ++i)
                memory[80 + i] = chip8_fontset[i];

            // Reset timers
            delay_timer = 60;       // the timer "chips" on the chip8 run at 60hz
            sound_timer = 60;

            // load file from memory into a buffer into the emulator's memory
            // Create the reader for data.
            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);            
            BinaryReader r = new BinaryReader(fs);
            // Read data from Test.data.
            for (int i = 0; i < fs.Length; i++)
                memory[pc + i] = r.ReadByte();
            r.Close();
            fs.Close();            
        }

        /// <summary>
        /// Step one iteration through the cpu cycle
        /// </summary>
        public void cycleCPU()
        {
            //if (gfx.Length != 32 * 64)
              //  System.Diagnostics.Debugger.Break();
            // fetch opcode
            opcode = (ushort)((memory[pc] << 8) | memory[pc + 1]);
            switch (opcode & 0xf000)   // decode opcode
            {
                // execute opcode
                case 0x0000:
                    switch (opcode & 0x000f)
                    {
                        case 0x0000: // 0x00E0: Clears the screen        
                            for(int b = 0; b < gfx.Length; b++)
                                gfx[b] = 0;
                            drawflag = true;
                            pc += 2;
                            break;

                        case 0x000E: // 0x00EE: Returns from subroutine          
                            sp--;
                            pc = (ushort)(stack[sp] + 2);
                            break;
                    }
                    break;
                case 0x1000: //Jumps to address NNN.
                    //stack[sp] = pc;
                    pc = (ushort)(opcode & 0x0fff);
                    //sp++;
                    break;
                case 0x2000:
                    // execute opcode
                    stack[sp] = pc;
                    ++sp;
                    pc = (ushort)(opcode & 0x0FFF);
                    break;
                case 0x3000: // Skips the next instruction if VX equals NN.
                    if (V[(opcode & 0x0f00) >> 8] == (byte)(opcode & 0x00ff))
                        pc += 4;
                    else
                        pc += 2;
                    break;
                case 0x4000: // Skips the next instruction if VX doesn't equal NN.
                    if (V[(opcode & 0x0f00) >> 8] != (byte)(opcode & 0x00ff))
                        pc += 4;
                    else
                        pc += 2;
                    break;
                case 0x5000: // Skips the next instruction if VX equals VY.
                    if (V[(opcode & 0x0f00) >> 8] == V[(opcode & 0x00f0) >> 4])
                        pc += 4;
                    else
                        pc += 2;
                    break;
                case 0x6000: // Sets VX to NN
                    V[(opcode & 0x0f00) >> 8] = (byte)(opcode & 0x00ff);
                    pc += 2;
                    break;
                case 0x7000: // 7XNN 	Adds NN to VX.
                    V[(opcode & 0x0f00) >> 8] += (byte)(opcode & 0x00ff);
                    pc += 2;
                    break;
                case 0x8000:
                    switch (opcode & 0x000f)
                    {
                        case 0x0000: // 8XY0 	Sets VX to the value of VY.
                            V[(opcode & 0x0f00) >> 8] = V[(opcode & 0x00f0) >> 4];
                            pc += 2;
                            break;
                        case 0x0001: // 8XY1 	Sets VX to VX | VY.
                            V[(opcode & 0x0f00) >> 8] = (byte)(V[(opcode & 0x0f00) >> 8] | V[(opcode & 0x00f0) >> 4]);
                            pc += 2;
                            break;
                        case 0x0002: // 8XY2 	Sets VX to VX & VY.
                            V[(opcode & 0x0f00) >> 8] = (byte)(V[(opcode & 0x0f00) >> 8] & V[(opcode & 0x00f0) >> 4]);
                            pc += 2;
                            break;
                        case 0x0003: // 8XY3 	Sets VX to VX xor VY.
                            V[(opcode & 0x0f00) >> 8] = (byte)(V[(opcode & 0x0f00) >> 8] ^ V[(opcode & 0x00f0) >> 4]);
                            pc += 2;
                            break;
                        case 0x0004:
                            // execute opcode: Adds VY to VX. VF is set to 1 when there's a carry, and to 0 when there isn't.
                            if (V[(opcode & 0x00F0) >> 4] > (0xFF - V[(opcode & 0x0F00) >> 8]))
                                V[0xF] = 1; //carry
                            else
                                V[0xF] = 0;
                            V[(opcode & 0x0F00) >> 8] += V[(opcode & 0x00F0) >> 4];
                            pc += 2; 
                            break;
                        case 0x0005: // 8XY5 	VY is subtracted from VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
                            if (V[(opcode & 0x00F0) >> 4] > (V[(opcode & 0x0F00) >> 8]))
                                V[0xF] = 0; // borrow
                            else
                                V[0xF] = 1;
                            V[(opcode & 0x0F00) >> 8] -= V[(opcode & 0x00F0) >> 4];
                            pc += 2; 
                            break;
                        case 0x0006: // 8XY6 	Shifts VX right by one. VF is set to the value of the least significant bit of VX before the shift.
                            V[0x0f] = (byte)(V[(opcode & 0x0F00) >> 8] & 0x01);
                            V[(opcode & 0x0F00) >> 8] >>= 1;
                            pc += 2;
                            break;
                        case 0x0007: // 8XY7 	Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
                            if (V[(opcode & 0x00f0) >> 4] < V[(opcode & 0x0f00) >> 8])
                                V[0x0f] = 0; // borrow
                            else
                                V[0x0f] = 1;
                            V[(opcode & 0x0f00) >> 8] = (byte)(V[(opcode & 0x00f0) >> 4] - V[(opcode & 0x0f00) >> 8]);
                            pc += 2;
                            break;
                        case 0x000e: // 8XYE 	Shifts VX left by one. VF is set to the value of the most significant bit of VX before the shift.
                            V[0x0f] = (byte)(V[(opcode & 0x0F00) >> 8] & 0x80); // 8 will & the highest bit, but nothing else
                            V[(opcode & 0x0F00) >> 8] <<= 1;
                            pc += 2;
                            break;
                    }
                    break;
                case 0x9000: // 9XY0 	Skips the next instruction if VX doesn't equal VY.
                    if (V[(opcode & 0x0F00) >> 8] != V[(opcode & 0x00f0) >> 4])
                        pc += 4;
                    else
                        pc += 2;
                    break;
                case 0xa000: // ANNN 	Sets I to the address NNN.
                    I = (ushort)(opcode & 0x0FFF);
                    pc += 2;
                    break;
                case 0xb000: // BNNN 	Jumps to the address NNN plus V0.
                    //stack[sp] = pc;
                    pc = (ushort)((opcode & 0x0fff) + V[0x00]);
                    //sp++;
                    break;
                case 0xc000: // CXNN 	Sets VX to a random number and NN.
                    V[(opcode & 0x0F00) >> 8] = (byte)(random.Next(256) & (opcode & 0x00ff));
                    pc += 2;
                    break;
                case 0xd000:
                    {
                        /* DXYN     Draws a sprite at coordinate (VX, VY) that has a width of 8 pixels and a height of N pixels. 
                         * Each row of 8 pixels is read as bit-coded (with the most significant bit of each byte displayed on the left) 
                         * starting from memory location I; I value doesn't change after the execution of this instruction. 
                         * As described above, VF is set to 1 if any screen pixels are flipped from set to unset when the sprite is drawn, 
                         * and to 0 if that doesn't happen. */
                        ushort x = V[(opcode & 0x0F00) >> 8];
                        ushort y = V[(opcode & 0x00F0) >> 4];
                        ushort height = (ushort)(opcode & 0x000F);
                        byte pixel;

                        V[0xF] = 0;
                        for (int yline = 0; yline < height; yline++)
                        {
                            pixel = memory[I + yline];
                            for (int xline = 0; xline < 8; xline++)
                            {
                                if ((pixel & (0x80 >> xline)) != 0)
                                {
                                    if (gfx[(x + xline + ((y + yline) * 64))] == 1)
                                        V[0xF] = 1;
                                    gfx[x + xline + ((y + yline) * 64)] ^= 1;
                                }
                            }
                        }

                        drawflag = true;
                        pc += 2;
                    }
                    break;
                case 0xe000:
                    switch (opcode & 0x00ff)
                    {
                        case 0x009E:  // EX9E   Skips the next instruction if the key stored in VX is pressed
                            if (key[V[(opcode & 0x0F00) >> 8]] != 0)
                                pc += 4;
                            else
                                pc += 2;
                            break;
                        case 0x00a1:  // EXA1 	Skips the next instruction if the key stored in VX isn't pressed.
                            if (key[V[(opcode & 0x0F00) >> 8]] == 0)
                                pc += 4;
                            else
                                pc += 2;
                            break;
                    }
                    break;
                case 0xf000:  
                    switch (opcode & 0x00ff)
                    {
                        case 0x0007:  // FX07 	Sets VX to the value of the delay timer.
                            V[(opcode & 0x0F00) >> 8] = delay_timer;
                            pc += 2;
                            break;
                        case 0x000a:  // FX0A 	A key press is awaited, and then stored in VX.
                            keywaitflag = true;
                            whichkey = (byte)((opcode & 0x0F00) >> 8);
                            pc += 2;
                            break;
                        case 0x0015:  // FX15 	Sets the delay timer to VX.
                            delay_timer = V[(opcode & 0x0F00) >> 8];
                            pc += 2;
                            break;
                        case 0x0018: // FX18 	Sets the sound timer to VX.
                            sound_timer = V[(opcode & 0x0F00) >> 8];
                            pc += 2;
                            break;
                        case 0x001e: // FX1E 	Adds VX to I
                            I += V[(opcode & 0x0F00) >> 8];
                            pc += 2;
                            break;
                        case 0x0029: // FX29 	Sets I to the location of the sprite for the character in VX. 
                                     // Characters 0-F (in hexadecimal) are represented by a 4x5 font.                            
                            I = (ushort)(80 + (V[(opcode & 0x0F00) >> 8] * 5));
                            pc += 2;
                            break;
                        case 0x0033:
                            /* execute opcode: Stores the Binary-coded decimal representation of VX, 
                             with the most significant of three digits at the address in I, the middle 
                             digit at I plus 1, and the least significant digit at I plus 2. Not sure I get this one.*/
                            memory[I] = (byte)(V[(opcode & 0x0F00) >> 8] / 100);
                            memory[I + 1] = (byte)((V[(opcode & 0x0F00) >> 8] / 10) % 10);
                            memory[I + 2] = (byte)((V[(opcode & 0x0F00) >> 8] % 100) % 10);
                            pc += 2;
                            break;
                        case 0x0055: // FX55 	Stores V0 to VX in memory starting at address I
                            for (byte i = 0; i < (opcode & 0x0F00) >> 8; i++)
                                memory[I + i] = V[i];
                            pc += 2;
                            break;
                        case 0x0065: // FX65 	Fills V0 to VX with values from memory starting at address I
                            for (byte i = 0; i < (opcode & 0x0F00) >> 8; i++)
                                V[i] = memory[I + i];
                            pc += 2;
                            break;
                    }
                    break;
                default:
                    System.Windows.Forms.MessageBox.Show("unknown opcode: 0x" + Convert.ToString(opcode, 16));
                    break;
            }

            // Update timers
            if (delay_timer > 0)
                delay_timer--;

            if (sound_timer > 0)
            {
                if (sound_timer == 1)
                    System.Media.SystemSounds.Beep.Play();
                sound_timer--;
            }            
        }

        private byte[] chip8_fontset = new byte[80]
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
            0xF0, 0x80, 0xF0, 0x80, 0x80};  // F

        public void keyWaitExecution()
        {
            V[Voffset] = whichkey;
        }
  
    }
}
