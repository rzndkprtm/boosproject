Imports System.Data.SqlClient

Partial Class Stocks
    Inherits Page

    Dim stockClass As New StockClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            BindRoller(txtSearchRoller.Text)
            BindDesignShades()
            BindCurtain(txtSearchCurtain.Text)
        End If
    End Sub

    Protected Sub btnSearchRoller_Click(sender As Object, e As EventArgs)
        BindRoller(txtSearchRoller.Text)
    End Sub

    Protected Sub BindRoller(searchText As String)
        MessageError_Roller(False, String.Empty)
        Try
            Dim companyDetailId As String = String.Empty
            If Session("RoleName") = "Customer" OrElse Session("RoleName") = "Sales" OrElse Session("RoleName") = "Customer Service" Then
                companyDetailId = Session("CompanyDetailId")
            End If
            Dim paramsItem As New List(Of SqlParameter) From {
                    New SqlParameter("@DesignId", "12"),
                    New SqlParameter("@CompanyId", companyDetailId),
                    New SqlParameter("@Search", searchText)
                }
            gvListRoller.DataSource = stockClass.GetDataTableSP("sp_GetFabricColoursPivot", paramsItem)
            gvListRoller.DataBind()
        Catch ex As Exception
            MessageError_Roller(True, ex.ToString())
        End Try
    End Sub

    Protected Sub gvListRoller_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 1 To 8
                Dim colName As String = "Col" & i & "Active"
                Dim isActiveObj = DataBinder.Eval(e.Row.DataItem, colName)

                If isActiveObj IsNot Nothing AndAlso Not IsDBNull(isActiveObj) Then
                    Dim isActive As Integer = Convert.ToInt32(isActiveObj)

                    If isActive = 0 Then
                        e.Row.Cells(i).BackColor = Drawing.Color.LightCoral
                        e.Row.Cells(i).ForeColor = Drawing.Color.White
                    End If
                End If
            Next
        End If
    End Sub

    Protected Sub MessageError_Roller(visible As Boolean, message As String)
        divErrorRoller.Visible = visible : msgErrorRoller.InnerText = message
    End Sub

    Protected Sub BindDesignShades()
        MessageError_Profile(False, String.Empty)
        Try
            Dim companyDetailId As String = String.Empty
            If Session("RoleName") = "Customer" OrElse Session("RoleName") = "Sales" OrElse Session("RoleName") = "Customer Service" Then
                companyDetailId = Session("CompanyDetailId")
            End If
            Dim paramsItem As New List(Of SqlParameter) From {
                    New SqlParameter("@DesignId", "4"),
                    New SqlParameter("@CompanyId", companyDetailId),
                    New SqlParameter("@Search", String.Empty)
                }
            gvListProfile.DataSource = stockClass.GetDataTableSP("sp_GetFabricColoursPivot", paramsItem)
            gvListProfile.DataBind()
        Catch ex As Exception
            MessageError_Profile(True, ex.ToString())
        End Try
    End Sub

    Protected Sub gvListProfile_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 1 To 8
                Dim colName As String = "Col" & i & "Active"
                Dim isActiveObj = DataBinder.Eval(e.Row.DataItem, colName)

                If isActiveObj IsNot Nothing AndAlso Not IsDBNull(isActiveObj) Then
                    Dim isActive As Integer = Convert.ToInt32(isActiveObj)

                    If isActive = 0 Then
                        e.Row.Cells(i).BackColor = Drawing.Color.LightCoral
                        e.Row.Cells(i).ForeColor = Drawing.Color.White
                    End If
                End If
            Next
        End If
    End Sub

    Protected Sub MessageError_Profile(visible As Boolean, message As String)
        divErrorProfile.Visible = visible : msgErrorProfile.InnerText = message
    End Sub

    'CURTAIN

    Protected Sub BindCurtain(searchText As String)
        MessageError_Curtain(False, String.Empty)
        Try
            Dim companyDetailId As String = String.Empty
            If Session("RoleName") = "Customer" OrElse Session("RoleName") = "Sales" OrElse Session("RoleName") = "Customer Service" Then
                companyDetailId = Session("CompanyDetailId")
            End If
            Dim paramsItem As New List(Of SqlParameter) From {
                    New SqlParameter("@DesignId", "3"),
                    New SqlParameter("@CompanyId", companyDetailId),
                    New SqlParameter("@Search", searchText)
                }
            gvListCurtain.DataSource = stockClass.GetDataTableSP("sp_GetFabricColoursPivot", paramsItem)
            gvListCurtain.DataBind()
        Catch ex As Exception
            MessageError_Curtain(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSearchCurtain_Click(sender As Object, e As EventArgs)
        BindCurtain(txtSearchCurtain.Text)
    End Sub

    Protected Sub gvListCurtain_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 1 To 8
                Dim colName As String = "Col" & i & "Active"
                Dim isActiveObj = DataBinder.Eval(e.Row.DataItem, colName)

                If isActiveObj IsNot Nothing AndAlso Not IsDBNull(isActiveObj) Then
                    Dim isActive As Integer = Convert.ToInt32(isActiveObj)

                    If isActive = 0 Then
                        e.Row.Cells(i).BackColor = Drawing.Color.LightCoral
                        e.Row.Cells(i).ForeColor = Drawing.Color.White
                    End If
                End If
            Next
        End If
    End Sub

    Protected Sub MessageError_Curtain(visible As Boolean, message As String)
        divErrorCurtain.Visible = visible : msgErrorCurtain.InnerText = message
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
