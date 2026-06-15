Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Customer_Product_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer/product/", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("productid")) Then
            Response.Redirect("~/setting/customer/product/", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("productid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim designId As String = String.Empty
            Dim tags As String = String.Empty
            For Each item As ListItem In lbProduct.Items
                If item.Selected Then
                    tags += item.Value.ToString() & ","
                End If
            Next
            If Not tags = "" Then
                designId = tags.Remove(tags.Length - 1).ToString()
            End If

            If msgError.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    thisConn.Open()
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerProductAccess SET DesignId=@DesignId WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        myCmd.Parameters.AddWithValue("@DesignId", designId)

                        myCmd.ExecuteNonQuery()
                    End Using

                    thisConn.Close()
                End Using

                Dim dataLog As Object() = {"CustomerProductAccess", lblId.Text, Session("LoginId").ToString(), "Customer Product Access Updated"}
                settingClass.Logs(dataLog)

                Dim url As String = String.Format("~/setting/customer/detail?customerid={0}", lblId.Text)
                Response.Redirect(Url, False)
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
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindData(productId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerProductAccess WHERE Id='" & productId & "'")
            If thisData Is Nothing Then Exit Sub

            Dim customerId As String = thisData("Id").ToString()
            Dim companyId As String = settingClass.GetItemData("SELECT CompanyId FROM Customers WHERE Id='" & customerId & "'")

            BindCustomer()
            BindProduct(companyId)

            ddlCustomer.SelectedValue = customerId

            Dim tagsArray() As String = thisData("DesignId").ToString().Split(",")
            Dim tagsList As List(Of String) = tagsArray.ToList()
            For Each i In tagsArray
                If Not (i.Equals(String.Empty)) Then
                    lbProduct.Items.FindByValue(i).Selected = True
                End If
            Next

            ddlCustomer.Enabled = False
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
        Catch ex As Exception
            ddlCustomer.Items.Clear()
            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub BindProduct(companyId As String)
        lbProduct.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(companyId) Then
                lbProduct.DataSource = settingClass.GetDataTable("SELECT * FROM Designs CROSS APPLY STRING_SPLIT(CompanyId, ',') AS companyArray WHERE companyArray.VALUE='" & companyId & "' ORDER BY Name ASC")
                lbProduct.DataTextField = "Name"
                lbProduct.DataValueField = "Id"
                lbProduct.DataBind()
                If lbProduct.Items.Count > 1 Then
                    lbProduct.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            lbProduct.Items.Clear()
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
