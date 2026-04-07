
Imports System.Data

Partial Class Setting_Customer_Quote
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/customer", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchCustomerQuote")
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

    Protected Sub BindData(searchText As String)
        Session("SearchCustomerQuote") = String.Empty
        Try
            Dim search As String = String.Empty

            If Not String.IsNullOrEmpty(searchText) Then
                search = "WHERE Customers.Id LIKE '%" & searchText.Trim() & "%' OR Customers.Name LIKE '%" & searchText.Trim() & "%' OR Customers.DebtorCode LIKE '%" & searchText.Trim() & "%'"
            End If

            Dim thisQuery As String = String.Format("SELECT CustomerQuotes.*, Customers.Name AS CustomerName FROM CustomerQuotes LEFT JOIN Customers ON CustomerQuotes.Id=Customers.Id {0} ORDER BY Customers.Id ASC", search)

            gvList.DataSource = settingClass.GetDataTable(thisQuery)
            gvList.DataBind()
            gvList.Columns(1).Visible = PageAction("Visible ID") ' ID
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindDataCustomer()

    End Sub

    Protected Function BindAddress(customerId As String) As String
        Dim result As String = String.Empty

        If Not String.IsNullOrEmpty(customerId) Then
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM CustomerQuotes WHERE Id='" & customerId & "'")

            If thisData IsNot Nothing Then
                Dim parts As New List(Of String)

                Dim address As String = thisData("Address").ToString().Trim()
                Dim suburb As String = thisData("Suburb").ToString().Trim()
                Dim state As String = thisData("State").ToString().Trim()
                Dim postCode As String = thisData("PostCode").ToString().Trim()

                If Not String.IsNullOrEmpty(address) Then parts.Add(address)
                If Not String.IsNullOrEmpty(suburb) Then parts.Add(suburb)

                Dim statePostCode As String = (state & " " & postCode).Trim()
                If Not String.IsNullOrEmpty(statePostCode) Then parts.Add(statePostCode)

                result = String.Join(", ", parts)
            End If
        End If

        Return result
    End Function

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
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
