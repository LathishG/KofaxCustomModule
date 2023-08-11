namespace Bregs.Kofax.Docusign
{
    partial class DocumentUpload
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
            this.btnOpenBatch = new System.Windows.Forms.Button();
            this.lblBatchId = new System.Windows.Forms.Label();
            this.lblDocumentCount = new System.Windows.Forms.Label();
            this.lbldocumentName = new System.Windows.Forms.Label();
            this.lblIndexField = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOpenBatch
            // 
            this.btnOpenBatch.Location = new System.Drawing.Point(99, 44);
            this.btnOpenBatch.Name = "btnOpenBatch";
            this.btnOpenBatch.Size = new System.Drawing.Size(137, 50);
            this.btnOpenBatch.TabIndex = 0;
            this.btnOpenBatch.Text = "Open Batch";
            this.btnOpenBatch.UseVisualStyleBackColor = true;
            this.btnOpenBatch.Click += new System.EventHandler(this.btnOpenBatch_Click);
            // 
            // lblBatchId
            // 
            this.lblBatchId.AutoSize = true;
            this.lblBatchId.Location = new System.Drawing.Point(260, 44);
            this.lblBatchId.Name = "lblBatchId";
            this.lblBatchId.Size = new System.Drawing.Size(55, 13);
            this.lblBatchId.TabIndex = 1;
            this.lblBatchId.Text = "[Batch ID]";
            this.lblBatchId.Click += new System.EventHandler(this.lblBatchId_Click);
            // 
            // lblDocumentCount
            // 
            this.lblDocumentCount.AutoSize = true;
            this.lblDocumentCount.Location = new System.Drawing.Point(260, 81);
            this.lblDocumentCount.Name = "lblDocumentCount";
            this.lblDocumentCount.Size = new System.Drawing.Size(99, 13);
            this.lblDocumentCount.TabIndex = 2;
            this.lblDocumentCount.Text = "[ Document Count ]";
            // 
            // lbldocumentName
            // 
            this.lbldocumentName.AutoSize = true;
            this.lbldocumentName.Location = new System.Drawing.Point(260, 110);
            this.lbldocumentName.Name = "lbldocumentName";
            this.lbldocumentName.Size = new System.Drawing.Size(93, 13);
            this.lbldocumentName.TabIndex = 3;
            this.lbldocumentName.Text = "[Document Name]";
            // 
            // lblIndexField
            // 
            this.lblIndexField.AutoSize = true;
            this.lblIndexField.Location = new System.Drawing.Point(260, 143);
            this.lblIndexField.Name = "lblIndexField";
            this.lblIndexField.Size = new System.Drawing.Size(92, 13);
            this.lblIndexField.TabIndex = 4;
            this.lblIndexField.Text = "[IndexField Count]";
            // 
            // DocumentUpload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 201);
            this.Controls.Add(this.lblIndexField);
            this.Controls.Add(this.lbldocumentName);
            this.Controls.Add(this.lblDocumentCount);
            this.Controls.Add(this.lblBatchId);
            this.Controls.Add(this.btnOpenBatch);
            this.Name = "DocumentUpload";
            this.Text = "DocumentUpload";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenBatch;
        private System.Windows.Forms.Label lblBatchId;
        private System.Windows.Forms.Label lblDocumentCount;
        private System.Windows.Forms.Label lbldocumentName;
        private System.Windows.Forms.Label lblIndexField;
    }
}