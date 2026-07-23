Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Public Class SalesClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetDataRow(thisString As String) As DataRow
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using thisAdapter As New SqlDataAdapter(thisCmd)
                        Dim thisTable As New DataTable()
                        thisAdapter.Fill(thisTable)
                        If thisTable.Rows.Count > 0 Then
                            Return thisTable.Rows(0)
                        Else
                            Return Nothing
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetDataTable(thisString As String) As DataTable
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using thisAdapter As New SqlDataAdapter(thisCmd)
                        Dim thisTable As New DataTable()
                        thisAdapter.Fill(thisTable)
                        Return thisTable
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetItemData(thisString As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = thisCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0).ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GetItemData_Integer(thisString As String) As Integer
        Dim result As Integer = 0
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = thisCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0)
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = 0
        End Try
        Return result
    End Function

    Public Function GetItemData_Decimal(thisString As String) As Decimal
        Dim result As Decimal = 0D
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = thisCmd.ExecuteReader()
                        If rdResult.Read() Then
                            Dim value As Object = rdResult.Item(0)
                            If Not IsDBNull(value) Then
                                result = Convert.ToDecimal(value.ToString(), CultureInfo.InvariantCulture)
                            End If
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            result = 0D
        End Try
        Return result
    End Function

    Public Function GetItemData_Boolean(thisString As String) As Boolean
        Dim result As Boolean = False
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = thisCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0)
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    Public Function GetItemData_Date(thisString As String) As Date
        Dim result As Date = Date.MinValue
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = thisCmd.ExecuteReader()
                        While rdResult.Read()
                            If Not IsDBNull(rdResult.Item(0)) Then
                                result = Convert.ToDateTime(rdResult.Item(0))
                            End If
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = Date.MinValue
        End Try
        Return result
    End Function

    Public Function CreateId(thisString As String) As String
        Dim result As String = String.Empty
        Try
            Dim id As Integer = 0
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult As SqlDataReader = thisCmd.ExecuteReader()
                        If rdResult.Read() Then
                            Integer.TryParse(rdResult(0).ToString(), id)
                        End If
                    End Using
                End Using
            End Using
            result = (id + 1).ToString()
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Sub RefreshData(companyId As String)
        Try
            If Not String.IsNullOrEmpty(companyId) Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As New SqlCommand("sp_Sales_Refresh", thisConn)
                        thisCmd.CommandType = CommandType.StoredProcedure
                        thisCmd.Parameters.AddWithValue("@CompanyId", companyId)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
