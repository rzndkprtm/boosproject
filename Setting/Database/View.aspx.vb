Imports System.Data.SqlClient

Partial Class Setting_Database_View
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/database", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchView")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchView") = txtSearch.Text

        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add View"
            txtName.Enabled = True

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim viewName As String = e.CommandArgument.ToString()

            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Session("SearchView") = txtSearch.Text
                Try
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit View"

                    txtName.Text = viewName

                    Dim query As String = "SELECT definition FROM sys.sql_modules WHERE object_id = OBJECT_ID('" & viewName.Replace("'", "''") & "')"

                    Dim result As String = settingClass.GetItemData(query)

                    Dim upperResult As String = result.ToUpper()

                    Dim pos As Integer = upperResult.IndexOf(" AS")

                    If pos >= 0 Then
                        result = result.Substring(pos + 3)
                    End If

                    txtQuery.Text = result.Trim()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            Dim viewName As String = txtName.Text.Trim()
            Dim selectQuery As String = txtQuery.Text.Trim()

            If msgErrorProcess.InnerText = "" Then
                Dim thisQuery As String = "IF EXISTS ( SELECT * FROM sys.views WHERE name = '" & viewName.Replace("'", "''") & "') BEGIN EXEC(' ALTER VIEW [" & viewName.Replace("'", "''") & "] AS " & selectQuery.Replace("'", "''") & " ') END ELSE BEGIN EXEC(' CREATE VIEW [" & viewName.Replace("'", "''") & "] AS " & selectQuery.Replace("'", "''") & " ') END"

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand(thisQuery, thisConn)
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Session("SearchView") = txtSearch.Text
                Response.Redirect("~/setting/database/view", False)
                Exit Sub
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchView") = String.Empty
        Try
            Dim search As String = String.Empty
            If Not searchText = "" Then
                search = "WHERE v.name LIKE '%" & searchText & "%'"
            End If
            Dim thisString As String = String.Format("SELECT v.name AS ViewName, s.name AS SchemaName, v.create_date, v.modify_date FROM sys.views v INNER JOIN sys.schemas s ON v.schema_id = s.schema_id {0} ORDER BY v.name ASC", search)

            gvList.DataSource = settingClass.GetDataTable(thisString)
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
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
