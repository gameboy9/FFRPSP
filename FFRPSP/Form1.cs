using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace FFRPSP
{
    public partial class Form1 : Form
    {
        byte[] romData;
        byte[] livePatch;
        bool loading = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnFF1Monsters_Click(object sender, EventArgs e)
        {
            if (!loadRom()) return;            

            adjustEncounterRate();
            adjustXPRequirements();
            boostXP();

            Random r1 = new Random(Convert.ToInt32(txtSeed.Text));
            randomizeEquipment(r1);
            randomizeMonsterPatterns(r1);
            randomizeTreasures(r1);
            randomizeMonsterZonesV2(r1);
            randomizeMagic(r1);
            randomizeStores(r1);
            overrideEboot();

            saveRom();
            lblResults.Text = "Hacking complete!  (" + Path.Combine(Path.GetDirectoryName(txtFileName.Text), Path.GetFileNameWithoutExtension(txtFileName.Text) + "_" + txtSeed.Text + "_" + txtFlags.Text + ".iso)");
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
            using (StreamWriter writer = File.CreateText(@"c:\bizhawk\ff test\magic_info.txt"))
            {
                for (int i = 0; i < 0x50; i++)
                {
                    int byteToUse = 0x2b30d6e + (14 * i);
                    string line = "";
                    for (int j = 0; j < 14; j++)
                    {
                        line += romData[byteToUse + j].ToString("X2") + " ";
                    }
                    writer.WriteLine(line);
                }
            }
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
            for (int lnI = 0x2b216a8; lnI < 0x2b21768; lnI += 2)
            {
                int encounterRate = (romData[lnI + 1] * 256) + romData[lnI];
                encounterRate = encounterRate * trkEncounterRate.Value * 10 / 100;
                romData[lnI] = (byte)(encounterRate % 256);
                romData[lnI + 1] = (byte)(encounterRate / 256);
            }
        }

        private void adjustXPRequirements()
        {
            for (int lnI = 0x2b2f438; lnI < 0x2b2f5c0; lnI += 4)
            {
                int xpRequired = (romData[lnI + 3] * 16777216) + (romData[lnI + 2] * 65536) + (romData[lnI + 1] * 256) + romData[lnI];
                xpRequired = xpRequired * trkXPReqAdj.Value * 5 / 100;
                romData[lnI] = (byte)(xpRequired % 256);
                romData[lnI + 1] = (byte)((xpRequired / 256) % 256);
                romData[lnI + 2] = (byte)((xpRequired / 65536) % 256);
                romData[lnI + 3] = (byte)(xpRequired / 1677216);
            }
        }

        private void boostXP()
        {
            if (trkXPBoost.Value != 0)
            {
                for (int lnI = 0x2b28480; lnI < 0x2b28480 + (203 * 0x24); lnI += 0x24)
                {
                    int XP = (romData[lnI + 1] * 256) + romData[lnI];
                    XP += (trkXPBoost.Value * 5);
                    romData[lnI] = (byte)(XP % 256);
                    romData[lnI + 1] = (byte)(XP / 256);
                    int GP = (romData[lnI + 3] * 256) + romData[lnI + 2];
                    GP += (trkXPBoost.Value * 5);
                    romData[lnI + 2] = (byte)(GP % 256);
                    romData[lnI + 3] = (byte)(GP / 256);
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

        private void randomizeMonsterZonesV2(Random r1)
        {
            if (chkRandomizeMonsterZones.Checked)
            {
                // Mark overworld zones, then mark caves
                // Overworld starts at 0x2b218e4
                int[] overworldZones = { 10, 10, 10, 10, 10, 10, 10, 10,
                                       10, 10, 10, 10, 10, 10, 10, 10,
                                       10, 10, -1, 10, -1, 10, 10, 10,
                                       -1, -1, -1,  3,  3,  4, -1, 10,
                                       -1,  7,  5,  5,  2,  4,  4, 10,
                                        7,  7,  7,  5,  1,  5, 10, 10,
                                       -1, -1,  6,  5,  5,  8, 10, 10,
                                       -1, -1,  6,  6,  5,  8, 10, 10 };

                // Caves start at 0x2b21ff0
                int[] caveZones =
                    { -2, 9, 9, 9, 9, 9, 9, -2,
                    -2, -2, -2, -2, -2, -2, -2, -2,
                    -2, -2, -2, -2, -2, -2, -2, 25, // 0x10
                    25, 25, 25, 25, 25, 25, 25, 3,
                    40, 40, 40, 40, 40, 40, 40, 40, // 0x20
                     8, 12, 12, 12, 12, 12, 12, -2,
                    -2, -2, -2, -2, -2, -2, -2, -2, // 0x30
                    -2, -2, -2, -2, -2, -2, -2, -2,
                    -2, -2, 15, 15, 15, 15, 15, -2, // 0x40
                    -2, -2, -2, -2, -2, 20, 20, 20,
                    30, 30, 30, 28, -2, -2, -2, -2, // 0x50
                    -2,  7,  7,  7, 35, 35, 35, 35,
                    35, -2, -2, -2, -2, -2, -2, -2, // 0x60
                    -2, -2, -2, -2, -2, -2, -2, -2,
                    -2, -2, -2, -2, -2, -2 };       // 0x70

                int[] battleRank =
                {
                    0x00, 0x80, 0x82, 0x05, 0x01, 0x04, 0x06, 0x86, 0x07, 0x02, 0x81, 0x03, 0x09, 0x08, 0x83, 0x84,
                    0x7f, 0x7e, 0x85, 0x87, 0x0a, 0x8a, 0x0b, 0x0c, 0x8b, 0x0d, 0x8e, 0x5b, 0xdb, 0xdc, 0x5c, 0xdd,
                    0x5d, 0x5e, 0xde, 0xe6, 0x66, 0x14, 0x94, 0x12, 0x92, 0x0f, 0x88, 0x8d, 0x8c, 0x0e, 0x8f, 0x10,
                    0x90, 0x91, 0x11, 0x13, 0x93, 0x15, 0x1e, 0x1c, 0x9c, 0x95, 0x2b, 0x89, 0x16, 0x96, 0x17, 0xab,
                    0x9f, 0x97, 0x9e, 0x18, 0x19, 0x1a, 0x1b, 0x1d, 0x1f, 0x98, 0x99, 0x9a, 0x9b, 0x9d, 0x20, 0xa0,
                    0x21, 0x22, 0xa2, 0x7d, 0x23, 0xa3, 0x24, 0xa4, 0x25, 0xa5, 0x2c, 0x26, 0x27, 0xa6, 0xa7, 0x28,
                    0x29, 0x2a, 0xa8, 0xac, 0xaa, 0x2d, 0xad, 0x2e, 0x2f, 0xaf, 0xa9, 0xae, 0xa1, 0x30, 0x31, 0x32,
                    0xb2, 0xb5, 0x33, 0x34, 0x35, 0xb4, 0x36, 0x37, 0xd5, 0xb7, 0xb6, 0xbe, 0x39, 0x3a, 0x3b, 0xba,
                    0xb9, 0xbb, 0x3c, 0x3d, 0xbd, 0xbc, 0x3f, 0xb0, 0xb1, 0xb3, 0xbf, 0x40, 0xc0, 0x41, 0xc1, 0x42,
                    0xc2, 0x43, 0xc3, 0xc4, 0x44, 0x45, 0xcb, 0x47, 0x48, 0x49, 0xca, 0xc9, 0xcc, 0x7c, 0x4b, 0x4c,
                    0x4d, 0xcf, 0x4f, 0xc5, 0xc7, 0xc8, 0x4a, 0xcd, 0x50, 0xd0, 0x51, 0xd1, 0x52, 0xd2, 0x53, 0xd3,
                    0x54, 0xd4, 0x58, 0xd8, 0x5a, 0xda, 0x5f, 0xdf, 0xd6, 0xe0, 0x60, 0xe1, 0x61, 0x62, 0xe2, 0x63,
                    0xe3, 0x64, 0xe4, 0x65, 0xe5, 0x67, 0xe7, 0x68, 0x6a, 0xea, 0x6b, 0xeb, 0x6d, 0xed, 0xee, 0x6e,
                    0x6f, 0xef, 0x70, 0xf0, 0x72, 0xf2, 0x4e, 0xce, 0xc6, 0xe8, 0x55, 0x57, 0xd7, 0x69, 0x6c, 0xec,
                    0xe9, 0x38, 0xb8, 0x71, 0xf1, 0x59, 0xd9, 0x3e, 0x46, 0x56, 0x7a, 0x79, 0x78, 0x77, 0x73, 0x74, 0x75, 0x76

                };

                for (int lnI = 0; lnI < caveZones.Length; lnI++)
                {
                    int lowZone = 0;
                    int highZone = 0;

                    switch (caveZones[lnI])
                    {
                        case -2:
                            lowZone = 0; highZone = 0; break;
                        case 3:
                            lowZone = 0; highZone = 3; break;
                        case 7:
                            lowZone = 0; highZone = 66; break;
                        case 8:
                            lowZone = 0; highZone = 83; break;
                        case 9:
                            lowZone = 0; highZone = 90; break;
                        case 12:
                            lowZone = 22; highZone = 237; break;
                        case 15:
                            lowZone = 51; highZone = 237; break;
                        case 20:
                            lowZone = 67; highZone = 237; break;
                        case 25:
                            lowZone = 91; highZone = 241; break;
                        case 28:
                            lowZone = 100; highZone = 241; break;
                        case 30:
                            lowZone = 113; highZone = 241; break;
                        case 35:
                            lowZone = 130; highZone = 241; break;
                        case 40:
                            lowZone = 150; highZone = 241; break;
                        default:
                            lowZone = 0; highZone = 0; break;
                    }

                    int byteToUse = 0x2b21ff0 + (0x8 * lnI); // s/b 2b21ff0?!
                    for (int lnJ = 0; lnJ < 8; lnJ++)
                        romData[byteToUse + lnJ] = (byte)(battleRank[r1.Next() % (highZone - lowZone + 1) + lowZone]);
                }

                for (int lnI = 0; lnI < overworldZones.Length; lnI++)
                {
                    int lowZone = 0;
                    int highZone = 0;

                    switch (overworldZones[lnI])
                    {
                        case -1:
                            lowZone = 0; highZone = 0; break;
                        case 1:
                            lowZone = 0; highZone = 3; break;
                        case 2:
                            lowZone = 0; highZone = 6; break;
                        case 3:
                            lowZone = 0; highZone = 12; break;
                        case 4:
                            lowZone = 0; highZone = 24; break;
                        case 5:
                            lowZone = 0; highZone = 38; break;
                        case 6:
                            lowZone = 0; highZone = 48; break;
                        case 7:
                            lowZone = 0; highZone = 66; break;
                        case 8:
                            lowZone = 0; highZone = 83; break;
                        case 9:
                            lowZone = 0; highZone = 0; break;
                        case 10:
                            lowZone = 0; highZone = 237; break;
                        default:
                            lowZone = 0; highZone = 0; break;
                    }

                    int byteToUse = 0x2b218e4 + (0x8 * lnI);
                    for (int lnJ = 0; lnJ < 8; lnJ++)
                        romData[byteToUse + lnJ] = (byte)(battleRank[r1.Next() % (highZone - lowZone + 1) + lowZone]);
                }
            }
        }

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

                    //romData[byteToUse + 4] = (byte)inverted_power_curve(2, 150, .25, r1);
                    //romData[byteToUse + 5] = (byte)inverted_power_curve(2, 150, .25, r1);
                    //if (r1.Next() % 10 == 0)
                    //{
                    //    romData[byteToUse + 6] = (byte)inverted_power_curve(1, 50, .2, r1);
                    //    if (r1.Next() % 5 == 0)
                    //        romData[byteToUse + 6] = (byte)(256 - (romData[byteToUse + 6]));
                    //}
                    //else
                    //    romData[byteToUse + 6] = 0;

                    //if (r1.Next() % 4 == 0)
                    //    romData[byteToUse + 7] = (byte)(r1.Next() % 0x41);
                    //else
                    //    romData[byteToUse + 7] = 0;
                    //if (romData[byteToUse + 7] == 0x16 || romData[byteToUse + 7] == 0x33)
                    //    romData[byteToUse + 7] = 0;

                    //for (int lnJ = 11; lnJ <= 14; lnJ++)
                    //{
                    //    if (r1.Next() % 9 == 0)
                    //    {
                    //        romData[byteToUse + lnJ] = (byte)inverted_power_curve(1, 30, .25, r1);
                    //        if (r1.Next() % 5 == 0)
                    //            romData[byteToUse + lnJ] = (byte)(256 - (romData[byteToUse + lnJ]));
                    //    }
                    //    else
                    //        romData[byteToUse + lnJ] = 0;
                    //}
                    //romData[byteToUse + 15] = (byte)inverted_power_curve(1, 70, .5, r1);
                    //int spellPrice = (romData[byteToUse + 7] > 32 ? romData[byteToUse + 7] - 32 : romData[byteToUse + 7]);
                    //double price = Math.Pow(romData[byteToUse + 4], 1.9) +
                    //               Math.Pow(romData[byteToUse + 5], 1.85) +
                    //               Math.Pow((romData[byteToUse + 6] > 128 ? 0 : romData[byteToUse + 6]), 2.2) +
                    //               Math.Pow(spellPrice % 32, 2.7) +
                    //               Math.Pow((romData[byteToUse + 11] > 128 ? 0 : romData[byteToUse + 11]), 2.7) +
                    //               Math.Pow((romData[byteToUse + 12] > 128 ? 0 : romData[byteToUse + 12]), 2.7) +
                    //               Math.Pow((romData[byteToUse + 13] > 128 ? 0 : romData[byteToUse + 13]), 2.7) +
                    //               Math.Pow((romData[byteToUse + 14] > 128 ? 0 : romData[byteToUse + 14]), 2.7) +
                    //               Math.Pow((romData[byteToUse + 15] > 128 ? 0 : romData[byteToUse + 15]), 2.2);
                    //price = Math.Ceiling(price);
                    //price = (price > 99999 ? 99999 : price);
                    //price = (price < 4 ? 4 : price);
                    //int buyPrice = (int)price;
                    //buyPrice = buyPrice * trkXPReqAdj.Value * 5 / 100;
                    //buyPrice = ScaleValue(buyPrice, trkRandomPrices.Value / 10, 1.0, r1);

                    //int sellPrice = r1.Next() % buyPrice;
                    //romData[byteToUse + 20] = (byte)(buyPrice % 256);
                    //romData[byteToUse + 21] = (byte)((buyPrice / 256) % 256);
                    //romData[byteToUse + 22] = (byte)((buyPrice / 65536) % 256);
                    //romData[byteToUse + 23] = (byte)(buyPrice / 1677216);

                    //romData[byteToUse + 24] = (byte)(sellPrice % 256);
                    //romData[byteToUse + 25] = (byte)((sellPrice / 256) % 256);
                    //romData[byteToUse + 26] = (byte)((sellPrice / 65536) % 256);
                    //romData[byteToUse + 27] = (byte)(sellPrice / 1677216);
                }

                // Randomize Armor
                for (int lnI = 0; lnI < 0x4b; lnI++)
                {
                    int byteToUse = 0x2b30390 + (lnI * 28);
                    int equip1 = r1.Next() % 0x40;
                    int equip2 = (r1.Next() % 0x40) | equip1;
                    romData[byteToUse + 2] = (byte)(equip1);
                    romData[byteToUse + 3] = (byte)(equip2);

                    //romData[byteToUse + 4] = (byte)inverted_power_curve(2, 60, .5, r1);
                    //romData[byteToUse + 5] = (byte)inverted_power_curve(2, 40, .5, r1);
                    //if (r1.Next() % 6 == 0)
                    //    romData[byteToUse + 6] = (byte)inverted_power_curve(1, 60, .2, r1);
                    //else
                    //    romData[byteToUse + 6] = 0;

                    //if (r1.Next() % 15 == 0)
                    //    romData[byteToUse + 7] = (byte)(r1.Next() % 0x41);
                    //else
                    //    romData[byteToUse + 7] = 0;

                    //for (int lnJ = 10; lnJ <= 13; lnJ++)
                    //{
                    //    if (r1.Next() % 6 == 0)
                    //    {
                    //        romData[byteToUse + lnJ] = (byte)inverted_power_curve(1, 30, .5, r1);
                    //        if (r1.Next() % 5 == 0)
                    //            romData[byteToUse + lnJ] = (byte)(256 - (romData[byteToUse + lnJ]));
                    //    }
                    //    else
                    //        romData[byteToUse + lnJ] = 0;
                    //}

                    //for (int lnJ = 14; lnJ <= 15; lnJ++)
                    //{
                    //    if (r1.Next() % 20 == 0)
                    //        romData[byteToUse + lnJ] = (byte)inverted_power_curve(1, 30, .5, r1);
                    //    else
                    //        romData[byteToUse + lnJ] = 0;
                    //}

                    //int spellPrice = (romData[byteToUse + 7] > 32 ? romData[byteToUse + 7] - 32 : romData[byteToUse + 7]);
                    //double price = Math.Pow(romData[byteToUse + 4], 2.3) +
                    //               Math.Pow(romData[byteToUse + 6] + romData[byteToUse + 5], 2) +
                    //               Math.Pow(spellPrice, 2.7) +
                    //               Math.Pow((romData[byteToUse + 10] > 128 ? 0 : romData[byteToUse + 10]), 2.7) +
                    //               Math.Pow((romData[byteToUse + 11] > 128 ? 0 : romData[byteToUse + 11]), 2.7) +
                    //               Math.Pow((romData[byteToUse + 12] > 128 ? 0 : romData[byteToUse + 12]), 2.7) +
                    //               Math.Pow((romData[byteToUse + 13] > 128 ? 0 : romData[byteToUse + 13]), 2.7) +
                    //               Math.Pow(romData[byteToUse + 14], 2.7) +
                    //               Math.Pow(romData[byteToUse + 15], 2.7);
                    //price = Math.Ceiling(price);
                    //price = (price > 99999 ? 99999 : price);
                    //price = (price < 4 ? 4 : price);
                    //int buyPrice = (int)price;
                    //buyPrice = buyPrice * trkXPReqAdj.Value * 5 / 100;
                    //buyPrice = ScaleValue(buyPrice, trkRandomPrices.Value / 10, 1.0, r1);

                    //int sellPrice = r1.Next() % buyPrice;
                    //romData[byteToUse + 20] = (byte)(buyPrice % 256);
                    //romData[byteToUse + 21] = (byte)((buyPrice / 256) % 256);
                    //romData[byteToUse + 22] = (byte)((buyPrice / 65536) % 256);
                    //romData[byteToUse + 23] = (byte)(buyPrice / 1677216);

                    //romData[byteToUse + 24] = (byte)(sellPrice % 256);
                    //romData[byteToUse + 25] = (byte)((sellPrice / 256) % 256);
                    //romData[byteToUse + 26] = (byte)((sellPrice / 65536) % 256);
                    //romData[byteToUse + 27] = (byte)(sellPrice / 1677216);
                }
            }

            using (StreamWriter writer = File.CreateText(
              Path.Combine(Path.GetDirectoryName(txtFileName.Text), Path.GetFileNameWithoutExtension(txtFileName.Text) + "_" + txtSeed.Text + "_" + txtFlags.Text + "_equipment.txt")))
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
            if (chkRandomizeTreasures.Checked)
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
                        int[] finalItemArray = { 1, 2, 4, 5, 7, 9, 10, 11, 12, 13,
                            14, 15, 16, 17, 19, 20, 21, 22, 23, 24,
                            25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35 };
                        int finalItem = finalItemArray[itemChoice];

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
                    }
                    else if (r1.Next() % 2 == 1)
                    {
                        int itemChoice = r1.Next() % 12;
                        int[] finalItemArray = { 3, 6, 8, 18, 36, 37, 38, 39, 40, 41, 42, 43 };
                        int finalItem = finalItemArray[itemChoice];

                        romData[lnI + 3] = 0x80;
                        romData[lnI + 2] = 0x00;
                        romData[lnI + 1] = (byte)finalItem;
                        romData[lnI + 0] = 0x01;

                    }
                    else
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
        }

        private int ScaleValue(int value, double scale, double adjustment, Random r1)
        {
            var exponent = (double)r1.Next() / int.MaxValue * 2.0 - 1.0;
            var adjustedScale = 1.0 + adjustment * (scale - 1.0);

            return (int)Math.Round(Math.Pow(adjustedScale, exponent) * value, MidpointRounding.AwayFromZero);
        }

        private void randomizeStores(Random r1)
        {
            // Need to set buy prices for various items... start at 0x2b2f95b
            int[] itemPrices = 
                { 40, 150, 1500, 150, 500, 10000, 15000, 100000,
                  500, 1500, 50, 500, 50, 50, 1000, 50,
                  160, 2000, 300, 500, 500, 500, 1000, 300,
                  300, 300, 500, 6500, 1000, 5000, 1500, 1500,
                  1000, 1000, 1000, 50000, 30000, 100000, 100000, 100000,
                  100000, 100000, 100000 };
            for (int lnI = 0; lnI < 0x2b; lnI++)
            {
                int byteToUse = 0x2b2f95c + (0x10 * lnI);
                itemPrices[lnI] = itemPrices[lnI] * trkGilReqAdj.Value * 5 / 100;

                itemPrices[lnI] = ScaleValue(itemPrices[lnI], trkRandomPrices.Value / 10, 1.0, r1);
                itemPrices[lnI] = Math.Min(99999, itemPrices[lnI]);

                romData[byteToUse + 0] = (byte)(itemPrices[lnI] % 256);
                romData[byteToUse + 1] = (byte)((itemPrices[lnI] / 256) % 256);
                romData[byteToUse + 2] = (byte)((itemPrices[lnI] / 65536) % 256);
                romData[byteToUse + 3] = 0;
            }

            // Randomize magic store prices - 0x2b30d6e + 12/13
            for (int lnI = 0; lnI <= 64; lnI++)
            {
                int byteToUse = (0x2b30d6e + (14 * lnI) + 12);
                int price = (romData[byteToUse + 1] * 256) + romData[byteToUse];
                price = price * trkGilReqAdj.Value * 5 / 100;
                price = ScaleValue(price, trkRandomPrices.Value / 10, 1.0, r1);
                price = (price > 65500 ? 65500 : price);
                romData[byteToUse] = (byte)(price % 256);
                romData[byteToUse + 1] = (byte)(price / 256);
            }

            // Randomize weapon store prices
            for (int lnI = 0; lnI < 0x43; lnI++)
            {
                int byteToUse = 0x2b2fc20 + (lnI * 28);
                int price = romData[byteToUse + 20] + (romData[byteToUse + 21] * 256) + (romData[byteToUse + 22] * 65536);
                if (lnI == 40) price = 50000;
                if (lnI == 41) price = 100000;
                if (lnI == 42) price = 50000;
                if (lnI == 43) price = 77777;
                if (lnI == 44) price = 60000;
                if (lnI == 45) price = 100000;
                price = price * trkGilReqAdj.Value * 5 / 100;
                price = ScaleValue(price, trkRandomPrices.Value / 10, 1.0, r1);
                price = Math.Min(99999, price);
                romData[byteToUse + 20] = (byte)(price % 256);
                romData[byteToUse + 21] = (byte)((price % 65536) / 256);
                romData[byteToUse + 22] = (byte)(price / 65536);

                price /= 2;
                romData[byteToUse + 24] = (byte)(price % 256);
                romData[byteToUse + 25] = (byte)((price % 65536) / 256);
                romData[byteToUse + 26] = (byte)(price / 65536);
            }

            // Randomize armor store prices
            for (int lnI = 0; lnI < 0x4b; lnI++)
            {
                int byteToUse = 0x2b30390 + (lnI * 28);

                int price = romData[byteToUse + 20] + (romData[byteToUse + 21] * 256) + (romData[byteToUse + 22] * 65536);
                price = price * trkGilReqAdj.Value * 5 / 100;
                price = ScaleValue(price, trkRandomPrices.Value / 10, 1.0, r1);
                price = Math.Min(99999, price);
                romData[byteToUse + 20] = (byte)(price % 256);
                romData[byteToUse + 21] = (byte)((price % 65536) / 256);
                romData[byteToUse + 22] = (byte)(price / 65536);

                price /= 2;
                romData[byteToUse + 24] = (byte)(price % 256);
                romData[byteToUse + 25] = (byte)((price % 65536) / 256);
                romData[byteToUse + 26] = (byte)(price / 65536);
            }

            if (chkRandomizeItemStores.Checked || chkRandomizeEquipStores.Checked || chkRandomizeMagicStores.Checked)
            {
                int[] cityStarts = { 0, 28, 60, 100, 124, 156, 172, 204, 216 };
                // Order:  weapons, armor, items, white 1, white 2, black 1, black 2
                int[,] stores =
                {
                    { 1, 1, 1, 1, 1, -1, 1, -1, -1 },
                    { 9, 9, 9, 9, 9, -1, 5, -1, -1 },
                    { 13, 17, 17, -1, 17, 1, 9, -1, 0 },
                    { 20, 24, 24, 16, 24, 8, 16, 0, -1 },
                    { -1, -1, 28, -1, -1, -1, 20, -1, -1 },
                    { 24, 28, 32, 20, 28, 12, 24, 4, -1 },
                    { -1, -1, 36, -1, -1, -1, 28, -1, -1 }
                };
                int[,] storeSizes =
                {
                    { 5, 4, 4, 4, 4, -1, 1, -1, -1 }, // Weapons
                    { 3, 5, 5, 5, 5, -1, 2, -1, -1 }, // Armor
                    { 7, 7, 7, -1, 7, 7, 7, -1, 7 }, // Items
                    { 4, 4, 4, 4, 4, 2, 2, 1, -1 }, // White Magic
                    { -1, -1, 4, -1, -1, -1, 3, -1, -1 }, // White Magic 2
                    { 4, 4, 4, 4, 4, 2, 2, 1, -1 }, // Black Magic
                    { -1, -1, 4, -1, -1, -1, 3, -1, -1 } // Black Magic 2
                };

                if (chkRandomizeItemStores.Checked)
                {
                    // Ensure item stores have seven slots instead of four or five.
                    romData[0x2b1a314] = romData[0x2b1a33c] = romData[0x2b1a364] = romData[0x2b1a3bc] = romData[0x2b1a3d4] = romData[0x2b1a3fc] = 0x37;
                    romData[0x2b1a43c] = 0x47;

                    romData[0x2b1a600] = romData[0x2b1a628] = romData[0x2b1a650] = romData[0x2b1a6a8] = romData[0x2b1a6c0] = romData[0x2b1a6e8] = 0x37;
                    romData[0x2b1a728] = 0x47;
                }

                int[] minNumber = { 1, 1, 1, 1, 1, 0x21, 0x21 };
                int[] maxNumber = { 0x43, 0x4b, 0x2b, 0x20, 0x20, 0x40, 0x40 };

                int[] commonItems = { 0x01, 0x02, 0x04, 0x05, 0x09, 0x0b, 0x0c, 0x0f, 0x10, 0x11 };
                int[] rareItems = { 0x03, 0x06, 0x07, 0x08, 0x0a, 0x1f, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b };
                for (int lnI = 0; lnI < 9; lnI++)
                    for (int lnJ = 0; lnJ < 7; lnJ++)
                    {
                        if ((lnJ == 0 || lnJ == 1) && !chkRandomizeEquipStores.Checked) continue;
                        if (lnJ == 2 && !chkRandomizeItemStores.Checked) continue;
                        if (lnJ >= 3 && !chkRandomizeMagicStores.Checked) continue;

                        List<int> storeStack = new List<int>();
                        for (int lnK = 0; lnK < storeSizes[lnJ, lnI]; lnK++)
                        {
                            if (stores[lnJ, lnI] == -1) continue;
                            int byteToUse = 0x2b1a1dc + (cityStarts[lnI] + stores[lnJ, lnI]) + lnK;
                            int byteToUse2 = 0x2b1a4c8 + (cityStarts[lnI] + stores[lnJ, lnI]) + lnK;
                            if (lnJ == 2 && (((lnI == 0 || lnI == 1) && lnK <= 4) || ((lnI == 2 || lnI == 3) && lnK <= 1)))
                            {
                                romData[byteToUse] = (byte)(commonItems[r1.Next() % commonItems.Length]);
                                romData[byteToUse2] = (byte)(commonItems[r1.Next() % commonItems.Length]);
                            }
                            else if (lnJ == 2 && (((lnI == 5 || lnI == 6) && lnK <= 1) || lnI == 8))
                            {
                                romData[byteToUse] = (byte)(rareItems[r1.Next() % rareItems.Length]);
                                romData[byteToUse2] = (byte)(rareItems[r1.Next() % rareItems.Length]);
                            }
                            else
                            {
                                romData[byteToUse] = (byte)(r1.Next() % (maxNumber[lnJ] - minNumber[lnJ] + 1) + minNumber[lnJ]);
                                romData[byteToUse2] = (byte)(r1.Next() % (maxNumber[lnJ] - minNumber[lnJ] + 1) + minNumber[lnJ]);
                            }
                            if (storeStack.IndexOf(romData[byteToUse]) != -1) lnK--;
                            else storeStack.Add(romData[byteToUse]);
                        }
                    }
            }

            if (chkShuffleMagicStores.Checked)
            {
                int[] whiteMagicStores = { 0x2b1a1f0, 0x2b1a1f1, 0x2b1a1f2, 0x2b1a1f3, 0x2b1a210, 0x2b1a211, 0x2b1a212, 0x2b1a213,
                                           0x2b1a230, 0x2b1a231, 0x2b1a232, 0x2b1a233, 0x2b1a234, 0x2b1a235, 0x2b1a236, 0x2b1a237,
                                           0x2b1a250, 0x2b1a251, 0x2b1a252, 0x2b1a253, 0x2b1a270, 0x2b1a271, 0x2b1a272, 0x2b1a273,
                                           0x2b1a280, 0x2b1a281, 0x2b1a298, 0x2b1a299, 0x2b1a29c, 0x2b1a29d, 0x2b1a29e, 0x2b1a2a8 };
                int[] blackMagicStores = { 0x2b1a1f4, 0x2b1a1f5, 0x2b1a1f6, 0x2b1a1f7, 0x2b1a214, 0x2b1a215, 0x2b1a216, 0x2b1a217,
                                           0x2b1a238, 0x2b1a239, 0x2b1a23a, 0x2b1a23b, 0x2b1a23c, 0x2b1a23d, 0x2b1a23e, 0x2b1a23f,
                                           0x2b1a254, 0x2b1a255, 0x2b1a256, 0x2b1a257, 0x2b1a274, 0x2b1a275, 0x2b1a276, 0x2b1a277,
                                           0x2b1a284, 0x2b1a285, 0x2b1a2a0, 0x2b1a2a1, 0x2b1a2a4, 0x2b1a2a5, 0x2b1a2a6, 0x2b1a2ac };

                for (int lnI = 0; lnI < 32 * 40; lnI++)
                {
                    int first = r1.Next() % 32;
                    int second = r1.Next() % 32;

                    byte hold = romData[whiteMagicStores[first]];
                    romData[whiteMagicStores[first]] = romData[whiteMagicStores[second]];
                    romData[whiteMagicStores[second]] = hold;

                    first = r1.Next() % 32;
                    second = r1.Next() % 32;

                    hold = romData[blackMagicStores[first]];
                    romData[blackMagicStores[first]] = romData[blackMagicStores[second]];
                    romData[blackMagicStores[second]] = hold;
                }
            }
        }

        private void randomizeMagic(Random r1)
        {
            if (chkRandomizeMagic.Checked)
            {
                // Allow all characters to have level 8 magic immediately.
                romData[0x2b316d0] = 0x08;
                romData[0x2b316e0] = 0x08;
                romData[0x2b316f0] = 0x08;
                romData[0x2b31700] = 0x08;
                romData[0x2b31710] = 0x08;
                romData[0x2b31720] = 0x08;
                // Remove the check for who you are to allow level 8 magic for all characters.
                romData[0x2aacd9c] = romData[0x2aad8b0] = 0x00;
                romData[0x2aacd9d] = romData[0x2aad8b1] = 0x00;
                romData[0x2aacd9e] = romData[0x2aad8b2] = 0x01;
                romData[0x2aacd9f] = romData[0x2aad8b3] = 0x24;

                // Randomize "level" of magic.  This will set price and MP cost instead of the actual magic level.
                for (int lnI = 0; lnI < 64; lnI++)
                {
                    int byteToUseMagic = 0x2b30d6e + (lnI * 14);
                    int magicLevel = r1.Next() % 8;
                    int mpCost = (magicLevel == 0 ? 5 : magicLevel == 1 ? 8 : magicLevel == 2 ? 10 : magicLevel == 3 ? 15 : magicLevel == 4 ? 20 : magicLevel == 5 ? 25 : magicLevel == 6 ? 35 : 50);

                    int gilCost = (magicLevel == 0 ? 50 : magicLevel == 1 ? 250 : magicLevel == 2 ? 1000 : magicLevel == 3 ? 2500 : magicLevel == 4 ? 4000 : magicLevel == 5 ? 13000 : magicLevel == 6 ? 30000 : 40000);

                    romData[byteToUseMagic + 10] = (byte)mpCost;
                    romData[byteToUseMagic + 12] = (byte)(gilCost % 256);
                    romData[byteToUseMagic + 13] = (byte)(gilCost / 256);
                }

                // Randomize who can learn magic.  75% chance for each spell for each of the white/black mages, 100% for the white/black wizards.  50% chance for red mages, an additional 50% for red wizard.  33% chance for Ninjas.
                for (int lnI = 0; lnI < 64; lnI++)
                {
                    int byteToUse = 0x2b313ca + (2 * lnI);
                    romData[byteToUse] = 0;
                    romData[byteToUse + 1] = (byte)(lnI < 32 ? 0x10 : 0x20);

                    bool firstChance = (r1.Next() % 4 > 0);
                    if (firstChance) romData[byteToUse] += (byte)(lnI < 32 ? 0x10 : 0x20);

                    firstChance = (r1.Next() % 2 == 0);
                    if (firstChance) romData[byteToUse] += 0x08;
                    if (firstChance) romData[byteToUse + 1] += 0x08;

                    if (!firstChance)
                    {
                        firstChance = (r1.Next() % 2 == 0);
                        if (firstChance) romData[byteToUse + 1] += 0x08;
                    }

                    firstChance = (r1.Next() % 3 == 0);
                    if (firstChance) romData[byteToUse + 1] += (byte)(lnI < 32 ? 0x01 : 0x02);
                }

                //// Need to set up a "magic array" in order to resolve the field magic bug.
                //byte[] magicNumbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64 };
                //// We'll want to shuffle not only the magic attributes, but the text as well.  We'll need several holding variables...
                //byte[] textHold = { 0, 0 };
                //byte[] magicHold = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                //// Shuffle white magic, then black magic.
                //int byteToUseText = 0x2b30bc8;
                //int byteToUseMagic = 0x2b30d6e;

                //for (int lnK = 0; lnK < 2; lnK++)
                //    for (int lnI = 0; lnI < 32 * 40; lnI++)
                //    {
                //        int firstMagic = r1.Next() % 32 + (32 * lnK);
                //        int secondMagic = r1.Next() % 32 + (32 * lnK);

                //        byte magicHoldInt = magicNumbers[firstMagic];
                //        magicNumbers[firstMagic] = magicNumbers[secondMagic];
                //        magicNumbers[secondMagic] = magicHoldInt;

                //        ////////////////////////////////////////////////////////////

                //        textHold[0] = romData[byteToUseText + (firstMagic * 4) + 0];
                //        textHold[1] = romData[byteToUseText + (firstMagic * 4) + 2];

                //        for (int lnJ = 0; lnJ < 9; lnJ++)
                //            magicHold[lnJ] = romData[byteToUseMagic + (firstMagic * 14) + lnJ];

                //        romData[byteToUseText + (firstMagic * 4) + 0] = romData[byteToUseText + (secondMagic * 4) + 0];
                //        romData[byteToUseText + (firstMagic * 4) + 2] = romData[byteToUseText + (secondMagic * 4) + 2];

                //        for (int lnJ = 0; lnJ < 9; lnJ++)
                //            romData[byteToUseMagic + (firstMagic * 14) + lnJ] = romData[byteToUseMagic + (secondMagic * 14) + lnJ];

                //        romData[byteToUseText + (secondMagic * 4) + 0] = textHold[0];
                //        romData[byteToUseText + (secondMagic * 4) + 2] = textHold[1];

                //        for (int lnJ = 0; lnJ < 9; lnJ++)
                //            romData[byteToUseMagic + (secondMagic * 14) + lnJ] = magicHold[lnJ];
                //    }

                //for (int lnI = 0; lnI < magicNumbers.Length; lnI++)
                //{
                //    int booster = -99;
                //    switch (magicNumbers[lnI])
                //    {
                //        case 1:
                //            // For correct healing.  (all extra lines are for that purpose)
                //            romData[0x2aae1c4] = (byte)(lnI + 1);
                //            booster = 11;
                //            break;
                //        case 9:
                //            romData[0x2aae1ac] = (byte)(lnI + 1);
                //            booster = 10;
                //            break;
                //        case 17:
                //            romData[0x2aae194] = (byte)(lnI + 1);
                //            booster = 9;
                //            break;
                //        case 25:
                //            booster = 8;
                //            break;
                //        case 12:
                //            romData[0x2aae1b8] = (byte)(lnI + 1);
                //            booster = 7;
                //            break;
                //        case 20:
                //            romData[0x2aae1a0] = (byte)(lnI + 1);
                //            booster = 6;
                //            break;
                //        case 28:
                //            romData[0x2aae188] = (byte)(lnI + 1);
                //            booster = 5;
                //            break;
                //        case 18:
                //            booster = 4;
                //            break;
                //        case 29:
                //            booster = 3;
                //            break;
                //        case 13:
                //            booster = 2;
                //            break;
                //        case 21:
                //            booster = 1;
                //            break;
                //        case 22:
                //            booster = 0;
                //            break;
                //        case 51:
                //            booster = -1;
                //            break;
                //    }

                //    if (booster == -1)
                //    {
                //        romData[0x2a9eef4] = (byte)(lnI + 1);
                //        romData[0x2aa0040] = (byte)(lnI + 1);
                //        romData[0x2aa0320] = (byte)(lnI + 1);
                //        romData[0x2aaf648] = (byte)(lnI + 1);
                //    }
                //    if (booster == 0)
                //    {
                //        romData[0x2a9eee8] = (byte)(lnI + 1);
                //    }
                //    if (booster >= 0)
                //    {
                //        romData[0x2aa0054 + (booster * 12)] = (byte)(lnI + 1);
                //        romData[0x2aa0338 + (booster * 12)] = (byte)(lnI + 1);
                //        //romData[0x2aaf654 + (booster * 12)] = (byte)(lnI + 1);
                //    }
                //}
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
            lblXPReqAdj.Text = "XP Req Adjustment\r\n" + (trkXPReqAdj.Value * 5).ToString() + "%";
        }


        private void trkGilReqAdj_Scroll(object sender, EventArgs e)
        {
            lblGilReqAdj.Text = "Gil Req Adjustment\r\n" + (trkGilReqAdj.Value * 5).ToString() + "%";
        }

        private void trkXPBoost_Scroll(object sender, EventArgs e)
        {
            lblXPBoost.Text = "Monster XP Boost\r\n+" + (trkXPBoost.Value * 5).ToString();
        }

        private void trkEncounterRate_Scroll(object sender, EventArgs e)
        {
            lblEncounterRate.Text = "Encounter Rate\r\n" + (trkEncounterRate.Value * 10).ToString() + "%";
        }

        private void trkRandomPrices_Scroll(object sender, EventArgs e)
        {
            int minimum = 10000 / trkRandomPrices.Value / 10;
            lblRandomPrices.Text = "Prices\r\n" + minimum.ToString() + "-" + (trkRandomPrices.Value * 10).ToString() + "%";
        }

        private void trkRandomStats_Scroll(object sender, EventArgs e)
        {
            int minimum = 10000 / trkRandomStats.Value / 10;
            lblRandomEnemyStats.Text = "Enemy Stats\r\n" + minimum.ToString() + "-" + (trkRandomPrices.Value * 10).ToString() + "%";
        }

        private void cmdPrintInfo_Click(object sender, EventArgs e)
        {
            if (!loadRom()) return;

            printInformation();
        }

        private void determineFlags(object sender, EventArgs e)
        {
            if (loading)
                return;

            string flags = "";
            int number = (chkRandomizeMonsterZones.Checked ? 1 : 0) + (chkRandomizeSpecialMonsters.Checked ? 2 : 0) + (chkRandomizeMonsterPatterns.Checked ? 4 : 0) +
                (chkRandomizeEquipment.Checked ? 8 : 0) + (chkRandomizeTreasures.Checked ? 16 : 0) + (chkRandomizeMagic.Checked ? 32 : 0);
            flags += convertIntToChar(number);
            number = (chkRandomizeItemStores.Checked ? 1 : 0) + (chkRandomizeEquipStores.Checked ? 2 : 0) + (chkRandomizeMagicStores.Checked ? 4 : 0) + (chkShuffleMagicStores.Checked ? 8 : 0);
            flags += convertIntToChar(number);
            flags += convertIntToChar(trkXPReqAdj.Value);
            flags += convertIntToChar(trkGilReqAdj.Value);
            flags += convertIntToChar(trkXPBoost.Value);
            flags += convertIntToChar(trkEncounterRate.Value);
            flags += convertIntToChar(trkRandomPrices.Value);
            flags += convertIntToChar(trkRandomStats.Value);

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
            chkRandomizeMagic.Checked = (number % 64 >= 32);

            number = convertChartoInt(Convert.ToChar(flags.Substring(1, 1)));
            chkRandomizeItemStores.Checked = (number % 2 == 1);
            chkRandomizeEquipStores.Checked = (number % 4 >= 2);
            chkRandomizeMagicStores.Checked = (number % 8 >= 4);
            chkShuffleMagicStores.Checked = (number % 16 >= 8);
            trkXPReqAdj.Value = convertChartoInt(Convert.ToChar(flags.Substring(2, 1)));
            trkXPReqAdj_Scroll(null, null);
            trkGilReqAdj.Value = convertChartoInt(Convert.ToChar(flags.Substring(3, 1)));
            trkGilReqAdj_Scroll(null, null);
            trkXPBoost.Value = convertChartoInt(Convert.ToChar(flags.Substring(4, 1)));
            trkXPBoost_Scroll(null, null);
            trkEncounterRate.Value = convertChartoInt(Convert.ToChar(flags.Substring(5, 1)));
            trkEncounterRate_Scroll(null, null);
            trkRandomPrices.Value = convertChartoInt(Convert.ToChar(flags.Substring(6, 1)));
            trkRandomPrices_Scroll(null, null);
            trkRandomStats.Value = convertChartoInt(Convert.ToChar(flags.Substring(7, 1)));
            trkRandomStats_Scroll(null, null);
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
                    txtFileName.Text = reader.ReadLine();
                    loading = false;
                }
            } catch
            {
                trkXPReqAdj.Value = 20;
                trkXPBoost.Value = 0;
                trkEncounterRate.Value = 10;
                determineFlags(null, null);
                loading = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (StreamWriter writer = File.CreateText("lastFFR.txt"))
            {
                writer.WriteLine(txtFlags.Text);
                writer.WriteLine(txtFileName.Text);
            }
        }

        private void chkShuffleMagicStores_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                loading = true;
                chkRandomizeMagicStores.Checked = false;
                loading = false;
                determineFlags(sender, e);
            }
        }

        private void chkRandomizeMagicStores_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                loading = true;
                chkShuffleMagicStores.Checked = false;
                loading = false;
                determineFlags(sender, e);
            }
        }

        private void cmdStdShortcut_Click(object sender, EventArgs e)
        {
            chkRandomizeMonsterZones.Checked = true;
            chkRandomizeMonsterPatterns.Checked = false;
            chkRandomizeSpecialMonsters.Checked = true;
            chkRandomizeTreasures.Checked = true;
            chkRandomizeEquipment.Checked = false;
            chkRandomizeMagic.Checked = true;
            chkRandomizeEquipStores.Checked = true;
            chkRandomizeItemStores.Checked = true;
            chkRandomizeMagicStores.Checked = false;
            chkShuffleMagicStores.Checked = true;
            trkEncounterRate.Value = 10;
            trkXPReqAdj.Value = 10;
            trkGilReqAdj.Value = 4;
            trkXPBoost.Value = 20;
            trkRandomPrices.Value = 20;
            trkRandomStats.Value = 20;
            trkEncounterRate_Scroll(null, null);
            trkXPReqAdj_Scroll(null, null);
            trkGilReqAdj_Scroll(null, null);
            trkXPBoost_Scroll(null, null);
            trkRandomPrices_Scroll(null, null);
            trkRandomStats_Scroll(null, null);
        }

        private void btnQuickShortcut_Click(object sender, EventArgs e)
        {
            chkRandomizeMonsterZones.Checked = true;
            chkRandomizeMonsterPatterns.Checked = false;
            chkRandomizeSpecialMonsters.Checked = true;
            chkRandomizeTreasures.Checked = true;
            chkRandomizeEquipment.Checked = false;
            chkRandomizeMagic.Checked = true;
            chkRandomizeEquipStores.Checked = true;
            chkRandomizeItemStores.Checked = true;
            chkRandomizeMagicStores.Checked = false;
            chkShuffleMagicStores.Checked = true;
            trkEncounterRate.Value = 5;
            trkXPReqAdj.Value = 5;
            trkGilReqAdj.Value = 4;
            trkXPBoost.Value = 20;
            trkRandomPrices.Value = 20;
            trkRandomStats.Value = 20;
            trkEncounterRate_Scroll(null, null);
            trkXPReqAdj_Scroll(null, null);
            trkGilReqAdj_Scroll(null, null);
            trkXPBoost_Scroll(null, null);
            trkRandomPrices_Scroll(null, null);
            trkRandomStats_Scroll(null, null);
        }

        private void btnChaosShortcut_Click(object sender, EventArgs e)
        {
            chkRandomizeMonsterZones.Checked = true;
            chkRandomizeMonsterPatterns.Checked = true;
            chkRandomizeSpecialMonsters.Checked = true;
            chkRandomizeTreasures.Checked = true;
            chkRandomizeEquipment.Checked = true;
            chkRandomizeMagic.Checked = true;
            chkRandomizeEquipStores.Checked = true;
            chkRandomizeItemStores.Checked = true;
            chkRandomizeMagicStores.Checked = true;
            chkShuffleMagicStores.Checked = false;
            trkEncounterRate.Value = 5;
            trkXPReqAdj.Value = 4;
            trkGilReqAdj.Value = 4;
            trkXPBoost.Value = 20;
            trkRandomPrices.Value = 20;
            trkRandomStats.Value = 20;
            trkEncounterRate_Scroll(null, null);
            trkXPReqAdj_Scroll(null, null);
            trkGilReqAdj_Scroll(null, null);
            trkXPBoost_Scroll(null, null);
            trkRandomPrices_Scroll(null, null);
            trkRandomStats_Scroll(null, null);
        }
    }
}
