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

        Dim companyId As String = settingClass.GetItemData("SELECT CompanyId FROM Promos WHERE Id='" & promoId & "'")
        For Each dr As DataRow In dt.Rows
            Dim typeName As String = ""
            Dim typeValue As String = dr("Type").ToString()
            Dim method As String = dr("Method").ToString()
            Dim dataId As String = dr("DataId").ToString()

            If String.IsNullOrEmpty(typeValue) Then
                typeName = ""
            ElseIf typeValue = "FrameColours" Then
                typeName = dataId
            ElseIf typeValue = "RollerFabrics" OrElse typeValue = "CurtainFabrics" Then
                typeName = settingClass.GetItemData(String.Format("SELECT Name FROM Fabrics WHERE Id='{0}'", dataId))
            ElseIf typeValue = "RollerFabricColours" OrElse typeValue = "CurtainFabricColours" Then
                typeName = settingClass.GetItemData(String.Format("SELECT Name FROM FabricColours WHERE Id='{0}'", dataId))
            Else
                typeName = settingClass.GetItemData(String.Format("SELECT Name FROM {0} WHERE Id='{1}'", typeValue, dataId.Replace("'", "''")))
            End If

            Dim discountText As String = "ERROR"
            Dim discount As Decimal

            If Decimal.TryParse(dr("Discount").ToString(), discount) Then
                If discount > 0 Then
                    discountText = discount.ToString("G29") & "%"
                    If method = "Value" Then
                        If companyId = "2" Then
                            discountText = "$" & discount.ToString("G29")
                        End If
                        If companyId = "3" Then
                            discountText = "Rp" & discount.ToString("G29")
                        End If
                    End If
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

            gvList.DataSource = settingClass.GetDataTableSP("sp_CustomerPromos_List", params)
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
