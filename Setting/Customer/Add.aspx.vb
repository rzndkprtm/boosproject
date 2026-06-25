Imports System.Data.SqlClient

Partial Class Setting_Customer_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer/list", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindSponsor()
            BindCompany()
            BindCompanyDetail(ddlCompany.SelectedValue)
            BindOperator(ddlCompany.SelectedValue)
            BindPriceGroup(ddlCompany.SelectedValue)
            BindPriceGroup_Shutter(ddlCompany.SelectedValue)
            BindPriceGroup_Door(ddlCompany.SelectedValue)

            BindComponentForm()
        End If
    End Sub

    Protected Sub ddlCompany_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindCompanyDetail(ddlCompany.SelectedValue)
        BindOperator(ddlCompany.SelectedValue)
        BindPriceGroup(ddlCompany.SelectedValue)
        BindPriceGroup_Shutter(ddlCompany.SelectedValue)
        BindPriceGroup_Door(ddlCompany.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If txtName.Text = "" Then
                MessageError(True, "CUSTOMER NAME IS REQUIRED !")
                Exit Sub
            End If

            If Session("RoleName") = "Developer" OrElse Session("RoleName") = "IT" Then
                If ddlLevel.SelectedValue = "" Then
                    MessageError(True, "CUSTOMER LEVEL IS REQUIRED !")
                    Exit Sub
                End If
                If ddlLevel.SelectedValue = "Referral" AndAlso ddlSponsor.SelectedValue = "" Then
                    MessageError(True, "CUSTOMER SPONSOR IS REQUIRED !")
                    Exit Sub
                End If
            End If

            If ddlCompany.SelectedValue = "" Then
                MessageError(True, "COMPANY IS REQUIRED !")
                Exit Sub
            End If

            If ddlCompanyDetail.SelectedValue = "" Then
                MessageError(True, "SUB COMPANY IS REQUIRED !")
                Exit Sub
            End If

            If ddlCompany.SelectedValue = "2" Then
                If ddlArea.SelectedValue = "" Then
                    MessageError(True, "AREA IS REQUIRED !")
                    Exit Sub
                End If
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
                If Not String.IsNullOrEmpty(lbSales.SelectedValue) Then
                    operatorReps = String.Join(",", lbSales.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) i.Value))
                End If
                If Not ddlCompany.SelectedValue = "2" Then
                    ddlArea.SelectedValue = "" : operatorReps = String.Empty
                End If

                Dim sponsorId As String = ddlSponsor.SelectedValue

                If ddlLevel.SelectedValue = "" Then ddlLevel.SelectedValue = "Member"
                If ddlLevel.SelectedValue = "Sponsor" OrElse ddlLevel.SelectedValue = "Member" Then sponsorId = String.Empty

                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Customers ORDER BY Id DESC")
                Dim logoCustomer As String = "yourlogo.png"
                Dim dataProductAccess As String = settingClass.GetProductAccess(ddlCompany.SelectedValue)

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO Customers VALUES (@Id, @DebtorCode, @Level, @Sponsor, @Name, @Company, @CompanyDetail, @Area, @Operator, @PriceGroup, @ShutterPriceGroup, @DoorPriceGroupId, @OnStop, @CashSale, @Newsletter, @MinSurcharge, @Active); INSERT INTO CustomerQuotes(Id, Logo) VALUES (@Id, @Logo); INSERT INTO CustomerProductAccess VALUES (@Id, @DesignId)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@DebtorCode", txtDebtorCode.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Level", ddlLevel.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Sponsor", If(String.IsNullOrEmpty(sponsorId), CType(DBNull.Value, Object), sponsorId))
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
                        thisCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Logo", logoCustomer)
                        thisCmd.Parameters.AddWithValue("@DesignId", dataProductAccess)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Customers", thisId, Session("LoginId").ToString(), "Customer Created"}
                settingClass.Logs(dataLog)

                dataLog = {"CustomerQuotes", thisId, Session("LoginId").ToString(), "Customer Quote Created"}
                settingClass.Logs(dataLog)

                dataLog = {"CustomerProductAccess", thisId, Session("LoginId").ToString(), "Customer Product Access Created"}
                settingClass.Logs(dataLog)

                Dim url As String = String.Format("~/setting/customer/detail?customerid={0}", thisId)
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
        Response.Redirect("~/setting/customer/list", False)
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

            If ddlCompany.Items.Count > 1 Then
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

                ddlCompanyDetail.Enabled = True
                If Session("RoleName") = "Sales" Then
                    ddlCompanyDetail.SelectedValue = Session("CompanyDetailId")
                    ddlCompanyDetail.Enabled = False
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
        lbSales.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(companyId) Then
                lbSales.DataSource = settingClass.GetDataTable("SELECT Logins.* FROM Logins LEFT JOIN Customers ON Logins.CustomerId=Customers.Id WHERE Customers.CompanyId='" & companyId & "' AND Logins.RoleId='4' AND Logins.LevelId='2' ORDER BY Logins.UserName ASC")
                lbSales.DataTextField = "FullName"
                lbSales.DataValueField = "Id"
                lbSales.DataBind()

                If lbSales.Items.Count > 0 Then
                    lbSales.Items.Insert(0, New ListItem("", ""))
                End If

                lbSales.Enabled = True
                If Session("RoleName") = "Sales" AndAlso Session("LevelName") = "Member" Then
                    lbSales.SelectedValue = Session("LoginId").ToString()
                    lbSales.Enabled = False
                End If
            End If
        Catch ex As Exception
            lbSales.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindPriceGroup(companyId As String)
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
            End If
        End Try
    End Sub

    Protected Sub BindPriceGroup_Shutter(companyId As String)
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
            End If
        End Try
    End Sub

    Protected Sub BindComponentForm()
        divDebtorCode.Visible = LoginAccess("Visible Debtor Code")
        divLevelSponsor.Visible = LoginAccess("Visible Level Sponsor")
        divCompany.Visible = LoginAccess("Visible Company")
        divAreaOperator.Visible = LoginAccess("Visible Area Operator")
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
