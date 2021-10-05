Partial Class ReadParamForm
	''' <summary>
	''' Required designer variable.
	''' </summary>
	Private components As System.ComponentModel.IContainer = Nothing

	''' <summary>
	''' Clean up any resources being used.
	''' </summary>
	''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	Protected Overrides Sub Dispose(disposing As Boolean)
		If disposing AndAlso (components IsNot Nothing) Then
			components.Dispose()
		End If
		MyBase.Dispose(disposing)
	End Sub

	#Region "Windows Form Designer generated code"

	''' <summary>
	''' Required method for Designer support - do not modify
	''' the contents of this method with the code editor.
	''' </summary>
	Private Sub InitializeComponent()
		Me.btnCancel = New System.Windows.Forms.Button()
		Me.btnOK = New System.Windows.Forms.Button()
		Me.readerCtrl = New CSLReaderParamControl.CSLReaderParams()
		Me.SuspendLayout()
		' 
		' btnCancel
		' 
		Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.btnCancel.Location = New System.Drawing.Point(307, 219)
		Me.btnCancel.Name = "btnCancel"
		Me.btnCancel.Size = New System.Drawing.Size(87, 32)
		Me.btnCancel.TabIndex = 2
		Me.btnCancel.Text = "Cancel"
		Me.btnCancel.UseVisualStyleBackColor = True
		' 
		' btnOK
		' 
		Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.btnOK.Location = New System.Drawing.Point(215, 219)
		Me.btnOK.Name = "btnOK"
		Me.btnOK.Size = New System.Drawing.Size(86, 32)
		Me.btnOK.TabIndex = 2
		Me.btnOK.Text = "OK"
		Me.btnOK.UseVisualStyleBackColor = True
		' 
		' readerCtrl
		' 
		Me.readerCtrl.Location = New System.Drawing.Point(4, 7)
		Me.readerCtrl.Name = "readerCtrl"
		Me.readerCtrl.Size = New System.Drawing.Size(395, 205)
		Me.readerCtrl.TabIndex = 1
		' 
		' ReadParamForm
		' 
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(402, 258)
		Me.Controls.Add(Me.btnOK)
		Me.Controls.Add(Me.btnCancel)
		Me.Controls.Add(Me.readerCtrl)
		Me.Name = "ReadParamForm"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
		Me.Text = "Reader Parameters"
		AddHandler Me.FormClosing, New System.Windows.Forms.FormClosingEventHandler(AddressOf Me.ReadParamForm_FormClosing)
		AddHandler Me.Load, New System.EventHandler(AddressOf Me.ReadParamForm_Load)
		Me.ResumeLayout(False)

	End Sub

	#End Region

	Private btnCancel As System.Windows.Forms.Button
	Private btnOK As System.Windows.Forms.Button
	Private readerCtrl As CSLReaderParamControl.CSLReaderParams
End Class
