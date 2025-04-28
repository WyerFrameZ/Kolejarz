namespace kolejarka
{
    partial class Form1
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cnComboBox = new System.Windows.Forms.ComboBox();
            this.cnTextBox = new System.Windows.Forms.TextBox();
            this.addCnButton = new System.Windows.Forms.Button();
            this.cnLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 47);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(837, 579);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 23.25F);
            this.label1.Location = new System.Drawing.Point(217, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(360, 35);
            this.label1.TabIndex = 1;
            this.label1.Text = "KUP BILET NA POCIĄG ";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F);
            this.label2.Location = new System.Drawing.Point(855, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(146, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Stan bazy danych:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // cnLabel
            // 
            this.cnLabel.AutoSize = true;
            this.cnLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F);
            this.cnLabel.Location = new System.Drawing.Point(855, 100);
            this.cnLabel.Name = "cnLabel";
            this.cnLabel.Size = new System.Drawing.Size(146, 20);
            this.cnLabel.TabIndex = 13;
            this.cnLabel.Text = "Edytuj numer CN:";
            // 
            // cnComboBox
            // 
            this.cnComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cnComboBox.FormattingEnabled = true;
            this.cnComboBox.Location = new System.Drawing.Point(855, 130);
            this.cnComboBox.Name = "cnComboBox";
            this.cnComboBox.Size = new System.Drawing.Size(200, 21);
            this.cnComboBox.TabIndex = 12;
            this.cnComboBox.SelectedIndexChanged += new System.EventHandler(this.cnComboBox_SelectedIndexChanged);
            // 
            // cnTextBox
            // 
            this.cnTextBox.Location = new System.Drawing.Point(855, 160);
            this.cnTextBox.Name = "cnTextBox";
            this.cnTextBox.Size = new System.Drawing.Size(120, 20);
            this.cnTextBox.TabIndex = 14;
            // 
            // addCnButton
            // 
            this.addCnButton.Location = new System.Drawing.Point(980, 160);
            this.addCnButton.Name = "addCnButton";
            this.addCnButton.Size = new System.Drawing.Size(75, 23);
            this.addCnButton.TabIndex = 15;
            this.addCnButton.Text = "Dodaj CN";
            this.addCnButton.UseVisualStyleBackColor = true;
            this.addCnButton.Click += new System.EventHandler(this.addCnButton_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.Location = new System.Drawing.Point(660, 394);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(67, 48);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(645, 316);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(82, 47);
            this.button2.TabIndex = 4;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(406, 394);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(67, 44);
            this.button3.TabIndex = 5;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(604, 73);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(70, 45);
            this.button4.TabIndex = 6;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(509, 189);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(68, 44);
            this.button5.TabIndex = 7;
            this.button5.Text = "button5";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(304, 67);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(76, 51);
            this.button6.TabIndex = 8;
            this.button6.Text = "button6";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(123, 272);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(71, 48);
            this.button7.TabIndex = 9;
            this.button7.Text = "button7";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(123, 390);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(71, 48);
            this.button8.TabIndex = 10;
            this.button8.Text = "button8";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(146, 67);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(63, 48);
            this.button9.TabIndex = 11;
            this.button9.Text = "button9";
            this.button9.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1135, 638);
            this.Controls.Add(this.addCnButton);
            this.Controls.Add(this.cnTextBox);
            this.Controls.Add(this.cnLabel);
            this.Controls.Add(this.cnComboBox);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cnComboBox;
        private System.Windows.Forms.TextBox cnTextBox;
        private System.Windows.Forms.Button addCnButton;
        private System.Windows.Forms.Label cnLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
    }
}

