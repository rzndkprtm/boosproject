Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_ChainRemote
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchChain")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Session("SearchChain") = txtSearch.Text

        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            BindDesign()
            BindControl()
            BindCompany()

            lblAction.Text = "Add"
            titleProcess.InnerText = "Add Chain - Remote"

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
            Session("SearchChain") = txtSearch.Text
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_Process(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcess(); };"
                Try
                    lblId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcess.InnerText = "Edit Chain - Remote"

                    BindDesign()
                    BindControl()
                    BindCompany()

                    Dim myData As DataSet = settingClass.GetListData("SELECT * FROM Chains WHERE Id='" & lblId.Text & "'")
                    txtBoeId.Text = myData.Tables(0).Rows(0).Item("BoeId").ToString()
                    txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
                    ddlChainType.SelectedValue = myData.Tables(0).Rows(0).Item("ChainType").ToString()
                    txtDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
                    ddlActive.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))

                    If Not myData.Tables(0).Rows(0).Item("DesignId").ToString() = "" Then
                        Dim thisArray() As String = myData.Tables(0).Rows(0).Item("DesignId").ToString().Split(",")
                        For Each i In thisArray
                            If Not (i.Equals(String.Empty)) Then
                                lbDesign.Items.FindByValue(i).Selected = True
                            End If
                        Next
                    End If

                    If Not myData.Tables(0).Rows(0).Item("ControlTypeId").ToString() = "" Then
                        Dim thisArray() As String = myData.Tables(0).Rows(0).Item("ControlTypeId").ToString().Split(",")
                        For Each i In thisArray
                            If Not (i.Equals(String.Empty)) Then
                                lbControl.Items.FindByValue(i).Selected = True
                            End If
                        Next
                    End If

                    If Not myData.Tables(0).Rows(0).Item("CompanyDetailId").ToString() = "" Then
                        Dim thisArray() As String = myData.Tables(0).Rows(0).Item("CompanyDetailId").ToString().Split(",")
                        For Each i In thisArray
                            If Not (i.Equals(String.Empty)) Then
                                lbCompany.Items.FindByValue(i).Selected = True
                            End If
                        Next
                    End If

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
                    gvListLogs.DataSource = settingClass.GetListData("SELECT * FROM Logs WHERE DataId='" & dataId & "' AND Type='Chains' ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()

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
            If Not String.IsNullOrEmpty(txtBoeId.Text) Then
                If Not IsNumeric(txtBoeId.Text) Then
                    MessageError_Process(True, "BOE ID SHOULD BE NUMERIC !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                    Exit Sub
                End If
            End If
            If txtName.Text = "" Then
                MessageError_Process(True, "CHAIN / REMOTE NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If lbDesign.SelectedValue = "" Then
                MessageError_Process(True, "DESIGN TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If lbControl.SelectedValue = "" Then
                MessageError_Process(True, "CONTROL TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If lbCompany.SelectedValue = "" Then
                MessageError_Process(True, "COMPANY IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
                Dim selectedDesign As String = String.Empty
                For Each item As ListItem In lbDesign.Items
                    If item.Selected Then
                        selectedDesign += item.Value & ","
                    End If
                Next

                Dim selectedControl As String = String.Empty
                For Each item As ListItem In lbControl.Items
                    If item.Selected Then
                        selectedControl += item.Value & ","
                    End If
                Next

                Dim selectedCompany As String = String.Empty
                For Each item As ListItem In lbCompany.Items
                    If item.Selected Then
                        selectedCompany += item.Value & ","
                    End If
                Next

                Dim designType As String = selectedDesign.Remove(selectedDesign.Length - 1).ToString()
                Dim controlType As String = selectedControl.Remove(selectedControl.Length - 1).ToString()
                Dim companyDetail As String = selectedCompany.Remove(selectedCompany.Length - 1).ToString()

                If msgErrorProcess.InnerText = "" Then
                    Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                    If lblAction.Text = "Add" Then
                        Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Chains ORDER BY Id DESC")

                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Chains VALUES (@Id, @BoeId, @Name, @DesignId, @ControlTypeId, @CompanyDetailId, @ChainType, @Description, @Active)", thisConn)
                                myCmd.Parameters.AddWithValue("@Id", thisId)
                                myCmd.Parameters.AddWithValue("@BoeId", If(String.IsNullOrEmpty(txtBoeId.Text), CType(DBNull.Value, Object), txtBoeId.Text))
                                myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                                myCmd.Parameters.AddWithValue("@DesignId", designType)
                                myCmd.Parameters.AddWithValue("@ControlTypeId", controlType)
                                myCmd.Parameters.AddWithValue("@CompanyDetailId", companyDetail)
                                myCmd.Parameters.AddWithValue("@ChainType", ddlChainType.SelectedValue)
                                myCmd.Parameters.AddWithValue("@Description", descText)
                                myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                            End Using
                        End Using

                        Dim dataLog As Object() = {"Chains", thisId, Session("LoginId").ToString(), "Chain / Remote Created"}
                        settingClass.Logs(dataLog)

                        Session("SearchChain") = txtSearch.Text
                        Response.Redirect("~/setting/specification/chainremote", False)
                    End If

                    If lblAction.Text = "Edit" Then
                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As SqlCommand = New SqlCommand("UPDATE Chains SET BoeId=@BoeId, Name=@Name, DesignId=@DesignId, ControlTypeId=@ControlTypeId, CompanyDetailId=@CompanyDetailId, ChainType=@ChainType, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                                myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                                myCmd.Parameters.AddWithValue("@BoeId", If(String.IsNullOrEmpty(txtBoeId.Text), CType(DBNull.Value, Object), txtBoeId.Text))
                                myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                                myCmd.Parameters.AddWithValue("@DesignId", designType)
                                myCmd.Parameters.AddWithValue("@ControlTypeId", controlType)
                                myCmd.Parameters.AddWithValue("@CompanyDetailId", companyDetail)
                                myCmd.Parameters.AddWithValue("@ChainType", ddlChainType.SelectedValue)
                                myCmd.Parameters.AddWithValue("@Description", descText)
                                myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                            End Using
                        End Using

                        Dim dataLog As Object() = {"Chains", lblId.Text, Session("LoginId").ToString(), "Chain / Remote Updated"}
                        settingClass.Logs(dataLog)

                        Session("SearchChain") = txtSearch.Text
                        Response.Redirect("~/setting/specification/chainremote", False)
                    End If
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

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Chains WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM Logs WHERE Type='Chains' AND DataId=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            Session("SearchChain") = txtSearch.Text
            Response.Redirect("~/setting/specification/chainremote", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchChain") = String.Empty
        Try
            Dim search As String = String.Empty
            If Not searchText = "" Then
                search = " WHERE Id LIKE '%" & searchText & "%' OR BoeId LIKE '%" & searchText & "%' OR Name LIKE '%" & searchText & "%' OR Description LIKE '%" & searchText & "%'"
            End If
            Dim thisString As String = String.Format("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Chains {0} ORDER BY Name ASC", search)
            gvList.DataSource = settingClass.GetListData(thisString)
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

    Protected Sub BindDesign()
        lbDesign.Items.Clear()
        Try
            lbDesign.DataSource = settingClass.GetListData("SELECT * FROM Designs ORDER BY Name ASC")
            lbDesign.DataTextField = "Name"
            lbDesign.DataValueField = "Id"
            lbDesign.DataBind()

            If lbDesign.Items.Count > 0 Then
                lbDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindControl()
        lbControl.Items.Clear()
        Try
            lbControl.DataSource = settingClass.GetListData("SELECT * FROM ProductControls ORDER BY Name ASC")
            lbControl.DataTextField = "Name"
            lbControl.DataValueField = "Id"
            lbControl.DataBind()

            If lbControl.Items.Count > 0 Then
                lbControl.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompany()
        lbCompany.Items.Clear()
        Try
            lbCompany.DataSource = settingClass.GetListData("SELECT * FROM CompanyDetails ORDER BY Name ASC")
            lbCompany.DataTextField = "Name"
            lbCompany.DataValueField = "Id"
            lbCompany.DataBind()

            If lbCompany.Items.Count > 0 Then
                lbCompany.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
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
