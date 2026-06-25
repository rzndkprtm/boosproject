Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Web.Services

Partial Class Setting_Customer_Promo_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing
    Dim enUS As CultureInfo = New CultureInfo("en-US")

    <WebMethod()>
    Public Shared Function GetPromoDetail(customerPromoId As String) As List(Of PromoDetailModel)
        Dim result As New List(Of PromoDetailModel)

        Dim settingClass As New SettingClass()

        Dim promoId As String = settingClass.GetItemData("SELECT PromoId FROM CustomerPromos WHERE Id='" & customerPromoId.Replace("'", "''") & "'")
        If String.IsNullOrEmpty(promoId) Then
            Return result
        End If

        Dim dt As DataTable = settingClass.GetDataTable("SELECT * FROM PromoDetails WHERE PromoId='" & promoId.Replace("'", "''") & "'")
        For Each dr As DataRow In dt.Rows
            Dim typeName As String = ""
            Dim typeValue As String = dr("Type").ToString()
            Dim dataId As String = dr("DataId").ToString()

            If String.IsNullOrEmpty(typeValue) Then
                typeName = ""
            ElseIf typeValue = "FrameColours" Then
                typeName = dataId
            Else
                typeName = settingClass.GetItemData(String.Format("SELECT Name FROM {0} WHERE Id='{1}'", typeValue, dataId.Replace("'", "''")))
            End If

            Dim discountText As String = "ERROR"

            Dim discount As Decimal

            If Decimal.TryParse(dr("Discount").ToString(), discount) Then
                If discount > 0 Then
                    discountText = discount.ToString("G29") & "%"
                End If
            End If

            result.Add(New PromoDetailModel With {.Id = dr("Id").ToString(), .Type = typeName, .Discount = discountText})
        Next
        Return result
    End Function

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchCustomerPromo")
            BindData(txtSearch.Text)

            BindDataCustomer()
            BindDataPromo()
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/customer/promo/add", False)
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Page" Then
            gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        BuildPager()
    End Sub

    Protected Sub btnProcess_Click(sender As Object, e As EventArgs)
        MessageError_Process(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcess(); };"
        Try
            If ddlCustomer.SelectedValue = "" Then
                MessageError_Process(True, "CUSTOMER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If ddlPromo.SelectedValue = "" Then
                MessageError_Process(True, "PROMO IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            Dim checkData As Integer = settingClass.GetItemData_Integer("SELECT COUNT(*) FROM CustomerPromos WHERE CustomerId='" & lblId.Text & "' AND PromoId='" & ddlPromo.SelectedValue & "'")
            If checkData > 0 Then
                MessageError_Process(True, "THIS PROMO IS ALREADY REGISTERED. PLEASE USE A DIFFERENT PROMO !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcess.InnerText = "" Then
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

                dataLog = {"CustomerPromos", thisId, Session("LoginId").ToString(), "Customer Promo Created"}
                settingClass.Logs(dataLog)

                Session("SearchCustomerPromo") = txtSearch.Text
                Response.Redirect("~/setting/customer/promo", False)
            End If
        Catch ex As Exception
            MessageError_Process(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError_Process(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcess", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtDeleteId.Text
            Dim customerId As String = txtDeleteCustomerId.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerPromos WHERE Id=@Id; DELETE FROM Logs WHERE Type='CustomerPromos' AND DataId=@Id;", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"Customers", customerId, Session("LoginId"), "Customer Promo Deleted"}
            settingClass.Logs(dataLog)

            Session("SearchCustomerPromo") = txtSearch.Text
            Response.Redirect("~/setting/customer/promo", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchCustomerPromo") = String.Empty
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), CType(DBNull.Value, Object), searchText.Trim())),
                New SqlParameter("@RoleName", Session("RoleName").ToString()),
                New SqlParameter("@LevelName", Session("LevelName").ToString()),
                New SqlParameter("@CompanyId", If(Session("CompanyId") Is Nothing, CType(DBNull.Value, Object), Session("CompanyId"))),
                New SqlParameter("@LoginId", Session("LoginId"))
            }

            gvList.DataSource = settingClass.GetDataTableSP("sp_CustomerPromos", params)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID") ' ID

            btnAdd.Visible = LoginAccess("Add")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDataCustomer()
        ddlCustomer.Items.Clear()
        Try
            Dim thisString As String = "SELECT * FROM Customers ORDER BY Name ASC"
            If Session("RoleName") = "Account" Then
                thisString = "SELECT * FROM Customers WHERE CompanyId='" & Session("CompanyId").ToString() & "' ORDER BY Name ASC"
            End If
            If Session("RoleName") = "Sales" Then
                thisString = "SELECT * FROM Customers WHERE CompanyId='" & Session("CompanyId").ToString() & "' ORDER BY Name ASC"
                If Session("LevelName") = "Member" Then
                    thisString = "SELECT * FROM Customers CROSS APPLY STRING_SPLIT(Operator, ',') AS operatorArray WHERE CompanyId='" & Session("CompanyId").ToString() & "' AND operatorArray.VALUE='" & Session("LoginId").ToString() & "' ORDER BY Name ASC"
                End If
            End If

            ddlCustomer.DataSource = settingClass.GetDataTable(thisString)
            ddlCustomer.DataTextField = "Name"
            ddlCustomer.DataValueField = "Id"
            ddlCustomer.DataBind()

            If ddlCustomer.Items.Count > 1 Then
                ddlCustomer.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCustomer.Items.Clear()
        End Try
    End Sub

    Protected Sub BindDataPromo()
        ddlPromo.Items.Clear()
        Try
            ddlPromo.DataSource = settingClass.GetDataTable("SELECT * FROM Promos WHERE Active=1 ORDER BY Name ASC")
            ddlPromo.DataTextField = "Name"
            ddlPromo.DataValueField = "Id"
            ddlPromo.DataBind()

            If ddlPromo.Items.Count > 1 Then
                ddlPromo.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlPromo.Items.Clear()
        End Try
    End Sub

    Protected Sub BuildPager()
        Try
            If gvList.PageCount <= 1 Then
                navPager.Visible = False
                Return
            End If

            navPager.Visible = True

            Dim currentPage As Integer = gvList.PageIndex
            Dim totalPages As Integer = gvList.PageCount

            Dim pages As New List(Of Object)

            If currentPage > 0 Then
                pages.Add(New With {.Text = "Previous", .PageIndex = currentPage - 1, .CssClass = ""})
            End If

            Dim startPage As Integer = Math.Max(0, currentPage - 2)
            Dim endPage As Integer = Math.Min(totalPages - 1, currentPage + 2)

            For i As Integer = startPage To endPage
                pages.Add(New With {.Text = (i + 1).ToString(), .PageIndex = i, .CssClass = If(i = currentPage, "active", "")})
            Next

            If currentPage < totalPages - 1 Then
                pages.Add(New With {.Text = "Next", .PageIndex = currentPage + 1, .CssClass = ""})
            End If

            rptPager.DataSource = pages
            rptPager.DataBind()
        Catch ex As Exception
            navPager.Visible = False
        End Try
    End Sub

    Protected Function PromoTitle(type As String, dataId As String) As String
        If String.IsNullOrEmpty(type) Then Return String.Empty
        If type = "FrameColours" Then Return dataId
        Return settingClass.GetItemData(String.Format("SELECT Name FROM {0} WHERE Id='{1}'", type, dataId))
    End Function

    Protected Function PromoValue(data As Decimal) As String
        If data > 0 Then Return data.ToString("G29", enUS) & "%"
        Return "ERROR"
    End Function

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Process(visible As Boolean, message As String)
        divErrorProcess.Visible = visible : msgErrorProcess.InnerText = message
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

Public Class PromoDetailModel
    Public Property Id As String
    Public Property Type As String
    Public Property Discount As String
End Class
