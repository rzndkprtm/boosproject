Imports System.Data.SqlClient

Partial Class Setting_Customer_Promo_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer/promo/", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("custid")) Then
            lblCustomerId.Text = Request.QueryString("custid").ToString()
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindCustomer(lblCustomerId.Text)
            BindPromo(ddlCustomer.SelectedValue)
        End If
    End Sub

    Protected Sub ddlCustomer_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindPromo(ddlCustomer.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlCustomer.SelectedValue = "" Then
                MessageError(True, "ACCOUNT IS REQUIRED !")
                Exit Sub
            End If
            If ddlPromo.SelectedValue = "" Then
                MessageError(True, "PROMO IS REQUIRED !")
                Exit Sub
            End If
            If Session("RoleName") = "Sales" OrElse Session("Account") Then
                If ddlCustomer.SelectedValue = Session("CustomerId") Then
                    MessageError(True, "ACCESS DENIED !")
                    Exit Sub
                End If
            End If
            Dim checkData As Integer = settingClass.GetItemData_Integer("SELECT COUNT(*) FROM CustomerPromos WHERE CustomerId='" & ddlCustomer.SelectedValue & "' AND PromoId='" & ddlPromo.SelectedValue & "'")
            If checkData > 0 Then
                MessageError(True, "THIS PROMO IS ALREADY REGISTERED. PLEASE USE A DIFFERENT PROMO !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerPromos ORDER BY Id DESC")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerPromos VALUES (@Id, @CustomerId, @PromoId)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@CustomerId", ddlCustomer.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@PromoId", ddlPromo.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"CustomerPromos", thisId, Session("LoginId").ToString(), "Customer Promo Created"}
                settingClass.Logs(dataLog)

                url = "~/setting/customer/promo"
                If lblReturnPage.Text = "detail" Then
                    url = String.Format("~/setting/customer/detail?customerid={0}", ddlCustomer.SelectedValue)
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
        url = "~/setting/customer/promo"
        If lblReturnPage.Text = "detail" Then
            url = String.Format("~/setting/customer/detail?customerid={0}", ddlCustomer.SelectedValue)
        End If
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindCustomer(customerId As String)
        ddlCustomer.Items.Clear()
        Try
            Dim role As String = String.Empty
            If Session("RoleName") = "Sales" Then
                role = "AND CompanyId='" & Session("CompanyId").ToString() & "'"
                If Session("LevelName") = "Member" Then
                    role = "AND (Id = '" & Session("CustomerId") & "' OR EXISTS (SELECT 1 FROM STRING_SPLIT(Operator, ',') WHERE value = '" & Session("LoginId") & "'))"
                End If
            End If

            Dim thisQuery As String = String.Format("SELECT * FROM Customers WHERE Active=1 {0} ORDER BY Name ASC", role)

            ddlCustomer.DataSource = settingClass.GetDataTable(thisQuery)
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()

            If ddlCustomer.Items.Count > 1 Then
                ddlCustomer.Items.Insert(0, New ListItem("", ""))
            End If
            ddlCustomer.SelectedValue = customerId

            ddlCustomer.Enabled = False
            If String.IsNullOrEmpty(customerId) Then ddlCustomer.Enabled = True
        Catch ex As Exception
            ddlCustomer.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub BindPromo(customerId As String)
        ddlPromo.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(customerId) Then
                Dim companyDetailId As String = settingClass.GetItemData("SELECT CompanyDetailId FROM Customers WHERE Id='" & customerId & "'")
                ddlPromo.DataSource = settingClass.GetDataTable("SELECT * FROM Promos WHERE CompanyDetailId='" & companyDetailId & "' AND Active=1 ORDER BY Name ASC")
                ddlPromo.DataTextField = "Name"
                ddlPromo.DataValueField = "Id"
                ddlPromo.DataBind()

                If ddlPromo.Items.Count > 1 Then
                    ddlPromo.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            ddlPromo.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
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
