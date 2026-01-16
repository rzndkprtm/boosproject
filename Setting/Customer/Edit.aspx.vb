Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_Edit
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing

    Dim settingClass As New SettingClass
    Dim mailingClass As New MailingClass

    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("customeredit")) Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("customeredit").ToString()
        If Not IsPostBack Then
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub ddlCompany_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindCompanyDetail(ddlCompany.SelectedValue)
        BindPriceGroup(ddlCompany.SelectedValue)
        BindPriceGroup_Shutter(ddlCompany.SelectedValue)
        BindPriceGroup_Door(ddlCompany.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        BackColor()
        Try
            If txtName.Text = "" Then
                MessageError(True, "CUSTOMER NAME IS REQUIRED !")
                txtName.BackColor = Drawing.Color.Red
                txtName.Focus()
                Exit Sub
            End If

            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" Then
                If ddlLevel.SelectedValue = "" Then
                    MessageError(True, "CUSTOMER LEVEL IS REQUIRED !")
                    ddlLevel.BackColor = Drawing.Color.Red
                    ddlLevel.Focus()
                    Exit Sub
                End If
                If ddlLevel.SelectedValue = "Referral" AndAlso ddlSponsor.SelectedValue = "" Then
                    MessageError(True, "CUSTOMER SPONSOR IS REQUIRED !")
                    ddlSponsor.BackColor = Drawing.Color.Red
                    ddlSponsor.Focus()
                    Exit Sub
                End If
            End If

            If ddlCompany.SelectedValue = "" Then
                MessageError(True, "COMPANY IS REQUIRED !")
                ddlCompany.BackColor = Drawing.Color.Red
                ddlCompany.Focus()
                Exit Sub
            End If

            If ddlCompanyDetail.SelectedValue = "" Then
                MessageError(True, "SUB COMPANY IS REQUIRED !")
                ddlCompanyDetail.BackColor = Drawing.Color.Red
                ddlCompanyDetail.Focus()
                Exit Sub
            End If

            If ddlCompany.SelectedValue = "2" Then
                If ddlArea.SelectedValue = "" Then
                    MessageError(True, "AREA IS REQUIRED !")
                    ddlArea.BackColor = Drawing.Color.Red
                    ddlArea.Focus()
                    Exit Sub
                End If
            End If

            If ddlPriceGroup.SelectedValue = "" Then
                MessageError(True, "PRICE GROUP IS REQUIRED !")
                ddlPriceGroup.BackColor = Drawing.Color.Red
                ddlPriceGroup.Focus()
                Exit Sub
            End If

            If ddlPriceGroupShutter.SelectedValue = "" Then
                MessageError(True, "SHUTTER PRICE GROUP IS REQUIRED !")
                ddlPriceGroupShutter.BackColor = Drawing.Color.Red
                ddlPriceGroupShutter.Focus()
                Exit Sub
            End If

            If ddlPriceGroupDoor.SelectedValue = "" Then
                MessageError(True, "DOOR PRICE GROUP IS REQUIRED !")
                ddlPriceGroupDoor.BackColor = Drawing.Color.Red
                ddlPriceGroupDoor.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                If Not ddlCompany.SelectedValue = "2" Then
                    ddlArea.SelectedValue = "" : ddlOperator.SelectedValue = ""
                End If

                Dim sponsorId As String = ddlSponsor.SelectedValue

                If ddlLevel.SelectedValue = "" Then ddlLevel.SelectedValue = "Member"
                If ddlLevel.SelectedValue = "Sponsor" OrElse ddlLevel.SelectedValue = "Member" Then sponsorId = String.Empty

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE Customers SET DebtorCode=@DebtorCode, Name=@Name, Level=@Level, SponsorId=@Sponsor, CompanyId=@Company, CompanyDetailId=@CompanyDetail, Area=@Area, Operator=@Operator, PriceGroupId=@PriceGroup, ShutterPriceGroupId=@ShutterPriceGroup, DoorPriceGroupId=@DoorPriceGroupId, OnStop=@OnStop, CashSale=@CashSale, Newsletter=@Newsletter, MinSurcharge=@MinSurcharge, Active=@Active WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        myCmd.Parameters.AddWithValue("@DebtorCode", txtDebtorCode.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Level", ddlLevel.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Sponsor", If(String.IsNullOrEmpty(sponsorId), CType(DBNull.Value, Object), sponsorId))
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Company", If(String.IsNullOrEmpty(ddlCompany.SelectedValue), CType(DBNull.Value, Object), ddlCompany.SelectedValue))
                        myCmd.Parameters.AddWithValue("@CompanyDetail", If(String.IsNullOrEmpty(ddlCompanyDetail.SelectedValue), CType(DBNull.Value, Object), ddlCompanyDetail.SelectedValue))
                        myCmd.Parameters.AddWithValue("@Area", ddlArea.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Operator", If(String.IsNullOrEmpty(ddlOperator.SelectedValue), CType(DBNull.Value, Object), ddlOperator.SelectedValue))
                        myCmd.Parameters.AddWithValue("@PriceGroup", ddlPriceGroup.SelectedValue)
                        myCmd.Parameters.AddWithValue("@ShutterPriceGroup", ddlPriceGroupShutter.SelectedValue)
                        myCmd.Parameters.AddWithValue("@DoorPriceGroupId", ddlPriceGroupDoor.SelectedValue)
                        myCmd.Parameters.AddWithValue("@OnStop", ddlOnStop.SelectedValue)
                        myCmd.Parameters.AddWithValue("@CashSale", ddlCashSale.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Newsletter", ddlNewsletter.SelectedValue)
                        myCmd.Parameters.AddWithValue("@MinSurcharge", ddlMinSurcharge.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Customers", lblId.Text, Session("LoginId").ToString(), "Customer Updated"}
                settingClass.Logs(dataLog)

                url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "btnSubmit_Click", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Private Sub BindData(customerId As String)
        BackColor()
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM Customers WHERE Id='" & customerId & "'")

            If myData Is Nothing Then
                Response.Redirect("~/setting/customer", False)
                Exit Sub
            End If

            Dim companyId As String = myData("CompanyId").ToString()

            BindSponsor()
            BindCompany()
            BindCompanyDetail(companyId)
            BindOperator()
            BindPriceGroup(companyId)
            BindPriceGroup_Shutter(companyId)
            BindPriceGroup_Door(companyId)

            txtDebtorCode.Text = myData("DebtorCode").ToString()
            txtName.Text = myData("Name").ToString()
            ddlLevel.SelectedValue = myData("Level").ToString()
            ddlSponsor.SelectedValue = myData("SponsorId").ToString()
            ddlCompany.SelectedValue = myData("CompanyId").ToString()
            ddlCompanyDetail.SelectedValue = myData("CompanyDetailId").ToString()
            ddlArea.SelectedValue = myData("Area").ToString()
            ddlOperator.SelectedValue = myData("Operator").ToString()
            ddlPriceGroup.SelectedValue = myData("PriceGroupId").ToString()
            ddlPriceGroupShutter.SelectedValue = myData("ShutterPriceGroupId").ToString()
            ddlPriceGroupDoor.SelectedValue = myData("DoorPriceGroupId").ToString()
            ddlOnStop.SelectedValue = Convert.ToInt32(myData("OnStop"))
            ddlCashSale.SelectedValue = Convert.ToInt32(myData("CashSale"))
            ddlNewsletter.SelectedValue = Convert.ToInt32(myData("Newsletter"))
            ddlMinSurcharge.SelectedValue = Convert.ToInt32(myData("MinSurcharge"))
            ddlActive.SelectedValue = Convert.ToInt32(myData("Active"))

            BindComponentForm()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindData", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindSponsor()
        ddlSponsor.Items.Clear()
        Try
            ddlSponsor.DataSource = settingClass.GetDataTable("SELECT * FROM Customers WHERE [Level]='Sponsor' ORDER BY Id ASC")
            ddlSponsor.DataTextField = "Name"
            ddlSponsor.DataValueField = "Id"
            ddlSponsor.DataBind()

            If ddlSponsor.Items.Count > 0 Then
                ddlSponsor.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlSponsor.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindSponsor", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Private Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT * FROM Companys ORDER BY Id ASC"
            If Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" Then
                thisQuery = "SELECT * FROM Companys WHERE Id<>'1' ORDER BY Id ASC"
            End If
            If Session("RoleName") = "Sales" OrElse Session("RoleName") = "Account" Then
                thisQuery = "SELECT * FROM Companys WHERE Id='" & Session("CompanyId") & "' ORDER BY Id ASC"
            End If

            ddlCompany.DataSource = settingClass.GetDataTable(thisQuery)
            ddlCompany.DataTextField = "Name"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 0 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
            End If

            ddlCompany.Enabled = False
            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" Then
                ddlCompany.Enabled = True
            End If
        Catch ex As Exception
            ddlCompany.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindCompany", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Private Sub BindCompanyDetail(companyId As String)
        ddlCompanyDetail.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(companyId) Then
                ddlCompanyDetail.DataSource = settingClass.GetDataTable("SELECT * FROM CompanyDetails WHERE CompanyId='" & companyId & "' ORDER BY Name ASC")
                ddlCompanyDetail.DataTextField = "Name"
                ddlCompanyDetail.DataValueField = "Id"
                ddlCompanyDetail.DataBind()

                If ddlCompanyDetail.Items.Count > 0 Then
                    ddlCompanyDetail.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            ddlCompanyDetail.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindCompanyDetail", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Private Sub BindOperator()
        ddlOperator.Items.Clear()
        Try
            ddlOperator.DataSource = settingClass.GetDataTable("SELECT * FROM CustomerLogins WHERE RoleId='4' ORDER BY UserName ASC")
            ddlOperator.DataTextField = "FullName"
            ddlOperator.DataValueField = "Id"
            ddlOperator.DataBind()

            If ddlOperator.Items.Count > 0 Then
                ddlOperator.Items.Insert(0, New ListItem("", ""))
            End If

            ddlOperator.Enabled = True
            If Session("RoleName") = "Sales" AndAlso Session("LevelName") = "Member" Then
                ddlOperator.SelectedValue = Session("LoginId").ToString()
                ddlOperator.Enabled = False
            End If
        Catch ex As Exception
            ddlOperator.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindOperator", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Private Sub BindPriceGroup(companyId As String)
        ddlPriceGroup.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(companyId) Then
                ddlPriceGroup.DataSource = settingClass.GetDataTable("SELECT * FROM PriceGroups WHERE Type='Blinds' AND CompanyId='" & companyId & "' ORDER BY Name ASC")
                ddlPriceGroup.DataTextField = "Name"
                ddlPriceGroup.DataValueField = "Id"
                ddlPriceGroup.DataBind()

                If ddlPriceGroup.Items.Count > 0 Then
                    ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            ddlPriceGroup.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindPriceGroup", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Private Sub BindPriceGroup_Shutter(companyId As String)
        ddlPriceGroupShutter.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(companyId) Then
                ddlPriceGroupShutter.DataSource = settingClass.GetDataTable("SELECT * FROM PriceGroups WHERE Type='Shutters' AND CompanyId='" & companyId & "' ORDER BY Name ASC")
                ddlPriceGroupShutter.DataTextField = "Name"
                ddlPriceGroupShutter.DataValueField = "Id"
                ddlPriceGroupShutter.DataBind()

                If ddlPriceGroupShutter.Items.Count > 0 Then
                    ddlPriceGroupShutter.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            ddlPriceGroupShutter.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindPriceGroup_Shutter", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Protected Sub BindPriceGroup_Door(companyId As String)
        ddlPriceGroupDoor.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(companyId) Then
                ddlPriceGroupDoor.DataSource = settingClass.GetDataTable("SELECT * FROM PriceGroups WHERE Type='Doors' AND CompanyId='" & companyId & "' ORDER BY Name ASC")
                ddlPriceGroupDoor.DataTextField = "Name"
                ddlPriceGroupDoor.DataValueField = "Id"
                ddlPriceGroupDoor.DataBind()

                If ddlPriceGroupDoor.Items.Count > 0 Then
                    ddlPriceGroupDoor.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            ddlPriceGroupDoor.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                dataMailing = {Session("LoginId").ToString(), Session("CompanyId").ToString(), Page.Title, "BindPriceGroup_Door", ex.ToString()}
                mailingClass.WebError(dataMailing)
            End If
        End Try
    End Sub

    Private Sub BindComponentForm()
        divDebtorCode.Visible = PageAction("Visible Debtor Code")
        divLevelSponsor.Visible = PageAction("Visible Level Sponsor")
        divCompany.Visible = PageAction("Visible Company")
        divAreaOperator.Visible = PageAction("Visible Area Operator")
    End Sub

    Private Sub BackColor()
        MessageError(False, String.Empty)

        txtDebtorCode.BackColor = Drawing.Color.Empty
        txtName.BackColor = Drawing.Color.Empty
        ddlLevel.BackColor = Drawing.Color.Empty
        ddlSponsor.BackColor = Drawing.Color.Empty
        ddlCompany.BackColor = Drawing.Color.Empty
        ddlCompanyDetail.BackColor = Drawing.Color.Empty
        ddlArea.BackColor = Drawing.Color.Empty
        ddlOperator.BackColor = Drawing.Color.Empty
        ddlOnStop.BackColor = Drawing.Color.Empty
        ddlCashSale.BackColor = Drawing.Color.Empty
        ddlPriceGroup.BackColor = Drawing.Color.Empty
        ddlPriceGroupShutter.BackColor = Drawing.Color.Empty
        ddlPriceGroupDoor.BackColor = Drawing.Color.Empty
        ddlNewsletter.BackColor = Drawing.Color.Empty
        ddlMinSurcharge.BackColor = Drawing.Color.Empty
        ddlActive.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
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
