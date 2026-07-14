Imports System.ComponentModel.Design
Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer/list", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("customerid")) Then
            Response.Redirect("~/setting/customer/list", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        lblId.Text = Request.QueryString("customerid").ToString()

        If Session("RoleName") = "Sales" AndAlso Session("CustomerId") = lblId.Text Then
            Response.Redirect("~/setting/customer/list", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub ddlCompany_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindCompanyDetail(ddlCompany.SelectedValue)
        BindOperator(ddlCompany.SelectedValue)
        BindPrimary(ddlCompany.SelectedValue)
        BindPriceGroup(ddlCompany.SelectedValue)
        BindPriceGroup_Shutter(ddlCompany.SelectedValue)
        BindPriceGroup_Door(ddlCompany.SelectedValue)
    End Sub

    Protected Sub ddlLevel_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindComponentForm(ddlLevel.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If txtName.Text = "" Then
                MessageError(True, "CUSTOMER NAME IS REQUIRED !")
                Exit Sub
            End If

            If ddlCompany.SelectedValue = "" Then
                MessageError(True, "COMPANY IS REQUIRED !")
                Exit Sub
            End If
            If ddlCompanyDetail.SelectedValue = "" Then
                MessageError(True, "SUB COMPANY IS REQUIRED !")
                Exit Sub
            End If
            If ddlLevel.SelectedValue = "" Then
                MessageError(True, "CUSTOMER LEVEL IS REQUIRED !")
                Exit Sub
            End If
            If ddlLevel.SelectedValue = "Referral" AndAlso ddlPrimary.SelectedValue = "" Then
                MessageError(True, "PRIMARY CUSTOMER IS REQUIRED !")
                Exit Sub
            End If

            If ddlPriceGroup.SelectedValue = "" Then
                MessageError(True, "PRICE GROUP IS REQUIRED !")
                Exit Sub
            End If
            If ddlPriceGroupShutter.SelectedValue = "" Then
                MessageError(True, "SHUTTER PRICE GROUP IS REQUIRED !")
                Exit Sub
            End If
            If ddlPriceGroupDoor.SelectedValue = "" Then
                MessageError(True, "DOOR PRICE GROUP IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim operatorReps As String = String.Empty
                If Not String.IsNullOrEmpty(lbOperator.SelectedValue) Then
                    operatorReps = String.Join(",", lbOperator.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) i.Value))
                End If

                Dim primaryId As String = ddlPrimary.SelectedValue
                If ddlLevel.SelectedValue = "Primary" OrElse ddlLevel.SelectedValue = "Standard" Then primaryId = String.Empty

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE Customers SET DebtorCode=@DebtorCode, Name=@Name, Level=@Level, PrimaryId=@PrimaryId, CompanyId=@Company, CompanyDetailId=@CompanyDetail, Area=@Area, Operator=@Operator, PriceGroupId=@PriceGroup, ShutterPriceGroupId=@ShutterPriceGroup, DoorPriceGroupId=@DoorPriceGroupId, OnStop=@OnStop, CashSale=@CashSale, Newsletter=@Newsletter, MinSurcharge=@MinSurcharge WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@DebtorCode", txtDebtorCode.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Level", ddlLevel.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@PrimaryId", If(String.IsNullOrEmpty(primaryId), CType(DBNull.Value, Object), primaryId))
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Company", If(String.IsNullOrEmpty(ddlCompany.SelectedValue), CType(DBNull.Value, Object), ddlCompany.SelectedValue))
                        thisCmd.Parameters.AddWithValue("@CompanyDetail", If(String.IsNullOrEmpty(ddlCompanyDetail.SelectedValue), CType(DBNull.Value, Object), ddlCompanyDetail.SelectedValue))
                        thisCmd.Parameters.AddWithValue("@Area", ddlArea.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Operator", operatorReps)
                        thisCmd.Parameters.AddWithValue("@PriceGroup", ddlPriceGroup.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@ShutterPriceGroup", ddlPriceGroupShutter.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@DoorPriceGroupId", ddlPriceGroupDoor.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@OnStop", ddlOnStop.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@CashSale", ddlCashSale.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Newsletter", ddlNewsletter.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@MinSurcharge", ddlMinSurcharge.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Customers", lblId.Text, Session("LoginId").ToString(), "Customer Updated"}
                settingClass.Logs(dataLog)

                url = "~/setting/customer/list"
                If lblReturnPage.Text = "detail" Then
                    url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                End If
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        url = "~/setting/customer/list"
        If lblReturnPage.Text = "detail" Then
            url = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
        End If
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindData(customerId As String)
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM Customers WHERE Id='" & customerId & "'")
            If myData Is Nothing Then
                Response.Redirect("~/setting/customer/list", False)
                Exit Sub
            End If

            Dim companyId As String = myData("CompanyId").ToString()

            BindPrimary(companyId)
            BindCompany()
            BindCompanyDetail(companyId)
            BindOperator(companyId)
            BindPriceGroup(companyId)
            BindPriceGroup_Shutter(companyId)
            BindPriceGroup_Door(companyId)

            txtDebtorCode.Text = myData("DebtorCode").ToString()
            txtName.Text = myData("Name").ToString()
            ddlLevel.SelectedValue = myData("Level").ToString()
            ddlPrimary.SelectedValue = myData("PrimaryId").ToString()
            ddlCompany.SelectedValue = myData("CompanyId").ToString()
            ddlCompanyDetail.SelectedValue = myData("CompanyDetailId").ToString()
            ddlArea.SelectedValue = myData("Area").ToString()
            If Not myData("Operator").ToString() = "" Then
                Dim operatorArray() As String = myData("Operator").ToString().Split(",")
                For Each i In operatorArray
                    If Not String.IsNullOrEmpty(i) Then
                        Dim item = lbOperator.Items.FindByValue(i)
                        If item IsNot Nothing Then
                            item.Selected = True
                        End If
                    End If
                Next
            End If
            ddlPriceGroup.SelectedValue = myData("PriceGroupId").ToString()
            ddlPriceGroupShutter.SelectedValue = myData("ShutterPriceGroupId").ToString()
            ddlPriceGroupDoor.SelectedValue = myData("DoorPriceGroupId").ToString()
            ddlOnStop.SelectedValue = Convert.ToInt32(myData("OnStop"))
            ddlCashSale.SelectedValue = Convert.ToInt32(myData("CashSale"))
            ddlNewsletter.SelectedValue = Convert.ToInt32(myData("Newsletter"))
            ddlMinSurcharge.SelectedValue = Convert.ToInt32(myData("MinSurcharge"))

            BindComponentForm(ddlLevel.SelectedValue)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompany()
        ddlCompany.Items.Clear()
        Try
            ddlCompany.DataSource = settingClass.GetDataTable("SELECT * FROM Companys ORDER BY Id ASC")
            ddlCompany.DataTextField = "Alias"
            ddlCompany.DataValueField = "Id"
            ddlCompany.DataBind()

            If ddlCompany.Items.Count > 0 Then
                ddlCompany.Items.Insert(0, New ListItem("", ""))
            End If

            If Session("RoleName") = "Sales" OrElse Session("RoleName") = "Account" Then
                ddlCompany.SelectedValue = Session("CompanyId").ToString()
            End If

            ddlCompany.Enabled = False
            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" OrElse Session("RoleName") = "Factory Office" Then
                ddlCompany.Enabled = True
            End If
        Catch ex As Exception
            ddlCompany.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompanyDetail(companyId As String)
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
            End If
        End Try
    End Sub

    Protected Sub BindOperator(companyId As String)
        lbOperator.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(companyId) Then
                lbOperator.DataSource = settingClass.GetDataTable("SELECT Logins.* FROM Logins LEFT JOIN Customers ON Logins.CustomerId=Customers.Id WHERE Customers.CompanyId='" & companyId & "' AND Logins.RoleId='4' AND Logins.LevelId='2' ORDER BY Logins.UserName ASC")
                lbOperator.DataTextField = "FullName"
                lbOperator.DataValueField = "Id"
                lbOperator.DataBind()

                If lbOperator.Items.Count > 0 Then
                    lbOperator.Items.Insert(0, New ListItem("", ""))
                End If

                lbOperator.Enabled = True
                If Session("RoleName") = "Sales" AndAlso Session("LevelName") = "Member" Then
                    lbOperator.SelectedValue = Session("LoginId").ToString()
                    lbOperator.Enabled = False
                End If
            End If
        Catch ex As Exception
            lbOperator.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindPrimary(companyId As String)
        ddlPrimary.Items.Clear()
        Try
            ddlPrimary.DataSource = settingClass.GetDataTable("SELECT * FROM Customers WHERE [Level]='Primary' AND CompanyId='" & companyId & "' ORDER BY Id ASC")
            ddlPrimary.DataTextField = "Name"
            ddlPrimary.DataValueField = "Id"
            ddlPrimary.DataBind()

            If ddlPrimary.Items.Count > 0 Then
                ddlPrimary.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlPrimary.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindPriceGroup(companyId As String)
        ddlPriceGroup.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(companyId) Then
                ddlPriceGroup.DataSource = settingClass.GetDataTable("SELECT * FROM PriceGroups WHERE Type='Blinds' AND CompanyId='" & companyId & "' AND Active=1 ORDER BY Name ASC")
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
            End If
        End Try
    End Sub

    Protected Sub BindPriceGroup_Shutter(companyId As String)
        ddlPriceGroupShutter.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(companyId) Then
                ddlPriceGroupShutter.DataSource = settingClass.GetDataTable("SELECT * FROM PriceGroups WHERE Type='Shutters' AND CompanyId='" & companyId & "' AND Active=1 ORDER BY Name ASC")
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
            End If
        End Try
    End Sub

    Protected Sub BindPriceGroup_Door(companyId As String)
        ddlPriceGroupDoor.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(companyId) Then
                ddlPriceGroupDoor.DataSource = settingClass.GetDataTable("SELECT * FROM PriceGroups WHERE Type='Doors' AND CompanyId='" & companyId & "' AND Active=1 ORDER BY Name ASC")
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
            End If
        End Try
    End Sub

    Protected Sub BindComponentForm(levelCust As String)
        Try
            divDebtorCode.Visible = False
            divLinked.Visible = False

            If Session("RoleName") = "Developer" Then divDebtorCode.Visible = True
            If Session("RoleName") = "IT" Then divDebtorCode.Visible = True
            If Session("RoleName") = "Factory Office" Then divDebtorCode.Visible = True
            If Session("RoleName") = "Account" Then divDebtorCode.Visible = True

            If levelCust = "Linked" Then
                divLinked.Visible = True
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
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
