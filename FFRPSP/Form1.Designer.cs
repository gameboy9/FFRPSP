namespace FFRPSP
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.btnFF1Monsters = new System.Windows.Forms.Button();
            this.chkRandomizeEquipment = new System.Windows.Forms.CheckBox();
            this.trkXPReqAdj = new System.Windows.Forms.TrackBar();
            this.lblXPReqAdj = new System.Windows.Forms.Label();
            this.lblXPBoost = new System.Windows.Forms.Label();
            this.trkXPBoost = new System.Windows.Forms.TrackBar();
            this.trkEncounterRate = new System.Windows.Forms.TrackBar();
            this.lblEncounterRate = new System.Windows.Forms.Label();
            this.chkRandomizeMonsterZones = new System.Windows.Forms.CheckBox();
            this.chkRandomizeMonsterPatterns = new System.Windows.Forms.CheckBox();
            this.chkRandomizeTreasures = new System.Windows.Forms.CheckBox();
            this.chkNoROM = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblLivePatchCRC = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSeed = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtFlags = new System.Windows.Forms.TextBox();
            this.chkRandomizeSpecialMonsters = new System.Windows.Forms.CheckBox();
            this.cmdPrintInfo = new System.Windows.Forms.Button();
            this.lblResults = new System.Windows.Forms.Label();
            this.chkRandomizeStores = new System.Windows.Forms.CheckBox();
            this.lblRandomPrices = new System.Windows.Forms.Label();
            this.trkRandomPrices = new System.Windows.Forms.TrackBar();
            this.lblRandomEnemyStats = new System.Windows.Forms.Label();
            this.trkRandomStats = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.trkXPReqAdj)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkXPBoost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkEncounterRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkRandomPrices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkRandomStats)).BeginInit();
            this.SuspendLayout();
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(448, 14);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 32;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "ROM Image";
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(119, 15);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(320, 20);
            this.txtFileName.TabIndex = 31;
            // 
            // btnFF1Monsters
            // 
            this.btnFF1Monsters.Location = new System.Drawing.Point(12, 305);
            this.btnFF1Monsters.Name = "btnFF1Monsters";
            this.btnFF1Monsters.Size = new System.Drawing.Size(75, 24);
            this.btnFF1Monsters.TabIndex = 42;
            this.btnFF1Monsters.Text = "Hack";
            this.btnFF1Monsters.UseVisualStyleBackColor = true;
            this.btnFF1Monsters.Click += new System.EventHandler(this.btnFF1Monsters_Click);
            // 
            // chkRandomizeEquipment
            // 
            this.chkRandomizeEquipment.AutoSize = true;
            this.chkRandomizeEquipment.Location = new System.Drawing.Point(352, 103);
            this.chkRandomizeEquipment.Name = "chkRandomizeEquipment";
            this.chkRandomizeEquipment.Size = new System.Drawing.Size(132, 17);
            this.chkRandomizeEquipment.TabIndex = 44;
            this.chkRandomizeEquipment.Text = "Randomize Equipment";
            this.chkRandomizeEquipment.UseVisualStyleBackColor = true;
            this.chkRandomizeEquipment.CheckedChanged += new System.EventHandler(this.determineFlags);
            // 
            // trkXPReqAdj
            // 
            this.trkXPReqAdj.LargeChange = 4;
            this.trkXPReqAdj.Location = new System.Drawing.Point(18, 182);
            this.trkXPReqAdj.Maximum = 40;
            this.trkXPReqAdj.Minimum = 4;
            this.trkXPReqAdj.Name = "trkXPReqAdj";
            this.trkXPReqAdj.Size = new System.Drawing.Size(104, 45);
            this.trkXPReqAdj.SmallChange = 2;
            this.trkXPReqAdj.TabIndex = 45;
            this.trkXPReqAdj.TickFrequency = 4;
            this.trkXPReqAdj.Value = 20;
            this.trkXPReqAdj.Scroll += new System.EventHandler(this.trkXPReqAdj_Scroll);
            this.trkXPReqAdj.ValueChanged += new System.EventHandler(this.determineFlags);
            // 
            // lblXPReqAdj
            // 
            this.lblXPReqAdj.AutoSize = true;
            this.lblXPReqAdj.Location = new System.Drawing.Point(17, 230);
            this.lblXPReqAdj.Name = "lblXPReqAdj";
            this.lblXPReqAdj.Size = new System.Drawing.Size(107, 13);
            this.lblXPReqAdj.TabIndex = 46;
            this.lblXPReqAdj.Text = "XP/GP Reqs = 100%";
            // 
            // lblXPBoost
            // 
            this.lblXPBoost.AutoSize = true;
            this.lblXPBoost.Location = new System.Drawing.Point(155, 230);
            this.lblXPBoost.Name = "lblXPBoost";
            this.lblXPBoost.Size = new System.Drawing.Size(92, 13);
            this.lblXPBoost.TabIndex = 48;
            this.lblXPBoost.Text = "XP/Gil Boost = +0";
            // 
            // trkXPBoost
            // 
            this.trkXPBoost.Location = new System.Drawing.Point(149, 178);
            this.trkXPBoost.Maximum = 20;
            this.trkXPBoost.Name = "trkXPBoost";
            this.trkXPBoost.Size = new System.Drawing.Size(104, 45);
            this.trkXPBoost.SmallChange = 2;
            this.trkXPBoost.TabIndex = 47;
            this.trkXPBoost.TickFrequency = 2;
            this.trkXPBoost.Scroll += new System.EventHandler(this.trkXPBoost_Scroll);
            this.trkXPBoost.ValueChanged += new System.EventHandler(this.determineFlags);
            // 
            // trkEncounterRate
            // 
            this.trkEncounterRate.LargeChange = 10;
            this.trkEncounterRate.Location = new System.Drawing.Point(281, 178);
            this.trkEncounterRate.Maximum = 40;
            this.trkEncounterRate.Minimum = 2;
            this.trkEncounterRate.Name = "trkEncounterRate";
            this.trkEncounterRate.Size = new System.Drawing.Size(104, 45);
            this.trkEncounterRate.SmallChange = 5;
            this.trkEncounterRate.TabIndex = 49;
            this.trkEncounterRate.TickFrequency = 5;
            this.trkEncounterRate.Value = 10;
            this.trkEncounterRate.Scroll += new System.EventHandler(this.trkEncounterRate_Scroll);
            this.trkEncounterRate.ValueChanged += new System.EventHandler(this.determineFlags);
            // 
            // lblEncounterRate
            // 
            this.lblEncounterRate.AutoSize = true;
            this.lblEncounterRate.Location = new System.Drawing.Point(273, 230);
            this.lblEncounterRate.Name = "lblEncounterRate";
            this.lblEncounterRate.Size = new System.Drawing.Size(120, 13);
            this.lblEncounterRate.TabIndex = 50;
            this.lblEncounterRate.Text = "Encounter Rate = 100%";
            // 
            // chkRandomizeMonsterZones
            // 
            this.chkRandomizeMonsterZones.AutoSize = true;
            this.chkRandomizeMonsterZones.Location = new System.Drawing.Point(159, 103);
            this.chkRandomizeMonsterZones.Name = "chkRandomizeMonsterZones";
            this.chkRandomizeMonsterZones.Size = new System.Drawing.Size(153, 17);
            this.chkRandomizeMonsterZones.TabIndex = 51;
            this.chkRandomizeMonsterZones.Text = "Randomize Monster Zones";
            this.chkRandomizeMonsterZones.UseVisualStyleBackColor = true;
            this.chkRandomizeMonsterZones.CheckedChanged += new System.EventHandler(this.determineFlags);
            // 
            // chkRandomizeMonsterPatterns
            // 
            this.chkRandomizeMonsterPatterns.AutoSize = true;
            this.chkRandomizeMonsterPatterns.Location = new System.Drawing.Point(159, 149);
            this.chkRandomizeMonsterPatterns.Name = "chkRandomizeMonsterPatterns";
            this.chkRandomizeMonsterPatterns.Size = new System.Drawing.Size(162, 17);
            this.chkRandomizeMonsterPatterns.TabIndex = 52;
            this.chkRandomizeMonsterPatterns.Text = "Randomize Monster Patterns";
            this.chkRandomizeMonsterPatterns.UseVisualStyleBackColor = true;
            this.chkRandomizeMonsterPatterns.CheckedChanged += new System.EventHandler(this.determineFlags);
            // 
            // chkRandomizeTreasures
            // 
            this.chkRandomizeTreasures.AutoSize = true;
            this.chkRandomizeTreasures.Location = new System.Drawing.Point(352, 126);
            this.chkRandomizeTreasures.Name = "chkRandomizeTreasures";
            this.chkRandomizeTreasures.Size = new System.Drawing.Size(129, 17);
            this.chkRandomizeTreasures.TabIndex = 53;
            this.chkRandomizeTreasures.Text = "Randomize Treasures";
            this.chkRandomizeTreasures.UseVisualStyleBackColor = true;
            this.chkRandomizeTreasures.CheckedChanged += new System.EventHandler(this.determineFlags);
            // 
            // chkNoROM
            // 
            this.chkNoROM.AutoSize = true;
            this.chkNoROM.Location = new System.Drawing.Point(529, 17);
            this.chkNoROM.Name = "chkNoROM";
            this.chkNoROM.Size = new System.Drawing.Size(100, 17);
            this.chkNoROM.TabIndex = 54;
            this.chkNoROM.Text = "No ROM Image";
            this.chkNoROM.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 55;
            this.label2.Text = "LivePatch CRC";
            // 
            // lblLivePatchCRC
            // 
            this.lblLivePatchCRC.AutoSize = true;
            this.lblLivePatchCRC.Location = new System.Drawing.Point(119, 43);
            this.lblLivePatchCRC.Name = "lblLivePatchCRC";
            this.lblLivePatchCRC.Size = new System.Drawing.Size(103, 13);
            this.lblLivePatchCRC.TabIndex = 56;
            this.lblLivePatchCRC.Text = "Not implemented yet";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 58;
            this.label4.Text = "Seed";
            // 
            // txtSeed
            // 
            this.txtSeed.Location = new System.Drawing.Point(119, 66);
            this.txtSeed.Name = "txtSeed";
            this.txtSeed.Size = new System.Drawing.Size(112, 20);
            this.txtSeed.TabIndex = 57;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(280, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 60;
            this.label5.Text = "Flags";
            // 
            // txtFlags
            // 
            this.txtFlags.Location = new System.Drawing.Point(330, 66);
            this.txtFlags.Name = "txtFlags";
            this.txtFlags.Size = new System.Drawing.Size(178, 20);
            this.txtFlags.TabIndex = 59;
            this.txtFlags.Leave += new System.EventHandler(this.determineChecks);
            // 
            // chkRandomizeSpecialMonsters
            // 
            this.chkRandomizeSpecialMonsters.AutoSize = true;
            this.chkRandomizeSpecialMonsters.Location = new System.Drawing.Point(159, 126);
            this.chkRandomizeSpecialMonsters.Name = "chkRandomizeSpecialMonsters";
            this.chkRandomizeSpecialMonsters.Size = new System.Drawing.Size(182, 17);
            this.chkRandomizeSpecialMonsters.TabIndex = 61;
            this.chkRandomizeSpecialMonsters.Text = "Randomize Special Atk Monsters";
            this.chkRandomizeSpecialMonsters.UseVisualStyleBackColor = true;
            this.chkRandomizeSpecialMonsters.CheckedChanged += new System.EventHandler(this.determineFlags);
            // 
            // cmdPrintInfo
            // 
            this.cmdPrintInfo.Location = new System.Drawing.Point(119, 306);
            this.cmdPrintInfo.Name = "cmdPrintInfo";
            this.cmdPrintInfo.Size = new System.Drawing.Size(75, 23);
            this.cmdPrintInfo.TabIndex = 62;
            this.cmdPrintInfo.Text = "Print Info";
            this.cmdPrintInfo.UseVisualStyleBackColor = true;
            this.cmdPrintInfo.Click += new System.EventHandler(this.cmdPrintInfo_Click);
            // 
            // lblResults
            // 
            this.lblResults.AutoSize = true;
            this.lblResults.Location = new System.Drawing.Point(222, 311);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(0, 13);
            this.lblResults.TabIndex = 63;
            // 
            // chkRandomizeStores
            // 
            this.chkRandomizeStores.AutoSize = true;
            this.chkRandomizeStores.Location = new System.Drawing.Point(352, 149);
            this.chkRandomizeStores.Name = "chkRandomizeStores";
            this.chkRandomizeStores.Size = new System.Drawing.Size(112, 17);
            this.chkRandomizeStores.TabIndex = 64;
            this.chkRandomizeStores.Text = "Randomize Stores";
            this.chkRandomizeStores.UseVisualStyleBackColor = true;
            this.chkRandomizeStores.CheckedChanged += new System.EventHandler(this.determineFlags);
            // 
            // lblRandomPrices
            // 
            this.lblRandomPrices.AutoSize = true;
            this.lblRandomPrices.Location = new System.Drawing.Point(416, 230);
            this.lblRandomPrices.Name = "lblRandomPrices";
            this.lblRandomPrices.Size = new System.Drawing.Size(89, 13);
            this.lblRandomPrices.TabIndex = 66;
            this.lblRandomPrices.Text = "Prices = 50-200%";
            // 
            // trkRandomPrices
            // 
            this.trkRandomPrices.LargeChange = 10;
            this.trkRandomPrices.Location = new System.Drawing.Point(408, 178);
            this.trkRandomPrices.Maximum = 50;
            this.trkRandomPrices.Minimum = 10;
            this.trkRandomPrices.Name = "trkRandomPrices";
            this.trkRandomPrices.Size = new System.Drawing.Size(104, 45);
            this.trkRandomPrices.SmallChange = 5;
            this.trkRandomPrices.TabIndex = 65;
            this.trkRandomPrices.TickFrequency = 5;
            this.trkRandomPrices.Value = 10;
            this.trkRandomPrices.Scroll += new System.EventHandler(this.trkRandomPrices_Scroll);
            // 
            // lblRandomEnemyStats
            // 
            this.lblRandomEnemyStats.AutoSize = true;
            this.lblRandomEnemyStats.Location = new System.Drawing.Point(531, 230);
            this.lblRandomEnemyStats.Name = "lblRandomEnemyStats";
            this.lblRandomEnemyStats.Size = new System.Drawing.Size(119, 13);
            this.lblRandomEnemyStats.TabIndex = 68;
            this.lblRandomEnemyStats.Text = "Enemy Stats = 50-200%";
            // 
            // trkRandomStats
            // 
            this.trkRandomStats.LargeChange = 10;
            this.trkRandomStats.Location = new System.Drawing.Point(538, 178);
            this.trkRandomStats.Maximum = 50;
            this.trkRandomStats.Minimum = 10;
            this.trkRandomStats.Name = "trkRandomStats";
            this.trkRandomStats.Size = new System.Drawing.Size(104, 45);
            this.trkRandomStats.SmallChange = 5;
            this.trkRandomStats.TabIndex = 67;
            this.trkRandomStats.TickFrequency = 5;
            this.trkRandomStats.Value = 10;
            this.trkRandomStats.Scroll += new System.EventHandler(this.trkRandomStats_Scroll);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 341);
            this.Controls.Add(this.lblRandomEnemyStats);
            this.Controls.Add(this.trkRandomStats);
            this.Controls.Add(this.lblRandomPrices);
            this.Controls.Add(this.trkRandomPrices);
            this.Controls.Add(this.chkRandomizeStores);
            this.Controls.Add(this.lblResults);
            this.Controls.Add(this.cmdPrintInfo);
            this.Controls.Add(this.chkRandomizeSpecialMonsters);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtFlags);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtSeed);
            this.Controls.Add(this.lblLivePatchCRC);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkNoROM);
            this.Controls.Add(this.chkRandomizeTreasures);
            this.Controls.Add(this.chkRandomizeMonsterPatterns);
            this.Controls.Add(this.chkRandomizeMonsterZones);
            this.Controls.Add(this.lblEncounterRate);
            this.Controls.Add(this.trkEncounterRate);
            this.Controls.Add(this.lblXPBoost);
            this.Controls.Add(this.trkXPBoost);
            this.Controls.Add(this.lblXPReqAdj);
            this.Controls.Add(this.trkXPReqAdj);
            this.Controls.Add(this.chkRandomizeEquipment);
            this.Controls.Add(this.btnFF1Monsters);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFileName);
            this.Name = "Form1";
            this.Text = "FFRPSP";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trkXPReqAdj)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkXPBoost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkEncounterRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkRandomPrices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkRandomStats)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button btnFF1Monsters;
        private System.Windows.Forms.CheckBox chkRandomizeEquipment;
        private System.Windows.Forms.TrackBar trkXPReqAdj;
        private System.Windows.Forms.Label lblXPReqAdj;
        private System.Windows.Forms.Label lblXPBoost;
        private System.Windows.Forms.TrackBar trkXPBoost;
        private System.Windows.Forms.TrackBar trkEncounterRate;
        private System.Windows.Forms.Label lblEncounterRate;
        private System.Windows.Forms.CheckBox chkRandomizeMonsterZones;
        private System.Windows.Forms.CheckBox chkRandomizeMonsterPatterns;
        private System.Windows.Forms.CheckBox chkRandomizeTreasures;
        private System.Windows.Forms.CheckBox chkNoROM;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblLivePatchCRC;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSeed;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtFlags;
        private System.Windows.Forms.CheckBox chkRandomizeSpecialMonsters;
        private System.Windows.Forms.Button cmdPrintInfo;
        private System.Windows.Forms.Label lblResults;
        private System.Windows.Forms.CheckBox chkRandomizeStores;
        private System.Windows.Forms.Label lblRandomPrices;
        private System.Windows.Forms.TrackBar trkRandomPrices;
        private System.Windows.Forms.Label lblRandomEnemyStats;
        private System.Windows.Forms.TrackBar trkRandomStats;
    }
}

