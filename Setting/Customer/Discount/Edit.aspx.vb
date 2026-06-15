Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class Setting_Customer_Discount_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer/discount/", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("discountid")) Then
            Response.Redirect("~/setting/customer/discount/", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("discountid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlCustomer.SelectedValue = "" Then
                MessageError(True, "ACCOUNT IS REQUIRED !")
                Exit Sub
            End If
            If ddlType.SelectedValue = "" Then
                MessageError(True, "ACCOUNT IS REQUIRED !")
                Exit Sub
            End If
            If ddlType.SelectedValue = "productgroup" AndAlso ddlProduct.SelectedValue = "" Then
                MessageError(True, "ACCOUNT IS REQUIRED !")
                Exit Sub
            End If
            If txtDiscount.Text = "" Then
                MessageError(True, "ACCOUNT IS REQUIRED !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()

                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerDiscounts SET Discount=@Discount, Description=@Description WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        myCmd.Parameters.AddWithValue("@Discount", txtDiscount.Text)
                        myCmd.Parameters.AddWithValue("@Description", txtDescription.Text)

                        myCmd.ExecuteNonQuery()
                    End Using

                    thisConn.Close()
                End Using

                Dim dataLog As Object() = {"CustomerDiscounts", lblId.Text, Session("LoginId").ToString(), "Customer Discount Updated"}
                settingClass.Logs(dataLog)

                Dim url As String = String.Format("~/setting/customer/detail?customerid={0}", ddlCustomer.SelectedValue)
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
        Dim url As String = String.Format("~/setting/customer/detail?customerid={0}", ddlCustomer.SelectedValue)
        If ddlCustomer.SelectedValue = "" Then url = "~/setting/customer/discount"
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindData(discountId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerDiscounts WHERE Id='" & discountId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/customer/discount/", False)
                Exit Sub
            End If

            BindCustomer()

            Dim customerId As String = thisData("CustomerId").ToString()
            Dim type As String = thisData("Type").ToString()
            Dim aliasType As String = String.Empty
            If type = "Designs" Then aliasType = "product"
            If type = "PriceProductGroups" Then aliasType = "productgroup"
            lblCompanyId.Text = settingClass.GetItemData("SELECT CompanyId FROM Customers WHERE Id='" & customerId & "'")
            lblCompanyDetailId.Text = settingClass.GetItemData("SELECT CompanyDetailId FROM Customers WHERE Id='" & customerId & "'")

            BindProduct(aliasType, lblCompanyId.Text, lblCompanyDetailId.Text)

            ddlCustomer.SelectedValue = customerId
            ddlType.SelectedValue = aliasType
            ddlProduct.SelectedValue = thisData("DataId").ToString()
            txtDiscount.Text = Convert.ToDecimal(thisData("Discount")).ToString("G29", enUS)
            txtDescription.Text = thisData("Description").ToString()

            ddlCustomer.Enabled = False
            ddlType.Enabled = False : ddlProduct.Enabled = False
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCustomer()
        ddlCustomer.Items.Clear()
        Try
            ddlCustomer.DataSource = settingClass.GetDataTable("SELECT * FROM Customers ORDER BY Name ASC")
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()

            If ddlCustomer.Items.Count > 1 Then
                ddlCustomer.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCustomer.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub BindProduct(type As String, companyId As String, companyDetailId As String)
        ddlProduct.Items.Clear()
        Try
            Dim thisString As String = String.Empty
            If type = "product" Then
                thisString = "SELECT * FROM Designs CROSS APPLY STRING_SPLIT(CompanyId, ',') AS companyArray CROSS APPLY STRING_SPLIT(AppliesTo, ',') AS applyArray WHERE companyArray.VALUE='" & companyId & "' AND applyArray.VALUE='Discounts' ORDER BY Name ASC"
            End If
            If type = "productgroup" Then
                thisString = "SELECT PriceProductGroups.* FROM PriceProductGroups LEFT JOIN Designs ON PriceProductGroups.DesignId=Designs.Id CROSS APPLY STRING_SPLIT(PriceProductGroups.CompanyDetailId, ',') AS companyArray WHERE companyArray.VALUE='" & companyDetailId & "' AND PriceProductGroups.Active=1 AND Designs.Type='Blinds' AND PriceProductGroups.Name NOT LIKE '%Panel Glide - Panel Only%' AND PriceProductGroups.Name NOT LIKE '%Panel Glide - Track Only%' ORDER BY PriceProductGroups.Name ASC"
            End If
            ddlProduct.DataSource = settingClass.GetDataTable(thisString)
            ddlProduct.DataTextField = "Name"
            ddlProduct.DataValueField = "Id"
            ddlProduct.DataBind()

            If ddlProduct.Items.Count > 1 Then
                ddlProduct.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlProduct.Items.Clear()
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
