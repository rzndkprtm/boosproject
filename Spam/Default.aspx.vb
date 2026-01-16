Imports System.Data
Imports System.Data.SqlClient

Partial Class Spam_Default
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim webOrder As String = ConfigurationManager.ConnectionStrings("WebOrder").ConnectionString
    Dim url As String = String.Empty

    Dim orderClass As New OrderClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim action As String = Request.QueryString("action").ToString()
        Dim headerid As String = Request.QueryString("headerid").ToString()
        Dim itemid As String = Request.QueryString("itemid").ToString()
        Dim designId As String = Request.QueryString("designid").ToString()
        Dim debtorcode As String = Request.QueryString("debtorcode").ToString()
        Dim ordernumber As String = Request.QueryString("ordernumber").ToString()
        Dim ordername As String = Request.QueryString("ordername").ToString()
        Dim username As String = Request.QueryString("username").ToString()

        If Not IsPostBack Then
            Dim customerId As String = orderClass.GetItemData("SELECT Id FROM Customers WHERE DebtorCode='" & debtorcode & "'")
            Dim loginId As String = orderClass.GetItemData("SELECT Id FROM CustomerLogins WHERE UserName='" & username & "' AND CustomerId='" & customerId & "'")

            Dim thisQuery As String = "SELECT CustomerLogins.*, Companys.Id AS CompanyId, Companys.Name AS CompanyName, Companys.Active AS CompanyActive, Customers.Active AS CustomerActive, CASE WHEN CustomerLogins.Pricing=1 THEN 'Yes' ELSE '' END AS PriceAccess, CustomerLoginRoles.Name AS RoleName, CustomerLoginRoles.Active AS RoleActive, CustomerLoginLevels.Name AS LevelName, CustomerLoginLevels.Active AS LevelActive FROM CustomerLogins INNER JOIN CustomerLoginRoles ON CustomerLogins.RoleId=CustomerLoginRoles.Id INNER JOIN CustomerLoginLevels ON CustomerLogins.LevelId=CustomerLoginLevels.Id INNER JOIN Customers ON CustomerLogins.CustomerId=Customers.Id INNER JOIN Companys ON Customers.CompanyId=Companys.Id WHERE CustomerLogins.Id='" & loginId & "'"

            Dim myData As DataSet = orderClass.GetListData(thisQuery)

            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("https://ordersblindonline.com/viewOrder.aspx")
                Exit Sub
            End If

            Dim checkData As Integer = orderClass.GetItemData_Integer("SELECT COUNT(*) FROM OrderHeaders WHERE Id='" & headerid & "'")

            If checkData = 0 Then
                Dim companyDetail As String = orderClass.GetCompanyAliasByCustomer(customerId)

                Dim orderId As String = String.Format("{0}-{1}", companyDetail, headerid)

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderHeaders(Id, OrderId, CustomerId, OrderNumber, OrderName, Status, CreatedBy, CreatedDate, Active) VALUES (@Id, @OrderId, @CustomerId, @OrderNumber, @OrderName, 'Unsubmitted', @CreatedBy, GETDATE(), 1); INSERT INTO OrderQuotes VALUES(@Id, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0.00, 0.00, 0.00, 0.00);", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", headerid)
                        myCmd.Parameters.AddWithValue("@OrderId", orderId)
                        myCmd.Parameters.AddWithValue("@CustomerId", customerId)
                        myCmd.Parameters.AddWithValue("@OrderNumber", ordernumber)
                        myCmd.Parameters.AddWithValue("@OrderName", ordername)
                        myCmd.Parameters.AddWithValue("@CreatedBy", loginId)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using
            End If

            If designId = "14" Then
                url = String.Format("~/order/shutterexpress?do={0}&orderid={1}&itemid={2}&type={3}&uid={4}", action, headerid, itemid, designId, loginId)
            ElseIf designId = "15" Then
                url = String.Format("~/order/shutterocean?do={0}&orderid={1}&itemid={2}&type={3}&uid={4}", action, headerid, itemid, designId, loginId)
            Else
                url = "https://ordersblindonline.com/ViewOrder.aspx"
            End If

            Response.Redirect(url, False)
        End If
    End Sub
End Class
