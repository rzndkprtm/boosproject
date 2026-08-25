Imports System.Data.SqlClient

Partial Class Setting_Specification_Remote_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchRemote")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("SearchRemote") = txtSearch.Text
        Response.Redirect("~/setting/specification/remote/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)

        Session("SearchRemote") = txtSearch.Text
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Page" Then
            gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        BuildPager()
    End Sub

    Protected Sub btnChangeStatus_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdStatus.Text
            Dim newStatus As String = ddlNewStatus.SelectedValue
            Dim oldStatus As String = txtOldStatus.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE Chains SET Status=@Status WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisCmd.Parameters.AddWithValue("@Status", newStatus)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim changeDesc As String = String.Format("Change Status Remote : {0}", newStatus)

            dataLog = {"Chains", thisId, Session("LoginId").ToString(), changeDesc}
            settingClass.Logs(dataLog)

            Session("SearchRemote") = txtSearch.Text
            Response.Redirect("~/setting/specification/remote", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Try
            Dim search As String = String.Empty
            If Not searchText = "" Then
                search = "AND (Id LIKE '%" & searchText & "%' OR BoeId LIKE '%" & searchText & "%' OR Name LIKE '%" & searchText & "%' OR Description LIKE '%" & searchText & "%' OR Status LIKE '%" & searchText & "%')"
            End If
            Dim thisString As String = String.Format("SELECT * FROM Chains WHERE ControlTypeId<>'1' {0} ORDER BY ControlTypeId, Name ASC", search)

            gvList.DataSource = settingClass.GetDataTable(thisString)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID")

            btnAdd.Visible = LoginAccess("Add")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
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
                pages.Add(New With {.Text = "Previous", .PageIndex = currentPage - 1, .CssClass = ""})
            End If

            Dim startPage As Integer = Math.Max(0, currentPage - 2)
            Dim endPage As Integer = Math.Min(totalPages - 1, currentPage + 2)

            For i As Integer = startPage To endPage
                pages.Add(New With {.Text = (i + 1).ToString(), .PageIndex = i, .CssClass = If(i = currentPage, "active", "")})
            Next

            If currentPage < totalPages - 1 Then
                pages.Add(New With {.Text = "Next", .PageIndex = currentPage + 1, .CssClass = ""})
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
