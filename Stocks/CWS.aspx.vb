Imports System.Data
Imports System.Data.SqlClient

Partial Class Stocks_CWS
    Inherits Page

    Dim stockClass As New StockClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindRoller(txtSearchRoller.Text, "4")
            BindVertical(txtSearchVertical.Text, "4")
            BindSaphora("4")
            BindCellularShades("4")
            BindDesignShades("4")
            BindCurtain(txtSearchCurtain.Text, "4")
            BindVenetian("4")

            BindFabricChart(txtSearchFabricChart.Text, "4")
        End If
    End Sub

    Protected Sub btnSearchRoller_Click(sender As Object, e As EventArgs)
        BindRoller(txtSearchRoller.Text, "4")
    End Sub

    Protected Sub btnSearchVertical_Click(sender As Object, e As EventArgs)
        BindVertical(txtSearchVertical.Text, "4")
    End Sub

    Protected Sub btnSearchCurtain_Click(sender As Object, e As EventArgs)
        BindCurtain(txtSearchCurtain.Text, "4")
    End Sub

    Protected Sub btnFabricChart_Click(sender As Object, e As EventArgs)
        BindFabricChart(txtSearchFabricChart.Text, "4")
    End Sub

    Protected Sub gvRoller_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType <> DataControlRowType.DataRow Then Return

        For i As Integer = 1 To 6
            Dim colour As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i))
            Dim status As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i & "Status"))
            Dim description As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i & "Description"))

            Dim cell As TableCell = e.Row.Cells(i)

            If String.IsNullOrWhiteSpace(colour) Then
                cell.CssClass = "stock-empty"
                cell.Text = "-"
                Continue For
            End If

            cell.Text = "<div class='stock-colour'>" & Server.HtmlEncode(colour.ToUpper()) & "</div><div class='stock-info'>" & description & "</div>"

            Select Case status.Trim().ToUpper()
                Case "IN STOCK"
                    cell.BackColor = Drawing.Color.DarkGreen
                    cell.ForeColor = Drawing.Color.White
                Case "LIMITED STOCK"
                    cell.BackColor = Drawing.Color.Yellow
                    cell.ForeColor = Drawing.Color.Black
                Case "OUT OF STOCK"
                    cell.BackColor = Drawing.Color.DarkRed
                    cell.ForeColor = Drawing.Color.White
                Case "DISCONTINUED"
                    cell.BackColor = Drawing.Color.Gray
                    cell.ForeColor = Drawing.Color.White
            End Select
        Next
    End Sub

    Protected Sub gvVertical_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType <> DataControlRowType.DataRow Then Return

        For i As Integer = 1 To 6
            Dim colour As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i))
            Dim status As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i & "Status"))
            Dim description As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i & "Description"))

            Dim cell As TableCell = e.Row.Cells(i)

            If String.IsNullOrWhiteSpace(colour) Then
                cell.CssClass = "stock-empty"
                cell.Text = "-"
                Continue For
            End If

            cell.Text = "<div class='stock-colour'>" & Server.HtmlEncode(colour.ToUpper()) & "</div><div class='stock-info'>" & description & "</div>"

            Select Case status.Trim().ToUpper()
                Case "IN STOCK"
                    cell.BackColor = Drawing.Color.DarkGreen
                    cell.ForeColor = Drawing.Color.White
                Case "LIMITED STOCK"
                    cell.BackColor = Drawing.Color.Yellow
                    cell.ForeColor = Drawing.Color.Black
                Case "OUT OF STOCK"
                    cell.BackColor = Drawing.Color.DarkRed
                    cell.ForeColor = Drawing.Color.White
                Case "DISCONTINUED"
                    cell.BackColor = Drawing.Color.Gray
                    cell.ForeColor = Drawing.Color.White
            End Select
        Next
    End Sub

    Protected Sub gvSaphora_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType <> DataControlRowType.DataRow Then Return

        For i As Integer = 1 To 6
            Dim colour As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i))
            Dim status As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i & "Status"))
            Dim description As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i & "Description"))

            Dim cell As TableCell = e.Row.Cells(i)

            If String.IsNullOrWhiteSpace(colour) Then
                cell.CssClass = "stock-empty"
                cell.Text = "-"
                Continue For
            End If

            cell.Text = "<div class='stock-colour'>" & Server.HtmlEncode(colour.ToUpper()) & "</div><div class='stock-info'>" & description & "</div>"

            Select Case status.Trim().ToUpper()
                Case "IN STOCK"
                    cell.BackColor = Drawing.Color.DarkGreen
                    cell.ForeColor = Drawing.Color.White
                Case "LIMITED STOCK"
                    cell.BackColor = Drawing.Color.Yellow
                    cell.ForeColor = Drawing.Color.Black
                Case "OUT OF STOCK"
                    cell.BackColor = Drawing.Color.DarkRed
                    cell.ForeColor = Drawing.Color.White
                Case "DISCONTINUED"
                    cell.BackColor = Drawing.Color.Gray
                    cell.ForeColor = Drawing.Color.White
            End Select
        Next
    End Sub

    Protected Sub gvCellular_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType <> DataControlRowType.DataRow Then Return

        For i As Integer = 1 To 6
            Dim colour As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i))
            Dim status As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i & "Status"))
            Dim description As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i & "Description"))

            Dim cell As TableCell = e.Row.Cells(i)

            If String.IsNullOrWhiteSpace(colour) Then
                cell.CssClass = "stock-empty"
                cell.Text = "-"
                Continue For
            End If

            cell.Text = "<div class='stock-colour'>" & Server.HtmlEncode(colour.ToUpper()) & "</div><div class='stock-info'>" & description & "</div>"

            Select Case status.Trim().ToUpper()
                Case "IN STOCK"
                    cell.BackColor = Drawing.Color.DarkGreen
                    cell.ForeColor = Drawing.Color.White
                Case "LIMITED STOCK"
                    cell.BackColor = Drawing.Color.Yellow
                    cell.ForeColor = Drawing.Color.Black
                Case "OUT OF STOCK"
                    cell.BackColor = Drawing.Color.DarkRed
                    cell.ForeColor = Drawing.Color.White
                Case "DISCONTINUED"
                    cell.BackColor = Drawing.Color.Gray
                    cell.ForeColor = Drawing.Color.White
            End Select
        Next
    End Sub

    Protected Sub gvDesignShades_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType <> DataControlRowType.DataRow Then Return

        For i As Integer = 1 To 6
            Dim colour As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i))
            Dim status As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i & "Status"))
            Dim description As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i & "Description"))

            Dim cell As TableCell = e.Row.Cells(i)

            If String.IsNullOrWhiteSpace(colour) Then
                cell.CssClass = "stock-empty"
                cell.Text = "-"
                Continue For
            End If

            cell.Text = "<div class='stock-colour'>" & Server.HtmlEncode(colour.ToUpper()) & "</div><div class='stock-info'>" & description & "</div>"

            Select Case status.Trim().ToUpper()
                Case "IN STOCK"
                    cell.BackColor = Drawing.Color.DarkGreen
                    cell.ForeColor = Drawing.Color.White
                Case "LIMITED STOCK"
                    cell.BackColor = Drawing.Color.Yellow
                    cell.ForeColor = Drawing.Color.Black
                Case "OUT OF STOCK"
                    cell.BackColor = Drawing.Color.DarkRed
                    cell.ForeColor = Drawing.Color.White
                Case "DISCONTINUED"
                    cell.BackColor = Drawing.Color.Gray
                    cell.ForeColor = Drawing.Color.White
            End Select
        Next
    End Sub

    Protected Sub gvCurtain_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType <> DataControlRowType.DataRow Then Return

        For i As Integer = 1 To 6
            Dim colour As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i))
            Dim status As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i & "Status"))
            Dim description As String = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Col" & i & "Description"))

            Dim cell As TableCell = e.Row.Cells(i)

            If String.IsNullOrWhiteSpace(colour) Then
                cell.CssClass = "stock-empty"
                cell.Text = "-"
                Continue For
            End If

            cell.Text = "<div class='stock-colour'>" & Server.HtmlEncode(colour.ToUpper()) & "</div><div class='stock-info'>" & description & "</div>"

            Select Case status.Trim().ToUpper()
                Case "IN STOCK"
                    cell.BackColor = Drawing.Color.DarkGreen
                    cell.ForeColor = Drawing.Color.White
                Case "LIMITED STOCK"
                    cell.BackColor = Drawing.Color.Yellow
                    cell.ForeColor = Drawing.Color.Black
                Case "OUT OF STOCK"
                    cell.BackColor = Drawing.Color.DarkRed
                    cell.ForeColor = Drawing.Color.White
                Case "DISCONTINUED"
                    cell.BackColor = Drawing.Color.Gray
                    cell.ForeColor = Drawing.Color.White
            End Select
        Next
    End Sub
    Protected Sub gvListVenetian_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 1 To 6
                Dim colColour As String = "Col" & i
                Dim colStatus As String = "Col" & i & "Status"
                Dim colDesc As String = "Col" & i & "Description"

                Dim colourObj = DataBinder.Eval(e.Row.DataItem, colColour)
                Dim statusObj = DataBinder.Eval(e.Row.DataItem, colStatus)
                Dim descObj = DataBinder.Eval(e.Row.DataItem, colDesc)

                Dim colour As String = If(colourObj Is Nothing OrElse IsDBNull(colourObj), "", colourObj.ToString())
                Dim status As String = If(statusObj Is Nothing OrElse IsDBNull(statusObj), "", statusObj.ToString())
                Dim desc As String = If(descObj Is Nothing OrElse IsDBNull(descObj), "", descObj.ToString())

                ' RESET CELL
                e.Row.Cells(i).Text = ""
                e.Row.Cells(i).Attributes.Clear()

                ' TAMPILKAN
                If colour <> "" Then
                    e.Row.Cells(i).Text =
                    colour & If(desc <> "", "<br/>(" & desc & ")", "")
                End If

                ' STATUS COLORING
                If status = "In Stock" Then
                    e.Row.Cells(i).BackColor = Drawing.Color.DarkGreen
                    e.Row.Cells(i).ForeColor = Drawing.Color.White

                ElseIf status = "Out of Stock" Then
                    e.Row.Cells(i).BackColor = Drawing.Color.DarkRed
                    e.Row.Cells(i).ForeColor = Drawing.Color.White

                ElseIf status = "Limited Stock" Then
                    e.Row.Cells(i).BackColor = Drawing.Color.Yellow
                    e.Row.Cells(i).ForeColor = Drawing.Color.Black

                ElseIf status = "Discontinued" Then
                    e.Row.Cells(i).BackColor = Drawing.Color.Gray
                    e.Row.Cells(i).ForeColor = Drawing.Color.White
                End If
            Next
        End If
        If e.Row.RowType = DataControlRowType.Footer Then
            For i As Integer = 0 To gvListVenetian.Columns.Count - 1
                Dim bf As BoundField = TryCast(gvListVenetian.Columns(i), BoundField)
                If bf IsNot Nothing Then
                    e.Row.Cells(i).Text = bf.HeaderText
                End If
            Next
        End If
    End Sub

    Protected Sub gvListFabricChart_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 1 To e.Row.Cells.Count - 1
                Dim cellText As String = e.Row.Cells(i).Text.Trim()

                e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Center
                e.Row.Cells(i).VerticalAlign = VerticalAlign.Middle

                If cellText.Equals("No", StringComparison.OrdinalIgnoreCase) Then
                    e.Row.Cells(i).BackColor = Drawing.Color.DarkRed
                    e.Row.Cells(i).ForeColor = Drawing.Color.White
                    e.Row.Cells(i).Font.Bold = True
                End If
            Next
        End If
        If e.Row.RowType = DataControlRowType.Footer Then
            For i As Integer = 0 To gvListFabricChart.Columns.Count - 1
                Dim bf As BoundField = TryCast(gvListFabricChart.Columns(i), BoundField)
                If bf IsNot Nothing Then
                    e.Row.Cells(i).Text = bf.HeaderText
                End If
            Next
        End If
    End Sub

    Protected Sub gvRoller_PreRender(sender As Object, e As EventArgs) Handles gvRoller.PreRender
        If gvRoller.Rows.Count > 0 Then
            gvRoller.UseAccessibleHeader = True
            gvRoller.HeaderRow.TableSection = TableRowSection.TableHeader
        End If
    End Sub

    Protected Sub gvVertical_PreRender(sender As Object, e As EventArgs) Handles gvVertical.PreRender
        If gvVertical.Rows.Count > 0 Then
            gvVertical.UseAccessibleHeader = True
            gvVertical.HeaderRow.TableSection = TableRowSection.TableHeader
        End If
    End Sub

    Protected Sub gvSaphora_PreRender(sender As Object, e As EventArgs) Handles gvSaphora.PreRender
        If gvSaphora.Rows.Count > 0 Then
            gvSaphora.UseAccessibleHeader = True
            gvSaphora.HeaderRow.TableSection = TableRowSection.TableHeader
        End If
    End Sub

    Protected Sub gvCellular_PreRender(sender As Object, e As EventArgs)
        If gvCellular.Rows.Count > 0 Then
            gvCellular.UseAccessibleHeader = True
            gvCellular.HeaderRow.TableSection = TableRowSection.TableHeader
        End If
    End Sub

    Protected Sub gvDesignShades_PreRender(sender As Object, e As EventArgs)
        If gvDesignShades.Rows.Count > 0 Then
            gvDesignShades.UseAccessibleHeader = True
            gvDesignShades.HeaderRow.TableSection = TableRowSection.TableHeader
        End If
    End Sub

    Protected Sub gvCurtain_PreRender(sender As Object, e As EventArgs)
        If gvCurtain.Rows.Count > 0 Then
            gvCurtain.UseAccessibleHeader = True
            gvCurtain.HeaderRow.TableSection = TableRowSection.TableHeader
        End If
    End Sub

    Protected Sub gvListVenetian_PreRender(sender As Object, e As EventArgs)
        If gvListVenetian.Rows.Count > 0 Then
            gvListVenetian.UseAccessibleHeader = True
            gvListVenetian.HeaderRow.TableSection = TableRowSection.TableHeader
        End If
    End Sub

    Protected Sub BindRoller(searchText As String, companyDetail As String)
        MessageError_Roller(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "12"),
                New SqlParameter("@CompanyDetailId", companyDetail),
                New SqlParameter("@SearchText", searchText),
                New SqlParameter("@Type", "ROLLER")
            }
            Dim dt As DataTable = stockClass.GetDataTableSP("sp_Stock_Fabric", paramsItem)

            gvRoller.DataSource = GetRollerGridData(dt, companyDetail)
            gvRoller.DataBind()
        Catch ex As Exception
            MessageError_Roller(True, ex.Message)
        End Try
    End Sub

    Protected Sub BindVertical(searchText As String, companyDetail As String)
        MessageError_Vertical(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "11"),
                New SqlParameter("@CompanyDetailId", companyDetail),
                New SqlParameter("@SearchText", searchText),
                New SqlParameter("@Type", "VERTICAL")
            }

            Dim dt As DataTable = stockClass.GetDataTableSP("sp_Stock_Fabric", paramsItem)

            gvVertical.DataSource = GetVerticalGridData(dt, companyDetail)
            gvVertical.DataBind()
        Catch ex As Exception
            MessageError_Vertical(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindSaphora(companyDetail As String)
        MessageError_Saphora(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "19"),
                New SqlParameter("@CompanyDetailId", companyDetail),
                New SqlParameter("@SearchText", String.Empty),
                New SqlParameter("@Type", "SAPHORA")
            }
            Dim dt As DataTable = stockClass.GetDataTableSP("sp_Stock_Fabric", paramsItem)

            gvSaphora.DataSource = GetSaphoraGridData(dt, companyDetail)
            gvSaphora.DataBind()
        Catch ex As Exception
            MessageError_Saphora(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindCellularShades(companyDetail As String)
        MessageError_Cellular(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "2"),
                New SqlParameter("@CompanyDetailId", companyDetail),
                New SqlParameter("@SearchText", String.Empty),
                New SqlParameter("@Type", "CELLULAR SHADES")
            }
            Dim dt As DataTable = stockClass.GetDataTableSP("sp_Stock_Fabric", paramsItem)

            gvCellular.DataSource = GetCellularGridData(dt, companyDetail)
            gvCellular.DataBind()
        Catch ex As Exception
            MessageError_Cellular(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindDesignShades(companyDetail As String)
        MessageError_DesignShades(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "4"),
                New SqlParameter("@CompanyDetailId", companyDetail),
                New SqlParameter("@SearchText", String.Empty),
                New SqlParameter("@Type", "DESIGN SHADES")
            }
            Dim dt As DataTable = stockClass.GetDataTableSP("sp_Stock_Fabric", paramsItem)

            gvDesignShades.DataSource = GetDesignShadesGridData(dt, companyDetail)
            gvDesignShades.DataBind()
        Catch ex As Exception
            MessageError_DesignShades(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindCurtain(searchText As String, companyDetail As String)
        MessageError_Curtain(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "3"),
                New SqlParameter("@CompanyDetailId", companyDetail),
                New SqlParameter("@SearchText", searchText),
                New SqlParameter("@Type", "CURTAIN")
            }
            Dim dt As DataTable = stockClass.GetDataTableSP("sp_Stock_Fabric", paramsItem)

            gvCurtain.DataSource = GetCurtainGridData(dt, companyDetail)
            gvCurtain.DataBind()
        Catch ex As Exception
            MessageError_Curtain(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindVenetian(companyDetail As String)
        MessageError_Venetian(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@CompanyDetailId", companyDetail)
            }
            gvListVenetian.DataSource = stockClass.GetDataTableSP("sp_Stock_Venetian", paramsItem)
            gvListVenetian.DataBind()
        Catch ex As Exception
            MessageError_Venetian(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindFabricChart(searchText As String, companyDetail As String)
        MessageError_FabricChart(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", searchText),
                New SqlParameter("@CompanyDetailId", companyDetail)
            }
            gvListFabricChart.DataSource = stockClass.GetDataTableSP("sp_Stock_Fabric_Chart", paramsItem)
            gvListFabricChart.DataBind()
        Catch ex As Exception
            MessageError_FabricChart(True, ex.ToString())
        End Try
    End Sub

    Private Function GetRollerGridData(fabricData As DataTable, companyDetail As String) As DataTable
        Dim result As New DataTable()
        Try
            result.Columns.Add("Name", GetType(String))

            For i As Integer = 1 To 6
                result.Columns.Add("Col" & i, GetType(String))
                result.Columns.Add("Col" & i & "Status", GetType(String))
                result.Columns.Add("Col" & i & "Description", GetType(String))
            Next

            If fabricData Is Nothing OrElse fabricData.Rows.Count = 0 Then
                Return result
            End If

            For Each fabric As DataRow In fabricData.Rows
                Dim fabricId As Integer = Convert.ToInt32(fabric("Id"))
                Dim fabricName As String = Convert.ToString(fabric("Name"))

                Dim query As String = "SELECT Id, Colour, Status, Width, CASE WHEN RollQty IS NULL THEN '' ELSE CONVERT(VARCHAR(50), RollQty) + ' Roll' END AS RollQty, CASE WHEN ETAFactory IS NULL THEN '' ELSE CONVERT(VARCHAR(11), ETAFactory, 106) END AS ETAFactory FROM FabricColours WHERE FabricId=" & fabricId & " AND Factory='Express' AND (Status='In Stock' OR Status='Out of Stock' OR Status='Limited Stock' OR Status='Discontinued')"

                If Not String.IsNullOrEmpty(companyDetail) Then
                    query &= " AND EXISTS (SELECT 1 FROM STRING_SPLIT(CompanyDetailId, ',') AS C WHERE C.VALUE='" & companyDetail.Replace("'", "''") & "')"
                End If

                query &= " ORDER BY Colour"

                Dim colourData As DataTable = stockClass.GetDataTable(query)

                If colourData Is Nothing OrElse colourData.Rows.Count = 0 Then
                    Dim emptyRow As DataRow = result.NewRow()
                    emptyRow("Name") = fabricName
                    result.Rows.Add(emptyRow)
                    Continue For
                End If

                Dim index As Integer = 0
                Dim currentRow As DataRow = Nothing

                For Each colour As DataRow In colourData.Rows
                    If index Mod 6 = 0 Then
                        currentRow = result.NewRow()
                        currentRow("Name") = fabricName
                    End If

                    Dim colIndex As Integer = (index Mod 6) + 1

                    currentRow("Col" & colIndex) = Convert.ToString(colour("Colour"))
                    currentRow("Col" & colIndex & "Status") = Convert.ToString(colour("Status"))

                    Dim description As String = String.Empty

                    If Convert.ToString(colour("Status")) <> "Discontinued" Then
                        If Not IsDBNull(colour("Width")) AndAlso Not String.IsNullOrEmpty(Convert.ToString(colour("Width")).Trim()) Then
                            description = "MAX : " & Convert.ToString(colour("Width")) & " MM"
                        End If

                        If Not IsDBNull(colour("RollQty")) AndAlso Not String.IsNullOrEmpty(Convert.ToString(colour("RollQty")).Trim()) Then
                            description &= If(description <> "", "<br/>", "") & "ROLL : " & Convert.ToString(colour("RollQty")) & " QTY"
                        End If

                        If Not IsDBNull(colour("ETAFactory")) AndAlso Not String.IsNullOrEmpty(Convert.ToString(colour("ETAFactory")).Trim()) Then
                            description &= If(description <> "", "<br/>", "") & "ETA : " & Convert.ToString(colour("ETAFactory"))
                        End If
                    End If

                    currentRow("Col" & colIndex & "Description") = description

                    If index Mod 6 = 5 OrElse index = colourData.Rows.Count - 1 Then
                        result.Rows.Add(currentRow)
                    End If

                    index += 1
                Next
            Next
        Catch
            Throw
        End Try
        Return result
    End Function

    Protected Function GetVerticalGridData(fabricData As DataTable, companyDetail As String) As DataTable
        Dim result As New DataTable()
        Try
            result.Columns.Add("Name", GetType(String))

            For i As Integer = 1 To 6
                result.Columns.Add("Col" & i, GetType(String))
                result.Columns.Add("Col" & i & "Status", GetType(String))
                result.Columns.Add("Col" & i & "Description", GetType(String))
            Next

            If fabricData Is Nothing OrElse fabricData.Rows.Count = 0 Then
                Return result
            End If

            For Each fabric As DataRow In fabricData.Rows
                Dim fabricId As Integer = Convert.ToInt32(fabric("Id"))
                Dim fabricName As String = Convert.ToString(fabric("Name"))

                Dim query As String = "SELECT Id, Colour, Status, Width, CASE WHEN RollQty IS NULL THEN '' ELSE CONVERT(VARCHAR(50), RollQty) + ' Roll' END AS RollQty, CASE WHEN ETAFactory IS NULL THEN '' ELSE CONVERT(VARCHAR(11), ETAFactory, 106) END AS ETAFactory FROM FabricColours WHERE FabricId=" & fabricId & " AND Factory='Express' AND (Status='In Stock' OR Status='Out of Stock' OR Status='Limited Stock' OR Status='Discontinued')"

                If Not String.IsNullOrEmpty(companyDetail) Then
                    query &= " AND EXISTS (SELECT 1 FROM STRING_SPLIT(CompanyDetailId, ',') AS C WHERE C.VALUE='" & companyDetail.Replace("'", "''") & "')"
                End If

                query &= " ORDER BY Colour"

                Dim colourData As DataTable = stockClass.GetDataTable(query)

                If colourData Is Nothing OrElse colourData.Rows.Count = 0 Then
                    Dim emptyRow As DataRow = result.NewRow()
                    emptyRow("Name") = fabricName
                    result.Rows.Add(emptyRow)
                    Continue For
                End If

                Dim index As Integer = 0
                Dim currentRow As DataRow = Nothing

                For Each colour As DataRow In colourData.Rows
                    If index Mod 6 = 0 Then
                        currentRow = result.NewRow()
                        currentRow("Name") = fabricName
                    End If

                    Dim colIndex As Integer = (index Mod 6) + 1

                    currentRow("Col" & colIndex) = Convert.ToString(colour("Colour"))
                    currentRow("Col" & colIndex & "Status") = Convert.ToString(colour("Status"))

                    Dim description As String = ""

                    If Not IsDBNull(colour("RollQty")) AndAlso Convert.ToString(colour("RollQty")) <> "" Then
                        description &= If(description <> "", "<br/>", "") & "Roll QTY: " & Convert.ToString(colour("RollQty"))
                    End If

                    If Not IsDBNull(colour("ETAFactory")) AndAlso Convert.ToString(colour("ETAFactory")) <> "" Then
                        description &= If(description <> "", "<br/>", "") & "ETA Factory: " & Convert.ToString(colour("ETAFactory"))
                    End If

                    currentRow("Col" & colIndex & "Description") = description

                    If index Mod 6 = 5 OrElse index = colourData.Rows.Count - 1 Then
                        result.Rows.Add(currentRow)
                    End If

                    index += 1
                Next
            Next
        Catch
            Throw
        End Try
        Return result
    End Function

    Protected Function GetSaphoraGridData(fabricData As DataTable, companyDetail As String) As DataTable
        Dim result As New DataTable()
        Try
            result.Columns.Add("Name", GetType(String))

            For i As Integer = 1 To 6
                result.Columns.Add("Col" & i, GetType(String))
                result.Columns.Add("Col" & i & "Status", GetType(String))
                result.Columns.Add("Col" & i & "Description", GetType(String))
            Next

            If fabricData Is Nothing OrElse fabricData.Rows.Count = 0 Then
                Return result
            End If

            For Each fabric As DataRow In fabricData.Rows
                Dim fabricId As Integer = Convert.ToInt32(fabric("Id"))
                Dim fabricName As String = Convert.ToString(fabric("Name"))

                Dim query As String = "SELECT Id, Colour, Status, Width, CASE WHEN RollQty IS NULL THEN '' ELSE CONVERT(VARCHAR(50), RollQty) + ' Roll' END AS RollQty, CASE WHEN ETAFactory IS NULL THEN '' ELSE CONVERT(VARCHAR(11), ETAFactory, 106) END AS ETAFactory FROM FabricColours WHERE FabricId=" & fabricId & " AND Factory='Express' AND (Status='In Stock' OR Status='Out of Stock' OR Status='Limited Stock' OR Status='Discontinued')"

                If Not String.IsNullOrEmpty(companyDetail) Then
                    query &= " AND EXISTS (SELECT 1 FROM STRING_SPLIT(CompanyDetailId, ',') AS C WHERE C.VALUE='" & companyDetail.Replace("'", "''") & "')"
                End If

                query &= " ORDER BY Colour"

                Dim colourData As DataTable = stockClass.GetDataTable(query)

                If colourData Is Nothing OrElse colourData.Rows.Count = 0 Then
                    Dim emptyRow As DataRow = result.NewRow()
                    emptyRow("Name") = fabricName
                    result.Rows.Add(emptyRow)
                    Continue For
                End If

                Dim index As Integer = 0
                Dim currentRow As DataRow = Nothing

                For Each colour As DataRow In colourData.Rows
                    If index Mod 6 = 0 Then
                        currentRow = result.NewRow()
                        currentRow("Name") = fabricName
                    End If

                    Dim colIndex As Integer = (index Mod 6) + 1

                    currentRow("Col" & colIndex) = Convert.ToString(colour("Colour"))
                    currentRow("Col" & colIndex & "Status") = Convert.ToString(colour("Status"))

                    Dim description As String = String.Empty

                    If Convert.ToString(colour("Status")) <> "Discontinued" Then
                        If Not IsDBNull(colour("Width")) AndAlso Not String.IsNullOrEmpty(Convert.ToString(colour("Width")).Trim()) Then
                            description = "MAX : " & Convert.ToString(colour("Width")) & " MM"
                        End If

                        If Not IsDBNull(colour("RollQty")) AndAlso Not String.IsNullOrEmpty(Convert.ToString(colour("RollQty")).Trim()) Then
                            description &= If(description <> "", "<br/>", "") & "ROLL : " & Convert.ToString(colour("RollQty")) & " QTY"
                        End If

                        If Not IsDBNull(colour("ETAFactory")) AndAlso Not String.IsNullOrEmpty(Convert.ToString(colour("ETAFactory")).Trim()) Then
                            description &= If(description <> "", "<br/>", "") & "ETA : " & Convert.ToString(colour("ETAFactory"))
                        End If
                    End If

                    currentRow("Col" & colIndex & "Description") = description

                    If index Mod 6 = 5 OrElse index = colourData.Rows.Count - 1 Then
                        result.Rows.Add(currentRow)
                    End If

                    index += 1
                Next
            Next
        Catch
            Throw
        End Try
        Return result
    End Function

    Private Function GetCellularGridData(fabricData As DataTable, companyDetail As String) As DataTable
        Dim result As New DataTable()
        Try
            result.Columns.Add("Name", GetType(String))

            For i As Integer = 1 To 6
                result.Columns.Add("Col" & i, GetType(String))
                result.Columns.Add("Col" & i & "Status", GetType(String))
                result.Columns.Add("Col" & i & "Description", GetType(String))
            Next

            If fabricData Is Nothing OrElse fabricData.Rows.Count = 0 Then
                Return result
            End If

            For Each fabric As DataRow In fabricData.Rows
                Dim fabricId As Integer = Convert.ToInt32(fabric("Id"))
                Dim fabricName As String = Convert.ToString(fabric("Name"))

                Dim query As String = "SELECT Id, Colour, Status, Width, CASE WHEN RollQty IS NULL THEN '' ELSE CONVERT(VARCHAR(50), RollQty) + ' Roll' END AS RollQty, CASE WHEN ETAFactory IS NULL THEN '' ELSE CONVERT(VARCHAR(11), ETAFactory, 106) END AS ETAFactory FROM FabricColours WHERE FabricId=" & fabricId & " AND Factory='Express' AND (Status='In Stock' OR Status='Out of Stock' OR Status='Limited Stock' OR Status='Discontinued')"

                If Not String.IsNullOrEmpty(companyDetail) Then
                    query &= " AND EXISTS (SELECT 1 FROM STRING_SPLIT(CompanyDetailId, ',') AS C WHERE C.VALUE='" & companyDetail.Replace("'", "''") & "')"
                End If

                query &= " ORDER BY Colour"

                Dim colourData As DataTable = stockClass.GetDataTable(query)

                If colourData Is Nothing OrElse colourData.Rows.Count = 0 Then
                    Dim emptyRow As DataRow = result.NewRow()
                    emptyRow("Name") = fabricName
                    result.Rows.Add(emptyRow)
                    Continue For
                End If

                Dim index As Integer = 0
                Dim currentRow As DataRow = Nothing

                For Each colour As DataRow In colourData.Rows
                    If index Mod 6 = 0 Then
                        currentRow = result.NewRow()
                        currentRow("Name") = fabricName
                    End If

                    Dim colIndex As Integer = (index Mod 6) + 1

                    currentRow("Col" & colIndex) = Convert.ToString(colour("Colour"))
                    currentRow("Col" & colIndex & "Status") = Convert.ToString(colour("Status"))

                    Dim description As String = String.Empty

                    If Convert.ToString(colour("Status")) <> "Discontinued" Then
                        If Not IsDBNull(colour("RollQty")) AndAlso Not String.IsNullOrEmpty(Convert.ToString(colour("RollQty")).Trim()) Then
                            description &= If(description <> "", "<br/>", "") & "ROLL : " & Convert.ToString(colour("RollQty")) & " QTY"
                        End If

                        If Not IsDBNull(colour("ETAFactory")) AndAlso Not String.IsNullOrEmpty(Convert.ToString(colour("ETAFactory")).Trim()) Then
                            description &= If(description <> "", "<br/>", "") & "ETA : " & Convert.ToString(colour("ETAFactory"))
                        End If
                    End If

                    currentRow("Col" & colIndex & "Description") = description

                    If index Mod 6 = 5 OrElse index = colourData.Rows.Count - 1 Then
                        result.Rows.Add(currentRow)
                    End If

                    index += 1
                Next
            Next
        Catch
            Throw
        End Try
        Return result
    End Function

    Private Function GetDesignShadesGridData(fabricData As DataTable, companyDetail As String) As DataTable
        Dim result As New DataTable()
        Try
            result.Columns.Add("Name", GetType(String))

            For i As Integer = 1 To 6
                result.Columns.Add("Col" & i, GetType(String))
                result.Columns.Add("Col" & i & "Status", GetType(String))
                result.Columns.Add("Col" & i & "Description", GetType(String))
            Next

            If fabricData Is Nothing OrElse fabricData.Rows.Count = 0 Then
                Return result
            End If

            For Each fabric As DataRow In fabricData.Rows
                Dim fabricId As Integer = Convert.ToInt32(fabric("Id"))
                Dim fabricName As String = Convert.ToString(fabric("Name"))

                Dim query As String = "SELECT Id, Colour, Status, Width, CASE WHEN RollQty IS NULL THEN '' ELSE CONVERT(VARCHAR(50), RollQty) + ' Roll' END AS RollQty, CASE WHEN ETAFactory IS NULL THEN '' ELSE CONVERT(VARCHAR(11), ETAFactory, 106) END AS ETAFactory FROM FabricColours WHERE FabricId=" & fabricId & " AND Factory='Express' AND (Status='In Stock' OR Status='Out of Stock' OR Status='Limited Stock' OR Status='Discontinued')"

                If Not String.IsNullOrEmpty(companyDetail) Then
                    query &= " AND EXISTS (SELECT 1 FROM STRING_SPLIT(CompanyDetailId, ',') AS C WHERE C.VALUE='" & companyDetail.Replace("'", "''") & "')"
                End If

                query &= " ORDER BY Colour"

                Dim colourData As DataTable = stockClass.GetDataTable(query)

                If colourData Is Nothing OrElse colourData.Rows.Count = 0 Then
                    Dim emptyRow As DataRow = result.NewRow()
                    emptyRow("Name") = fabricName
                    result.Rows.Add(emptyRow)
                    Continue For
                End If

                Dim index As Integer = 0
                Dim currentRow As DataRow = Nothing

                For Each colour As DataRow In colourData.Rows
                    If index Mod 6 = 0 Then
                        currentRow = result.NewRow()
                        currentRow("Name") = fabricName
                    End If

                    Dim colIndex As Integer = (index Mod 6) + 1

                    currentRow("Col" & colIndex) = Convert.ToString(colour("Colour"))
                    currentRow("Col" & colIndex & "Status") = Convert.ToString(colour("Status"))

                    Dim description As String = String.Empty

                    If Convert.ToString(colour("Status")) <> "Discontinued" Then
                        If Not IsDBNull(colour("Width")) AndAlso Not String.IsNullOrEmpty(Convert.ToString(colour("Width")).Trim()) Then
                            description = "MAX : " & Convert.ToString(colour("Width")) & " MM"
                        End If

                        If Not IsDBNull(colour("RollQty")) AndAlso Not String.IsNullOrEmpty(Convert.ToString(colour("RollQty")).Trim()) Then
                            description &= If(description <> "", "<br/>", "") & "ROLL : " & Convert.ToString(colour("RollQty")) & " QTY"
                        End If

                        If Not IsDBNull(colour("ETAFactory")) AndAlso Not String.IsNullOrEmpty(Convert.ToString(colour("ETAFactory")).Trim()) Then
                            description &= If(description <> "", "<br/>", "") & "ETA : " & Convert.ToString(colour("ETAFactory"))
                        End If
                    End If

                    currentRow("Col" & colIndex & "Description") = description

                    If index Mod 6 = 5 OrElse index = colourData.Rows.Count - 1 Then
                        result.Rows.Add(currentRow)
                    End If

                    index += 1
                Next
            Next
        Catch
            Throw
        End Try
        Return result
    End Function

    Private Function GetCurtainGridData(fabricData As DataTable, companyDetail As String) As DataTable
        Dim result As New DataTable()
        Try
            result.Columns.Add("Name", GetType(String))

            For i As Integer = 1 To 6
                result.Columns.Add("Col" & i, GetType(String))
                result.Columns.Add("Col" & i & "Status", GetType(String))
                result.Columns.Add("Col" & i & "Description", GetType(String))
            Next

            If fabricData Is Nothing OrElse fabricData.Rows.Count = 0 Then
                Return result
            End If

            For Each fabric As DataRow In fabricData.Rows
                Dim fabricId As Integer = Convert.ToInt32(fabric("Id"))
                Dim fabricName As String = Convert.ToString(fabric("Name"))

                Dim query As String = "SELECT Id, Colour, Status, Width, CASE WHEN RollQty IS NULL THEN '' ELSE CONVERT(VARCHAR(50), RollQty) + ' Roll' END AS RollQty, CASE WHEN ETAFactory IS NULL THEN '' ELSE CONVERT(VARCHAR(11), ETAFactory, 106) END AS ETAFactory FROM FabricColours WHERE FabricId=" & fabricId & " AND Factory='Express' AND (Status='In Stock' OR Status='Out of Stock' OR Status='Limited Stock' OR Status='Discontinued')"

                If Not String.IsNullOrEmpty(companyDetail) Then
                    query &= " AND EXISTS (SELECT 1 FROM STRING_SPLIT(CompanyDetailId, ',') AS C WHERE C.VALUE='" & companyDetail.Replace("'", "''") & "')"
                End If

                query &= " ORDER BY Colour"

                Dim colourData As DataTable = stockClass.GetDataTable(query)

                If colourData Is Nothing OrElse colourData.Rows.Count = 0 Then
                    Dim emptyRow As DataRow = result.NewRow()
                    emptyRow("Name") = fabricName
                    result.Rows.Add(emptyRow)
                    Continue For
                End If

                Dim index As Integer = 0
                Dim currentRow As DataRow = Nothing

                For Each colour As DataRow In colourData.Rows
                    If index Mod 6 = 0 Then
                        currentRow = result.NewRow()
                        currentRow("Name") = fabricName
                    End If

                    Dim colIndex As Integer = (index Mod 6) + 1

                    currentRow("Col" & colIndex) = Convert.ToString(colour("Colour"))
                    currentRow("Col" & colIndex & "Status") = Convert.ToString(colour("Status"))

                    Dim description As String = String.Empty

                    If Convert.ToString(colour("Status")) <> "Discontinued" Then
                        If Not IsDBNull(colour("Width")) AndAlso Not String.IsNullOrEmpty(Convert.ToString(colour("Width")).Trim()) Then
                            description = "MAX : " & Convert.ToString(colour("Width")) & " MM"
                        End If

                        If Not IsDBNull(colour("RollQty")) AndAlso Not String.IsNullOrEmpty(Convert.ToString(colour("RollQty")).Trim()) Then
                            description &= If(description <> "", "<br/>", "") & "ROLL : " & Convert.ToString(colour("RollQty")) & " QTY"
                        End If

                        If Not IsDBNull(colour("ETAFactory")) AndAlso Not String.IsNullOrEmpty(Convert.ToString(colour("ETAFactory")).Trim()) Then
                            description &= If(description <> "", "<br/>", "") & "ETA : " & Convert.ToString(colour("ETAFactory"))
                        End If
                    End If

                    currentRow("Col" & colIndex & "Description") = description

                    If index Mod 6 = 5 OrElse index = colourData.Rows.Count - 1 Then
                        result.Rows.Add(currentRow)
                    End If

                    index += 1
                Next
            Next
        Catch
            Throw
        End Try
        Return result
    End Function

    Protected Sub MessageError_Roller(visible As Boolean, message As String)
        divErrorRoller.Visible = visible : msgErrorRoller.InnerText = message
    End Sub

    Protected Sub MessageError_Vertical(visible As Boolean, message As String)
        divErrorVertical.Visible = visible : msgErrorVertical.InnerText = message
    End Sub

    Protected Sub MessageError_Saphora(visible As Boolean, message As String)
        divErrorSaphora.Visible = visible : msgErrorSaphora.InnerText = message
    End Sub

    Protected Sub MessageError_Cellular(visible As Boolean, message As String)
        divErrorCellular.Visible = visible : msgErrorCellular.InnerText = message
    End Sub

    Protected Sub MessageError_DesignShades(visible As Boolean, message As String)
        divErrorDesignShades.Visible = visible : msgErrorDesignShades.InnerText = message
    End Sub

    Protected Sub MessageError_Curtain(visible As Boolean, message As String)
        divErrorCurtain.Visible = visible : msgErrorCurtain.InnerText = message
    End Sub

    Protected Sub MessageError_Venetian(visible As Boolean, message As String)
        divErrorVenetian.Visible = visible : msgErrorVenetian.InnerText = message
    End Sub

    Protected Sub MessageError_FabricChart(visible As Boolean, message As String)
        divErrorFabricChart.Visible = visible : msgErrorFabricChart.InnerText = message
    End Sub
End Class
