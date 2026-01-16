Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_General_Tutorial
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Dim settingClass As New SettingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
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
        MessageError_Process(False, String.Empty)
        Session("SearchTutorial") = txtSearch.Text

        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Tutorial"

            BindCompany()

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
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
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("SearchTutorial") = txtSearch.Text

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Tutorial"

                    Dim myData As DataSet = settingClass.GetListData("SELECT * FROM Tutorials WHERE Id='" & lblId.Text & "'")

                    BindCompany()

                    ddlCompanyId.SelectedValue = myData.Tables(0).Rows(0).Item("CompanyId").ToString()
                    txtTitle.Text = myData.Tables(0).Rows(0).Item("Title").ToString()
                    txtDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
                    ddlActive.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Catch ex As Exception
                    MessageError_Process(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLog.DataSource = settingClass.GetListData("SELECT * FROM Logs WHERE DataId='" & dataId & "' AND Type='Tutorials' ORDER BY ActionDate DESC")
                    gvListLog.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_Log(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If ddlCompanyId.SelectedValue = "" Then
                MessageError_Process(True, "COMPANY IS REQUIRED  !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Exit Sub
            End If
            If txtTitle.Text = "" Then
                MessageError_Process(True, "TITLE TUTORIAL IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Tutorials ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Tutorials VALUES (@Id, @CompanyId, @Title, @Description, @File, @Video, @Active)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@CompanyId", ddlCompanyId.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Description", txtDescription.Text)
                            myCmd.Parameters.AddWithValue("@File", txtFile.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Video", txtVideo.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"Tutorials", thisId, Session("LoginId").ToString(), "Tutorial Created"}
                    settingClass.Logs(dataLog)

                    Session("SearchTutorial") = txtSearch.Text
                    Response.Redirect("~/setting/general/tutorial", False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE Tutorials SET CompanyId=@CompanyId, Title=@Title, Description=@Description, File=@File, Video=@Video, Active=@Active WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@CompanyId", ddlCompanyId.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Description", txtDescription.Text)
                            myCmd.Parameters.AddWithValue("@File", txtFile.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Video", txtVideo.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"Tutorials", lblId.Text, Session("LoginId").ToString(), "Tutorial Updated"}
                    settingClass.Logs(dataLog)

                    Session("SearchTutorial") = txtSearch.Text
                    Response.Redirect("~/setting/general/tutorial", False)
                End If
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdDelete.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Tutorials WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='Tutorials' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

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
            Dim search As String = String.Empty
            If Not searchText = "" Then
                search = "WHERE Tutorials.Title LIKE '%" & searchText.Trim() & "%' OR Companys.Name LIKE '%" & searchText.Trim() & "%'"
            End If

            Dim thisQuery As String = String.Format("SELECT Tutorials.*, Companys.Name AS CompanyName, CASE WHEN Tutorials.Active=1 THEN 'Yes' WHEN Tutorials.Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Tutorials LEFT JOIN Companys ON Tutorials.CompanyId=Companys.Id {0} ORDER BY Companys.Id, Tutorials.Title ASC", search)

            gvList.DataSource = settingClass.GetListData(thisQuery)
            gvList.DataBind()

            gvList.Columns(1).Visible = PageAction("Visible ID")
            btnAdd.Visible = PageAction("Add")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompany()
        ddlCompanyId.Items.Clear()
        Try
            ddlCompanyId.DataSource = settingClass.GetListData("SELECT * FROM Companys ORDER BY Name ASC")
            ddlCompanyId.DataTextField = "Name"
            ddlCompanyId.DataValueField = "Id"
            ddlCompanyId.DataBind()

            If ddlCompanyId.Items.Count > 1 Then
                ddlCompanyId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
    End Sub

    Protected Sub MessageError_Log(visible As Boolean, message As String)
        divErrorLog.Visible = visible : msgErrorLog.InnerText = message
    End Sub

    Protected Function BindTextLog(logId As String) As String
        Return SettingClass.GetTextLog(logId)
    End Function

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
