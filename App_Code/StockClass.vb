Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports OfficeOpenXml
Imports OfficeOpenXml.Style

Public Class StockClass
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

    Public Function GetDataTableSP(spName As String, params As List(Of SqlParameter)) As DataTable
        Dim thisTable As New DataTable()
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(spName, thisConn)
                    thisCmd.CommandType = CommandType.StoredProcedure
                    If params IsNot Nothing AndAlso params.Count > 0 Then
                        thisCmd.Parameters.AddRange(params.ToArray())
                    End If
                    Using thisAdapter As New SqlDataAdapter(thisCmd)
                        thisAdapter.Fill(thisTable)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            thisTable = New DataTable()
        End Try
        Return thisTable
    End Function

    Public Function GetItemData(thisString As String) As String
        Dim result As String = String.Empty
        Try
            If String.IsNullOrWhiteSpace(thisString) Then
                Return String.Empty
            End If
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = thisCmd.ExecuteReader()
                        If rdResult.Read() Then
                            If Not IsDBNull(rdResult(0)) Then
                                result = rdResult(0).ToString()
                            End If
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function GenerateStockExcel(companyDetailId As String) As Byte()
        ExcelPackage.LicenseContext = LicenseContext.Commercial

        Using package As New ExcelPackage()
            Dim dtRoller As DataTable = GetStockFabric(12, "ROLLER", companyDetailId)
            Dim dtVertical As DataTable = GetStockFabric(11, "VERTICAL", companyDetailId)
            Dim dtCellular As DataTable = GetStockFabric(2, "CELLULAR SHADES", companyDetailId)
            Dim dtDesignShades As DataTable = GetStockFabric(4, "DESIGN SHADES", companyDetailId)
            Dim dtCurtain As DataTable = GetStockFabric(3, "CURTAIN", companyDetailId)
            Dim dtVenetian As DataTable = GetStockVenetian(10, companyDetailId)
            Dim dtAluminium As DataTable = GetStockVenetian(1, companyDetailId)
            Dim dtFabricChart As DataTable = GetFabricChart(companyDetailId)

            AddFabricWorksheet(package, "Roller", dtRoller, True)
            AddFabricWorksheet(package, "Vertical", dtVertical, True)
            AddFabricWorksheet(package, "Cellular Shades", dtCellular, True)
            AddFabricWorksheet(package, "Design Shades", dtDesignShades, True)
            AddFabricWorksheet(package, "Curtain", dtCurtain, False)
            AddMatrixWorksheet(package, "Venetian Blind", dtVenetian)
            AddMatrixWorksheet(package, "Aluminium Blind", dtAluminium)
            AddFabricChartWorksheet(package, dtFabricChart)

            Return package.GetAsByteArray()
        End Using
    End Function

    Private Function GetStockFabric(designId As Integer, stockType As String, companyDetailId As String) As DataTable
        Dim thisTable As New DataTable()
        Using thisConn As New SqlConnection(ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
            Using thisCmd As New SqlCommand("sp_Stock_Fabric", thisConn)
                thisCmd.CommandType = CommandType.StoredProcedure
                thisCmd.Parameters.Add("@DesignId", SqlDbType.Int).Value = designId
                thisCmd.Parameters.Add("@CompanyDetailId", SqlDbType.NVarChar, 255).Value = companyDetailId
                thisCmd.Parameters.Add("@Type", SqlDbType.NVarChar, 100).Value = stockType
                thisCmd.Parameters.Add("@SearchText", SqlDbType.NVarChar, 255).Value = CType(DBNull.Value, Object)

                Using da As New SqlDataAdapter(thisCmd)
                    da.Fill(thisTable)
                End Using
            End Using
        End Using
        Return thisTable
    End Function

    Private Function GetStockVenetian(designId As Integer, companyDetailId As String) As DataTable
        Dim thisTable As New DataTable()
        Using thisConn As New SqlConnection(ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
            Using thisCmd As New SqlCommand("sp_Stock_Venetian", thisConn)
                thisCmd.CommandType = CommandType.StoredProcedure
                thisCmd.Parameters.Add("@DesignId", SqlDbType.Int).Value = designId
                thisCmd.Parameters.Add("@CompanyDetailId", SqlDbType.NVarChar, 255).Value = companyDetailId
                Using da As New SqlDataAdapter(thisCmd)
                    da.Fill(thisTable)
                End Using
            End Using
        End Using
        Return thisTable
    End Function

    Private Function GetFabricChart(companyDetailId As String) As DataTable
        Dim thisTable As New DataTable()
        Using thisConn As New SqlConnection(ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
            Using thisCmd As New SqlCommand("sp_Stock_Fabric_Chart", thisConn)
                thisCmd.CommandType = CommandType.StoredProcedure
                thisCmd.Parameters.Add("@SearchText", SqlDbType.NVarChar, 255).Value = CType(DBNull.Value, Object)
                thisCmd.Parameters.Add("@CompanyDetailId", SqlDbType.NVarChar, 255).Value = companyDetailId
                Using da As New SqlDataAdapter(thisCmd)
                    da.Fill(thisTable)
                End Using
            End Using
        End Using
        Return thisTable
    End Function

    Private Sub AddFabricWorksheet(package As ExcelPackage, sheetName As String, dt As DataTable, expressOnly As Boolean)
        Dim ws As ExcelWorksheet = package.Workbook.Worksheets.Add(sheetName)

        Dim row As Integer = 1

        ' TITLE
        ws.Cells(row, 1, row, 6).Merge = True
        ws.Cells(row, 1).Value = sheetName.ToUpper()
        StyleTitle(ws.Cells(row, 1, row, 6))

        row += 2

        ' HEADER
        Dim headers As String() = {"FABRIC TYPE", "COLOUR", "STATUS", "MAXIMUM WIDTH", "ROLL QTY", "ETA FACTORY"}
        For i As Integer = 0 To headers.Length - 1
            ws.Cells(row, i + 1).Value = headers(i)
        Next
        StyleHeader(ws.Cells(row, 1, row, 6))

        row += 1

        Using thisConn As New SqlConnection(ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
            thisConn.Open()

            For Each fabric As DataRow In dt.Rows
                Dim fabricId As Integer = Convert.ToInt32(fabric("Id"))
                Dim firstRow As Integer = row

                Dim sql As String = "SELECT Id, Colour, Status, Width, RollQty, ETAFactory FROM FabricColours WHERE FabricId = @FabricId"

                If expressOnly Then
                    sql &= " AND Factory = 'Express'"
                End If

                sql &= " ORDER BY Colour"

                Using thisCmd As New SqlCommand(sql, thisConn)
                    thisCmd.Parameters.Add("@FabricId", SqlDbType.Int).Value = fabricId
                    Using dr As SqlDataReader = thisCmd.ExecuteReader()
                        While dr.Read()
                            ws.Cells(row, 1).Value = If(row = firstRow, fabric("Name").ToString(), "")
                            ws.Cells(row, 2).Value = If(IsDBNull(dr("Colour")), "", dr("Colour").ToString().ToUpper())
                            ws.Cells(row, 3).Value = If(IsDBNull(dr("Status")), "", dr("Status").ToString())
                            ws.Cells(row, 4).Value = If(IsDBNull(dr("Width")), "", dr("Width").ToString())
                            ws.Cells(row, 5).Value = If(IsDBNull(dr("RollQty")), "", dr("RollQty").ToString() & " Roll")

                            If Not IsDBNull(dr("ETAFactory")) Then
                                ws.Cells(row, 6).Value = Convert.ToDateTime(dr("ETAFactory")).ToString("dd/MM/yyyy")
                            Else
                                ws.Cells(row, 6).Value = ""
                            End If

                            StyleStatus(ws.Cells(row, 3))

                            row += 1
                        End While
                    End Using
                End Using

                ' FABRIC GROUP BORDER
                If row > firstRow Then
                    With ws.Cells(firstRow, 1, row - 1, 6).Style.Border
                        .Bottom.Style = ExcelBorderStyle.Thin
                        .Bottom.Color.SetColor(Color.LightGray)
                    End With

                    ws.Cells(firstRow, 1, row - 1, 1).Style.Font.Bold = True
                    ws.Cells(firstRow, 1, row - 1, 1).Style.Fill.PatternType = ExcelFillStyle.Solid
                    ws.Cells(firstRow, 1, row - 1, 1).Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 247, 250))
                End If
            Next
        End Using

        If row = 4 Then
            ws.Cells(row, 1).Value = "No stock data found."
        End If

        StyleData(ws, row - 1, 6)

        ws.Column(1).Width = 30
        ws.Column(2).Width = 25
        ws.Column(3).Width = 22
        ws.Column(4).Width = 20
        ws.Column(5).Width = 18
        ws.Column(6).Width = 20

        ws.View.FreezePanes(4, 2)
    End Sub

    Private Sub AddMatrixWorksheet(package As ExcelPackage, sheetName As String, dt As DataTable)
        Dim ws As ExcelWorksheet = package.Workbook.Worksheets.Add(sheetName)

        ' TITLE
        ws.Cells(1, 1, 1, 7).Merge = True
        ws.Cells(1, 1).Value = sheetName.ToUpper()
        StyleTitle(ws.Cells(1, 1, 1, 7))

        ' STATUS LEGEND
        ws.Cells(3, 1).Value = "IN STOCK"
        ws.Cells(3, 2).Value = "LIMITED STOCK"
        ws.Cells(3, 3).Value = "OUT OF STOCK"
        ws.Cells(3, 4).Value = "DISCONTINUED"

        StyleStatus(ws.Cells(3, 1))
        StyleStatus(ws.Cells(3, 2))
        StyleStatus(ws.Cells(3, 3))
        StyleStatus(ws.Cells(3, 4))

        ' HEADER
        Dim headers As String() = {"BLIND NAME", "COLOUR", "COLOUR", "COLOUR", "COLOUR", "COLOUR", "COLOUR"}

        For i As Integer = 0 To headers.Length - 1
            ws.Cells(5, i + 1).Value = headers(i)
        Next

        StyleHeader(ws.Cells(5, 1, 5, 7))

        ' DATA
        For r As Integer = 0 To dt.Rows.Count - 1
            For c As Integer = 0 To 6
                Dim cell As ExcelRange = ws.Cells(r + 6, c + 1)

                cell.Value = dt.Rows(r)(c).ToString()

                If c > 0 Then
                    StyleMatrixStatus(cell)
                End If
            Next
        Next

        StyleData(ws, dt.Rows.Count + 5, 7)

        ws.Column(1).Width = 30

        For c As Integer = 2 To 7
            ws.Column(c).Width = 25
        Next

        ws.View.FreezePanes(6, 2)
    End Sub

    Private Sub AddFabricChartWorksheet(package As ExcelPackage, dt As DataTable)
        Dim ws As ExcelWorksheet = package.Workbook.Worksheets.Add("Fabric Chart")

        Dim columnCount As Integer = dt.Columns.Count

        ' TITLE
        ws.Cells(1, 1, 1, columnCount).Merge = True
        ws.Cells(1, 1).Value = "FABRIC CHART"
        StyleTitle(ws.Cells(1, 1, 1, columnCount))

        ' HEADER
        For c As Integer = 0 To columnCount - 1
            ws.Cells(3, c + 1).Value = dt.Columns(c).ColumnName
        Next

        StyleHeader(ws.Cells(3, 1, 3, columnCount))

        ' DATA
        For r As Integer = 0 To dt.Rows.Count - 1
            For c As Integer = 0 To columnCount - 1
                Dim cell As ExcelRange = ws.Cells(r + 4, c + 1)

                cell.Value = dt.Rows(r)(c).ToString()

                If c > 0 AndAlso cell.Value.ToString().Trim().ToUpper() = "NO" Then
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid
                    cell.Style.Fill.BackgroundColor.SetColor(Color.DarkRed)
                    cell.Style.Font.Color.SetColor(Color.White)
                    cell.Style.Font.Bold = True
                End If
            Next
        Next

        StyleData(ws, dt.Rows.Count + 3, columnCount)

        ws.Column(1).Width = 35

        For c As Integer = 2 To columnCount
            ws.Column(c).Width = 22
        Next

        ws.View.FreezePanes(4, 2)
    End Sub

    Private Sub StyleTitle(rng As ExcelRange)
        With rng.Style
            .Font.Bold = True
            .Font.Size = 16
            .Font.Color.SetColor(Color.White)

            .Fill.PatternType = ExcelFillStyle.Solid
            .Fill.BackgroundColor.SetColor(Color.FromArgb(45, 62, 80))

            .VerticalAlignment = ExcelVerticalAlignment.Center
            .HorizontalAlignment = ExcelHorizontalAlignment.Left
        End With
        rng.Worksheet.Row(rng.Start.Row).Height = 32
    End Sub

    Private Sub StyleHeader(rng As ExcelRange)
        With rng.Style
            .Font.Bold = True
            .Font.Color.SetColor(Color.White)

            .Fill.PatternType = ExcelFillStyle.Solid
            .Fill.BackgroundColor.SetColor(Color.FromArgb(52, 73, 94))

            .HorizontalAlignment = ExcelHorizontalAlignment.Center
            .VerticalAlignment = ExcelVerticalAlignment.Center

            .Border.Bottom.Style = ExcelBorderStyle.Thin
        End With

        rng.Worksheet.Row(rng.Start.Row).Height = 28
    End Sub

    Private Sub StyleData(ws As ExcelWorksheet, lastRow As Integer, lastCol As Integer)
        If lastRow < 3 Then Return

        Dim rng As ExcelRange = ws.Cells(3, 1, lastRow, lastCol)

        With rng.Style
            .VerticalAlignment = ExcelVerticalAlignment.Center

            .Border.Top.Style = ExcelBorderStyle.Thin
            .Border.Bottom.Style = ExcelBorderStyle.Thin
            .Border.Left.Style = ExcelBorderStyle.Thin
            .Border.Right.Style = ExcelBorderStyle.Thin

            .Border.Top.Color.SetColor(Color.LightGray)
            .Border.Bottom.Color.SetColor(Color.LightGray)
            .Border.Left.Color.SetColor(Color.LightGray)
            .Border.Right.Color.SetColor(Color.LightGray)
        End With
    End Sub

    Private Sub StyleStatus(cell As ExcelRange)
        If cell.Value Is Nothing Then Return

        Dim status As String = cell.Value.ToString().Trim().ToUpper()

        cell.Style.Font.Bold = True
        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center

        cell.Style.Fill.PatternType = ExcelFillStyle.Solid

        Select Case status
            Case "IN STOCK"
                cell.Style.Fill.BackgroundColor.SetColor(Color.DarkGreen)
                cell.Style.Font.Color.SetColor(Color.White)
            Case "OUT OF STOCK"
                cell.Style.Fill.BackgroundColor.SetColor(Color.DarkRed)
                cell.Style.Font.Color.SetColor(Color.White)
            Case "LIMITED STOCK"
                cell.Style.Fill.BackgroundColor.SetColor(Color.Yellow)
                cell.Style.Font.Color.SetColor(Color.Black)
            Case "DISCONTINUED"
                cell.Style.Fill.BackgroundColor.SetColor(Color.Gray)
                cell.Style.Font.Color.SetColor(Color.White)
        End Select
    End Sub

    Private Sub StyleMatrixStatus(cell As ExcelRange)
        If cell.Value Is Nothing Then Return

        Dim value As String = cell.Value.ToString().Trim().ToUpper()

        cell.Style.WrapText = True
        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center

        If value.Contains("IN STOCK") Then
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid
            cell.Style.Fill.BackgroundColor.SetColor(Color.DarkGreen)
            cell.Style.Font.Color.SetColor(Color.White)
        ElseIf value.Contains("LIMITED STOCK") Then
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid
            cell.Style.Fill.BackgroundColor.SetColor(Color.Yellow)
            cell.Style.Font.Color.SetColor(Color.Black)
        ElseIf value.Contains("OUT OF STOCK") Then
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid
            cell.Style.Fill.BackgroundColor.SetColor(Color.DarkRed)
            cell.Style.Font.Color.SetColor(Color.White)
        ElseIf value.Contains("DISCONTINUED") Then
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid
            cell.Style.Fill.BackgroundColor.SetColor(Color.Gray)
            cell.Style.Font.Color.SetColor(Color.White)
        End If
    End Sub
End Class
