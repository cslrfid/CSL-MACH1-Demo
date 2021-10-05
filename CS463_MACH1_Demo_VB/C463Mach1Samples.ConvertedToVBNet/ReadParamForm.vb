Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms

Imports CSLMach1.CSL
Imports CSLMach1.CSL.Mach1

Public Partial Class ReadParamForm
	Inherits Form
    Public mode As CSLReaderParamControl.MODE = CSLReaderParamControl.MODE.[NEW]
    Public settings As CSLReaderSettings

	Public Sub New()
		InitializeComponent()
	End Sub

	Private Sub ReadParamForm_Load(sender As Object, e As EventArgs)
		readerCtrl.SetMode(mode)
		readerCtrl.UpdateForm(settings)
	End Sub

	Private Sub ReadParamForm_FormClosing(sender As Object, e As FormClosingEventArgs)
		settings = readerCtrl.GetSettings()
	End Sub
End Class
