Imports System.Globalization

Partial Class Setting_Customer_Discount
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing
    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchCustomerDiscount")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()

            If e.CommandName = "Detail" Then
                MessageError_DetailDiscount(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showDetailDiscount(); };"
                Try
                    gvListDetailDiscount.DataSource = settingClass.GetDataTable("SELECT * FROM CustomerDiscounts WHERE CustomerId='" & dataId & "' ORDER BY CASE WHEN Type='Designs' THEN 1 ELSE 2 END, DataId ASC")
                    gvListDetailDiscount.DataBind()

                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetailDiscount", thisScript, True)
                Catch ex As Exception
                    MessageError_DetailDiscount(True, ex.ToString())
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetailDiscount", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub BindData(searchText As String)
        Session("SearchCustomerDiscount") = String.Empty
        Try
            Dim search As String = String.Empty
            If Not String.IsNullOrEmpty(searchText) Then
                search = "AND Cust.Name LIKE '%" & searchText & "%' OR Cust.DebtorCode LIKE '%" & searchText & "%'"
            End If
            Dim thisQuery As String = String.Format("SELECT Cust.Id, Cust.DebtorCode, Cust.Name AS CustomerName FROM Customers Cust WHERE EXISTS (SELECT 1 FROM CustomerDiscounts Disc WHERE Disc.CustomerId = Cust.Id) {0} ORDER BY Cust.Name ASC", search)

            gvList.DataSource = settingClass.GetDataTable(thisQuery)
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Function DiscountTitle(type As String, dataId As String) As String
        If String.IsNullOrEmpty(type) Then Return String.Empty
        Return settingClass.GetItemData(String.Format("SELECT Name FROM {0} WHERE Id='{1}'", type, dataId))
    End Function

    Protected Function DiscountValue(data As Decimal) As String
        If data > 0 Then Return data.ToString("G29", enUS) & "%"
        Return "ERROR"
    End Function

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_DetailDiscount(visible As Boolean, message As String)
        divErrorDetailDiscount.Visible = visible : msgErrorDetailDiscount.InnerText = message
    End Sub

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
