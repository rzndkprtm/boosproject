Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class Journal_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)

            BindSales()
            BindData()
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/journal/add", False)
    End Sub

    Protected Sub ddlSales_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSales.SelectedIndexChanged
        gvList.PageIndex = 0
        MessageError(False, String.Empty)
        BindData()
    End Sub

    Protected Sub txtDate_TextChanged(sender As Object, e As EventArgs) Handles txtDate.TextChanged
        gvList.PageIndex = 0
        MessageError(False, String.Empty)
        BindData()
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex
        MessageError(False, String.Empty)
        BindData()
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Page" Then
            gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
            BindData()
        End If
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        BuildPager()
    End Sub

    Protected Sub btnJournalReminder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim mailingClass As New MailingClass

            Dim salesData As DataTable = settingClass.GetDataTable("SELECT Id FROM Logins WHERE RoleId=4 AND LevelId=2")
            For i As Integer = 0 To salesData.Rows.Count - 1
                Dim loginId As String = salesData.Rows(i)("Id").ToString()
                mailingClass.JournalReminder(loginId)
            Next
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnJournalReport_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim mailingClass As New MailingClass
            mailingClass.JournalReport()
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim journalId As String = txtDeleteId.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("UPDATE Journals SET Status='Deleted' WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.Add("@Id", SqlDbType.Int).Value = CInt(journalId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim dataLog As Object() = {"Journals", journalId, Session("LoginId").ToString(), "Journal Deleted"}
            settingClass.Logs(dataLog)

            Response.Redirect("~/journal")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData()
        Try
            Dim params As New List(Of SqlParameter)

            Dim createdBy As Integer
            If Integer.TryParse(ddlSales.SelectedValue, createdBy) Then
                params.Add(New SqlParameter("@CreatedBy", SqlDbType.Int) With {.Value = createdBy})
            Else
                params.Add(New SqlParameter("@CreatedBy", SqlDbType.Int) With {.Value = DBNull.Value})
            End If

            Dim selectedDate As DateTime
            If DateTime.TryParseExact(txtDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, selectedDate) Then
                params.Add(New SqlParameter("@CreatedDate", SqlDbType.Date) With {.Value = selectedDate.Date})
            Else
                params.Add(New SqlParameter("@CreatedDate", SqlDbType.Date) With {.Value = DBNull.Value})
            End If

            gvList.DataSource = settingClass.GetDataTableSP("sp_Journals_List", params)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID")

            btnAdd.Visible = LoginAccess("Add")
            btnMail.Visible = LoginAccess("Mailing")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindSales()
        ddlSales.Items.Clear()
        Try
            ddlSales.DataSource = settingClass.GetDataTable("SELECT Id, FullName FROM Logins WHERE RoleId=4 ORDER BY Id ASC")
            ddlSales.DataTextField = "FullName"
            ddlSales.DataValueField = "Id"
            ddlSales.DataBind()

            If ddlSales.Items.Count > 0 Then
                ddlSales.Items.Insert(0, New ListItem("", ""))
            End If

            ddlSales.Enabled = True
            If Session("RoleName") = "Sales" AndAlso Session("LevelName") = "Member" Then
                ddlSales.SelectedValue = Session("LoginId").ToString()
                ddlSales.Enabled = False
            End If
        Catch ex As Exception
            ddlSales.Items.Clear()
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
        divError.Visible = visible
        msgError.InnerText = message
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