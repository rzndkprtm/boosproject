
Imports Org.BouncyCastle.Asn1.Cmp

Partial Class Setting_Customer_Multiple
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
            BindCustomer()
            BindNewData(ddlData.SelectedValue)
        End If
    End Sub

    Protected Sub ddlData_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindNewData(ddlData.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlData.SelectedValue = "" Then
                MessageError(True, "DATA TYPE IS REQUIRED !")
                Exit Sub
            End If
            If lbCustomer.SelectedValue = "" Then
                MessageError(True, "CUSTOEMR ACCOUNT IS REQURIED !")
                Exit Sub
            End If
            If lbNewData.SelectedValue = "" Then
                MessageError(True, "NEW DATA (CHANGE TO) IS REQURIED !")
                Exit Sub
            End If
            If {"SubCompany", "PriceGroup", "PriceGroupShutter", "PriceGroupDoor", "CashSale", "OnStop"}.Contains(ddlData.SelectedValue) AndAlso lbNewData.Items.Cast(Of ListItem)().Count(Function(x) x.Selected) > 1 Then
                MessageError(True, "ONLY ONE CUSTOMER CAN BE SELECTED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then

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

    Protected Sub BindCustomer()
        lbCustomer.Items.Clear()
        Try
            lbCustomer.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM Customers WHERE Status='Active' ORDER BY Id ASC")
            lbCustomer.DataTextField = "Name"
            lbCustomer.DataValueField = "Id"
            lbCustomer.DataBind()

            If lbCustomer.Items.Count > 1 Then
                lbCustomer.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            lbCustomer.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindNewData(data As String)
        lbNewData.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(data) Then
                Dim thisString As String = String.Empty
                If data = "PriceGroup" Then
                    lbNewData.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM PriceGroups WHERE Type='Blinds' AND Status='Active'")
                    lbNewData.DataTextField = "Name"
                    lbNewData.DataValueField = "Id"
                    lbNewData.DataBind()

                    If lbNewData.Items.Count > 1 Then
                        lbNewData.Items.Insert(0, New ListItem("", ""))
                    End If
                End If
                If data = "PriceGroupShutter" Then
                    lbNewData.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM PriceGroups WHERE Type='Shutters' AND Status='Active'")
                    lbNewData.DataTextField = "Name"
                    lbNewData.DataValueField = "Id"
                    lbNewData.DataBind()

                    If lbNewData.Items.Count > 1 Then
                        lbNewData.Items.Insert(0, New ListItem("", ""))
                    End If
                End If
                If data = "PriceGroupDoor" Then
                    lbNewData.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM PriceGroups WHERE Type='Doors' AND Status='Active'")
                    lbNewData.DataTextField = "Name"
                    lbNewData.DataValueField = "Id"
                    lbNewData.DataBind()

                    If lbNewData.Items.Count > 1 Then
                        lbNewData.Items.Insert(0, New ListItem("", ""))
                    End If
                End If
                If data = "Sales" Then
                    lbNewData.DataSource = settingClass.GetDataTable("SELECT Id, FullName AS Name FROM Logins WHERE RoleId=4 AND LevelId=2 AND Status='Active'")
                    lbNewData.DataTextField = "Name"
                    lbNewData.DataValueField = "Id"
                    lbNewData.DataBind()

                    If lbNewData.Items.Count > 1 Then
                        lbNewData.Items.Insert(0, New ListItem("", ""))
                    End If
                End If
                If data = "SubCompany" Then
                    lbNewData.DataSource = settingClass.GetDataTable("SELECT Id, Name AS Name FROM CompanyDetails WHERE Status='Active'")
                    lbNewData.DataTextField = "Name"
                    lbNewData.DataValueField = "Id"
                    lbNewData.DataBind()

                    If lbNewData.Items.Count > 1 Then
                        lbNewData.Items.Insert(0, New ListItem("", ""))
                    End If
                End If
                If data = "CashSale" OrElse data = "OnStop" Then
                    lbNewData.Items.Add(New ListItem("Yes", "Yes"))
                    lbNewData.Items.Add(New ListItem("No", "No"))
                End If
            End If
        Catch ex As Exception
            lbNewData.Items.Clear()
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
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
