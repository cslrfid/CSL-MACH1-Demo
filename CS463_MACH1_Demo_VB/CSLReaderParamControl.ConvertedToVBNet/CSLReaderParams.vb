Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Text
Imports System.Windows.Forms
Imports System.Collections

Imports CSLMach1.CSL
Imports CSLMach1.CSL.Mach1


Public Enum MODE
	[NEW]
	UPDATE
End Enum

Public Partial Class CSLReaderParams
	Inherits UserControl
	Private opMode As MODE = MODE.[NEW]

	Public Sub New()
		InitializeComponent()
	End Sub

	Private Sub tpAntenna_Click(sender As Object, e As EventArgs)

	End Sub

	Private Sub cbLBTMode_SelectedIndexChanged(sender As Object, e As EventArgs)

	End Sub

	Private Sub CSLReaderParams_Load(sender As Object, e As EventArgs)

	End Sub

	''' <summary>
	''' Update control based on input settings
	''' </summary>
	''' <param name="settings"></param>
	Public Sub UpdateForm(settings As CSLReaderSettings)
		If settings Is Nothing Then
			tbReaderName.Text = "192.168.25.200"
			cbRegion.SelectedIndex = 0
			cbFrequencyMode.Enabled = False
			cbFrequencyList.Enabled = False


			chAnt1.Checked = True
			chAnt2.Checked = True
			chAnt3.Checked = True
			chAnt4.Checked = True

			cbInventoryMode.SelectedIndex = 0
			cbAutoSetMode.SelectedIndex = 0
			cbSession.SelectedIndex = 0
			cbReaderMode.SelectedIndex = 1
			tbPopulation.Text = "32"
		Else
			tbReaderName.Text = settings.reader_information.reader_name

			cbRegion.SelectedIndex = settings.reader_information.region
			cbFrequencyMode.SelectedIndex = settings.reader_information.frequency_mode

			chAnt1.Checked = settings.antennas(0).enabled
			chAnt2.Checked = settings.antennas(1).enabled
			chAnt3.Checked = settings.antennas(2).enabled
			chAnt4.Checked = settings.antennas(3).enabled

			tbPower1.Text = settings.antennas(0).power.ToString()
			tbPower2.Text = settings.antennas(1).power.ToString()
			tbPower3.Text = settings.antennas(2).power.ToString()
			tbPower4.Text = settings.antennas(3).power.ToString()

			If settings.reader_information.frequency_mode = 0 Then
				Select Case settings.reader_information.region
					Case 2, 7
						cbFrequencyList.SelectedIndex = CInt(settings.reader_information.frequency - 114) \ 12
						Exit Select
					Case 5, 6
						cbFrequencyList.SelectedIndex = CInt(settings.reader_information.frequency - 1846) \ 2
						Exit Select
					Case Else
						cbFrequencyList.Enabled = False
						Exit Select
				End Select
			End If

			cbInventoryMode.SelectedIndex = settings.gen2_params.inventory_mode
			cbSession.SelectedIndex = settings.gen2_params.session
			cbAutoSetMode.SelectedIndex = 0
			' settings.gen2_params.auto_set_mode - 1;
			cbReaderMode.Enabled = If(settings.gen2_params.auto_set_mode = 2, True, False)
			If cbReaderMode.Enabled Then
				cbReaderMode.SelectedIndex = settings.gen2_params.mode_id
			End If
			tbPopulation.Text = settings.gen2_params.population.ToString()

			lbFirmware.Text = settings.reader_information.firmware_ver
			lbSoftware.Text = settings.reader_information.software_ver

			If opMode = MODE.UPDATE Then
				tbReaderName.Enabled = False
				cbRegion.Enabled = False

			End If
		End If
	End Sub

	Private Sub llCSL_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
		System.Diagnostics.Process.Start("http://www.convergence.com.hk/")
	End Sub

	''' <summary>
	''' Get reader settings from the control
	''' </summary>
	''' <returns></returns>
	Public Function GetSettings() As CSLReaderSettings
		Dim settings As New CSLReaderSettings()

		settings.gen2_params.mode_id = cbReaderMode.SelectedIndex
		settings.gen2_params.session = cbSession.SelectedIndex
		settings.gen2_params.auto_set_mode = 2
		'cbAutoSetMode.SelectedIndex + 1;
		settings.gen2_params.inventory_mode = cbInventoryMode.SelectedIndex
		settings.gen2_params.population = Int32.Parse(tbPopulation.Text)

		settings.antennas(0).enabled = chAnt1.Checked
		settings.antennas(1).enabled = chAnt2.Checked
		settings.antennas(2).enabled = chAnt3.Checked
		settings.antennas(3).enabled = chAnt4.Checked

		settings.antennas(0).power = Single.Parse(tbPower1.Text)
		settings.antennas(1).power = Single.Parse(tbPower2.Text)
		settings.antennas(2).power = Single.Parse(tbPower3.Text)
		settings.antennas(3).power = Single.Parse(tbPower4.Text)

		settings.reader_information.reader_name = tbReaderName.Text
		settings.reader_information.region = cbRegion.SelectedIndex
		settings.reader_information.frequency_mode = cbFrequencyMode.SelectedIndex
		If settings.reader_information.frequency_mode = 0 Then
			Select Case settings.reader_information.region
				Case 2, 7
					settings.reader_information.frequency = CUShort(114 + cbFrequencyList.SelectedIndex * 12)
					Exit Select
				Case 5, 6
					settings.reader_information.frequency = CUShort(1846 + cbFrequencyList.SelectedIndex * 2)
					Exit Select
			End Select
		End If

		Return settings
	End Function

	Private Sub cbRegion_SelectedIndexChanged(sender As Object, e As EventArgs)
		cbFrequencyList.Items.Clear()

		tbPower1.Text = "30.00"
		tbPower2.Text = "30.00"
		tbPower3.Text = "30.00"
		tbPower4.Text = "30.00"

		Select Case cbRegion.SelectedIndex
			Case 0
				cbFrequencyMode.Enabled = False
				cbFrequencyList.Enabled = False
				AddModeIdListItem(New Integer() {0, 1, 2, 3})
				AddAutoSetListItem(New Integer() {2})
				Exit Select
			Case 1
				cbFrequencyMode.Enabled = False
				cbFrequencyList.Enabled = False
				AddModeIdListItem(New Integer() {})
				AddAutoSetListItem(New Integer() {})
				Exit Select
			Case 2
				cbFrequencyMode.Enabled = True
				cbFrequencyList.Items.Add("4 - 865.70MHz (114)")
				cbFrequencyList.Items.Add("7 - 866.30MHz (126)")
				cbFrequencyList.Items.Add("10 - 866.70MHz (138)")
				cbFrequencyList.Items.Add("13 - 867.50MHz (150)")
				tbPower1.Text = "28.00"
				tbPower2.Text = "28.00"
				tbPower3.Text = "28.00"
				tbPower4.Text = "28.00"
				cbFrequencyMode.SelectedIndex = 1
				AddModeIdListItem(New Integer() {0, 1, 2, 3})
				AddAutoSetListItem(New Integer() {2})
				Exit Select
			Case 3
				cbFrequencyMode.Enabled = False
				cbFrequencyList.Enabled = False
				AddModeIdListItem(New Integer() {0, 1, 2, 3})
				AddAutoSetListItem(New Integer() {2})
				Exit Select
			Case 4
				cbFrequencyMode.Enabled = False
				cbFrequencyList.Enabled = False
				AddModeIdListItem(New Integer() {0, 1, 2, 3})
				AddAutoSetListItem(New Integer() {2})
				Exit Select
			Case 5
				cbFrequencyMode.Enabled = True
				'cbFrequencyList.Enabled = true;
				cbFrequencyList.Items.Add("1 - 952.30MHz (1846)")
				cbFrequencyList.Items.Add("2 - 952.40MHz (1848)")
				cbFrequencyList.Items.Add("3 - 952.50MHz (1850)")
				cbFrequencyList.Items.Add("4 - 952.60MHz (1852)")
				cbFrequencyList.Items.Add("5 - 952.70MHz (1854)")
				cbFrequencyList.Items.Add("6 - 952.80MHz (1856)")
				cbFrequencyList.Items.Add("7 - 952.90MHz (1858)")
				cbFrequencyList.Items.Add("8 - 953.00MHz (1860)")
				cbFrequencyList.Items.Add("9 - 953.10MHz (1862)")
				cbFrequencyList.Items.Add("10 - 953.20MHz (1864)")
				cbFrequencyList.Items.Add("11 - 953.30MHz (1866)")
				cbFrequencyList.Items.Add("12 - 953.40MHz (1868)")
				cbFrequencyList.Items.Add("13 - 953.50MHz (1870)")
				cbFrequencyList.Items.Add("14 - 953.60MHz (1872)")
				cbFrequencyList.Items.Add("15 - 953.70MHz (1874)")
				cbFrequencyList.Items.Add("16 - 953.80MHz (1876)")
				cbFrequencyMode.SelectedIndex = 1
				AddModeIdListItem(New Integer() {0, 1, 2, 3})
				AddAutoSetListItem(New Integer() {2})
				Exit Select
			Case 6
				cbFrequencyMode.Enabled = True
				'cbFrequencyList.Enabled = true;
				cbFrequencyList.Items.Add("1 - 952.30MHz (1846)")
				cbFrequencyList.Items.Add("2 - 952.50MHz (1850)")
				cbFrequencyList.Items.Add("3 - 952.70MHz (1854)")
				cbFrequencyList.Items.Add("4 - 952.90MHz (1858)")
				cbFrequencyList.Items.Add("5 - 953.10MHz (1862)")
				cbFrequencyList.Items.Add("6 - 953.30MHz (1866)")
				cbFrequencyList.Items.Add("7 - 953.50MHz (1870)")
				cbFrequencyList.Items.Add("8 - 953.70MHz (1874)")
				cbFrequencyList.Items.Add("9 - 953.90MHz (1878)")
				cbFrequencyList.Items.Add("10 - 954.10MHz (1882)")
				cbFrequencyList.Items.Add("11 - 954.30MHz (1886)")
				cbFrequencyList.Items.Add("12 - 954.50MHz (1890)")
				cbFrequencyList.Items.Add("13 - 954.70MHz (1894)")
				tbPower1.Text = "10.00"
				tbPower2.Text = "10.00"
				tbPower3.Text = "10.00"
				tbPower4.Text = "10.00"
				cbFrequencyMode.SelectedIndex = 1
				AddModeIdListItem(New Integer() {0, 1, 2, 3})
				AddAutoSetListItem(New Integer() {2})
				Exit Select
			Case 7
				cbFrequencyMode.Enabled = True
				'cbFrequencyList.Enabled = true;
				cbFrequencyList.Items.Add("4 - 865.70MHz (114)")
				cbFrequencyList.Items.Add("7 - 866.30MHz (126)")
				cbFrequencyList.Items.Add("10 - 866.70MHz (138)")
				cbFrequencyList.Items.Add("13 - 867.50MHz (150)")
				tbPower1.Text = "28.00"
				tbPower2.Text = "28.00"
				tbPower3.Text = "28.00"
				tbPower4.Text = "28.00"
				cbFrequencyMode.SelectedIndex = 1
				AddModeIdListItem(New Integer() {0, 1, 2, 3})
				AddAutoSetListItem(New Integer() {2})
				Exit Select
			Case 8
				cbFrequencyMode.Enabled = False
				cbFrequencyList.Enabled = False
				AddModeIdListItem(New Integer() {0, 1, 2, 3})
				AddAutoSetListItem(New Integer() {2})
				Exit Select
			Case 9
				cbFrequencyMode.Enabled = False
				cbFrequencyList.Enabled = False
				AddModeIdListItem(New Integer() {0, 1, 2, 3})
				AddAutoSetListItem(New Integer() {2})
				Exit Select
			Case 10
				cbFrequencyMode.Enabled = False
				cbFrequencyList.Enabled = False
				AddModeIdListItem(New Integer() {0, 1, 2})
				AddAutoSetListItem(New Integer() {2})
				Exit Select
		End Select

		Try
			cbAutoSetMode.SelectedIndex = 1
			cbReaderMode.SelectedIndex = 1
		Catch
		End Try
	End Sub

	'private void AddSessionListItem(int[] sessions)
	'{
	'    cbSession.Items.Clear();
	'    foreach (int i in sessions)
	'    {
	'        cbSession.Items.Add(string.Format("Session {0}", i));
	'    }
	'}

	Private Sub AddModeIdListItem(modeIDs As Integer())
		cbReaderMode.Items.Clear()
		For Each i As Integer In modeIDs
			Dim modeIdStr As String = Nothing
			Select Case i
				Case 0
					modeIdStr = "0"
					Exit Select
				Case 1
					modeIdStr = "1"
					Exit Select
				Case 2
					modeIdStr = "2"
					Exit Select
				Case 3
					modeIdStr = "3"
					Exit Select
			End Select
			cbReaderMode.Items.Add(modeIdStr)
		Next
	End Sub

	Private Sub AddAutoSetListItem(autoSetIDs As Integer())
		cbAutoSetMode.Items.Clear()

		For Each id As Integer In autoSetIDs
			Dim autosetId As String = Nothing
			Select Case id
				Case 2
					autosetId = "2 - By Mode ID"
					Exit Select

			End Select

			cbAutoSetMode.Items.Add(autosetId)
		Next
	End Sub


	Private Sub chSingleTarget_CheckedChanged(sender As Object, e As EventArgs)
		cbSession.SelectedIndex = 0
	End Sub

	Public Sub SetMode(mode As MODE)
		opMode = mode
	End Sub

	Private Sub cbAutoSetMode_SelectedIndexChanged(sender As Object, e As EventArgs)
		cbReaderMode.Enabled = If(cbAutoSetMode.SelectedIndex = 0, True, False)
	End Sub

	Private Sub cbFrequencyMode_SelectedIndexChanged(sender As Object, e As EventArgs)
		cbFrequencyList.Enabled = If(cbFrequencyMode.SelectedIndex = 0, True, False)
	End Sub
End Class
