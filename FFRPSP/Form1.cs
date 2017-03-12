using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace FF1PSPHacker
{
    public partial class Form1 : Form
    {
        byte[] romData;
        byte[] livePatch;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnFF1Monsters_Click(object sender, EventArgs e)
        {
            if (!loadRom()) return;            

            overrideEboot();
            adjustEncounterRate();
            adjustXPRequirements();
            boostXP();

            Random r1 = new Random(Convert.ToInt32(txtSeed.Text));
            randomizeEquipment(r1);
            randomizeMonsterPatterns(r1);
            randomizeTreasures(r1);

            saveRom();
            lblResults.Text = "Hacking complete!  (" + Path.Combine(Path.GetDirectoryName(txtFileName.Text), Path.GetFileNameWithoutExtension(txtFileName.Text) + "_" + txtSeed.Text + "_" + txtFlags.Text + ".iso");
        }

        private void printInformation()
        {
            //using (StreamWriter writer = File.CreateText(@"c:\bizhawk\ff test\armor_full.txt"))
            //{
            //    for (int i = 0; i < 0x4b; i++)
            //    {
            //        int byteToUse = 0x2b30390 + (0x1c * i);
            //        string line = "";
            //        for (int j = 0; j < 0x1c; j++)
            //        {
            //            line += romData[byteToUse + j].ToString("X2") + " ";
            //        }
            //        writer.WriteLine(line);
            //    }
            //}

            //using (StreamWriter writer = File.CreateText(@"c:\bizhawk\ff test\weapon_full.txt"))
            //{
            //    for (int i = 0; i < 0x43; i++)
            //    {
            //        int byteToUse = 0x2b2fc20 + (0x1c * i);
            //        string line = "";
            //        for (int j = 0; j < 0x1c; j++)
            //        {
            //            line += romData[byteToUse + j].ToString("X2") + " ";
            //        }
            //        writer.WriteLine(line);
            //    }
            //}


            //using (StreamWriter writer = File.CreateText(@"c:\bizhawk\ff test\monster_sets.txt"))
            //{
            //    for (int i = 0; i < 0xff; i++)
            //    {
            //        int byteToUse = 0x2b24d68 + (0xf * i);
            //        string line = "";
            //        for (int j = 0; j < 0xf; j++)
            //        {
            //            line += romData[byteToUse + j].ToString("X2") + " ";
            //        }
            //        writer.WriteLine(line);
            //    }
            //}
            //using (StreamWriter writer = File.CreateText(@"c:\bizhawk\ff test\monster_zones.txt"))
            //{
            //    for (int i = 0; i < 0x80; i++)
            //    {
            //        int byteToUse = 0x2b21ff0 + (0x8 * i);
            //        string line = "";
            //        for (int j = 0; j < 0x8; j++)
            //        {
            //            line += romData[byteToUse + j].ToString("X2") + " ";
            //        }
            //        writer.WriteLine(line);
            //    }
            //}
            //using (StreamWriter writer = File.CreateText(@"c:\bizhawk\ff test\monster_zones_overworld.txt"))
            //{
            //    for (int i = 0; i < 0x60; i++)
            //    {
            //        int byteToUse = 0x2b218e4 + (0x8 * i);
            //        string line = "";
            //        for (int j = 0; j < 0x8; j++)
            //        {
            //            line += romData[byteToUse + j].ToString("X2") + " ";
            //        }
            //        writer.WriteLine(line);
            //    }
            //}
        }

        private void overrideEboot()
        {
            // Overwrite the EBOOT.BIN with the BOOT.BIN  This makes EBOOT.BIN invalid, loading from BOOT.BIN instead.
            for (int lnI = 0x10000; lnI < 0x1d7390; lnI++)
            {
                romData[lnI] = romData[0x29e0000 + lnI];
            }
        }

        private void adjustEncounterRate()
        {
            if (trkEncounterRate.Value != 100)
            {
                for (int lnI = 0x2b216a8; lnI < 0x2b21768; lnI += 2)
                {
                    int encounterRate = (romData[lnI + 1] * 256) + romData[lnI];
                    encounterRate = encounterRate * trkEncounterRate.Value / 100;
                    romData[lnI] = (byte)(encounterRate % 256);
                    romData[lnI + 1] = (byte)(encounterRate / 256);
                }
            }
        }

        private void adjustXPRequirements()
        {
            if (trkXPReqAdj.Value != 100)
            {
                for (int lnI = 0x2b2f438; lnI < 0x2b2f5c0; lnI += 4)
                {
                    int xpRequired = (romData[lnI + 3] * 16777216) + (romData[lnI + 2] * 65536) + (romData[lnI + 1] * 256) + romData[lnI];
                    xpRequired = xpRequired * trkXPReqAdj.Value / 100;
                    romData[lnI] = (byte)(xpRequired % 256);
                    romData[lnI + 1] = (byte)((xpRequired / 256) % 256);
                    romData[lnI + 2] = (byte)((xpRequired / 65536) % 256);
                    romData[lnI + 3] = (byte)(xpRequired / 1677216);
                }
            }
        }

        private void boostXP()
        {
            if (trkXPBoost.Value != 0)
            {
                for (int lnI = 0x2b28480; lnI < 0x2b28480 + (203 * 0x24); lnI += 0x24)
                {
                    int XP = (romData[lnI + 1] * 256) + romData[lnI];
                    XP += trkXPBoost.Value;
                    romData[lnI] = (byte)(XP % 256);
                    romData[lnI + 1] = (byte)(XP / 256);
                }
            }
        }

        private void randomizeMonsterPatterns(Random r1)
        {
            if (chkRandomizeSpecialMonsters.Checked)
            {
                for (int lnI = 0x2b27180; lnI <= 0x2b2724a; lnI++)
                {
                    // Keep Chaos's attack pattern intact.
                    if (romData[lnI] == 0x2a) continue;

                    if (r1.Next() % 2 == 1)
                        romData[lnI] = (byte)(r1.Next() % 0x4d);
                    else
                        romData[lnI] = 0x2c;

                    // Do not issue Chaos's pattern to any monster except Chaos
                    if (romData[lnI] == 0x2a) lnI--;
                }
            }
            if (chkRandomizeMonsterPatterns.Checked)
            {
                for (int lnI = 0x2b2724c; lnI <= 0x2b2771b; lnI += 0x10)
                {
                    // Skip the all attack and Chaos's attack pattern.
                    if (lnI == 0x2b2750c || lnI == 0x2b274ec) continue;
                    romData[lnI + 0] = (byte)(r1.Next() % 13 * 8);
                    romData[lnI + 1] = (byte)(r1.Next() % 13 * 8);
                    for (int lnJ = 2; lnJ < 10; lnJ++)
                    {
                        romData[lnI + lnJ] = (byte)(r1.Next() % 0x41);
                        // Certain spells monsters can't cast or they outright don't work.
                        if (romData[lnI + lnJ] == 0x02 || romData[lnI + lnJ] == 0x0a || romData[lnI + lnJ] == 0x13 || romData[lnI + lnJ] == 0x1a || 
                          romData[lnI + lnJ] == 0x2f || romData[lnI + lnJ] == 0x12 || romData[lnI + lnJ] == 0x1d || romData[lnI + lnJ] == 0x16 || romData[lnI + lnJ] == 0x3f)
                            lnJ--;
                    }
                    romData[lnI + 10] = romData[lnI + 15] = 0xff;
                    for (int lnJ = 11; lnJ < 15; lnJ++)
                        romData[lnI + lnJ] = (byte)(r1.Next() % 0x25);
                }
            }
        }

        // Cannot randomize monster zones at this time; PPSSPP crashes every time... huh...

        //private void randomizeMonsterZones(Random r1)
        //{
        //    int[] monsterRank =
        //    {
        //        0x00, 0x15, 0x01, 0x02, 0x2b, 0x0c, 0x49, 0x0f,
        //        0x13, 0x10, 0x74, 0x17, 0x45, 0x27, 0x03, 0x2c,
        //        0x0d, 0x2d, 0x1e, 0x69, 0x3e, 0x04, 0x4a, 0x2e,
        //        0x06, 0x1f, 0x18, 0x51, 0x1b, 0x21, 0x28, 0x57,
        //        0x47, 0x46, 0x11, 0x67, 0x1c, 0x19, 0x4f, 0x16,
        //        0x3f, 0x05, 0x52, 0x29, 0x3a, 0x23, 0x58, 0x37,
        //        0x25, 0x22, 0x34, 0x1d, 0x36, 0x59, 0x68, 0x3b,
        //        0x26, 0x09, 0x0e, 0x5d, 0x20, 0x2a, 0x50, 0x24,
        //        0x72, 0x48, 0x4c, 0x53, 0x1a, 0x38, 0x3c, 0x5e,
        //        0x35, 0x54, 0x5f, 0x5b, 0x6c, 0x6f, 0x75, 0x4b,
        //        0x4d, 0x0b, 0x40, 0x62, 0x41, 0x31, 0x42, 0x0a,
        //        0x5a, 0x61, 0x08, 0x65, 0x33, 0x39, 0x71, 0x44,
        //        0x63, 0x12, 0x3d, 0x6d, 0x07, 0x4e, 0x30, 0x70,
        //        0x43, 0x5c, 0x32, 0x6b, 0x55, 0x73, 0x64, 0x14,
        //        0x60, 0x6a, 0x2f, 0x66, 0x6e, 0x56, 0x76, 0x77, // May want to stop at index 117.  Warmech is index 118, the fiends are indexes 119-126.
        //        0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e
        //    };

        //    int[] monsterSize =
        //    {
        //        1, 1, 1, 1, 1, 1, 2, 2,
        //        2, 2, 2, 2, 1, 1, 1, 1,
        //        1, 2, 2, 2, 2, 1, 1, 1,
        //        1, 2, 2, 2, 2, 2, 1, 1,
        //        1, 1, 1, 2, 2, 2, 2, 1,
        //        1, 1, 1, 1, 1, 1, 1, 2,
        //        2, 2, 2, 2, 1, 1, 1, 1,
        //        2, 2, 2, 2, 1, 1, 1, 1,
        //        2, 2, 2, 2, 2, 1, 1, 1,
        //        1, 1, 1, 2, 2, 2, 2, 1,
        //        1, 1, 1, 2, 2, 2, 2, 1,
        //        1, 1, 1, 2, 2, 2, 2, 1,
        //        1, 1, 1, 2, 2, 2, 2, 1,
        //        1, 3, 2, 2, 2, 2, 2, 1,
        //        1, 3, 1, 1, 2, 2, 2, 3,
        //        3, 3, 3, 3, 3, 3, 3
        //    };

        //    // Mark overworld zones, then mark caves
        //    // Overworld starts at 0x2b218e4
        //    int[] overworldZones = { 10, 10, 10, 10, 10, 10, 10, 10,
        //                           10, 10, 10, 10, 10, 10, 10, 10,
        //                           10, 10, -1, 10, -1, 10, 10, 10,
        //                           -1, -1, -1,  3,  3,  4, -1, 10,
        //                           -1,  7,  5,  5,  2,  4,  4, 10,
        //                            7,  7,  7,  5,  1,  5, 10, 10,
        //                           -1, -1,  6,  5,  5,  8, 10, 10,
        //                           -1, -1,  6,  6,  5,  8, 10, 10 };

        //    // Caves start at 0x2b21ff0
        //    int[] caveZones =
        //        { 9, 9, 9, 9, 9, 9, -2,
        //        -2, -2, -2, -2, -2, -2, -2, -2,
        //        -2, -2, -2, -2, -2, -2, -2, 25,
        //        25, 25, 25, 25, 25, 25, 25, 3,
        //        40, 40, 40, 40, 40, 40, 40, 40,
        //         8, 12, 12, 12, 12, 12, 12, -2,
        //        -2, -2, -2, -2, -2, -2, -2, -2,
        //        -2, -2, 15, 15, 15, 15, 15, -2,
        //        -2, -2, -2, -2, -2, 20, 20, 20,
        //        30, 30, 30, 28, -2, -2, -2, -2,
        //        -2,  7,  7,  7, 35, 35, 35, 35,
        //        35, -2, -2, -2, -2, -2, -2, -2,
        //        -2, -2, -2, -2, -2, -2, -2, -2,
        //        -2, -2, -2, -2, -2, -2 };

        //    // Fill in monster sets one at a time.  Maximum monster starts at 2, continuing to add 2 until you reach index 118, then, at set 128, add 1 to the minimum until you reach the maximum of 80.  255 sets total.
        //    for (int i = 1; i <= 256; i++)
        //    {
        //        // Do not alter fiend, Garland, and Chaos battles.  But do alter the nine pirate fight.  That will be a zone 5 fight.
        //        if ((i >= 116 && i <= 128) && i != 127) continue;

        //        int minimum = (i <= 128 ? 0 : i - 128);
        //        minimum = (minimum > 80 ? 80 : minimum);
        //        int maximum = (i * 2);
        //        maximum = (maximum > 118 ? 118 : maximum);

        //        List<monsterGroups> allGroups = new List<monsterGroups>();
        //        int groups = (r1.Next() % 4) + 1;
        //        int maxMonsters = (r1.Next() % 15) + 1;
        //        int totalMonsters = 0;
        //        int totalGroups = 0;
        //        for (int j = 0; j < groups && totalMonsters < 9; j++)
        //        {
        //            int monsterChoice = (r1.Next() % (maximum - minimum)) + minimum;
        //            int maxSet = r1.Next() % (int)(Math.Ceiling((double)maxMonsters / groups));
        //            int minSet = r1.Next() % maxSet;
        //            minSet = (minSet + totalMonsters > 9 ? 9 - totalMonsters : minSet);
        //            maxSet = (maxSet + totalMonsters > 9 ? 9 - totalMonsters : maxSet);
        //            totalMonsters += maxSet;
        //            totalGroups++;
        //            monsterGroups newGroup = new monsterGroups();
        //            newGroup.monster = monsterChoice;
        //            newGroup.minSet = minSet;
        //            newGroup.maxSet = maxSet;
        //            allGroups.Add(newGroup);
        //        }
        //        int screenMonsters = 0;
        //        bool largeMonster1 = false;
        //        bool largeMonster2 = false;
        //        bool superMonster = false;
        //        foreach (monsterGroups group in allGroups)
        //        {
        //            if (monsterSize[monsterRank[group.monster]] == 3)
        //            {
        //                if (screenMonsters > 0)
        //                    continue;
        //                else
        //                {
        //                    superMonster = true;
        //                    screenMonsters = 9;
        //                    totalGroups = 1;
        //                    break;
        //                }
        //            }
        //            if (monsterSize[monsterRank[group.monster]] == 2)
        //            {

        //            }
        //        }
        //    }
        //}

        //public class monsterGroups
        //{
        //    public int monster = 0;
        //    public int minSet = 0;
        //    public int maxSet = 0;
        //}

        private void randomizeEquipment(Random r1)
        {
            if (chkRandomizeEquipment.Checked)
            {
                // Randomize Weapons
                for (int lnI = 0; lnI < 0x43; lnI++)
                {
                    int byteToUse = 0x2b2fc20 + (lnI * 28);
                    int equip1 = r1.Next() % 0x40;
                    int equip2 = (r1.Next() % 0x40) | equip1;
                    romData[byteToUse + 2] = (byte)(equip1);
                    romData[byteToUse + 3] = (byte)(equip2);

                    romData[byteToUse + 4] = (byte)inverted_power_curve(2, 150, .25, r1);
                    romData[byteToUse + 5] = (byte)inverted_power_curve(2, 150, .25, r1);
                    if (r1.Next() % 10 == 0)
                    {
                        romData[byteToUse + 6] = (byte)inverted_power_curve(1, 50, .2, r1);
                        if (r1.Next() % 5 == 0)
                            romData[byteToUse + 6] = (byte)(256 - (romData[byteToUse + 6]));
                    }
                    else
                        romData[byteToUse + 6] = 0;

                    if (r1.Next() % 4 == 0)
                        romData[byteToUse + 7] = (byte)(r1.Next() % 0x41);
                    else
                        romData[byteToUse + 7] = 0;

                    for (int lnJ = 11; lnJ <= 14; lnJ++)
                    {
                        if (r1.Next() % 9 == 0)
                        {
                            romData[byteToUse + lnJ] = (byte)inverted_power_curve(1, 30, .25, r1);
                            if (r1.Next() % 5 == 0)
                                romData[byteToUse + lnJ] = (byte)(256 - (romData[byteToUse + lnJ]));
                        }
                        else
                            romData[byteToUse + lnJ] = 0;
                    }
                    romData[byteToUse + 15] = (byte)inverted_power_curve(1, 70, .5, r1);
                    double price = Math.Pow(romData[byteToUse + 4], 1.9) +
                                   Math.Pow(romData[byteToUse + 5], 1.85) +
                                   Math.Pow(romData[byteToUse + 6], 2.2) +
                                   Math.Pow(romData[byteToUse + 7], 2.3) +
                                   Math.Pow(romData[byteToUse + 11], 2.7) +
                                   Math.Pow(romData[byteToUse + 12], 2.7) +
                                   Math.Pow(romData[byteToUse + 13], 2.7) +
                                   Math.Pow(romData[byteToUse + 14], 2.7) +
                                   Math.Pow(romData[byteToUse + 15], 2.2);
                    price = Math.Ceiling(price);
                    price = (price > 99999 ? 99999 : price);
                    price = (price < 4 ? 4 : price);
                    int buyPrice = (int)price;
                    int sellPrice = r1.Next() % buyPrice;
                    romData[byteToUse + 20] = (byte)(buyPrice % 256);
                    romData[byteToUse + 21] = (byte)((buyPrice / 256) % 256);
                    romData[byteToUse + 22] = (byte)((buyPrice / 65536) % 256);
                    romData[byteToUse + 23] = (byte)(buyPrice / 1677216);

                    romData[byteToUse + 24] = (byte)(sellPrice % 256);
                    romData[byteToUse + 25] = (byte)((sellPrice / 256) % 256);
                    romData[byteToUse + 26] = (byte)((sellPrice / 65536) % 256);
                    romData[byteToUse + 27] = (byte)(sellPrice / 1677216);
                }

                // Randomize Armor
                for (int lnI = 0; lnI < 0x4b; lnI++)
                {
                    int byteToUse = 0x2b30390 + (lnI * 28);
                    int equip1 = r1.Next() % 0x40;
                    int equip2 = (r1.Next() % 0x40) | equip1;
                    romData[byteToUse + 2] = (byte)(equip1);
                    romData[byteToUse + 3] = (byte)(equip2);

                    romData[byteToUse + 4] = (byte)inverted_power_curve(2, 60, .5, r1);
                    romData[byteToUse + 5] = (byte)inverted_power_curve(2, 40, .5, r1);
                    if (r1.Next() % 6 == 0)
                        romData[byteToUse + 6] = (byte)inverted_power_curve(1, 60, .2, r1);
                    else
                        romData[byteToUse + 6] = 0;

                    if (r1.Next() % 15 == 0)
                        romData[byteToUse + 7] = (byte)(r1.Next() % 0x41);
                    else
                        romData[byteToUse + 7] = 0;

                    for (int lnJ = 10; lnJ <= 13; lnJ++)
                    {
                        if (r1.Next() % 6 == 0)
                        {
                            romData[byteToUse + lnJ] = (byte)inverted_power_curve(1, 30, .5, r1);
                            if (r1.Next() % 5 == 0)
                                romData[byteToUse + lnJ] = (byte)(256 - (romData[byteToUse + lnJ]));
                        }
                        else
                            romData[byteToUse + lnJ] = 0;
                    }

                    for (int lnJ = 14; lnJ <= 15; lnJ++)
                    {
                        if (r1.Next() % 20 == 0)
                            romData[byteToUse + lnJ] = (byte)inverted_power_curve(1, 30, .5, r1);
                        else
                            romData[byteToUse + lnJ] = 0;
                    }

                    double price = Math.Pow(romData[byteToUse + 4], 2.3) +
                                   Math.Pow(romData[byteToUse + 6] + romData[byteToUse + 5], 2) +
                                   Math.Pow(romData[byteToUse + 7], 2.3) +
                                   Math.Pow(romData[byteToUse + 10], 2.7) +
                                   Math.Pow(romData[byteToUse + 11], 2.7) +
                                   Math.Pow(romData[byteToUse + 12], 2.7) +
                                   Math.Pow(romData[byteToUse + 13], 2.7) +
                                   Math.Pow(romData[byteToUse + 14], 2.7) +
                                   Math.Pow(romData[byteToUse + 15], 2.7);
                    price = Math.Ceiling(price);
                    price = (price > 99999 ? 99999 : price);
                    price = (price < 4 ? 4 : price);
                    int buyPrice = (int)price;
                    int sellPrice = r1.Next() % buyPrice;
                    romData[byteToUse + 20] = (byte)(buyPrice % 256);
                    romData[byteToUse + 21] = (byte)((buyPrice / 256) % 256);
                    romData[byteToUse + 22] = (byte)((buyPrice / 65536) % 256);
                    romData[byteToUse + 23] = (byte)(buyPrice / 1677216);

                    romData[byteToUse + 24] = (byte)(sellPrice % 256);
                    romData[byteToUse + 25] = (byte)((sellPrice / 256) % 256);
                    romData[byteToUse + 26] = (byte)((sellPrice / 65536) % 256);
                    romData[byteToUse + 27] = (byte)(sellPrice / 1677216);
                }
            }

            using (StreamWriter writer = File.CreateText(@"c:\bizhawk\ff test\equipment.txt"))
            {
                string header = "";
                header += "DEF".PadLeft(6);
                header += "WGT".PadLeft(6);
                header += "Spell".PadLeft(12);
                header += "STR".PadLeft(6);
                header += "STA".PadLeft(6);
                header += "AGL".PadLeft(6);
                header += "INT".PadLeft(6);
                header += "HP%".PadLeft(6);
                header += "MP%".PadLeft(6);
                header += "<--- ARMOR".PadLeft(20);
                header += "WEAPON --->".PadRight(20);
                header += "ATK".PadLeft(6);
                header += "ACC".PadLeft(6);
                header += "EVA".PadLeft(6);
                header += "Spell".PadLeft(12);
                header += "STR".PadLeft(6);
                header += "STA".PadLeft(6);
                header += "AGL".PadLeft(6);
                header += "INT".PadLeft(6);
                header += "CRT".PadLeft(6);
                writer.WriteLine(header);

                for (int i = 0; i < 0x4b; i++)
                {
                    int byteToUse = 0x2b30390 + (i * 28);
                    string line = "";
                    // Print armor
                    line += romData[byteToUse + 4].ToString().PadLeft(6);
                    line += (0 - (romData[byteToUse + 6] - romData[byteToUse + 5])).ToString().PadLeft(6);
                    line += spellText(romData[byteToUse + 7]).PadLeft(12);
                    line += signedByte(romData[byteToUse + 10]).ToString().PadLeft(6);
                    line += signedByte(romData[byteToUse + 11]).ToString().PadLeft(6);
                    line += signedByte(romData[byteToUse + 12]).ToString().PadLeft(6);
                    line += signedByte(romData[byteToUse + 13]).ToString().PadLeft(6);
                    line += romData[byteToUse + 14].ToString().PadLeft(6);
                    line += romData[byteToUse + 15].ToString().PadLeft(6);
                    line += "   " + armorText(i).PadRight(17);

                    // Print weapon
                    if (i < 0x43)
                    {
                        byteToUse = 0x2b2fc20 + (i * 28);

                        line += weaponText(i).PadLeft(20);
                        line += romData[byteToUse + 4].ToString().PadLeft(6);
                        line += romData[byteToUse + 5].ToString().PadLeft(6);
                        line += romData[byteToUse + 6].ToString().PadLeft(6);
                        line += spellText(romData[byteToUse + 7]).PadLeft(12);
                        line += signedByte(romData[byteToUse + 11]).ToString().PadLeft(6);
                        line += signedByte(romData[byteToUse + 12]).ToString().PadLeft(6);
                        line += signedByte(romData[byteToUse + 13]).ToString().PadLeft(6);
                        line += signedByte(romData[byteToUse + 14]).ToString().PadLeft(6);
                        line += romData[byteToUse + 15].ToString().PadLeft(6);
                    }

                    writer.WriteLine(line);
                }
            }
        }

        private int signedByte(byte number)
        {
            return (number >= 128 ? 0 - (256 - number) : number);
        }

        private string spellText(byte spellToUse)
        {
            switch (spellToUse)
            {
                case 0:
                    return "";
                case 1:
                    return "Cure";
                case 2:
                    return "Dia";
                case 3:
                    return "Protect";
                case 4:
                    return "Blink";
                case 5:
                    return "Blindna";
                case 6:
                    return "Silence";
                case 7:
                    return "Nulshock";
                case 8:
                    return "Invis";
                case 9:
                    return "Cura";
                case 10:
                    return "Diara";
                case 11:
                    return "NulBlaze";
                case 12:
                    return "Heal";
                case 13:
                    return "Poisona";
                case 14:
                    return "Fear";
                case 15:
                    return "NulFrost";
                case 16:
                    return "Vox";
                case 17:
                    return "Curaga";
                case 18:
                    return "Life";
                case 19:
                    return "Diaga";
                case 20:
                    return "Healara";
                case 21:
                    return "Stona";
                case 22:
                    return "Exit";
                case 23:
                    return "Protera";
                case 24:
                    return "Invisira";
                case 25:
                    return "Curaja";
                case 26:
                    return "Diaja";
                case 27:
                    return "NulDeath";
                case 28:
                    return "Healaga";
                case 29:
                    return "Full-Life";
                case 30:
                    return "Holy";
                case 31:
                    return "NulAll";
                case 32:
                    return "Dispel";
                case 33:
                    return "Fire";
                case 34:
                    return "Sleep";
                case 35:
                    return "Focus";
                case 36:
                    return "Thunder";
                case 37:
                    return "Blizzard";
                case 38:
                    return "Dark";
                case 39:
                    return "Temper";
                case 40:
                    return "Slow";
                case 41:
                    return "Fira";
                case 42:
                    return "Hold";
                case 43:
                    return "Thundara";
                case 44:
                    return "Focara";
                case 45:
                    return "Sleepra";
                case 46:
                    return "Haste";
                case 47:
                    return "Confuse";
                case 48:
                    return "Blizzara";
                case 49:
                    return "Firaga";
                case 50:
                    return "Scuroge";
                case 51:
                    return "Teleport";
                case 52:
                    return "Slowra";
                case 53:
                    return "Thundaga";
                case 54:
                    return "Death";
                case 55:
                    return "Quake";
                case 56:
                    return "Stun";
                case 57:
                    return "Blizzaga";
                case 58:
                    return "Break";
                case 59:
                    return "Saber";
                case 60:
                    return "Blind";
                case 61:
                    return "Flare";
                case 62:
                    return "Stop";
                case 63:
                    return "Warp";
                case 64:
                    return "Kill";
            }
            return "";
        }

        private string armorText(int armor)
        {
            switch (armor)
            {
                case 0:
                    return "Clothes";
                case 1:
                    return "Leather Armor";
                case 2:
                    return "Chain Mail";
                case 3:
                    return "Iron Armor";
                case 4:
                    return "Knight's Armor";
                case 5:
                    return "Mythril Mail";
                case 6:
                    return "Flame Mail";
                case 7:
                    return "Ice Armor";
                case 8:
                    return "Diamond Armor";
                case 9:
                    return "Dragon Mail";
                case 10:
                    return "Copper Armlet";
                case 11:
                    return "Silver Armlet";
                case 12:
                    return "Ruby Armlet";
                case 13:
                    return "Diamond Armlet";
                case 14:
                    return "White Robe";
                case 15:
                    return "Black Robe";
                case 16:
                    return "Crystal Mail";
                case 17:
                    return "Thief's Armlet";
                case 18:
                    return "Black Garb";
                case 19:
                    return "Kenpogi";
                case 20:
                    return "Power Sash";
                case 21:
                    return "Red Jacket";
                case 22:
                    return "Sage's Surplice";
                case 23:
                    return "Light Robe";
                case 24:
                    return "Gaia Gear";
                case 25:
                    return "Bard's Tunic";
                case 26:
                    return "Genji Armor";
                case 27:
                    return "Maximillian";
                case 28:
                    return "Survival Vest";
                case 29:
                    return "Lordly Robes";
                case 30:
                    return "Leather Shield";
                case 31:
                    return "Iron Shield";
                case 32:
                    return "Mythril Shield";
                case 33:
                    return "Flame Shield";
                case 34:
                    return "Ice Shield";
                case 35:
                    return "Diamond Shield";
                case 36:
                    return "Aegis Shield";
                case 37:
                    return "Buckler";
                case 38:
                    return "Protect Cloak";
                case 39:
                    return "Genji Shield";
                case 40:
                    return "Crystal Shield";
                case 41:
                    return "Hero's Shield";
                case 42:
                    return "Zephyr Cape";
                case 43:
                    return "Elven Cloak";
                case 44:
                    return "Master Shield";
                case 45:
                    return "Leather Cap";
                case 46:
                    return "Helm";
                case 47:
                    return "Great Helm";
                case 48:
                    return "Mythril Helm";
                case 49:
                    return "Diamond Helm";
                case 50:
                    return "Healing Helm";
                case 51:
                    return "Ribbon";
                case 52:
                    return "Genji Helm";
                case 53:
                    return "Crystal Helm";
                case 54:
                    return "Black Cowl";
                case 55:
                    return "Twist Headband";
                case 56:
                    return "Tiger Mask";
                case 57:
                    return "Feathered Cap";
                case 58:
                    return "Red Cap";
                case 59:
                    return "Wizard's Hat";
                case 60:
                    return "Sage's Mitre";
                case 61:
                    return "Shadow Mask";
                case 62:
                    return "Leather Gloves";
                case 63:
                    return "Bronze Gloves";
                case 64:
                    return "Steel Gloves";
                case 65:
                    return "Mythril Gloves";
                case 66:
                    return "Gauntlets";
                case 67:
                    return "Giant's Gloves";
                case 68:
                    return "Diamond Gloves";
                case 69:
                    return "Protect Gloves";
                case 70:
                    return "Crystal Gloves";
                case 71:
                    return "Thief's Gloves";
                case 72:
                    return "Crystal Ring";
                case 73:
                    return "Angel's Ring";
                case 74:
                    return "Genji Gloves";
            }
            return "";
        }

        private string weaponText(int weapon)
        {
            switch (weapon)
            {
                case 0:
                    return "Nunchaku";
                case 1:
                    return "Knife";
                case 2:
                    return "Staff";
                case 3:
                    return "Rapier";
                case 4:
                    return "Hammer";
                case 5:
                    return "Broadsword";
                case 6:
                    return "Battle Axe";
                case 7:
                    return "Scimitar";
                case 8:
                    return "Iron Nunchaku";
                case 9:
                    return "Dagger";
                case 10:
                    return "Crosier";
                case 11:
                    return "Saber";
                case 12:
                    return "Longsword";
                case 13:
                    return "Great Axe";
                case 14:
                    return "Falchion";
                case 15:
                    return "Mythril Knife";
                case 16:
                    return "Mythril Sword";
                case 17:
                    return "Mythril Hammer";
                case 18:
                    return "Mythril Axe";
                case 19:
                    return "Flame Sword";
                case 20:
                    return "Ice Brand";
                case 21:
                    return "Wyrmkiller";
                case 22:
                    return "Great Sword";
                case 23:
                    return "Sun Blade";
                case 24:
                    return "Coral Sword";
                case 25:
                    return "Werebuster";
                case 26:
                    return "Rune Blade";
                case 27:
                    return "Power Staff";
                case 28:
                    return "Light Axe";
                case 29:
                    return "Healing Staff";
                case 30:
                    return "Mage's Staff";
                case 31:
                    return "Defender";
                case 32:
                    return "Wizard's Staff";
                case 33:
                    return "Vorpal Sword";
                case 34:
                    return "Cat Claws";
                case 35:
                    return "Thor's Hammer";
                case 36:
                    return "Razer";
                case 37:
                    return "Sasuke's Blade";
                case 38:
                    return "Excalibur";
                case 39:
                    return "Masamune";
                case 40:
                    return "Ultima Weapon";
                case 41:
                    return "Ragnarok";
                case 42:
                    return "Murasame";
                case 43:
                    return "Lightbringer";
                case 44:
                    return "Rune Staff";
                case 45:
                    return "Judgment Staff";
                case 46:
                    return "Dark Claymore";
                case 47:
                    return "Duel Rapier";
                case 48:
                    return "Braveheart";
                case 49:
                    return "Deathbringer";
                case 50:
                    return "Enhancer";
                case 51:
                    return "Gigantaxe";
                case 52:
                    return "Viking Axe";
                case 53:
                    return "Rune Axe";
                case 54:
                    return "Ogrekiller";
                case 55:
                    return "Kikuichimonji";
                case 56:
                    return "Asura";
                case 57:
                    return "Kotetsu";
                case 58:
                    return "War Hammer";
                case 59:
                    return "Assassin Dagger";
                case 60:
                    return "Orichalcum";
                case 61:
                    return "Mage Masher";
                case 62:
                    return "Gladius";
                case 63:
                    return "Sage's Staff";
                case 64:
                    return "Barbarian's Sword";
                case 65:
                    return "Lust Dagger";
                case 66:
                    return "Golden Staff";
            }
            return "";

        }

        private void randomizeTreasures(Random r1)
        {
            for (int lnI = 0x2b227d8; lnI < 0x2b22c0a; lnI += 0x04)
            {
                // Skip key items for now.  These key items also set triggers, and moving those around STOPS setting those triggers.
                if (romData[lnI + 3] == 0x80 && romData[lnI + 0] == 0x00)
                {
                    continue;
                }
                // Else, 50% common item, then 50% chance gold, then 50% rare item, then finally a weapon/armor
                if (r1.Next() % 2 == 1)
                {
                    int itemChoice = r1.Next() % 31;
                    int finalItem = 0;
                    switch (itemChoice)
                    {
                        case 0:
                            finalItem = 1; break;
                        case 1:
                            finalItem = 2; break;
                        case 2:
                            finalItem = 4; break;
                        case 3:
                            finalItem = 5; break;
                        case 4:
                            finalItem = 7; break;
                        case 5:
                            finalItem = 9; break;
                        case 6:
                            finalItem = 10; break;
                        case 7:
                            finalItem = 11; break;
                        case 8:
                            finalItem = 12; break;
                        case 9:
                            finalItem = 13; break;
                        case 10:
                            finalItem = 14; break;
                        case 11:
                            finalItem = 15; break;
                        case 12:
                            finalItem = 16; break;
                        case 13:
                            finalItem = 17; break;
                        case 14:
                            finalItem = 19; break;
                        case 15:
                            finalItem = 20; break;
                        case 16:
                            finalItem = 21; break;
                        case 17:
                            finalItem = 22; break;
                        case 18:
                            finalItem = 23; break;
                        case 19:
                            finalItem = 24; break;
                        case 20:
                            finalItem = 25; break;
                        case 21:
                            finalItem = 26; break;
                        case 22:
                            finalItem = 27; break;
                        case 23:
                            finalItem = 28; break;
                        case 24:
                            finalItem = 29; break;
                        case 25:
                            finalItem = 30; break;
                        case 26:
                            finalItem = 31; break;
                        case 27:
                            finalItem = 32; break;
                        case 28:
                            finalItem = 33; break;
                        case 29:
                            finalItem = 34; break;
                        case 30:
                            finalItem = 35; break;
                        default:
                            finalItem = 0; break;
                    }
                    romData[lnI + 3] = 0x80;
                    romData[lnI + 0] = 0x01;
                    romData[lnI + 1] = (byte)finalItem;
                    romData[lnI + 2] = 0x00;
                }
                else if (r1.Next() % 2 == 1)
                {
                    romData[lnI + 3] = 0x00;
                    int gold = inverted_power_curve(1, 100000, .1, r1);
                    // Limit to two significant digits
                    if (gold > 10000)
                        gold = gold / 1000 * 1000;
                    else if (gold > 1000)
                        gold = gold / 100 * 100;
                    else if (gold > 100)
                        gold = gold / 10 * 10;
                    romData[lnI + 0] = (byte)(gold % 256);
                    romData[lnI + 1] = (byte)((gold / 256) % 256);
                    romData[lnI + 2] = (byte)(gold / 65536);
                } else if (r1.Next() % 2 == 1)
                {
                    int itemChoice = r1.Next() % 12;
                    int finalItem = 0;
                    switch (itemChoice)
                    {
                        case 0:
                            finalItem = 3; break;
                        case 1:
                            finalItem = 6; break;
                        case 2:
                            finalItem = 8; break;
                        case 3:
                            finalItem = 18; break;
                        case 4:
                            finalItem = 36; break;
                        case 5:
                            finalItem = 37; break;
                        case 6:
                            finalItem = 38; break;
                        case 7:
                            finalItem = 39; break;
                        case 8:
                            finalItem = 40; break;
                        case 9:
                            finalItem = 41; break;
                        case 10:
                            finalItem = 42; break;
                        case 11:
                            finalItem = 43; break;
                        default:
                            finalItem = 0; break;
                    }

                    romData[lnI + 3] = 0x80;
                    romData[lnI + 2] = 0x00;
                    romData[lnI + 1] = (byte)finalItem;
                    romData[lnI + 0] = 0x01;

                } else
                {
                    int itemBank = (r1.Next() % 2) + 2;
                    int finalItem = 0;
                    if (itemBank == 2)
                        finalItem = r1.Next() % 0x44;
                    else
                        finalItem = r1.Next() % 0x4c;

                    romData[lnI + 0] = (byte)itemBank;
                    romData[lnI + 1] = (byte)finalItem;
                    romData[lnI + 2] = 0x00;
                    romData[lnI + 3] = 0x80;
                }
            }
        }

        private int inverted_power_curve(int min, int max, double powToUse, Random r1)
        {
            int range = max - min;
            double p_range = Math.Pow(range, 1 / powToUse);
            double section = (double)r1.Next() / int.MaxValue;

            return (int)Math.Round(max - Math.Pow(section * p_range, powToUse));
        }

        private bool loadRom(bool extra = false)
        {
            try
            {
                romData = File.ReadAllBytes(txtFileName.Text);
            }
            catch
            {
                MessageBox.Show("Empty file name(s) or unable to open files.  Please verify the files exist.");
                return false;
            }
            return true;
        }

        private void saveRom()
        {
            string finalFile = Path.Combine(Path.GetDirectoryName(txtFileName.Text), Path.GetFileNameWithoutExtension(txtFileName.Text) + "_" + txtSeed.Text + "_" + txtFlags.Text + ".iso");
            File.WriteAllBytes(finalFile, romData);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFileName.Text = openFileDialog1.FileName;
            }

            // Shuffle patterns:  0x894b12c-0x894b1f7.  2c = Straight fight
            // Actual patterns:  0x894b1f8-0x894b6c8.  Bytes 2-9 = Basic pattern, Bytes 11-14 = Special pattern

            // Equipment we'll address later...
        }

        private void trkXPReqAdj_Scroll(object sender, EventArgs e)
        {
            lblXPReqAdj.Text = "XP Req Adjustment = " + (trkXPReqAdj.Value * 5).ToString() + "%";
        }

        private void trkXPBoost_Scroll(object sender, EventArgs e)
        {
            lblXPBoost.Text = "Monster XP Boost = +" + (trkXPBoost.Value * 5).ToString();
        }

        private void trkEncounterRate_Scroll(object sender, EventArgs e)
        {
            lblEncounterRate.Text = "Encounter Rate = " + (trkEncounterRate.Value * 10).ToString() + "%";
        }

        private void cmdPrintInfo_Click(object sender, EventArgs e)
        {
            if (!loadRom()) return;

            printInformation();
        }

        private void determineFlags(object sender, EventArgs e)
        {
            string flags = "";
            int number = (chkRandomizeMonsterZones.Checked ? 1 : 0) + (chkRandomizeSpecialMonsters.Checked ? 2 : 0) + (chkRandomizeMonsterPatterns.Checked ? 4 : 0) + 
                (chkRandomizeEquipment.Checked ? 8 : 0) + (chkRandomizeTreasures.Checked ? 16 : 0);
            flags += convertIntToChar(number);
            flags += convertIntToChar(trkXPReqAdj.Value);
            flags += convertIntToChar(trkXPBoost.Value);
            flags += convertIntToChar(trkEncounterRate.Value);

            txtFlags.Text = flags;
        }

        private void determineChecks(object sender, EventArgs e)
        {
            string flags = txtFlags.Text;
            int number = convertChartoInt(Convert.ToChar(flags.Substring(0, 1)));
            chkRandomizeMonsterZones.Checked = (number % 2 == 1);
            chkRandomizeSpecialMonsters.Checked = (number % 4 >= 2);
            chkRandomizeMonsterPatterns.Checked = (number % 8 >= 4);
            chkRandomizeEquipment.Checked = (number % 16 >= 8);
            chkRandomizeTreasures.Checked = (number % 32 >= 16);
            trkXPReqAdj.Value = convertChartoInt(Convert.ToChar(flags.Substring(1, 1)));
            trkXPReqAdj_Scroll(null, null);
            trkXPBoost.Value = convertChartoInt(Convert.ToChar(flags.Substring(2, 1)));
            trkXPBoost_Scroll(null, null);
            trkEncounterRate.Value = convertChartoInt(Convert.ToChar(flags.Substring(3, 1)));
            trkEncounterRate_Scroll(null, null);
        }

        private string convertIntToChar(int number)
        {
            if (number >= 0 && number <= 9)
                return number.ToString();
            if (number >= 10 && number <= 35)
                return Convert.ToChar(55 + number).ToString();
            if (number >= 36 && number <= 61)
                return Convert.ToChar(61 + number).ToString();
            if (number == 62) return "!";
            if (number == 63) return "@";
            return "";
        }

        private int convertChartoInt(char character)
        {
            if (character >= Convert.ToChar("0") && character <= Convert.ToChar("9"))
                return character - 48;
            if (character >= Convert.ToChar("A") && character <= Convert.ToChar("Z"))
                return character - 55;
            if (character >= Convert.ToChar("a") && character <= Convert.ToChar("z"))
                return character - 61;
            if (character == Convert.ToChar("!")) return 62;
            if (character == Convert.ToChar("@")) return 63;
            return 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Random r1 = new Random();
            txtSeed.Text = r1.Next().ToString();

            try
            {
                using (TextReader reader = File.OpenText("lastFFR.txt"))
                {
                    txtFlags.Text = reader.ReadLine();
                    determineChecks(null, null);
                }
            } catch
            {
                trkXPReqAdj.Value = 20;
                trkXPBoost.Value = 0;
                trkEncounterRate.Value = 10;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (StreamWriter writer = File.CreateText("lastFFR.txt"))
            {
                writer.WriteLine(txtFlags.Text);
            }
        }
    }
}
