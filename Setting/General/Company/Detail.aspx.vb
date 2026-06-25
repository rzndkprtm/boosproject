Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_General_Company_Detail
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general/company", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("cid")) Then
            Response.Redirect("~/setting/general/company", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("cid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnAddDetail_Click(sender As Object, e As EventArgs)
        MessageError_ProcessDetail(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessDetail(); };"
        Try
            lblAction.Text = "Add"
            titleProcessDetail.InnerText = "Add Detail"
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDetail", thisScript, True)
        Catch ex As Exception
            MessageError_ProcessDetail(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcessDetail(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDetail", thisScript, True)
        End Try
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex

        MessageError(False, String.Empty)
        BindData(lblId.Text)
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError_ProcessDetail(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcessDetail(); };"
                Try
                    lblDetailId.Text = dataId
                    lblAction.Text = "Edit"
                    titleProcessDetail.InnerText = "Edit Company Detail"

                    Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM CompanyDetails WHERE Id='" & dataId & "'")
                    If myData Is Nothing Then Exit Sub

                    txtNameDetail.Text = myData("Name").ToString()
                    txtNameDetail.Text = myData("Name").ToString()
                    txtDescriptionDetail.Text = myData("Description").ToString()
                    ddlActiveDetail.SelectedValue = Convert.ToInt32(myData("Active"))

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDetail", thisScript, True)
                Catch ex As Exception
                    MessageError_ProcessDetail(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError_ProcessDetail(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDetail", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If txtName.Text = "" Then
                MessageError(True, "COMPANY NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE Companys SET Name=@Name, Alias=@Alias, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Alias", txtAlias.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"Companys", lblId.Text, Session("LoginId").ToString(), "Company Updated"}
                settingClass.Logs(dataLog)

                url = String.Format("~/setting/general/company/detail?cid={0}", lblId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnProcessDetail_Click(sender As Object, e As EventArgs)
        MessageError_ProcessDetail(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessDetail(); };"
        Try
            If txtNameDetail.Text = "" Then
                MessageError_ProcessDetail(True, "COMPANY DETAIL NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDetail", thisScript, True)
                Exit Sub
            End If
            If msgErrorProcessDetail.InnerText = "" Then
                Dim descText As String = txtDescriptionDetail.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                If lblAction.Text = "Add" Then
                    Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CompanyDetails ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO CompanyDetails VALUES(@Id, @Name, @CompanyId, @Description, @Active)", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", thisId)
                            thisCmd.Parameters.AddWithValue("@Name", txtNameDetail.Text.Trim())
                            thisCmd.Parameters.AddWithValue("@CompanyId", lblId.Text)
                            thisCmd.Parameters.AddWithValue("@Description", descText)
                            thisCmd.Parameters.AddWithValue("@Active", ddlActiveDetail.SelectedValue)
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"CompanyDetails", thisId, Session("LoginId").ToString(), "Company Detail Created"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/general/company/detail?cid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If

                If lblAction.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As SqlCommand = New SqlCommand("UPDATE CompanyDetails SET Name=@Name, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", lblDetailId.Text)
                            thisCmd.Parameters.AddWithValue("@Name", txtNameDetail.Text.Trim())
                            thisCmd.Parameters.AddWithValue("@Description", descText)
                            thisCmd.Parameters.AddWithValue("@Active", ddlActiveDetail.SelectedValue)
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"CompanyDetails", lblDetailId.Text, Session("LoginId").ToString(), "Company Detail Updated"}
                    settingClass.Logs(dataLog)

                    url = String.Format("~/setting/general/company/detail?cid={0}", lblId.Text)
                    Response.Redirect(url, False)
                End If
            End If
        Catch ex As Exception
            MessageError_ProcessDetail(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_ProcessDetail(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDetail", thisScript, True)
        End Try
    End Sub

    Protected Sub BindData(companyId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM Companys WHERE Id='" & companyId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/general/company", False)
                Exit Sub
            End If

            txtName.Text = thisData("Name").ToString()
            txtAlias.Text = thisData("Alias").ToString()
            txtDescription.Text = thisData("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(thisData("Active"))

            divEdit.Visible = LoginAccess("Edit")

            txtName.ReadOnly = True
            txtAlias.ReadOnly = True
            txtDescription.ReadOnly = True
            ddlActive.Enabled = False

            Dim editAccess As Boolean = LoginAccess("Edit")
            If editAccess = True Then
                txtName.ReadOnly = False
                txtAlias.ReadOnly = False
                txtDescription.ReadOnly = False
                ddlActive.Enabled = True
            End If

            gvList.DataSource = settingClass.GetDataTable("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM CompanyDetails WHERE CompanyId='" & companyId & "'")
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID Detail")

            btnAddDetail.Visible = LoginAccess("Add Detail")

        Catch ex As Exception
            MessageError(True, ex.ToString)
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_ProcessDetail(visible As Boolean, message As String)
        divErrorProcessDetail.Visible = visible : msgErrorProcessDetail.InnerText = message
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
