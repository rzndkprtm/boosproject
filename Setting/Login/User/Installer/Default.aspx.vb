Imports System.Data.SqlClient

Partial Class Setting_Login_User_Installer_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Dim pageAccess As Boolean = LoginAccess("Load")
        'If pageAccess = False Then
        '    Response.Redirect("~/setting/login/user", False)
        '    Exit Sub
        'End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/login/user/installer/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        Try
            If e.CommandName = "Page" Then
                gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
                BindData(txtSearch.Text)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        Try
            BuildPager()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try

        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        'Try
        '    Dim params As New List(Of SqlParameter) From {
        '        New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), "", searchText.Trim())),
        '        New SqlParameter("@RoleName", Session("RoleName").ToString()),
        '        New SqlParameter("@LevelName", Session("LevelName").ToString()),
        '        New SqlParameter("@LoginId", Session("LoginId").ToString())
        '    }
        '    gvList.DataSource = settingClass.GetDataTableSP("sp_LoginList", params)
        '    gvList.DataBind()
        'Catch ex As Exception
        '    MessageError(True, ex.ToString())
        '    If Not Session("RoleName") = "Developer" Then
        '        MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
        '    End If
        'End Try
    End Sub

    Protected Sub BuildPager()
        Try
            If gvList.PageCount <= 1 Then
                navPager.Visible = False
                Return
            End If

            navPager.Visible = True

            Dim currentPage As Integer = gvList.PageIndex
            Dim totalPages As Integer = gvList.PageCount

            Dim pages As New List(Of Object)

            If currentPage > 0 Then
                pages.Add(New With {
                    .Text = "Previous",
                    .PageIndex = currentPage - 1,
                    .CssClass = ""
                })
            End If

            Dim startPage As Integer = Math.Max(0, currentPage - 2)
            Dim endPage As Integer = Math.Min(totalPages - 1, currentPage + 2)

            For i As Integer = startPage To endPage
                pages.Add(New With {
                    .Text = (i + 1).ToString(),
                    .PageIndex = i,
                    .CssClass = If(i = currentPage, "active", "")
                })
            Next

            If currentPage < totalPages - 1 Then
                pages.Add(New With {
                    .Text = "Next",
                    .PageIndex = currentPage + 1,
                    .CssClass = ""
                })
            End If

            rptPager.DataSource = pages
            rptPager.DataBind()
        Catch ex As Exception
            navPager.Visible = False
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
