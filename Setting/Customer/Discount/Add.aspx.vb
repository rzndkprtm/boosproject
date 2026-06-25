Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_Discount_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer/discount/", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("custid")) Then
            Response.Redirect("~/setting/customer/discount/", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("type")) Then
            Response.Redirect("~/setting/customer/discount/", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        lblCustomerId.Text = Request.QueryString("custid").ToString()
        Dim type As String = Request.QueryString("type").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindCustomer(lblCustomerId.Text)
            BindPage(lblCustomerId.Text, type)
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
                Dim typeDisc As String = String.Empty
                Dim sql As String = String.Empty

                If ddlType.SelectedValue = "product" Then
                    typeDisc = "Designs"

                    sql = "SELECT * FROM Designs CROSS APPLY STRING_SPLIT(CompanyId, ',') AS companyArray CROSS APPLY STRING_SPLIT(AppliesTo, ',') AS applyArray WHERE applyArray.VALUE='Discounts' AND companyArray.VALUE='" & lblCompanyId.Text & "' ORDER BY Id ASC"
                ElseIf ddlType.SelectedValue = "productgroup" Then
                    typeDisc = "PriceProductGroups"

                    sql = "SELECT * FROM PriceProductGroups ORDER BY Id ASC"
                End If

                If ddlProduct.SelectedValue <> "" Then
                    SaveDiscount(ddlProduct.SelectedValue, typeDisc)
                Else
                    Dim data As DataTable = settingClass.GetDataTable(sql)
                    For Each row As DataRow In data.Rows
                        SaveDiscount(row("Id").ToString(), typeDisc)
                    Next
                End If

                url = "~/setting/customer/discount"
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
        url = "~/setting/customer/discount"
        If lblReturnPage.Text = "detail" Then
            url = String.Format("~/setting/customer/detail?customerid={0}", ddlCustomer.SelectedValue)
        End If
        Response.Redirect(url, False)
    End Sub

    Protected Sub SaveDiscount(dataId As String, typeDisc As String)
        Dim checkData As DataRow = settingClass.GetDataRow(String.Format("SELECT * FROM CustomerDiscounts WHERE CustomerId='{0}' AND Type='{1}' AND DataId='{2}'", lblCustomerId.Text, typeDisc, dataId))
        If checkData IsNot Nothing Then
            Dim thisId As String = checkData("Id").ToString()
            Dim thisDiscount As Decimal = CDec(checkData("Discount"))
            Dim newDisc As Decimal = settingClass.GetTotalDiscount(thisDiscount, txtDiscount.Text)

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("UPDATE CustomerDiscounts SET Discount=@Discount, Description=@Description WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisCmd.Parameters.AddWithValue("@Discount", newDisc)
                    thisCmd.Parameters.AddWithValue("@Description", txtDescription.Text)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using
            settingClass.Logs({"CustomerDiscounts", thisId, Session("LoginId").ToString(), "Customer Discount Added"})
        Else
            Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM CustomerDiscounts ORDER BY Id DESC")
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand("INSERT INTO CustomerDiscounts VALUES (@Id, @CustomerId, @Type, @DataId, @Discount, @Description)", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisCmd.Parameters.AddWithValue("@CustomerId", lblCustomerId.Text)
                    thisCmd.Parameters.AddWithValue("@Type", typeDisc)
                    thisCmd.Parameters.AddWithValue("@DataId", dataId)
                    thisCmd.Parameters.AddWithValue("@Discount", txtDiscount.Text)
                    thisCmd.Parameters.AddWithValue("@Description", txtDescription.Text)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            settingClass.Logs({"CustomerDiscounts", thisId, Session("LoginId").ToString(), "Customer Discount Created"})
        End If
    End Sub

    Protected Sub BindPage(customerId As String, type As String)
        Try
            ddlType.SelectedValue = type
            ddlType.Enabled = False

            lblCompanyId.Text = settingClass.GetItemData("SELECT CompanyId FROM Customers WHERE Id='" & customerId & "'")
            lblCompanyDetailId.Text = settingClass.GetItemData("SELECT CompanyDetailId FROM Customers WHERE Id='" & customerId & "'")

            BindProduct(type, lblCompanyId.Text, lblCompanyDetailId.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCustomer(customerId As String)
        ddlCustomer.Items.Clear()
        Try
            ddlCustomer.DataSource = settingClass.GetDataTable("SELECT * FROM Customers ORDER BY Name ASC")
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
