Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class Setting_Customer_Markup_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim enUS As CultureInfo = New CultureInfo("en-US")
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer/markup/", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("markupid")) Then
            Response.Redirect("~/setting/customer/markup/", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        lblId.Text = Request.QueryString("markupid").ToString()
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
                MessageError(True, "TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlType.SelectedValue = "productgroup" AndAlso ddlProduct.SelectedValue = "" Then
                MessageError(True, "PRODUCT GROUP IS REQUIRED !")
                Exit Sub
            End If
            If txtMarkup.Text = "" Then
                MessageError(True, "MARKUP IS REQUIRED !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE CustomerMarkups SET Markup=@Markup, Description=@Description WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@Markup", txtMarkup.Text)
                        thisCmd.Parameters.AddWithValue("@Description", txtDescription.Text)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"CustomerMarkups", lblId.Text, Session("LoginId").ToString(), "Customer Markup Updated"}
                settingClass.Logs(dataLog)

                url = "~/setting/customer/markup"
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
        url = "~/setting/customer/markup"
        If lblReturnPage.Text = "detail" Then
            url = String.Format("~/setting/customer/detail?customerid={0}", ddlCustomer.SelectedValue)
        End If
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindData(discountId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerMarkups WHERE Id='" & discountId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/customer/markup/", False)
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
            txtMarkup.Text = Convert.ToDecimal(thisData("Markup")).ToString("G29", enUS)
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
                thisString = "SELECT * FROM Designs CROSS APPLY STRING_SPLIT(CompanyId, ',') AS companyArray CROSS APPLY STRING_SPLIT(AppliesTo, ',') AS applyArray WHERE companyArray.VALUE='" & companyId & "' AND applyArray.VALUE='Markups' ORDER BY Name ASC"
            End If
            If type = "productgroup" Then
                thisString = "SELECT PriceProductGroups.Id, CASE WHEN PriceProductGroups.Status='Active' THEN PriceProductGroups.Name ELSE PriceProductGroups.Name + ' [' + UPPER(PriceProductGroups.Status) + ']' END AS Name FROM PriceProductGroups LEFT JOIN Designs ON PriceProductGroups.DesignId=Designs.Id CROSS APPLY STRING_SPLIT(PriceProductGroups.CompanyDetailId, ',') AS companyArray WHERE companyArray.VALUE='" & companyDetailId & "' AND Designs.Type='Blinds' AND PriceProductGroups.Name NOT LIKE '%Panel Glide - Panel Only%' AND PriceProductGroups.Name NOT LIKE '%Panel Glide - Track Only%' AND (PriceProductGroups.Status='Active' OR PriceProductGroups.Status='Inactive') ORDER BY PriceProductGroups.Name ASC"
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
