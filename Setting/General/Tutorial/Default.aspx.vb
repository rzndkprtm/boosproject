Imports System.Data.SqlClient

Partial Class Setting_General_Tutorial_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchTutorial")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("SearchTutorial") = txtSearch.Text
        Response.Redirect("~/setting/general/tutorial/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)

        Session("SearchTutorial") = txtSearch.Text
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

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        BuildPager()
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Page" Then
            gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("UPDATE Tutorials SET Status='Deleted', Title=CASE WHEN Title LIKE '%(DELETED)%' THEN Title ELSE Title + ' (DELETED)' END WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim dataLog As Object() = {"Tutorials", thisId, Session("LoginId").ToString(), "Tutorial Created"}
            settingClass.Logs(dataLog)

            Session("SearchTutorial") = txtSearch.Text
            Response.Redirect("~/setting/general/tutorial", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchTutorial") = String.Empty
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrWhiteSpace(searchText), CType(DBNull.Value, Object), searchText)),
                New SqlParameter("@CompanyId", If(Session("CompanyId") Is Nothing OrElse String.IsNullOrWhiteSpace(Session("CompanyId").ToString()), CType(DBNull.Value, Object), Convert.ToInt32(Session("CompanyId")))),
                New SqlParameter("@RoleName", Session("RoleName").ToString())
            }

            gvList.DataSource = settingClass.GetDataTableSP("sp_Tutorials_List", params)
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

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
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

    Protected Function GetFileName(fileUrl As Object) As String
        Try
            If fileUrl Is Nothing OrElse IsDBNull(fileUrl) Then Return String.Empty

            Dim filePath As String = Convert.ToString(fileUrl).Trim()

            If String.IsNullOrWhiteSpace(filePath) Then Return String.Empty

            filePath = filePath.Split("?"c)(0)
            filePath = filePath.Split("#"c)(0)
            filePath = filePath.Replace("\", "/")

            Return Uri.UnescapeDataString(filePath.Substring(filePath.LastIndexOf("/") + 1))
        Catch
            Return String.Empty
        End Try
    End Function

    Protected Function HasFile(fileUrl As Object) As Boolean
        Try
            If fileUrl Is Nothing OrElse IsDBNull(fileUrl) Then Return False
            Return Not String.IsNullOrWhiteSpace(Convert.ToString(fileUrl).Trim())
        Catch
            Return False
        End Try
    End Function

    Protected Function GetFileUrl(fileUrl As Object) As String
        Try
            If Not HasFile(fileUrl) Then Return String.Empty

            Dim filePath As String = Convert.ToString(fileUrl).Trim()

            If filePath.StartsWith("~/") Then
                Return ResolveUrl(filePath)
            End If

            Return filePath
        Catch
            Return String.Empty
        End Try
    End Function

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
