Imports System.Data.SqlClient
Imports System.Web.Services

Partial Class Stocks
    Inherits Page

    Dim stockClass As New StockClass

    <WebMethod(EnableSession:=True)>
    Public Shared Sub UpdateSession(value As String)
        HttpContext.Current.Session("selectedTabStocks") = value
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not Session("selectedTabStocks") = "" Then
            selected_tab.Value = Session("selectedTabStocks").ToString()
        End If

        If Not IsPostBack Then
            BindCompanyDetail()

            BindRoller(txtSearchRoller.Text, ddlCompanyDetail.SelectedValue)
            BindDesignShades(ddlCompanyDetail.SelectedValue)
            BindCurtain(txtSearchCurtain.Text, ddlCompanyDetail.SelectedValue)
            BindVertical(txtSearchVertical.Text, ddlCompanyDetail.SelectedValue)
            BindVenetian(ddlCompanyDetail.SelectedValue)
            BindAluminium(ddlCompanyDetail.SelectedValue)
            BindCellularShades(ddlCompanyDetail.SelectedValue)

            divCompanyDetail.Visible = PageAction("Visible Company Detail")
        End If
    End Sub

    Protected Sub ddlCompanyDetail_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindRoller(txtSearchRoller.Text, ddlCompanyDetail.SelectedValue)
        BindDesignShades(ddlCompanyDetail.SelectedValue)
        BindCurtain(txtSearchCurtain.Text, ddlCompanyDetail.SelectedValue)
        BindVertical(txtSearchVertical.Text, ddlCompanyDetail.SelectedValue)
        BindVenetian(ddlCompanyDetail.SelectedValue)
        BindAluminium(ddlCompanyDetail.SelectedValue)
        BindCellularShades(ddlCompanyDetail.SelectedValue)
    End Sub

    Protected Sub BindCompanyDetail()
        ddlCompanyDetail.Items.Clear()
        Try
            ddlCompanyDetail.DataSource = stockClass.GetDataTable("SELECT * FROM CompanyDetails ORDER BY Name ASC")
            ddlCompanyDetail.DataTextField = "Name"
            ddlCompanyDetail.DataValueField = "Id"
            ddlCompanyDetail.DataBind()

            If ddlCompanyDetail.Items.Count > 0 Then
                ddlCompanyDetail.Items.Insert(0, New ListItem("", ""))
            End If

            ddlCompanyDetail.SelectedValue = Session("CompanyDetailId").ToString()
        Catch ex As Exception
            ddlCompanyDetail.Items.Clear()
            ddlCompanyDetail.Items.Add(New ListItem("All", ""))
        End Try
    End Sub

    ' ROLLER

    Protected Sub BindRoller(searchText As String, companyDetail As String)
        MessageError_Roller(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "12"),
                New SqlParameter("@CompanyId", companyDetail),
                New SqlParameter("@Search", searchText)
            }
            gvListRoller.DataSource = stockClass.GetDataTableSP("sp_GetStockFabric", paramsItem)
            gvListRoller.DataBind()
        Catch ex As Exception
            MessageError_Roller(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSearchRoller_Click(sender As Object, e As EventArgs)
        BindRoller(txtSearchRoller.Text, ddlCompanyDetail.SelectedValue)
    End Sub

    Protected Sub gvListRoller_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 1 To 8
                Dim colColour As String = "Col" & i
                Dim colStatus As String = "Col" & i & "Status"
                Dim colWidth As String = "Col" & i & "Width"

                Dim colourObj = DataBinder.Eval(e.Row.DataItem, colColour)
                Dim statusObj = DataBinder.Eval(e.Row.DataItem, colStatus)
                Dim widthObj = DataBinder.Eval(e.Row.DataItem, colWidth)

                Dim colour As String = If(colourObj Is Nothing OrElse IsDBNull(colourObj), "", colourObj.ToString())
                Dim status As String = If(statusObj Is Nothing OrElse IsDBNull(statusObj), "", statusObj.ToString())
                Dim width As String = If(widthObj Is Nothing OrElse IsDBNull(widthObj), "", widthObj.ToString())

                e.Row.Cells(i).Text = ""
                e.Row.Cells(i).Attributes.Clear()

                If colour <> "" Then
                    e.Row.Cells(i).Text =
                    colour & If(width <> "", "<br/>(Max " & width & "mm)", "")
                End If

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

    Protected Sub BindDesignShades(companyDetail As String)
        MessageError_Profile(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "4"),
                New SqlParameter("@CompanyId", companyDetail),
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
                Dim colColour As String = "Col" & i
                Dim colStatus As String = "Col" & i & "Status"
                Dim colWidth As String = "Col" & i & "Width"

                Dim colourObj = DataBinder.Eval(e.Row.DataItem, colColour)
                Dim statusObj = DataBinder.Eval(e.Row.DataItem, colStatus)
                Dim widthObj = DataBinder.Eval(e.Row.DataItem, colWidth)

                Dim colour As String = If(colourObj Is Nothing OrElse IsDBNull(colourObj), "", colourObj.ToString())
                Dim status As String = If(statusObj Is Nothing OrElse IsDBNull(statusObj), "", statusObj.ToString())
                Dim width As String = If(widthObj Is Nothing OrElse IsDBNull(widthObj), "", widthObj.ToString())

                e.Row.Cells(i).Text = ""
                e.Row.Cells(i).Attributes.Clear()

                If colour <> "" Then
                    e.Row.Cells(i).Text =
                    colour & If(width <> "", "<br/>(Max " & width & "mm)", "")
                End If

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
            For i As Integer = 0 To gvListProfile.Columns.Count - 1
                Dim bf As BoundField = TryCast(gvListProfile.Columns(i), BoundField)
                If bf IsNot Nothing Then
                    e.Row.Cells(i).Text = bf.HeaderText
                End If
            Next
        End If
    End Sub


    'CURTAIN

    Protected Sub BindCurtain(searchText As String, companyDetail As String)
        MessageError_Curtain(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "3"),
                New SqlParameter("@CompanyId", companyDetail),
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
        BindCurtain(txtSearchCurtain.Text, ddlCompanyDetail.SelectedValue)
    End Sub

    Protected Sub gvListCurtain_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 1 To 8
                Dim colColour As String = "Col" & i
                Dim colStatus As String = "Col" & i & "Status"
                Dim colWidth As String = "Col" & i & "Width"

                Dim colourObj = DataBinder.Eval(e.Row.DataItem, colColour)
                Dim statusObj = DataBinder.Eval(e.Row.DataItem, colStatus)
                Dim widthObj = DataBinder.Eval(e.Row.DataItem, colWidth)

                Dim colour As String = If(colourObj Is Nothing OrElse IsDBNull(colourObj), "", colourObj.ToString())
                Dim status As String = If(statusObj Is Nothing OrElse IsDBNull(statusObj), "", statusObj.ToString())
                Dim width As String = If(widthObj Is Nothing OrElse IsDBNull(widthObj), "", widthObj.ToString())

                e.Row.Cells(i).Text = ""
                e.Row.Cells(i).Attributes.Clear()

                If colour <> "" Then
                    e.Row.Cells(i).Text =
                    colour & If(width <> "", "<br/>(Max " & width & "mm)", "")
                End If

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
            For i As Integer = 0 To gvListCurtain.Columns.Count - 1
                Dim bf As BoundField = TryCast(gvListCurtain.Columns(i), BoundField)
                If bf IsNot Nothing Then
                    e.Row.Cells(i).Text = bf.HeaderText
                End If
            Next
        End If
    End Sub


    ' VERTICAL

    Protected Sub BindVertical(searchText As String, companyDetail As String)
        MessageError_Vertical(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@CompanyId", companyDetail),
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
        BindVertical(txtSearchVertical.Text, ddlCompanyDetail.SelectedValue)
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

    Protected Sub BindVenetian(companyDetail As String)
        MessageError_Venetian(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "10"),
                New SqlParameter("@CompanyDetailId", companyDetail)
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


    ' ALUMINIUM
    Protected Sub BindAluminium(companyDetail As String)
        MessageError_Aluminium(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "1"),
                New SqlParameter("@CompanyDetailId", companyDetail)
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
            For i As Integer = 0 To gvListAluminium.Columns.Count - 1
                Dim bf As BoundField = TryCast(gvListAluminium.Columns(i), BoundField)
                If bf IsNot Nothing Then
                    e.Row.Cells(i).Text = bf.HeaderText
                End If
            Next
        End If
    End Sub

    ' CELLULAR SHADES

    Protected Sub BindCellularShades(companyDetail As String)
        MessageError_Cellular(False, String.Empty)
        Try
            Dim paramsItem As New List(Of SqlParameter) From {
                New SqlParameter("@DesignId", "2"),
                New SqlParameter("@CompanyId", companyDetail)
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

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
