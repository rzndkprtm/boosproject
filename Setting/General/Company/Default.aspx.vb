Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_General_Company_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchCompany")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchCompany") = txtSearch.Text

        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Company"

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
            Session("SearchCompany") = txtSearch.Text

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                url = String.Format("~/setting/general/company/detail?boosid={0}", dataId)
                Response.Redirect(url, False)
            ElseIf e.CommandName = "Ubah" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Company"

                    lblId.Text = dataId

                    Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM Companys WHERE Id='" & lblId.Text & "'")
                    If thisData Is Nothing Then Exit Sub

                    txtName.Text = thisData("Name").ToString()
                    txtAlias.Text = thisData("Alias").ToString()
                    txtDescription.Text = thisData("Description").ToString()
                    ddlActive.SelectedValue = Convert.ToInt32(thisData("Active"))

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
                    gvListLog.DataSource = settingClass.GetDataTable("SELECT * FROM Logs WHERE DataId='" & dataId & "' AND Type='Companys' ORDER BY ActionDate DESC")
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
            If txtName.Text = "" Then
                MessageError_Process(True, "COMPANY NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If txtAlias.Text = "" Then
                MessageError_Process(True, "COMPANY ALIAS IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Companys ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Companys VALUES (@Id, @Name, @Alias, NULL, @Description, @Active)", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Alias", txtAlias.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"Companys", thisId, Session("LoginId").ToString(), "Created"}
                    settingClass.Logs(dataLog)

                    Session("SearchCompany") = txtSearch.Text

                    url = String.Format("~/setting/general/company/detail?companyid={0}", thisId)
                    Response.Redirect(url, False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE Companys SET Name=@Name, Alias=@Alias, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Alias", txtAlias.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Description", descText)
                            myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"Companys", lblId.Text, Session("LoginId").ToString(), "Updated"}
                    settingClass.Logs(dataLog)

                    Session("SearchCompany") = txtSearch.Text
                    Response.Redirect("~/setting/general/company", False)
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
            'Dim thisId As String = txtIdDelete.Text

            'Using thisConn As New SqlConnection(myConn)
            '    Using myCmd As SqlCommand = New SqlCommand("UPDATE Companys SET Active=0 WHERE Id=@Id; UPDATE Customers SET CompanyId=NULL WHERE CompanyId=@Id; UPDATE Mailings SET CompanyId=NULL WHERE CompanyId=@Id; UPDATE Newsletters SET CompanyId=NULL WHERE CompanyId=@Id; UPDATE Tutorials SET CompanyId=NULL WHERE CompanyId=@Id; UPDATE PriceGroups SET CompanyId=NULL WHERE CompanyId=@Id;", thisConn)
            '        myCmd.Parameters.AddWithValue("@Id", thisId)

            '        thisConn.Open()
            '        myCmd.ExecuteNonQuery()
            '    End Using
            'End Using

            'dataLog = {"Companys", lblId.Text, Session("LoginId").ToString(), "Deleted"}
            'settingClass.Logs(dataLog)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchCompany") = String.Empty
        Try
            Dim searchString As String = String.Empty
            If Not searchText = "" Then
                searchString = "WHERE Id LIKE '%" & searchText & "%' OR Name LIKE '%" & searchText & "%' OR Alias LIKE '%" & searchText & "%' OR Description LIKE '%" & searchText & "%'"
            End If
            Dim thisString As String = String.Format("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Companys {0} ORDER BY Id ASC", searchString)

            gvList.DataSource = settingClass.GetDataTable(thisString)
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
        Return settingClass.getTextLog(logId)
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
