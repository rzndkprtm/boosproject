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
                        Dim dt As New DataTable()
                        thisAdapter.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            Return dt.Rows(0)
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
                    Using da As New SqlDataAdapter(thisCmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return dt
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
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
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
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
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
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader()
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
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
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
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader()
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

    Public Function GetProductSales(myCmd As SqlCommand) As DataSet
        Using thisConn As New SqlConnection(myConn)
            Using thisAdapter As New SqlDataAdapter()
                myCmd.Connection = thisConn
                thisAdapter.SelectCommand = myCmd

                Dim thisDataSet As New DataSet()
                thisAdapter.Fill(thisDataSet)
                Return thisDataSet
            End Using
        End Using
    End Function

    Public Function CreateId(thisString As String) As String
        Dim result As String = String.Empty
        Try
            Dim id As Integer = 0
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult As SqlDataReader = myCmd.ExecuteReader()
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

    Public Sub RefreshData(Optional salesId As String = "")
        'Try
        Dim thisQuery As String = "SELECT * FROM SALES ORDER BY SummaryDate DESC"
        If Not String.IsNullOrEmpty(salesId) Then
            thisQuery = "SELECT * FROM SALES WHERE Id='" & salesId & "' ORDER BY SummaryDate DESC"
        End If
        Dim salesData As DataTable = GetDataTable("SELECT * FROM SALES ORDER BY SummaryDate DESC")
        If salesData.Rows.Count > 0 Then
            For i As Integer = 0 To salesData.Rows.Count - 1
                Dim dataId As String = salesData.Rows(i)("Id").ToString()
                Dim summaryDate As Date = salesData.Rows(i)("SummaryDate")

                Dim orderData As DataTable = GetDataTable("SELECT OrderHeaders.* FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId=Customers.Id WHERE OrderHeaders.Active=1 AND Customers.CompanyId='2' AND ProductionDate=CONVERT(DATE, '" & summaryDate.ToString("yyyy-MM-dd") & "')")
                If orderData.Rows.Count > 0 Then
                    For iOrder As Integer = 0 To orderData.Rows.Count - 1
                        Dim headerId As String = orderData.Rows(iOrder)("Id").ToString()

                        Dim getPricing As DataRow = GetDataRow("SELECT (SUM(BuyPrice) * 1.10) AS BuyPrice, (SUM(SellPrice) * 1.10) AS SellPrice FROM OrderCostings WHERE Type='Final' AND HeaderId='" & headerId & "'")

                        Dim totalCostPrice As Decimal = If(IsDBNull(getPricing("BuyPrice")), 0D, Convert.ToDecimal(getPricing("BuyPrice")))
                        Dim totalSellingPrice As Decimal = If(IsDBNull(getPricing("SellPrice")), 0D, Convert.ToDecimal(getPricing("SellPrice")))

                        Dim totalPaidAmount As Decimal = GetItemData_Decimal("SELECT Amount FROM OrderInvoices WHERE Id='" & headerId & "' AND Payment=1")

                        Using thisConn As New SqlConnection(myConn)
                            Dim query As String = ""

                            If iOrder = 0 Then
                                query = "UPDATE Sales SET TotalCostPrice=@TotalCostPrice, TotalSellingPrice=@TotalSellingPrice, TotalPaidAmount=@TotalPaidAmount WHERE Id=@Id"
                            Else
                                query = "UPDATE Sales SET TotalCostPrice = TotalCostPrice + @TotalCostPrice, TotalSellingPrice = TotalSellingPrice + @TotalSellingPrice, TotalPaidAmount = TotalPaidAmount + @TotalPaidAmount WHERE Id=@Id"
                            End If

                            Using myCmd As SqlCommand = New SqlCommand(query, thisConn)
                                myCmd.Parameters.Add("@Id", SqlDbType.Int).Value = dataId
                                myCmd.Parameters.Add("@TotalCostPrice", SqlDbType.Decimal).Value = totalCostPrice
                                myCmd.Parameters.Add("@TotalSellingPrice", SqlDbType.Decimal).Value = totalSellingPrice
                                myCmd.Parameters.Add("@TotalPaidAmount", SqlDbType.Decimal).Value = totalPaidAmount

                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                            End Using
                        End Using
                    Next
                End If
            Next
        End If
        'Catch ex As Exception
        'End Try
    End Sub
End Class
