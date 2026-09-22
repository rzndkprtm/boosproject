Imports System.Data
Imports System.Data.SqlClient

Partial Class Stock
    Inherits Page

    Dim stockClass As New StockClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindRoller(txtSearchRoller.Text)
            BindDesignShades()
            BindCurtain(txtSearchCurtain.Text)
            BindVertical(txtSearchVertical.Text)
            BindVenetian()
            BindAluminium()
            BindCellularShades()
            BindFabricChart(txtSearchFabricChart.Text)
        End If
    End Sub


    ' ROLLER

    Protected Sub BindRoller(searchText As String)
        MessageError_Roller(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "12"),
                New SqlParameter("@CompanyDetailId", 2),
                New SqlParameter("@SearchText", searchText),
                New SqlParameter("@Type", "ROLLER")
            }
            Dim dt As DataTable = stockClass.GetDataTableSP("sp_Stock_Fabric", paramsItem)

            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                rptRoller.DataSource = Nothing
                rptRoller.DataBind()
                pnlNoDataRoller.Visible = True
                Return
            End If

            pnlNoDataRoller.Visible = False

            rptRoller.DataSource = dt
            rptRoller.DataBind()
        Catch ex As Exception
            MessageError_Roller(True, ex.Message)
        End Try
    End Sub

    Protected Sub rptRoller_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Try
            If e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem Then
                Return
            End If

            Dim fabricId As Integer = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "Id"))

            Dim rptRollerColour As Repeater = CType(e.Item.FindControl("rptRollerColour"), Repeater)

            Dim dt As DataTable = stockClass.GetDataTable("SELECT Id, Colour, Status, Width, CASE WHEN RollQty IS NULL THEN '' ELSE CONVERT(VARCHAR(50), RollQty) + ' Roll' END AS RollQty, CASE WHEN ETAFactory IS NULL THEN '' ELSE CONVERT(VARCHAR(10), ETAFactory, 103) END AS ETAFactory FROM FabricColours WHERE FabricId=" & fabricId & " AND Factory='Express' ORDER BY Colour")

            rptRollerColour.DataSource = dt
            rptRollerColour.DataBind()
        Catch ex As Exception
            MessageError_Roller(True, ex.Message)
        End Try
    End Sub

    Protected Sub btnSearchRoller_Click(sender As Object, e As EventArgs)
        BindRoller(txtSearchRoller.Text)
    End Sub

    Protected Sub MessageError_Roller(visible As Boolean, message As String)
        divErrorRoller.Visible = visible : msgErrorRoller.InnerText = message
    End Sub


    ' VERTICAL

    Protected Sub BindVertical(searchText As String)
        MessageError_Vertical(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "11"),
                New SqlParameter("@CompanyDetailId", 2),
                New SqlParameter("@SearchText", searchText),
                New SqlParameter("@Type", "VERTICAL")
            }
            Dim dt As DataTable = stockClass.GetDataTableSP("sp_Stock_Fabric", paramsItem)

            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                rptVertical.DataSource = Nothing
                rptVertical.DataBind()
                pnlNoDataVertical.Visible = True
                Return
            End If

            pnlNoDataVertical.Visible = False

            rptVertical.DataSource = dt
            rptVertical.DataBind()
        Catch ex As Exception
            MessageError_Vertical(True, ex.ToString())
        End Try
    End Sub

    Protected Sub rptVertical_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Try
            If e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem Then
                Return
            End If

            Dim fabricId As Integer = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "Id"))

            Dim rptVerticalColour As Repeater = CType(e.Item.FindControl("rptVerticalColour"), Repeater)

            Dim dt As DataTable = stockClass.GetDataTable("SELECT Id, Colour, Status, Width, CASE WHEN RollQty IS NULL THEN '' ELSE CONVERT(VARCHAR(50), RollQty) + ' Roll' END AS RollQty, CASE WHEN ETAFactory IS NULL THEN '' ELSE CONVERT(VARCHAR(10), ETAFactory, 103) END AS ETAFactory FROM FabricColours WHERE FabricId=" & fabricId & " AND Factory='Express' ORDER BY Name")

            rptVerticalColour.DataSource = dt
            rptVerticalColour.DataBind()
        Catch ex As Exception
            MessageError_Vertical(True, ex.Message)
        End Try
    End Sub

    Protected Sub MessageError_Vertical(visible As Boolean, message As String)
        divErrorVertical.Visible = visible : msgErrorVertical.InnerText = message
    End Sub

    Protected Sub btnSearchVertical_Click(sender As Object, e As EventArgs)
        BindVertical(txtSearchVertical.Text)
    End Sub


    ' CELLULAR SHADES

    Protected Sub BindCellularShades()
        MessageError_Cellular(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "2"),
                New SqlParameter("@CompanyDetailId", 2),
                New SqlParameter("@SearchText", String.Empty),
                New SqlParameter("@Type", "CELLULAR SHADES")
            }
            Dim dt As DataTable = stockClass.GetDataTableSP("sp_Stock_Fabric", paramsItem)

            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                rptCellular.DataSource = Nothing
                rptCellular.DataBind()
                pnlNoDataCellular.Visible = True
                Return
            End If

            pnlNoDataCellular.Visible = False

            rptCellular.DataSource = dt
            rptCellular.DataBind()
        Catch ex As Exception
            MessageError_Cellular(True, ex.ToString())
        End Try
    End Sub

    Protected Sub rptCellular_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Try
            If e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem Then
                Return
            End If

            Dim fabricId As Integer = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "Id"))

            Dim rptCellularColour As Repeater = CType(e.Item.FindControl("rptCellularColour"), Repeater)

            Dim dt As DataTable = stockClass.GetDataTable("SELECT Id, Colour, Status, Width, CASE WHEN RollQty IS NULL THEN '' ELSE CONVERT(VARCHAR(50), RollQty) + ' Roll' END AS RollQty, CASE WHEN ETAFactory IS NULL THEN '' ELSE CONVERT(VARCHAR(10), ETAFactory, 103) END AS ETAFactory FROM FabricColours WHERE FabricId=" & fabricId & " AND Factory='Express' ORDER BY Name")

            rptCellularColour.DataSource = dt
            rptCellularColour.DataBind()
        Catch ex As Exception
            MessageError_Cellular(True, ex.Message)
        End Try
    End Sub

    Protected Sub MessageError_Cellular(visible As Boolean, message As String)
        divErrorCellular.Visible = visible : msgErrorCellular.InnerText = message
    End Sub


    ' DESIGN SHADES

    Protected Sub BindDesignShades()
        MessageError_DesignShades(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "4"),
                New SqlParameter("@CompanyDetailId", 2),
                New SqlParameter("@SearchText", String.Empty),
                New SqlParameter("@Type", "DESIGN SHADES")
            }
            Dim dt As DataTable = stockClass.GetDataTableSP("sp_Stock_Fabric", paramsItem)

            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                rptDesignShades.DataSource = Nothing
                rptDesignShades.DataBind()
                pnlNoDataDesignShades.Visible = True
                Return
            End If

            pnlNoDataDesignShades.Visible = False

            rptDesignShades.DataSource = dt
            rptDesignShades.DataBind()
        Catch ex As Exception
            MessageError_DesignShades(True, ex.Message)
        End Try
    End Sub

    Protected Sub rptDesignShades_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Try
            If e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem Then
                Return
            End If

            Dim fabricId As Integer = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "Id"))

            Dim rptDesignShadesColour As Repeater = CType(e.Item.FindControl("rptDesignShadesColour"), Repeater)

            Dim dt As DataTable = stockClass.GetDataTable("SELECT Id, Colour, Status, Width, CASE WHEN RollQty IS NULL THEN '' ELSE CONVERT(VARCHAR(50), RollQty) + ' Roll' END AS RollQty, CASE WHEN ETAFactory IS NULL THEN '' ELSE CONVERT(VARCHAR(10), ETAFactory, 103) END AS ETAFactory FROM FabricColours WHERE FabricId=" & fabricId & " AND Factory='Express' ORDER BY Name")

            rptDesignShadesColour.DataSource = dt
            rptDesignShadesColour.DataBind()
        Catch ex As Exception
            MessageError_Roller(True, ex.Message)
        End Try
    End Sub

    Protected Sub MessageError_DesignShades(visible As Boolean, message As String)
        divErrorDesignShades.Visible = visible : msgErrorDesignShades.InnerText = message
    End Sub


    'CURTAIN

    Protected Sub BindCurtain(searchText As String)
        MessageError_Curtain(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "3"),
                New SqlParameter("@CompanyDetailId", 2),
                New SqlParameter("@SearchText", String.Empty),
                New SqlParameter("@Type", "CURTAIN")
            }
            Dim dt As DataTable = stockClass.GetDataTableSP("sp_Stock_Fabric", paramsItem)

            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                rptCurtain.DataSource = Nothing
                rptCurtain.DataBind()
                pnlNoDataCurtain.Visible = True
                Return
            End If

            pnlNoDataCurtain.Visible = False

            rptCurtain.DataSource = dt
            rptCurtain.DataBind()
        Catch ex As Exception
            MessageError_Curtain(True, ex.ToString())
        End Try
    End Sub

    Protected Sub MessageError_Curtain(visible As Boolean, message As String)
        divErrorCurtain.Visible = visible : msgErrorCurtain.InnerText = message
    End Sub

    Protected Sub btnSearchCurtain_Click(sender As Object, e As EventArgs)
        BindCurtain(txtSearchCurtain.Text)
    End Sub

    Protected Sub rptCurtain_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Try
            If e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem Then
                Return
            End If

            Dim fabricId As Integer = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "Id"))

            Dim rptCurtainColour As Repeater = CType(e.Item.FindControl("rptCurtainColour"), Repeater)

            Dim dt As DataTable = stockClass.GetDataTable("SELECT Id, Colour, Status, Width, CASE WHEN RollQty IS NULL THEN '' ELSE CONVERT(VARCHAR(50), RollQty) + ' Roll' END AS RollQty, CASE WHEN ETAFactory IS NULL THEN '' ELSE CONVERT(VARCHAR(10), ETAFactory, 103) END AS ETAFactory FROM FabricColours WHERE FabricId=" & fabricId & " AND Factory='Express' ORDER BY Name")

            rptCurtainColour.DataSource = dt
            rptCurtainColour.DataBind()
        Catch ex As Exception
            MessageError_Curtain(True, ex.Message)
        End Try
    End Sub


    ' VENETIAN

    Protected Sub BindVenetian()
        MessageError_Venetian(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "10"),
                New SqlParameter("@CompanyDetailId", "2")
            }
            gvListVenetian.DataSource = stockClass.GetDataTableSP("sp_Stock_Venetian", paramsItem)
            gvListVenetian.DataBind()
        Catch ex As Exception
            MessageError_Venetian(True, ex.ToString())
        End Try
    End Sub

    Protected Sub MessageError_Venetian(visible As Boolean, message As String)
        divErrorVenetian.Visible = visible : msgErrorVenetian.InnerText = message
    End Sub

    Protected Sub gvListVenetian_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 1 To 6
                Dim colName As String = "Col" & i & "Status"
                Dim isActiveObj = DataBinder.Eval(e.Row.DataItem, colName)

                If isActiveObj IsNot Nothing AndAlso Not IsDBNull(isActiveObj) Then
                    Dim thisStatus As String = isActiveObj.ToString()

                    If thisStatus = "In Stock" Then
                        e.Row.Cells(i).BackColor = Drawing.Color.DarkGreen
                        e.Row.Cells(i).ForeColor = Drawing.Color.White
                    End If
                    If thisStatus = "Out of Stock" Then
                        e.Row.Cells(i).BackColor = Drawing.Color.DarkRed
                        e.Row.Cells(i).ForeColor = Drawing.Color.White
                    End If
                    If thisStatus = "Limited Stock" Then
                        e.Row.Cells(i).BackColor = Drawing.Color.Yellow
                        e.Row.Cells(i).ForeColor = Drawing.Color.Black
                    End If
                    If thisStatus = "Discontinued" Then
                        e.Row.Cells(i).BackColor = Drawing.Color.Gray
                        e.Row.Cells(i).ForeColor = Drawing.Color.White
                    End If
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


    ' ALUMINIUM

    Protected Sub BindAluminium()
        MessageError_Aluminium(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "1"),
                New SqlParameter("@CompanyDetailId", "2")
            }
            gvListAluminium.DataSource = stockClass.GetDataTableSP("sp_Stock_Venetian", paramsItem)
            gvListAluminium.DataBind()
        Catch ex As Exception
            MessageError_Aluminium(True, ex.ToString())
        End Try
    End Sub

    Protected Sub MessageError_Aluminium(visible As Boolean, message As String)
        divErrorAluminium.Visible = visible : msgErrorAluminium.InnerText = message
    End Sub

    Protected Sub gvListAluminium_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 1 To 6
                Dim colName As String = "Col" & i & "Status"
                Dim isActiveObj = DataBinder.Eval(e.Row.DataItem, colName)

                If isActiveObj IsNot Nothing AndAlso Not IsDBNull(isActiveObj) Then
                    Dim thisStatus As String = isActiveObj.ToString()

                    If thisStatus = "In Stock" Then
                        e.Row.Cells(i).BackColor = Drawing.Color.DarkGreen
                        e.Row.Cells(i).ForeColor = Drawing.Color.White
                    End If
                    If thisStatus = "Out of Stock" Then
                        e.Row.Cells(i).BackColor = Drawing.Color.DarkRed
                        e.Row.Cells(i).ForeColor = Drawing.Color.White
                    End If
                    If thisStatus = "Limited Stock" Then
                        e.Row.Cells(i).BackColor = Drawing.Color.Yellow
                        e.Row.Cells(i).ForeColor = Drawing.Color.Black
                    End If
                    If thisStatus = "Discontinued" Then
                        e.Row.Cells(i).BackColor = Drawing.Color.Gray
                        e.Row.Cells(i).ForeColor = Drawing.Color.White
                    End If
                End If
            Next
        End If
        If e.Row.RowType = DataControlRowType.Footer Then
            For i As Integer = 0 To gvListAluminium.Columns.Count - 1
                Dim bf As BoundField = TryCast(gvListAluminium.Columns(i), BoundField)
                If bf IsNot Nothing Then
                    e.Row.Cells(i).Text = bf.HeaderText
                End If
            Next
        End If
    End Sub





    ' FABRIC CHART

    Protected Sub btnFabricChart_Click(sender As Object, e As EventArgs)
        BindFabricChart(txtSearchFabricChart.Text)
    End Sub

    Protected Sub gvListFabricChart_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.Footer Then
            For i As Integer = 0 To gvListFabricChart.Columns.Count - 1
                Dim bf As BoundField = TryCast(gvListFabricChart.Columns(i), BoundField)
                If bf IsNot Nothing Then
                    e.Row.Cells(i).Text = bf.HeaderText
                End If
            Next
        End If
    End Sub

    Protected Sub BindFabricChart(searchText As String)
        MessageError_FabricChart(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", searchText),
                New SqlParameter("@CompanyDetailId", "2")
            }
            gvListFabricChart.DataSource = stockClass.GetDataTableSP("sp_Stock_Fabric_Chart", paramsItem)
            gvListFabricChart.DataBind()
        Catch ex As Exception
            MessageError_FabricChart(True, ex.ToString())
        End Try
    End Sub

    Protected Sub MessageError_FabricChart(visible As Boolean, message As String)
        divErrorFabricChart.Visible = visible : msgErrorFabricChart.InnerText = message
    End Sub

    Protected Function GetStatusClass(status As Object) As String
        If status Is Nothing OrElse status Is DBNull.Value Then
            Return "badge bg-secondary"
        End If

        Select Case status.ToString().Trim()
            Case "In Stock"
                Return "badge bg-success"
            Case "Out of Stock"
                Return "badge bg-danger"
            Case "Limited Stock"
                Return "badge bg-warning text-dark"
            Case "Discontinued"
                Return "badge bg-secondary"
            Case Else
                Return "badge bg-secondary"
        End Select
    End Function
End Class
