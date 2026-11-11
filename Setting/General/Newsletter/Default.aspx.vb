Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_General_Newsletter_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object()

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchNewsletter")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("SearchNewsletter") = txtSearch.Text
        Response.Redirect("~/setting/general/newsletter/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)

        Session("SearchNewsletter") = txtSearch.Text
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

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("UPDATE Newsletters SET Status='Deleted', Name=CASE WHEN Name LIKE '%(DELETED)%' THEN Name ELSE Name + ' (DELETED)' END WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.Add("@Id", SqlDbType.Int).Value = CInt(thisId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"Newsletters", thisId, Session("LoginId").ToString(), "Newsletter Deleted"}
            settingClass.Logs(dataLog)

            Session("SearchNewsletter") = txtSearch.Text
            Response.Redirect("~/setting/general/newsletter", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnStatus_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdStatus.Text
            Dim thisStatus As String = txtStatus.Text
            Dim thisCompany As String = txtCompany.Text

            Dim newStatus As String = String.Empty
            If thisStatus = "Active" Then newStatus = "Inactive"
            If thisStatus = "Inactive" Then newStatus = "Active"


            Dim thisString As String = "UPDATE Newsletters SET Status=@Status WHERE Id=@Id"
            If newStatus = "Active" Then
                thisString = "UPDATE Newsletters SET Status='Inactive' WHERE CompanyId=@CompanyId; UPDATE Newsletters SET Status=@Status WHERE Id=@Id;"
            End If

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand(thisString, thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisCmd.Parameters.AddWithValue("@CompanyId", thisCompany)
                    thisCmd.Parameters.AddWithValue("@Status", newStatus)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim activeDesc As String = String.Empty
            If thisStatus = "Active" Then activeDesc = "Newsletter Has Been Activated"
            If thisStatus = "Inactive" Then activeDesc = "Newsletter Has Been Deactivated"

            dataLog = {"Newsletters", thisId, Session("LoginId").ToString(), activeDesc}
            settingClass.Logs(dataLog)

            Session("SearchNewsletter") = txtSearch.Text
            Response.Redirect("~/setting/general/newsletter", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchNewsletter") = String.Empty
        Try
            Dim search As String = String.Empty
            If Not searchText = "" Then
                search = "WHERE Newsletters.Id LIKE '%" & searchText.Trim() & "%' OR Newsletters.Name LIKE '%" & searchText.Trim() & "%' OR Newsletters.Description LIKE '%" & searchText.Trim() & "%'"
            End If
            Dim thisString As String = String.Format("SELECT Newsletters.*, Companys.Alias AS CompanyAlias FROM Newsletters LEFT JOIN Companys ON Newsletters.CompanyId=Companys.Id {0} ORDER BY Newsletters.Id DESC", search)

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

    Protected Function TextStatus(active As String) As String
        If active = "Active" Then Return "Deactivate"
        Return "Activate"
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
