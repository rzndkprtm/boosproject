Imports System.Data.SqlClient

Partial Class Setting_Database_Function
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/database", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchFunction")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchFunction") = txtSearch.Text

        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Function"
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
            Dim spName As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Session("SearchFunction") = txtSearch.Text
                Try
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Procedure"

                    txtName.Text = spName

                    Dim query As String = "SELECT OBJECT_DEFINITION(OBJECT_ID('" & spName.Replace("'", "''") & "'))"

                    Dim spText As String = settingClass.GetItemData(query)
                    If Not String.IsNullOrEmpty(spText) Then
                        spText = Regex.Replace(spText, "\bCREATE\s+PROCEDURE\b", "ALTER PROCEDURE", RegexOptions.IgnoreCase)
                        spText = Regex.Replace(spText, "\bCREATE\s+PROC\b", "ALTER PROCEDURE", RegexOptions.IgnoreCase)
                        txtQuery.Text = spText
                    End If

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
            If msgErrorProcess.InnerText = "" Then
                If lblAction.Text = "Add" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As New SqlCommand(txtQuery.Text, thisConn)
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Session("SearchFunction") = txtSearch.Text
                    Response.Redirect("~/setting/database/function", False)
                    Exit Sub
                End If
                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As New SqlCommand(txtQuery.Text, thisConn)
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Session("SearchFunction") = txtSearch.Text
                    Response.Redirect("~/setting/database/function", False)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim spName As String = txtFunctionDelete.Text
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DROP PROCEDURE " & spName & "", thisConn)
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Session("SearchFunction") = txtSearch.Text
            Response.Redirect("~/setting/database/function", False)
            Exit Sub
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchFunction") = String.Empty
        Try
            Dim search As String = String.Empty
            If Not searchText = "" Then
                search = "WHERE name LIKE '%" & searchText & "%'"
            End If
            Dim thisString As String = String.Format("SELECT * FROM sys.procedures {0} ORDER BY name ASC", search)

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
