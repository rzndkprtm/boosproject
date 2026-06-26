Imports System.Data.SqlClient

Partial Class JPMDStock
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
                New SqlParameter("@CompanyId", "2"),
                New SqlParameter("@Search", searchText)
            }
            gvListRoller.DataSource = stockClass.GetDataTableSP("sp_GetStockFabric", paramsItem)
            gvListRoller.DataBind()
        Catch ex As Exception
            MessageError_Roller(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSearchRoller_Click(sender As Object, e As EventArgs)
        BindRoller(txtSearchRoller.Text)
    End Sub

    Protected Sub gvListRoller_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 1 To 8
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
            For i As Integer = 0 To gvListRoller.Columns.Count - 1
                Dim bf As BoundField = TryCast(gvListRoller.Columns(i), BoundField)
                If bf IsNot Nothing Then
                    e.Row.Cells(i).Text = bf.HeaderText
                End If
            Next
        End If
    End Sub

    Protected Sub MessageError_Roller(visible As Boolean, message As String)
        divErrorRoller.Visible = visible : msgErrorRoller.InnerText = message
    End Sub


    ' DESIGN SHADES

    Protected Sub BindDesignShades()
        MessageError_Profile(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "4"),
                New SqlParameter("@CompanyId", "2"),
                New SqlParameter("@Search", String.Empty)
            }
            gvListProfile.DataSource = stockClass.GetDataTableSP("sp_GetStockFabric", paramsItem)
            gvListProfile.DataBind()
        Catch ex As Exception
            MessageError_Profile(True, ex.ToString())
        End Try
    End Sub

    Protected Sub MessageError_Profile(visible As Boolean, message As String)
        divErrorProfile.Visible = visible : msgErrorProfile.InnerText = message
    End Sub

    Protected Sub gvListProfile_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 1 To 8
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
            For i As Integer = 0 To gvListProfile.Columns.Count - 1
                Dim bf As BoundField = TryCast(gvListProfile.Columns(i), BoundField)
                If bf IsNot Nothing Then
                    e.Row.Cells(i).Text = bf.HeaderText
                End If
            Next
        End If
    End Sub

    'CURTAIN

    Protected Sub BindCurtain(searchText As String)
        MessageError_Curtain(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "3"),
                New SqlParameter("@CompanyId", "2"),
                New SqlParameter("@Search", searchText)
            }
            gvListCurtain.DataSource = stockClass.GetDataTableSP("sp_GetStockFabric", paramsItem)
            gvListCurtain.DataBind()
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

    Protected Sub gvListCurtain_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 1 To 8
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
            For i As Integer = 0 To gvListCurtain.Columns.Count - 1
                Dim bf As BoundField = TryCast(gvListCurtain.Columns(i), BoundField)
                If bf IsNot Nothing Then
                    e.Row.Cells(i).Text = bf.HeaderText
                End If
            Next
        End If
    End Sub


    ' VERTICAL

    Protected Sub BindVertical(searchText As String)
        MessageError_Vertical(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@CompanyId", "2"),
                New SqlParameter("@Search", searchText)
            }
            gvListVertical.DataSource = stockClass.GetDataTableSP("sp_GetStockFabricVertical", paramsItem)
            gvListVertical.DataBind()
        Catch ex As Exception
            MessageError_Vertical(True, ex.ToString())
        End Try
    End Sub

    Protected Sub MessageError_Vertical(visible As Boolean, message As String)
        divErrorVertical.Visible = visible : msgErrorVertical.InnerText = message
    End Sub

    Protected Sub btnSearchVertical_Click(sender As Object, e As EventArgs)
        BindVertical(txtSearchVertical.Text)
    End Sub

    Protected Sub gvListVertical_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 1 To 8
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
            For i As Integer = 0 To gvListVertical.Columns.Count - 1
                Dim bf As BoundField = TryCast(gvListVertical.Columns(i), BoundField)
                If bf IsNot Nothing Then
                    e.Row.Cells(i).Text = bf.HeaderText
                End If
            Next
        End If
    End Sub


    ' VENETIAN

    Protected Sub BindVenetian()
        MessageError_Venetian(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "10"),
                New SqlParameter("@CompanyDetailId", "2")
            }
            gvListVenetian.DataSource = stockClass.GetDataTableSP("sp_GetStockBlindColour", paramsItem)
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
            gvListAluminium.DataSource = stockClass.GetDataTableSP("sp_GetStockBlindColour", paramsItem)
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

    ' CELLULAR SHADES

    Protected Sub BindCellularShades()
        MessageError_Cellular(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "2"),
                New SqlParameter("@CompanyId", "2")
            }
            gvListCellular.DataSource = stockClass.GetDataTableSP("sp_GetStockFabric", paramsItem)
            gvListCellular.DataBind()
        Catch ex As Exception
            MessageError_Cellular(True, ex.ToString())
        End Try
    End Sub

    Protected Sub MessageError_Cellular(visible As Boolean, message As String)
        divErrorCellular.Visible = visible : msgErrorCellular.InnerText = message
    End Sub

    Protected Sub gvListCellular_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 1 To 8
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
            For i As Integer = 0 To gvListCellular.Columns.Count - 1
                Dim bf As BoundField = TryCast(gvListCellular.Columns(i), BoundField)
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
            gvListFabricChart.DataSource = stockClass.GetDataTableSP("sp_GetStockFabricAvailability", paramsItem)
            gvListFabricChart.DataBind()
        Catch ex As Exception
            MessageError_FabricChart(True, ex.ToString())
        End Try
    End Sub

    Protected Sub MessageError_FabricChart(visible As Boolean, message As String)
        divErrorFabricChart.Visible = visible : msgErrorFabricChart.InnerText = message
    End Sub
End Class