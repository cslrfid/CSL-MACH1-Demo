'*****************************************************************************
'**
'** @file Operation.cs
'**
'** Last update: Jan 05, 2009
'**
'** This file provides OCS command set definition and implementation.
'** 
'*****************************************************************************


'History
'Jan 05, 2009
'- Fixed a bug on InventoryNtf that does not read additional memory bank correctly.

Imports System.Collections.Generic
Imports System.Collections
Imports System.Text
Imports System.Xml

Namespace CSL.Mach1
	''' <summary>
	''' Mach1 Operation Command Set Class
	''' </summary>
	''' 
	<Serializable> _
	Public Class OPERATION_CMD
		#Region "CONST"
		Public Const GET_OCS_VERSION As Byte = &H0
		Public Const LOAD_FROM_PROFILE As Byte = &H1
		Public Const STORE_TO_PROFILE As Byte = &H2
		Public Const SET_TX_POWER As Byte = &H5
		Public Const GET_TX_POWER As Byte = &H6
		Public Const SET_ANTENNA As Byte = &H7
		Public Const GET_ANTENNA As Byte = &H8
		Public Const SET_REGULATORY_REGION As Byte = &H9
		Public Const GET_CAPABILITY As Byte = &Ha
		Public Const SET_TX_FREQUENCY As Byte = &Hb
		Public Const GET_TX_FREQUENCY As Byte = &Hc
		Public Const SET_GEN2_PARAMS As Byte = &Hd
		Public Const GET_GEN2_PARAMS As Byte = &He
		Public Const GET_SUPPORTED_GEN2_PARAMS As Byte = &H1c
		Public Const CHECK_ANTENNA As Byte = &H1d
		Public Const SET_INVENTORY_REPORT As Byte = &H1e
		Public Const SET_LBT_PARAMS As Byte = &H1f
		Public Const GET_LBT_PARAMS As Byte = &H20
		Public Const INVENTORY As Byte = &H13
		Public Const INVENTORY_CONTINUE As Byte = &H15
		Public Const MODEM_STOP As Byte = &H16
		Public Const RF_SURVEY As Byte = &H14
		Public Const TAG_READ As Byte = &H17
		Public Const TAG_WRITE As Byte = &H18
		Public Const TAG_LOCK As Byte = &H19
		Public Const TAG_KILL As Byte = &H1a
		Public Const TAG_CUSTOM As Byte = &H1b
		Public Const REPORT_INVENTORY As Byte = &H21
		Public Const SET_RX_CONFIG As Byte = &H22
		Public Const GET_RX_CONFIG As Byte = &H23
		Public Const SET_PROFILE_SEQUENCE As Byte = &H24

		Public Const ANTENNA_PORT_1 As Byte = &H1
		Public Const ANTENNA_PORT_2 As Byte = &H2
		Public Const ANTENNA_PORT_3 As Byte = &H4
		Public Const ANTENNA_PORT_4 As Byte = &H8
		#End Region

		Public Enum SOURCE
			STORE_TO_MODEM = 0
			RESET_TO_FACTORY_DEFAULT
		End Enum
		Public Enum FREQUENCY_SET_MODE
			CENTER_FREQUENCY
			CHOOSE_FROM_REGULATORY
			CHOOSE_FROM_LIST
			REDUCED_POWER_FREQUENCY_LIST
		End Enum
		Public Enum MEASUREMENT_BANDWIDTH
			HZ_100K = 1
			HZ_300K = 3
		End Enum
		Public Enum ANTENNA As Byte
			ANT1 = &H1
			ANT2 = &H2
			ANT3 = &H4
			ANT4 = &H8
		End Enum
		Public Enum MEMORY_BANK
			RESERVED = 0
			EPC
			TID
			USER
			[DEFAULT] = 1
		End Enum
		Public Enum SET_ANTENNA_RESULT
			SUCCESS
			PORT_NOT_AVAILABLE
		End Enum
		Public Enum SET_FREQUENCY_RESULT
			SUCCESS
			[ERROR]
			FREQUENCY_LIST_OUT_OF_RANGE
			AUTOSET_NOT_VALID
		End Enum
		Public Enum SET_GEN2_PARAMS_RESULT
			SUCCESS
			COMBINATION_NOT_SUPPORTED
			COMBINATION_NOT_SUPPORTED_BY_REGULATORY
			MODE_ID_NOT_SUPPORTED
			SESSION_AND_INVENTORY_MODE_COMBINATION_NOT_SUPPORTED
		End Enum
		Public Enum SET_RX_SENSITIVITY_MODE
			MAXIMUM_SENSITIVITY = 0
			FIXED_PER_ANTENNA = 1
		End Enum
		Public Enum SET_TX_POWER_RESULT
			SUCESS = 0
			ERROR_DISALLOWED
			ERROR_NOT_SUPPORTED
			ERROR_REQUIRE_PROFESSIONAL_INSTALLATION
		End Enum
		Public Enum REPORT_INVENTORY_RESULT
			SUCCESS_BUFFER_WILL_BE_FLUSH = 0
			SUCCESS_FLUSH_IN_PROCESS
			SUCCESS_BUFFER_EMPTY
			[ERROR]
		End Enum

		Public Enum INVENTORY_RESULT
			SUCCESS = 0
			FAIL_CONFIGURATION_ERROR
			GEN2READLENGTH_EXCEED
			PROFILE_SEQUENCING_DISABLED
			PROFILE_SEQUENCE_INDEX_INVALID
		End Enum

		''' <summary>
		''' Inventory Parameters Class. For how to use inventory parameters, refer to Mach1 - Operation Command Set
		''' </summary>
		<Serializable> _
		Public Class INVENTORY_PRAMA
			Public Enum INVENTORY_FILTER_OPERATION
				A = 0
				A_OR_B
				A_AND_B
				NONE
			End Enum
			Public Enum INVENTORY_HALT_OPERATION
				A = 0
				A_OR_B
				A_AND_B
				HALT_EVERY_TAG
			End Enum
			Public Enum LOGIC
				EQUALS = 0
				NOT_EQUAL
				GREATER_THAN
				LESS_THAN
			End Enum

			''' <summary>
			''' Inventory Filter Condition Class
			''' </summary>
			<Serializable> _
			Public Class INVENTORY_FILTER_CONDITION
				''' <summary>
				''' Inventory Filter Operation
				''' </summary>
				Public filter_operation As INVENTORY_FILTER_OPERATION = INVENTORY_FILTER_OPERATION.A
				''' <summary>
				''' A filter memory bank
				''' </summary>
				Public a_filter_memory_bank As MEMORY_BANK = MEMORY_BANK.[DEFAULT]
				''' <summary>
				''' B filter memory bank
				''' </summary>
				Public b_filter_memory_bank As MEMORY_BANK = MEMORY_BANK.[DEFAULT]
				''' <summary>
				''' A filter offset bit position 
				''' </summary>
				Public a_bit_offset As UInt16 = 0
				''' <summary>
				''' B filter offset bit position
				''' </summary>
				Public b_bit_offset As UInt16 = 0
				''' <summary>
				''' A filter pattern length
				''' </summary>
				Public a_length As UInt16 = 0
				''' <summary>
				''' B  filter pattern length
				''' </summary>
				Public b_length As UInt16 = 0
				''' <summary>
				''' A filter pattern
				''' </summary>
				Public a_pattern As String
				''' <summary>
				''' B filter pattern
				''' </summary>
				Public b_pattern As String
				''' <summary>
				''' A filter logic
				''' </summary>
				Public a_compare As LOGIC = LOGIC.EQUALS
				''' <summary>
				''' B filter logic
				''' </summary>
				Public b_compare As LOGIC = LOGIC.EQUALS

				Public Sub New()
					a_pattern = String.Empty
					b_pattern = String.Empty
				End Sub
			End Class

			''' <summary>
			''' Inventory Halt Condition Class. 
			''' </summary>
			<Serializable> _
			Public Class INVENTORY_HALT_CONDITION
				''' <summary>
				''' Halt Operation
				''' </summary>
				Public halt_operation As INVENTORY_HALT_OPERATION = INVENTORY_HALT_OPERATION.A
				''' <summary>
				''' A halt filter memory bank, only valid when halt_operation requires A filter
				''' </summary>
				Public halt_a_memory_bank As MEMORY_BANK = MEMORY_BANK.[DEFAULT]
				''' <summary>
				''' B halt filter memory bank, only valid when halt_operation requires B filter
				''' </summary>
				Public halt_b_memory_bank As MEMORY_BANK = MEMORY_BANK.[DEFAULT]
				''' <summary>
				''' A filter offset
				''' </summary>
				Public halt_a_bit_offset As UInt16
				''' <summary>
				''' B filter offset
				''' </summary>
				Public halt_b_bit_offset As UInt16
				''' <summary>
				''' A filter length
				''' </summary>
				Public halt_a_length As UInt16
				''' <summary>
				''' B filter length
				''' </summary>
				Public halt_b_length As UInt16
				''' <summary>
				''' A filter mask
				''' </summary>
				Public halt_a_mask As String
				''' <summary>
				''' B filter mask
				''' </summary>
				Public halt_b_mask As String
				''' <summary>
				''' A filter value, in form of bit array
				''' </summary>
				Public halt_a_value As String
				''' <summary>
				''' B filter value, in form of bit array
				''' </summary>
				Public halt_b_value As String
				''' <summary>
				''' A filter logic
				''' </summary>
				Public halt_a_compare As LOGIC = LOGIC.EQUALS
				''' <summary>
				''' B filter logic
				''' </summary>
				Public halt_b_compare As LOGIC = LOGIC.EQUALS

				Public Sub New()
					halt_a_mask = String.Empty
					halt_a_value = String.Empty
					halt_b_mask = String.Empty
					halt_b_value = String.Empty
				End Sub
			End Class

			''' <summary>
			''' Enable inventory filter
			''' </summary>
			Public enable_inventory_filter As Boolean = False

			''' <summary>
			''' Enable halt filter
			''' </summary>
			Public enable_halt_filter As Boolean = False

            Public _inventory_filter_condition As INVENTORY_FILTER_CONDITION
            Public _inventory_halt_condition As INVENTORY_HALT_CONDITION
            Public read_memory_bank As MEMORY_BANK = MEMORY_BANK.[DEFAULT]
            Public read_word_memory_address As UInt16 = 0
            Public read_length As Byte = 0
            Public estimated_tag_population As Int16 = -1
            Public estimated_tag_time_in_field As Int16 = -1

            Public emptyFieldTimeOut As Int16 = -1
            Public fieldPingInterval As Int16 = -1
            Public profileSequenceIndex As Int16 = -1
            Public reportNullEPCs As Boolean = False

            ''' <summary>
            ''' Default constructor
            ''' </summary>
            Public Sub New()
                _inventory_filter_condition = New INVENTORY_FILTER_CONDITION()
                _inventory_halt_condition = New INVENTORY_HALT_CONDITION()
            End Sub
            ''' <summary>
            ''' Constructor with parameters
            ''' </summary>
            ''' <param name="enable_inventory_filter"></param>
            ''' <param name="enable_halt_filter"></param>
            Public Sub New(enable_inventory_filter As Boolean, enable_halt_filter As Boolean)
                Me.enable_halt_filter = enable_halt_filter
                Me.enable_inventory_filter = enable_inventory_filter

                _inventory_filter_condition = New INVENTORY_FILTER_CONDITION()
                _inventory_halt_condition = New INVENTORY_HALT_CONDITION()
            End Sub

            ''' <summary>
            ''' Convert Inventory Parameters to byte array
            ''' </summary>
            ''' <returns></returns>
            Public Function ToByteArray() As Byte()
                Dim temp As Byte() = New Byte(1023) {}
                Dim idx As Integer = 0

                Try
                    '#Region "Inventory Filter"
                    If enable_inventory_filter Then
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H1
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_filter_condition.filter_operation)

                        'set A filter
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H2
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_filter_condition.a_filter_memory_bank)
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H3
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_filter_condition.a_bit_offset >> 8)
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_filter_condition.a_bit_offset And &HFF)
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H4
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_filter_condition.a_length >> 8)
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_filter_condition.a_length And &HFF)

                        Dim patternA As Byte() = Util.ConvertBinaryStringArrayToBytes(_inventory_filter_condition.a_pattern, _inventory_filter_condition.a_length)
                        If patternA.Length > 0 Then
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H5
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte((patternA.Length And &H300) >> 8)
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(patternA.Length And &HFF)
                            For i As Integer = 0 To patternA.Length - 1
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = patternA(i)
                            Next
                        End If

                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H6
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_filter_condition.a_compare)

                        'set B filter
                        Select Case _inventory_filter_condition.filter_operation
                            Case INVENTORY_FILTER_OPERATION.A
                                Exit Select
                            Case INVENTORY_FILTER_OPERATION.A_AND_B, INVENTORY_FILTER_OPERATION.A_OR_B
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H7
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_filter_condition.b_filter_memory_bank)
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H8
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_filter_condition.b_bit_offset >> 8)
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_filter_condition.b_bit_offset And &HFF)
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H9
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_filter_condition.b_length >> 8)
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_filter_condition.b_length And &HFF)
                                Dim patternB As Byte() = Util.ConvertBinaryStringArrayToBytes(_inventory_filter_condition.b_pattern, _inventory_filter_condition.b_length)
                                If patternB.Length > 0 Then
                                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &HA
                                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte((patternB.Length And &H300) >> 8)
                                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(patternB.Length And &HFF)
                                    For i As Integer = 0 To patternB.Length - 1
                                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = patternB(i)
                                    Next
                                End If
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &HB
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_filter_condition.b_compare)
                                Exit Select

                        End Select
                    End If
                    '#End Region

                    '#Region "Read Memory Bank"
                    If read_memory_bank <> MEMORY_BANK.[DEFAULT] Then
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &HC
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(read_memory_bank)
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &HD
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(read_word_memory_address >> 8)
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(read_word_memory_address And &HFF)
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &HE
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = read_length
                    End If

                    '#End Region

                    '#Region "Halt Filter"
                    If enable_halt_filter Then
                        If _inventory_halt_condition.halt_operation = INVENTORY_HALT_OPERATION.HALT_EVERY_TAG Then
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &HF
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H0
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H12
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H0
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H0
                        Else
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &HF
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_halt_condition.halt_operation)
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H12
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_halt_condition.halt_a_length >> 8)
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_halt_condition.halt_a_length And &HFF)

                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H10
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_halt_condition.halt_a_memory_bank)

                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H11
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_halt_condition.halt_a_bit_offset >> 8)
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_halt_condition.halt_a_bit_offset And &HFF)

                            Dim haltMask As Byte() = Util.ConvertBinaryStringArrayToBytes(_inventory_halt_condition.halt_a_mask, _inventory_halt_condition.halt_a_length)
                            If haltMask.Length > 0 Then
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H13
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte((haltMask.Length And &H300) >> 8)
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(haltMask.Length And &HFF)
                                For i As Integer = 0 To haltMask.Length - 1
                                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = haltMask(i)
                                Next
                            End If

                            Dim haltPA As Byte() = Util.ConvertBinaryStringArrayToBytes(_inventory_halt_condition.halt_a_value, _inventory_halt_condition.halt_a_length)
                            If haltPA.Length > 0 Then
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H14
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte((haltPA.Length And &H300) >> 8)
                                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(haltPA.Length And &HFF)
                                For i As Integer = 0 To haltPA.Length - 1
                                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = haltPA(i)
                                Next
                            End If

                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H15
                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_halt_condition.halt_a_compare)

                            Select Case _inventory_halt_condition.halt_operation
                                Case INVENTORY_HALT_OPERATION.A
                                    Exit Select
                                Case INVENTORY_HALT_OPERATION.A_AND_B, INVENTORY_HALT_OPERATION.A_OR_B
                                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H16
                                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_halt_condition.halt_b_memory_bank)
                                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H17
                                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_halt_condition.halt_b_bit_offset >> 8)
                                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_halt_condition.halt_b_bit_offset And &HFF)
                                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H18
                                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_halt_condition.halt_b_length >> 8)
                                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_halt_condition.halt_b_length And &HFF)

                                    haltMask = Util.ConvertBinaryStringArrayToBytes(_inventory_halt_condition.halt_b_mask, _inventory_halt_condition.halt_b_length)
                                    If haltMask.Length > 0 Then
                                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H19
                                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte((haltMask.Length And &H300) >> 8)
                                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(haltMask.Length And &HFF)
                                        For i As Integer = 0 To haltMask.Length - 1
                                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = haltMask(i)
                                        Next
                                    End If
                                    Dim haltPB As Byte() = Util.ConvertBinaryStringArrayToBytes(_inventory_halt_condition.halt_b_value, _inventory_halt_condition.halt_b_length)
                                    If haltPB.Length > 0 Then
                                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H1A
                                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte((haltPB.Length And &H300) >> 8)
                                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(haltPB.Length And &HFF)
                                        For i As Integer = 0 To haltPB.Length - 1
                                            temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = haltPB(i)
                                        Next
                                    End If

                                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H1B
                                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_inventory_halt_condition.halt_b_compare)
                                    Exit Select
                            End Select
                        End If
                    End If
                    '#End Region

                    If estimated_tag_population >= 0 Then
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H1C
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(estimated_tag_population >> 8)
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(estimated_tag_population And &HFF)
                    End If

                    If estimated_tag_time_in_field >= 0 Then
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H1D
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(estimated_tag_time_in_field >> 8)
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(estimated_tag_time_in_field And &HFF)
                    End If

                    If emptyFieldTimeOut >= 0 AndAlso fieldPingInterval >= 0 Then
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H1E
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(emptyFieldTimeOut >> 8)
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(emptyFieldTimeOut And &HFF)

                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H1F
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(fieldPingInterval >> 8)
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(fieldPingInterval And &HFF)
                    End If

                    If profileSequenceIndex > -1 Then
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H20
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(profileSequenceIndex And &HFF)
                    End If

                    If reportNullEPCs Then
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H21
                        temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H1
                    End If

                    Dim data As Byte()

                    If idx = 0 Then
                        data = Nothing
                    Else
                        data = New Byte(idx - 1) {}
                        Array.Copy(temp, data, idx)
                    End If
                    Return data
                Catch
                    Return Nothing
                End Try
            End Function
        End Class

        ''' <summary>
        ''' Gen2 Parameters Class. It is recommended to used Mode ID to set these parameters 
        ''' </summary>
        <Serializable>
        Public Class GEN2_PARAM
            Public Enum SESSION
                SESSION_0 = 0
                SESSION_1 = 1
                SESSION_2
                SESSION_3
            End Enum
            Public Enum GEN2_LINK_MODE
                BY_APPLICATION
                SELF_CONFIGURE_DENSE
                BY_MODE_ID_OF_APPLICATION
                SELF_CONFIGURE_SINGLE

            End Enum
            Public Enum MODEM_MODULATION
                PR_ASK
                DSB_ASK
            End Enum
            Public Enum TARI
                US_6_25
                US_7_14
                US_8_33
                US_10_0
                US_12_5
                US_16_67
                US_20_0
                US_25_0
            End Enum
            Public Enum PIE
                P_1_5_vs_1
                P_1_67_vs_1
                P_2_0_vs_1
            End Enum
            Public Enum PW
                [SHORT]
                [LONG]
            End Enum
            Public Enum TR_FREQUENCY
                HZ_40K
                HZ_64K
                HZ_80K
                HZ_128K
                HZ_160K
                HZ_213_3K
                HZ_256K
                HZ_320K
                HZ_640K
            End Enum
            Public Enum TR_LINK_MODULATION
                FM0 = 0
                MILLER_M2
                MILLER_M4
                MILLER_M8
            End Enum
            Public Enum DEVIDE_RATIO
                RATIO_64_3
                RATIO_8
            End Enum
            Public Enum INVENTORY_SEARCH_MODE
                [DEFAULT] = 0
                SINGLE_TARGET_INVENTORY
                SINGLE_TARGET_INVENTORY_WITH_SUPPRESSED_DUPLICATE_REDUNDANCY
            End Enum
            Public Enum MODE_ID
                MAX_THROUGHPUT = 0
                HYBRID_MODE
                DENSE_READER_M4
                DENSE_READER_M8
                MAX_THROUGH_PUT_MILLER
            End Enum

            Public _session As SESSION = SESSION.SESSION_1
            Public auto_set_mode As GEN2_LINK_MODE = GEN2_LINK_MODE.SELF_CONFIGURE_DENSE
            Public _modem_modulation As MODEM_MODULATION = _modem_modulation.PR_ASK
            Public _tari As TARI = _tari.US_12_5
            Public _pie As PIE = _pie.P_1_5_vs_1
            Public _pw As PW = _pw.[SHORT]
            Public tag_to_reader_link_rate As TR_FREQUENCY = TR_FREQUENCY.HZ_160K
            Public tag_to_reader_link_modulation As TR_LINK_MODULATION = TR_LINK_MODULATION.FM0
            Public handle_reporting As Boolean = False
            Public _devide_ratio As DEVIDE_RATIO = _devide_ratio.RATIO_64_3

            Public _mode_id As MODE_ID
            Public inv_search_mode As INVENTORY_SEARCH_MODE = INVENTORY_SEARCH_MODE.[DEFAULT]

            ''' <summary>
            ''' Default constructor
            ''' </summary>
            Public Sub New()
            End Sub

            ''' <summary>
            ''' Constructor for parsing Gen2 paramters 
            ''' </summary>
            ''' <param name="data"></param>
            Public Sub New(data As Byte())
                Try
                    _session = CType(data(0), SESSION)
                    auto_set_mode = CType(data(1), GEN2_LINK_MODE)
                    Dim offset As Integer = 2

                    While offset < data.Length
                        Select Case data(offset)
                            Case &H1
                                offset += 1
                                _modem_modulation = CType(data(offset), MODEM_MODULATION)
                                offset += 1
                                Exit Select
                            Case &H2
                                offset += 1
                                _tari = CType(data(offset), TARI)
                                offset += 1
                                Exit Select
                            Case &H3
                                offset += 1
                                _pie = CType(data(offset), PIE)
                                offset += 1
                                Exit Select
                            Case &H4
                                offset += 1
                                _pw = CType(data(offset), PW)
                                offset += 1
                                Exit Select
                            Case &H5
                                offset += 1
                                tag_to_reader_link_rate = CType(data(offset), TR_FREQUENCY)
                                offset += 1
                                Exit Select
                            Case &H6
                                offset += 1
                                tag_to_reader_link_modulation = CType(data(offset), TR_LINK_MODULATION)
                                offset += 1
                                Exit Select
                            Case &H7
                                offset += 1
                                handle_reporting = If((data(offset) = 1), True, False)
                                offset += 1
                                Exit Select
                            Case &H8
                                offset += 1
                                _devide_ratio = CType(data(offset), DEVIDE_RATIO)
                                offset += 1
                                Exit Select
                            Case &H9
                                offset += 1
                                _mode_id = CType(CType(data(offset) * 256 + data(offset + 1), UInt16), MODE_ID)
                                offset += 2
                                Exit Select
                            Case &HA
                                offset += 1
                                inv_search_mode = CType(data(offset), INVENTORY_SEARCH_MODE)
                                offset += 1
                                Exit Select

                        End Select

                    End While
                Catch
                End Try
            End Sub

            ''' <summary>
            ''' Generate byte array
            ''' </summary>
            ''' <returns></returns>
            Public Function ToByteArray() As Byte()
                Dim temp As Byte() = New Byte(1023) {}
                Dim idx As Integer = 0

                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_session)
                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(auto_set_mode)

                'Bug
                'Reported by Xiaoyong Su,  01/09/2008
                'Fixed by Xiaoyong Su, 01/09/2008
                If auto_set_mode = GEN2_LINK_MODE.BY_APPLICATION Then
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H1
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_modem_modulation)
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H2
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_tari)
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H3
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_pie)
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H4
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_pw)
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H5
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(tag_to_reader_link_rate)
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H6
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(tag_to_reader_link_modulation)
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H7
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(If(handle_reporting, 1, 0))
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H8
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(_devide_ratio)
                ElseIf auto_set_mode = GEN2_LINK_MODE.BY_MODE_ID_OF_APPLICATION Then
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H9
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(CType(_mode_id, UInt16) >> 8)
                    temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(CType(_mode_id, UInt16) And &HFF)
                End If

                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &HA
                temp(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(inv_search_mode)

                Dim data As Byte() = New Byte(idx - 1) {}

                Array.Copy(temp, data, idx)

                Return data
            End Function
        End Class

        ''' <summary>
        ''' Supported Gen2 Params Rsp Class
        ''' </summary>
        <Serializable>
        Public Class SUPPORTED_GEN2_PARAMS_RSP
            Public num_supported_sets As UInt16
            Public modulations As GEN2_PARAM.MODEM_MODULATION()
            Public taris As GEN2_PARAM.TARI()
            Public pies As GEN2_PARAM.PIE()
            Public pws As GEN2_PARAM.PW()
            Public tr_link_frequencyes As GEN2_PARAM.TR_FREQUENCY()
            Public tr_link_modulations As GEN2_PARAM.TR_LINK_MODULATION()
            Public d_ratios As GEN2_PARAM.DEVIDE_RATIO()
            Public mode_ids As GEN2_PARAM.MODE_ID()
            Public inv_search_modes As GEN2_PARAM.INVENTORY_SEARCH_MODE()

            ''' <summary>
            ''' Constructor for parsing data
            ''' </summary>
            ''' <param name="data"></param>
            Public Sub New(data As Byte())
                Dim offset As Integer = 0
                Try
                    num_supported_sets = data(0)
                    offset += 1
                    Dim result_sets As Integer = data(offset) * 256 + data(offset + 1)

                    offset += 2
                    modulations = New GEN2_PARAM.MODEM_MODULATION(result_sets - 1) {}
                    For i As Integer = 0 To result_sets - 1
                        modulations(i) = CType(data(Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)), GEN2_PARAM.MODEM_MODULATION)
                    Next

                    result_sets = data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) * 256 + data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1))
                    taris = New GEN2_PARAM.TARI(result_sets - 1) {}
                    For i As Integer = 0 To result_sets - 1
                        taris(i) = CType(data(Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)), GEN2_PARAM.TARI)
                    Next

                    result_sets = data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) * 256 + data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1))
                    pies = New GEN2_PARAM.PIE(result_sets - 1) {}
                    For i As Integer = 0 To result_sets - 1
                        pies(i) = CType(data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)), GEN2_PARAM.PIE)
                    Next

                    result_sets = data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) * 256 + data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1))
                    pws = New GEN2_PARAM.PW(result_sets - 1) {}
                    For i As Integer = 0 To result_sets - 1
                        pws(i) = CType(data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)), GEN2_PARAM.PW)
                    Next

                    result_sets = data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) * 256 + data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1))
                    tr_link_frequencyes = New GEN2_PARAM.TR_FREQUENCY(result_sets - 1) {}
                    For i As Integer = 0 To result_sets - 1
                        tr_link_frequencyes(i) = CType(data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)), GEN2_PARAM.TR_FREQUENCY)
                    Next

                    result_sets = data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) * 256 + data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1))
                    tr_link_modulations = New GEN2_PARAM.TR_LINK_MODULATION(result_sets - 1) {}
                    For i As Integer = 0 To result_sets - 1
                        tr_link_modulations(i) = CType(data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)), GEN2_PARAM.TR_LINK_MODULATION)
                    Next

                    result_sets = data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) * 256 + data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1))
                    d_ratios = New GEN2_PARAM.DEVIDE_RATIO(result_sets - 1) {}
                    For i As Integer = 0 To result_sets - 1
                        d_ratios(i) = CType(data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)), GEN2_PARAM.DEVIDE_RATIO)
                    Next

                    While offset < data.Length
                        If offset < data.Length AndAlso data(offset) = &H1 Then
                            Try
                                offset += 1
                                result_sets = data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) * 256 + data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1))
                                mode_ids = New GEN2_PARAM.MODE_ID(result_sets - 1) {}
                                For i As Integer = 0 To result_sets - 1
                                    mode_ids(i) = CType(data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)), GEN2_PARAM.MODE_ID)
                                Next
                            Catch
                            End Try


                        End If

                        If offset < data.Length AndAlso data(offset) = &H2 Then
                            Try
                                offset += 1
                                result_sets = data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) * 256 + data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1))
                                inv_search_modes = New GEN2_PARAM.INVENTORY_SEARCH_MODE(result_sets - 1) {}
                                For i As Integer = 0 To result_sets - 1
                                    inv_search_modes(i) = CType(data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)), GEN2_PARAM.INVENTORY_SEARCH_MODE)
                                Next
                            Catch
                            End Try


                        End If

                    End While
                Catch
                End Try
            End Sub
        End Class

        ''' <summary>
        ''' Operation Command Set Version Rsp Class
        ''' </summary>
        <Serializable>
        Public Class OCS_VERSION_RSP
            Public version As String = String.Empty

            Public Sub New(data As Byte())
                Try
                    version = String.Format("v.{0}.{1}.{2}", data(0), data(1), data(2))
                Catch
                End Try
            End Sub
        End Class

        ''' <summary>
        ''' Tag Lock Operation Command Class. Refer to EPC Gen2 protocol for more information
        ''' </summary>
        <Serializable>
        Public Class TAG_LOCK_OPERATION

            Public Class OPERATION
                Public enable_read_writeble_bit As Boolean = False
                Public read_writeble As Boolean = False
                Public enable_perma_lock_bit As Boolean = False
                Public perma_lock As Boolean = False
            End Class

            Public kill_pwd As OPERATION
            Public access_pwd As OPERATION
            Public epc_memory As OPERATION
            Public tid_memory As OPERATION
            Public user_memory As OPERATION

            Public Sub New()
                kill_pwd = New OPERATION()
                access_pwd = New OPERATION()
                epc_memory = New OPERATION()
                tid_memory = New OPERATION()
                user_memory = New OPERATION()
            End Sub

            Public Function ToByteArray() As Byte()
                Dim br As New BitArray(32)
                br.SetAll(False)

                br.[Set](12, kill_pwd.enable_read_writeble_bit)
                br.[Set](13, kill_pwd.enable_perma_lock_bit)
                br.[Set](14, access_pwd.enable_read_writeble_bit)
                br.[Set](15, access_pwd.enable_perma_lock_bit)
                br.[Set](16, epc_memory.enable_read_writeble_bit)
                br.[Set](17, epc_memory.enable_perma_lock_bit)
                br.[Set](18, tid_memory.enable_read_writeble_bit)
                br.[Set](19, tid_memory.enable_perma_lock_bit)
                br.[Set](20, user_memory.enable_read_writeble_bit)
                br.[Set](21, user_memory.enable_perma_lock_bit)

                br.[Set](22, kill_pwd.read_writeble)
                br.[Set](23, kill_pwd.perma_lock)
                br.[Set](24, access_pwd.read_writeble)
                br.[Set](25, access_pwd.perma_lock)
                br.[Set](26, epc_memory.read_writeble)
                br.[Set](27, epc_memory.perma_lock)
                br.[Set](28, tid_memory.read_writeble)
                br.[Set](29, tid_memory.perma_lock)
                br.[Set](30, user_memory.read_writeble)
                br.[Set](31, user_memory.perma_lock)

                Try
                    Dim s As String = String.Empty
                    For i As Integer = 0 To br.Length - 1
                        s += If(br(i), "1", "0")
                    Next
                    Return Util.ConvertBinaryStringArrayToBytes(s, 0)
                Catch
                    Return Nothing
                End Try

            End Function
        End Class

        ''' <summary>
        ''' Optional Inventory Report Params. Refer to Mach1 - Operation Command Set
        ''' </summary>
        <Serializable>
        Public Class OPTIONAL_INVENTORY_REPORT_PARAM
            Public Enum INVENTORY_REPORTING_MODE
                IMMEDIATE_REPORT = 0
                ACCUMULATED_REPORT
            End Enum
            Public Enum ADD_BEHAVIOR
                DONT_REPORT_WHEN_ADDED = 0
                REPORT_WHEN_ADDED
            End Enum
            Public Enum INVENTORY_ATTEMPT_COUNT_REPORTING
                DONT_REPORT = 0
                REPORT
            End Enum

            Public Enum DROP_BEHAVIOR
                DONT_REPORT_WHEN_DROPPED = 0
                REPORT_WHEN_DROPPED
            End Enum

            Public Enum BUFFER_FULL_BEHAVIOR
                DROP_NEWEST_INVENOTRY_NTF = 0
                DROP_ALL_ENTRIES
            End Enum

            Public inventory_report_mode As INVENTORY_REPORTING_MODE = INVENTORY_REPORTING_MODE.ACCUMULATED_REPORT
            Public _add_behavior As ADD_BEHAVIOR = ADD_BEHAVIOR.DONT_REPORT_WHEN_ADDED
            Public inventory_attemp_count_reporting As INVENTORY_ATTEMPT_COUNT_REPORTING = INVENTORY_ATTEMPT_COUNT_REPORTING.DONT_REPORT
            Public _drop_behavior As DROP_BEHAVIOR = _drop_behavior.DONT_REPORT_WHEN_DROPPED
            Public _buffer_full_behavior As BUFFER_FULL_BEHAVIOR = _buffer_full_behavior.DROP_ALL_ENTRIES
        End Class


#Region "GENERATE_PACKETS"
        ''' <summary>
        ''' Generate GetOCSVersionCmd Message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_GET_OCS_VERSION_CMD(include_timestamp As Boolean) As Byte()
            Dim cmd As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_OCS_VERSION, include_timestamp)
            Return cmd.PACKET
        End Function
        ''' <summary>
        ''' Generate LoadFromProfileCmd Message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="profile_index"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_LOAD_FROM_PROFILE_CMD(include_timestamp As Boolean, profile_index As UInt16) As Byte()
            Dim data As Byte() = New Byte(0) {}
            data(0) = CByte(profile_index)

            Dim cmd As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.LOAD_FROM_PROFILE, include_timestamp, data)
            Return cmd.PACKET
        End Function

        ''' <summary>
        ''' Generate StoreToProfileCmd Message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="idx"></param>
        ''' <param name="dst"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_STORE_TO_PROFILE_CMD(include_timestamp As Boolean, idx As UInt16, dst As SOURCE) As Byte()
            Dim data As Byte() = New Byte(1) {}

            data(0) = CByte(idx)
            data(1) = CByte(dst)

            Dim cmd As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.STORE_TO_PROFILE, include_timestamp, data)
            Return cmd.PACKET
        End Function

        ''' <summary>
        ''' Generate InventoryCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="prama"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_INVENTORY_CMD(include_timestamp As Boolean, prama As INVENTORY_PRAMA) As Byte()
            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.INVENTORY, include_timestamp, prama.ToByteArray())
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate SetTxPowerCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="txPower"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_SET_TX_POWER_CMD(include_timestamp As Boolean, txPower As Byte()) As Byte()
            Dim data As Byte()

            If txPower.Length = 1 Then
                data = New Byte(0) {}
                data(0) = txPower(0)
            Else
                data = New Byte(txPower.Length + 2) {}
                data(0) = txPower(0)

                data(1) = &H1
                data(2) = &H0
                data(3) = CByte(txPower.Length - 1)

                Array.Copy(txPower, 1, data, 4, txPower.Length - 1)
            End If

            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_TX_POWER, include_timestamp, data)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate GetTxPowerCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_GET_TX_POWER_CMD(include_timestamp As Boolean) As Byte()
            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_TX_POWER, include_timestamp)

            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate SetAntennaCmd message
        ''' </summary>
        ''' <param name="include_timerstamp"></param>
        ''' <param name="antennas"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_SET_ANTENNA_CMD(include_timerstamp As Boolean, antennas As Byte) As Byte()

            Dim data As Byte() = New Byte(0) {}
            data(0) = antennas

            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_ANTENNA, include_timerstamp, data)

            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate GetAntennaCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_GET_ANTENNA_CMD(include_timestamp As Boolean) As Byte()
            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_ANTENNA, include_timestamp)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate SetRegulatoryRegion message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="region"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_SET_REGULATORY_REGION(include_timestamp As Boolean, region As REGULATORY_REGION) As Byte()
            Dim data As Byte() = New Byte(1) {}

            data(0) = CByte(CType(region, UInt16) >> 8)
            data(1) = CByte(CType(region, UInt16) And &HFF)

            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_REGULATORY_REGION, include_timestamp, data)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate SetTxFrequency message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="mode"></param>
        ''' <param name="center_frequency_index"></param>
        ''' <param name="frequency_list"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_SET_TX_FREQUENCY(include_timestamp As Boolean, mode As FREQUENCY_SET_MODE, center_frequency_index As UInt16, frequency_list As UInt16(), reduced_power_frequence_list As UInt16()) As Byte()
            Dim data As Byte()

            Select Case mode
                Case FREQUENCY_SET_MODE.CENTER_FREQUENCY
                    data = New Byte(3) {}
                    data(1) = &H1
                    data(2) = CByte(CType(center_frequency_index, UInt16) >> 8)
                    data(3) = CByte(CType(center_frequency_index, UInt16) And &HFF)
                    Exit Select
                Case FREQUENCY_SET_MODE.CHOOSE_FROM_LIST
                    data = New Byte(5 + (frequency_list.Length - 1)) {}
                    data(0) = CByte(mode)
                    data(1) = &H2
                    data(2) = CByte((frequency_list.Length * 2 And &HFF00) >> 8)
                    data(3) = CByte(frequency_list.Length * 2 And &HFF)
                    For i As Integer = 0 To frequency_list.Length - 1
                        data(4 + 2 * i) = CByte(CType(frequency_list(i), UInt16) >> 8)
                        data(5 + 2 * i) = CByte(CType(frequency_list(i), UInt16) And &HFF)
                    Next
                    Exit Select
                Case FREQUENCY_SET_MODE.REDUCED_POWER_FREQUENCY_LIST
                    data = New Byte(5 + (reduced_power_frequence_list.Length - 1)) {}
                    data(0) = CByte(mode)
                    data(1) = &H2
                    data(2) = CByte((reduced_power_frequence_list.Length * 2 And &HFF00) >> 8)
                    data(3) = CByte(reduced_power_frequence_list.Length * 2 And &HFF)
                    For i As Integer = 0 To reduced_power_frequence_list.Length - 1
                        data(4 + 2 * i) = CByte(CType(reduced_power_frequence_list(i), UInt16) >> 8)
                        data(5 + 2 * i) = CByte(CType(reduced_power_frequence_list(i), UInt16) And &HFF)
                    Next
                    Exit Select
                Case FREQUENCY_SET_MODE.CHOOSE_FROM_REGULATORY
                    data = New Byte(0) {}
                    data(0) = CByte(mode)
                    Exit Select
                Case Else
                    data = New Byte(0) {}
                    data(0) = CByte(FREQUENCY_SET_MODE.CHOOSE_FROM_REGULATORY)
                    Exit Select
            End Select

            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_TX_FREQUENCY, include_timestamp, data)
            Return mf.PACKET
        End Function
        ''' <summary>
        ''' Generate GetTxFrequency message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_GET_TX_FREQUENCY(include_timestamp As Boolean) As Byte()
            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_TX_FREQUENCY, include_timestamp)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate SetGen2ParamsCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="param"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_SET_GEN2_PARAMS_CMD(include_timestamp As Boolean, param As GEN2_PARAM) As Byte()
            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_GEN2_PARAMS, include_timestamp, param.ToByteArray())
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate GetGen2ParamsCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_GET_GEN2_PARAMS_CMD(include_timestamp As Boolean) As Byte()
            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_GEN2_PARAMS, include_timestamp)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate GetGen2ParamsCmd message
        ''' </summary>
        ''' <param name="report_search_mode"></param>
        ''' <param name="include_timestamp"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_GET_GEN2_PARAMS_CMD(report_search_mode As Boolean, include_timestamp As Boolean) As Byte()
            Dim data As Byte() = New Byte(1) {}
            data(0) = &H1
            data(1) = CByte(If(report_search_mode, 1, 0))
            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_GEN2_PARAMS, include_timestamp, data)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate GetSupportedGen2ParamsCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="includeModeId"></param>
        ''' <param name="includeInventorySearchMode"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_GET_SUPPORTED_GEN2_PARAMS_CMD(include_timestamp As Boolean, includeModeId As Boolean, includeInventorySearchMode As Boolean) As Byte()
            Dim data As Byte() = New Byte(3) {}
            data(0) = &H1
            data(1) = CByte(If(includeModeId, 1, 0))
            data(2) = &H2
            data(3) = CByte(If(includeInventorySearchMode, 1, 0))

            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_SUPPORTED_GEN2_PARAMS, include_timestamp, data)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate CheckAntennaCmd Message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_CHECK_ANTENNA_CMD(include_timestamp As Boolean) As Byte()
            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.CHECK_ANTENNA, include_timestamp)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate GetCapabilitiesCmd Message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="report"></param>
        ''' <returns></returns>
        ''' 
        Public Shared Function GENERATE_GET_CAPABILITIES_CMD(include_timestamp As Boolean, reportPower As Boolean, frequey As Boolean) As Byte()
            Dim data As Byte() = New Byte(3) {}
            data(0) = &H1
            data(1) = CByte(If(reportPower, 1, 0))
            data(2) = &H2
            data(3) = CByte(If(frequey, 1, 0))

            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_CAPABILITY, include_timestamp, data)
            Return mf.PACKET
        End Function
        ''' <summary>
        ''' Generate SetInventoryReport message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="reportStatus"></param>
        ''' <param name="param"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_SET_INVENTORY_REPORT(include_timestamp As Boolean, reportStatus As Boolean, param As OPTIONAL_INVENTORY_REPORT_PARAM) As Byte()
            Dim temp_data As Byte() = New Byte(10) {}
            temp_data(0) = CByte(If(reportStatus, 0, 1))
            temp_data(1) = &H1
            temp_data(2) = CByte(param.inventory_report_mode)

            '''////------------Bug fixed------------///////
            'Reported by:  levinro 
            'Fixed by:     Xiaoyong Su
            'Date:         1/4/2008
            If param.inventory_report_mode = OPTIONAL_INVENTORY_REPORT_PARAM.INVENTORY_REPORTING_MODE.ACCUMULATED_REPORT Then
                temp_data(3) = &H2
                temp_data(4) = CByte(param._add_behavior)
                temp_data(5) = &H3
                temp_data(6) = CByte(param.inventory_attemp_count_reporting)
                temp_data(7) = &H4
                temp_data(8) = CByte(param._drop_behavior)
                temp_data(9) = &H5
                temp_data(10) = CByte(param._buffer_full_behavior)

                Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_INVENTORY_REPORT, include_timestamp, temp_data)
                Return mf.PACKET
            Else
                Dim data As Byte() = New Byte(2) {}
                Array.Copy(temp_data, data, 3)

                Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_INVENTORY_REPORT, include_timestamp, data)
                Return mf.PACKET
            End If
        End Function
        ''' <summary>
        ''' Generate SetLBTParamsCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="autoSet"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_SET_LBT_PARAMS_CMD(include_timestamp As Boolean, autoSet As Boolean) As Byte()
            Dim data As Byte() = New Byte(0) {}
            data(0) = CByte(If(autoSet, 0, 1))

            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_LBT_PARAMS, include_timestamp, data)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate GetLBTParamsCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_GET_LBT_PARAMS_CMD(include_timestamp As Boolean) As Byte()
            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_LBT_PARAMS, include_timestamp)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate InventoryContinueCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_INVENTORY_CONTINUE_CMD(include_timestamp As Boolean) As Byte()
            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.INVENTORY_CONTINUE, include_timestamp)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate SetRxConfigCmd message
        ''' </summary>
        ''' <param name="mode"></param>
        ''' <param name="sensitivities"></param>
        ''' <param name="include_timestamp"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_SET_RX_CONFIG_CMD(mode As SET_RX_SENSITIVITY_MODE, sensitivities As Short(), include_timestamp As Boolean) As Byte()
            Dim data As Byte()
            If mode = SET_RX_SENSITIVITY_MODE.MAXIMUM_SENSITIVITY Then
                data = New Byte(0) {}
                data(0) = CByte(SET_RX_SENSITIVITY_MODE.MAXIMUM_SENSITIVITY)
            Else
                data = New Byte(4 + (sensitivities.Length - 1)) {}
                data(0) = CByte(SET_RX_SENSITIVITY_MODE.FIXED_PER_ANTENNA)
                data(1) = &H1
                data(2) = CByte((sensitivities.Length And &HFF00) >> 8)
                data(3) = CByte(sensitivities.Length And &HFF)
                For i As Integer = 0 To sensitivities.Length - 1
                    data(4 + i) = CByte(((0 - sensitivities(i)) - 1) Xor &HFF)
                Next
            End If

            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.SET_RX_CONFIG, include_timestamp, data)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate GetRxConfigCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_GET_RX_CONFIG_CMD(include_timestamp As Boolean) As Byte()
            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.GET_RX_CONFIG, include_timestamp)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate ModemStopCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_MODEM_STOP_CMD(include_timestamp As Boolean) As Byte()
            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.MODEM_STOP, include_timestamp)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate RFSurveyCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="low_frequency_index"></param>
        ''' <param name="high_frequency_index"></param>
        ''' <param name="bw"></param>
        ''' <param name="antennas"></param>
        ''' <param name="sampleCount"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_RF_SURVEY_CMD(include_timestamp As Boolean, low_frequency_index As UInt16, high_frequency_index As UInt16, bw As MEASUREMENT_BANDWIDTH, antennas As Byte, sampleCount As UInt16) As Byte()
            Dim data As Byte() = New Byte(7) {}

            data(0) = CByte(low_frequency_index >> 8)
            data(1) = CByte(low_frequency_index And &HFF)

            data(2) = CByte(high_frequency_index >> 8)
            data(3) = CByte(high_frequency_index And &HFF)

            data(4) = CByte(bw)

            data(5) = antennas

            data(6) = CByte(sampleCount >> 8)
            data(7) = CByte(sampleCount And &HFF)

            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.RF_SURVEY, include_timestamp, data)

            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate TagReadCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="mb"></param>
        ''' <param name="addr"></param>
        ''' <param name="len"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_TAG_READ_CMD(include_timestamp As Boolean, mb As MEMORY_BANK, addr As UInt16, len As Byte) As Byte()
            Dim data As Byte() = New Byte(3) {}

            data(0) = CByte(mb)
            data(1) = CByte(addr >> 8)
            data(2) = CByte(addr And &HFF)
            data(3) = len

            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.TAG_READ, include_timestamp, data)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate TagReadCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="mb"></param>
        ''' <param name="addr"></param>
        ''' <param name="len"></param>
        ''' <param name="pass"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_TAG_READ_CMD(include_timestamp As Boolean, mb As MEMORY_BANK, addr As UInt16, len As Byte, pass As UInt32) As Byte()
            Dim data As Byte() = New Byte(8) {}

            data(0) = CByte(mb)
            data(1) = CByte(addr >> 8)
            data(2) = CByte(addr And &HFF)
            data(3) = len
            data(4) = &H1
            data(5) = CByte(pass >> 24)
            data(6) = CByte((pass And &HFF0000) >> 16)
            data(7) = CByte((pass And &HFF00) >> 8)
            data(8) = CByte(pass And &HFF)

            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.TAG_READ, include_timestamp, data)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate TagWriteCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="mb"></param>
        ''' <param name="addr"></param>
        ''' <param name="data"></param>
        ''' <param name="block_write"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_TAG_WRITE_CMD(include_timestamp As Boolean, mb As MEMORY_BANK, addr As UInt16, data As UInt16(), block_write As Boolean) As Byte()
            Dim dataLen As Integer = 2 * data.Length
            Dim temp As Byte() = New Byte(6 + (dataLen - 1)) {}

            temp(0) = CByte(mb)
            temp(1) = CByte(addr >> 8)
            temp(2) = CByte(addr And &HFF)
            temp(3) = CByte((dataLen And &H300) >> 8)
            temp(4) = CByte((dataLen) And &HFF)

            For i As Integer = 0 To data.Length - 1
                temp(5 + 2 * i) = CByte(data(i) >> 8)
                temp(6 + 2 * i) = CByte(data(i) And &HFF)
            Next

            temp(5 + dataLen) = CByte(If(block_write, 0, 1))

            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.TAG_WRITE, include_timestamp, temp)
            Return mf.PACKET

        End Function

        ''' <summary>
        ''' Generate TagWriteCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="mb"></param>
        ''' <param name="addr"></param>
        ''' <param name="data"></param>
        ''' <param name="block_write"></param>
        ''' <param name="pass"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_TAG_WRITE_CMD(include_timestamp As Boolean, mb As MEMORY_BANK, addr As UInt16, data As UInt16(), block_write As Boolean, pass As UInt32) As Byte()
            Dim dataLen As Integer = 2 * data.Length
            Dim temp As Byte() = New Byte(11 + (dataLen - 1)) {}

            temp(0) = CByte(mb)
            temp(1) = CByte(addr >> 8)
            temp(2) = CByte(addr And &HFF)
            temp(3) = CByte((dataLen And &H300) >> 8)
            temp(4) = CByte(dataLen And &HFF)
            For i As Integer = 0 To data.Length - 1
                temp(5 + 2 * i) = CByte(data(i) >> 8)
                temp(6 + 2 * i) = CByte(data(i) And &HFF)
            Next

            temp(5 + dataLen) = CByte(If(block_write, 0, 1))

            temp(6 + dataLen) = &H1
            temp(7 + dataLen) = CByte(pass >> 24)
            temp(8 + dataLen) = CByte((pass And &HFF0000) >> 16)
            temp(9 + dataLen) = CByte((pass And &HFF00) >> 8)
            temp(10 + dataLen) = CByte(pass And &HFF)

            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.TAG_WRITE, include_timestamp, temp)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate TagLockCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="tlo"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_TAG_LOCK_CMD(include_timestamp As Boolean, tlo As OPERATION_CMD.TAG_LOCK_OPERATION) As Byte()
            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.TAG_LOCK, include_timestamp, tlo.ToByteArray())
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate TagLockCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="tlo"></param>
        ''' <param name="pass"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_TAG_LOCK_CMD(include_timestamp As Boolean, tlo As OPERATION_CMD.TAG_LOCK_OPERATION, pass As UInt32) As Byte()
            Dim data As Byte() = New Byte(8) {}

            Array.Copy(tlo.ToByteArray(), data, 4)

            data(4) = &H1

            data(5) = CByte(pass >> 24)
            data(6) = CByte((pass And &HFF0000) >> 16)
            data(7) = CByte((pass And &HFF00) >> 8)
            data(8) = CByte(pass And &HFF)

            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.TAG_LOCK, include_timestamp, data)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate TagKillCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="pass"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_TAG_KILL_CMD(include_timestamp As Boolean, pass As UInt32) As Byte()
            Dim data As Byte() = New Byte(3) {}
            data(0) = CByte(pass >> 24)
            data(1) = CByte((pass And &HFF0000) >> 16)
            data(2) = CByte((pass And &HFF00) >> 8)
            data(3) = CByte(pass And &HFF)

            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.TAG_KILL, include_timestamp, data)
            Return mf.PACKET
        End Function

        ''' <summary>
        ''' Generate ReportInventoryCmd message
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <returns></returns>
        Public Shared Function GENERATE_REPORT_INVENTORY_CMD(include_timestamp As Boolean) As Byte()
            Dim mf As New MACH1_FRAME(CATEGORY.OPERATION_MODEL, OPERATION_CMD.REPORT_INVENTORY, include_timestamp)
            Return mf.PACKET
        End Function


        ''' <summary>
        ''' Generate SetProfileSequence command
        ''' </summary>
        ''' <param name="include_timestamp"></param>
        ''' <param name="enable">Enable sequence, default should be false</param>
        ''' <param name="sequence">sequence list, the maximum sequence number is 16. the length of the sequence should be equal to the length of the duration list</param>
        ''' <param name="durations">duration list</param>
        ''' <returns></returns>
        Public Shared Function GENERATE_SET_PROFILE_SEQUENCE_CMD(include_timestamp As Boolean, enabled As Boolean, sequence As ArrayList, durations As ArrayList) As Byte()
            If sequence.Count <> durations.Count Then
                Return Nothing
            End If

            Dim data As Byte()

            If enabled Then
                data = New Byte(7 + sequence.Count + (durations.Count * 2 - 1)) {}
            Else
                data = New Byte(0) {}
            End If

            Dim idx As Integer = 0
            data(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(If(enabled, 1, 0))

            If enabled Then
                data(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H1
                data(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(sequence.Count >> 8)
                data(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(sequence.Count And &HFF)

                For i As Integer = 0 To sequence.Count - 1
                    data(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(sequence(i))
                Next

                data(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = &H2
                data(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(durations.Count >> 8)
                data(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(durations.Count And &HFF)

                For i As Integer = 0 To durations.Count - 1
                    data(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(CInt(durations(i)) >> 8)
                    data(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = CByte(CInt(durations(i)) And &HFF)
                Next
            End If
            Return data
        End Function

#End Region
    End Class

    ''' <summary>
    ''' Mach1 operation notification set
    ''' </summary>
    ''' 
    <Serializable>
    Public Class OPERATION_NTF
        Public Const ANTENNA_ALERT As Byte = &H0
        Public Const SET_REGULATORY_REGION As Byte = &HA
        Public Const INVENTORY As Byte = &H1
        Public Const TAG_CUSTOM As Byte = &H9
        Public Const TAG_KILL As Byte = &H8
        Public Const TAG_LOCK As Byte = &H7
        Public Const TAG_WRITE As Byte = &H6
        Public Const TAG_READ As Byte = &H5
        Public Const RF_SURVEY As Byte = &H3
        Public Const MODEM_STOP As Byte = &H4
        Public Const INVENTORY_STATUS As Byte = &H2
        Public Const CHECK_ANTENNA As Byte = &HB
        Public Const ACCUMULATION_STATUS As Byte = &HC

        <Serializable>
        Public Class SET_REGULATORY_REGION_NTF
            Inherits MACH1_NTF
            Public Enum NTF_CODE
                SUCCESS
                NOT_VALID_REGULATORY_CALIBRATION
                ERROR_SETTING
            End Enum

            Public code As NTF_CODE

        End Class

        <Serializable>
        Public Class MODEM_STOPPED_NTF
            Inherits MACH1_NTF
            Public Enum NTF_CODE
                RESULT_OF_COMMAND
                TO_MEET_REGULATORY
                HARDWARE_ERROR
            End Enum

            Public code As NTF_CODE
        End Class

        Public Enum TAG_ACCESS_RESULT_CODE
            SUCCEEDED
            FAIL_NO_RESPONSE
            FAIL_MEMORY_LOCKED
            FAIL_MEMORY_OVERRUN
            FAIL_INSUFFICIENT_POWER
            FAIL_INVALID_PASSWORD
            FAIL_OTHER_TAG_ERROR
            FAIL_TAG_LOST
            FAIL_READER_ERROR
        End Enum

        Public Enum ANTENNA_STATUS
            READY = 1
            DISCONNECTED = 3
        End Enum

        <Serializable>
        Public Class ACCUMULATION_STATUS_NTF
            Inherits MACH1_NTF
            Public Enum NTF_CODE
                BUFFER_EMPTIED = 0
                BUFFER_FILLED
            End Enum

            Public code As NTF_CODE
        End Class


        <Serializable>
        Public Class ANTENNA_ALERT_NTF
            Inherits MACH1_NTF
            Public _antenna As OPERATION_CMD.ANTENNA
            Public antenna_status As ANTENNA_STATUS

            Public Sub New(data As Byte())
                Try
                    _antenna = CType(data(0), OPERATION_CMD.ANTENNA)
                    antenna_status = CType(data(1), ANTENNA_STATUS)
                Catch
                End Try
            End Sub
        End Class

        <Serializable>
        Public Class CHECK_ANTENNA_NTF
            Inherits MACH1_NTF
            Public antenna_status As ANTENNA_STATUS()
            Public Sub New(data As Byte())
                Try
                    Dim len As Integer = data.Length
                    antenna_status = New ANTENNA_STATUS(len - 1) {}
                    For i As Integer = 0 To len - 1
                        antenna_status(i) = CType(data(i), ANTENNA_STATUS)
                    Next
                Catch
                End Try
            End Sub
        End Class

        <Serializable>
        Public Class INVENTORY_NTF
            Inherits MACH1_NTF
            Public Enum GEN2_READ_RESULT_CODE
                READ_SUCESS = 0
                NO_RESPONSE
                CRC_ERROR
                MEMORY_LOCKED
                MEMORY_OVERRUN
                OTHER_TAG_ERROR
                OTHER_READER_ERROR
            End Enum
            Public Enum ANTENNA
                ANTENNA1 = 1
                ANTENNA2 = 2
                ANTENNA3 = 4
                ANTENNA4 = 8
            End Enum
            Public Enum ACCUMULATION_STATUS
                INVENOTRY_NTF_BYPASSED = 0
                INVENTORY_NTF_ADDED
                INVENTORY_NTF_REMOVED
                INVENTORY_NTF_OVERFLOWED
            End Enum

            Public m_epc As Byte()
            Public halted As Boolean
            Public rssi As Short
            Public gen2PC As Byte() = New Byte(1) {}
            Public epcCrc As Byte() = New Byte(1) {}
            Public _gen2_read_result_code As GEN2_READ_RESULT_CODE
            Public gen2ReadData As Byte()
            Public gen2Handle As UInt16
            Public _antenna As ANTENNA
            Public read_count As UInt16
            Public first_seen_ago As UInt32
            Public last_seen_ago As UInt32
            Public _accumulation_status As ACCUMULATION_STATUS
            Public phaseI As Short
            Public phaseQ As Short

            Public ReadOnly Property EPC() As String
                Get
                    Dim epc_code As String = String.Empty
                    For i As Integer = 0 To m_epc.Length - 1
                        epc_code += m_epc(i).ToString("X2")
                    Next

                    Return epc_code
                End Get
            End Property


            Public Sub New()
            End Sub

            Public Sub New(data As Byte())
                Try
                    Dim offset As Integer = 1
                    m_epc = New Byte(data(offset) - 1) {}
                    offset += 1
                    Array.Copy(data, offset, m_epc, 0, data(1))

                    offset += data(1)
                    halted = If((data(offset) = 1), True, False)

                    offset += 1

                    rssi = CShort(0 - ((data(offset) Xor &HFF) + 1))

                    offset += 1
                    gen2PC(0) = data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1))
                    gen2PC(1) = data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1))
                    epcCrc(0) = data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1))
                    epcCrc(1) = data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1))

                    If (epcCrc(0) And &H80) <> 0 Then
                        phaseI = CShort(0 - ((epcCrc(0) Xor &HFF) + 1))
                    Else
                        phaseI = epcCrc(0)
                    End If

                    If (epcCrc(1) And &H80) <> 0 Then
                        phaseQ = CShort(0 - ((epcCrc(1) Xor &HFF) + 1))
                    Else
                        phaseQ = epcCrc(1)
                    End If

                    'phaseI = epcCrc[0];
                    'phaseQ = epcCrc[1];

                    While offset < data.Length
                        Select Case data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1))
                            Case &H1
                                _gen2_read_result_code = CType(data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)), GEN2_READ_RESULT_CODE)
                                Exit Select
                            Case &H2
                                Dim gen2dataLen As Integer = data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) * &H100 + data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1))
                                '@derek
                                gen2ReadData = New Byte(gen2dataLen - 1) {}
                                Array.Copy(data, offset, gen2ReadData, 0, gen2dataLen)
                                offset += gen2dataLen
                                Exit Select
                            Case &H3
                                gen2Handle = CUShort(data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) * 256 + data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)))
                                Exit Select
                            Case &H4
                                _antenna = CType(data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)), ANTENNA)
                                Exit Select
                            Case &H5
                                read_count = CType(data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) * 256 + data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)), UInt16)
                                Exit Select
                            Case &H6
                                first_seen_ago = CType((data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) << 24) + (data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) << 16) + (data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) << 8) + data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)), UInt32)
                                Exit Select
                            Case &H7
                                last_seen_ago = CType((data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) << 24) + (data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) << 16) + (data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)) << 8) + data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)), UInt32)
                                Exit Select
                            Case &H8
                                _accumulation_status = CType(data(System.Math.Min(System.Threading.Interlocked.Increment(offset), offset - 1)), ACCUMULATION_STATUS)
                                Exit Select
							Case Else
								offset += 1
								Exit Select
						End Select
					End While
				Catch
				End Try
			End Sub
		End Class

		<Serializable> _
		Public Class INVENTORY_STATUS_NTF
			Inherits MACH1_NTF
			Public transmitter_status_enabled As Boolean = False
			Public enabled_antenna_byte As Byte = &H0
			Public center_frequncy_index As UInt16 = 0
			Public inventory_attempt_count As UInt16 = 0

			Public Sub New(data As Byte())
				transmitter_status_enabled = If((data(0) = 1), True, False)

				Try
					Dim offset As Integer = 1
					While offset < data.Length - 1
						Select Case data(offset)
							Case &H1
								offset += 1
								enabled_antenna_byte = data(offset)
								offset += 1
								Exit Select
							Case &H2
								offset += 1
								center_frequncy_index = CType(data(offset) * 256 + data(offset + 1), UInt16)
								offset += 2
								Exit Select
							Case &H3
								offset += 1
								inventory_attempt_count = CType(data(offset) * 256 + data(offset + 1), UInt16)
								offset += 2
								Exit Select
							Case Else
								offset += 1
								Exit Select
						End Select
					End While
				Catch

				End Try
			End Sub
		End Class

		<Serializable> _
		Public Class RF_SURVEY_NTF
			Inherits MACH1_NTF
			Public Enum RESULT_CODE
				SURVEY_IN_PROGRESS
				SUCCESS
				ABORT
			End Enum

            Public _result_code As RESULT_CODE
            Public low_frequency_index As UInt16
            Public high_frequency_index As UInt16
            Public bandwidth_index As Byte
            Public antenna_byte As Byte
            Public low_time As UInt32
            Public high_time As UInt32
            Public rssi_data As Int16()

            Public Sub New(data As Byte())
                Try
                    _result_code = CType(data(0), RESULT_CODE)
                    low_frequency_index = CType(data(1) * 256 + data(2), UInt16)
                    high_frequency_index = CType(data(3) * 256 + data(4), UInt16)

                    bandwidth_index = data(5)
                    antenna_byte = data(6)

                    low_time = (CType(data(7), UInt32) << 24) And (CType(data(8), UInt32) << 16) And (CType(data(9), UInt32) << 8) And CType(data(10), UInt32)
                    high_time = (CType(data(11), UInt32) << 24) And (CType(data(12), UInt32) << 16) And (CType(data(13), UInt32) << 8) And CType(data(14), UInt32)

                    If data.Length > 17 Then
                        If data(15) = &H1 Then
                            Dim rssi_data_len As Integer = (data(16) << 8) + data(17)

                            rssi_data = New Short(rssi_data_len - 1) {}

                            For i As Integer = 0 To rssi_data_len - 1
                                rssi_data(i) = CShort(0 - ((data(18 + i) Xor &HFF) + 1))

                            Next
                        End If
                    End If
                Catch

                End Try
            End Sub

        End Class

        <Serializable>
        Public Class TAG_READ_NTF
            Inherits MACH1_NTF
            Public Enum RESULT_CODE
                READ_SUCCEEDED
                NO_RESPONSE
                CRC_ERROR
                MEMORY_LOCKED
                MEMORY_OVERRUN
                INVALID_PASSWORD
                OTHER_TAG_ERROR
                TAG_LOST
                READER_ERROR
            End Enum

            Public _result_code As RESULT_CODE
            Public data As Byte()

            Public Sub New(data As Byte())
                Try
                    _result_code = CType(data(0), RESULT_CODE)
                    If data.Length > 4 Then
                        Dim len As Integer = data(2) * 256 + data(3)
                        Me.data = New Byte(len - 1) {}

                        Array.Copy(data, 4, Me.data, 0, len)
                    End If
                Catch
                End Try
            End Sub
        End Class

        <Serializable>
        Public Class TAG_WRITE_NTF
            Inherits MACH1_NTF
            Public _result_code As TAG_ACCESS_RESULT_CODE
            Public err_addr As UInt16

            Public Sub New(data As Byte())
                Try
                    _result_code = CType(data(0), TAG_ACCESS_RESULT_CODE)
                    If data.Length > 4 Then
						err_addr = CType(data(2) * 256 + data(3), UInt16)
					End If
				Catch
				End Try
			End Sub
		End Class

	End Class
End Namespace
