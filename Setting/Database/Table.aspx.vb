Imports System.Data.SqlClient

Partial Class Setting_Database_Table
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
            txtSearch.Text = Session("SearchTable")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchTable") = txtSearch.Text

        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Table"
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

    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If msgErrorProcess.InnerText = "" Then
                If lblAction.Text = "Add" Then
                    Dim thisQuery As String = "CREATE TABLE [" & txtName.Text.Trim() & "] ( " & txtQuery.Text.Trim() & " )"

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As New SqlCommand(thisQuery, thisConn)
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Session("SearchTable") = txtSearch.Text
                    Response.Redirect("~/setting/database/table", False)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchTable") = String.Empty
        Try
            Dim search As String = String.Empty
            If Not searchText = "" Then
                search = "AND t.TABLE_NAME LIKE '%" & searchText & "%'"
            End If
            Dim thisString As String = String.Format("SELECT t.TABLE_NAME AS TableName, p.rows AS TotalRow FROM INFORMATION_SCHEMA.TABLES t INNER JOIN sys.tables s ON t.TABLE_NAME=s.name INNER JOIN sys.partitions p ON s.object_id=p.object_id WHERE t.TABLE_TYPE='BASE TABLE' AND p.index_id IN (0,1) {0} ORDER BY t.TABLE_NAME", search)

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
