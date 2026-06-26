Imports System.Data.SqlClient

Partial Class Setting_Log
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Page" Then
            gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        BuildPager()
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub BindData(searchText As String)
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrEmpty(searchText), CType(DBNull.Value, Object), searchText.Trim()))
            }
            gvList.DataSource = settingClass.GetDataTableSP("sp_LogList", params)
            gvList.DataBind()
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

    Protected Function GetDataName(type As String, dataId As String) As String
        Try
            If Not String.IsNullOrEmpty(type) AndAlso Not String.IsNullOrEmpty(dataId) Then
                Dim thisQuery As String = String.Format("SELECT Name FROM {0} WHERE Id={1}", type, dataId)
                If type = "Blinds" Then
                    thisQuery = "SELECT Designs.Name + ' | ' + Blinds.Name FROM Blinds LEFT JOIN Designs ON Blinds.DesignId=Designs.Id WHERE Blinds.Id='" & dataId & "'"
                End If
                If type = "BottomColours" Then
                    thisQuery = "SELECT Name FROM BottomColours WHERE Id='" & dataId & "'"
                End If
                If type = "Bottoms" Then
                    thisQuery = "SELECT Name FROM Bottoms WHERE Id='" & dataId & "'"
                End If
                If type = "Chains" Then
                    thisQuery = "SELECT Name FROM Chains WHERE Id='" & dataId & "'"
                End If
                If type = "CompanyDetails" Then
                    thisQuery = "SELECT Companys.Name + ' | ' + CompanyDetails.Name FROM CompanyDetails LEFT JOIN Companys ON CompanyDetails.CompanyId=Companys.Id WHERE CompanyDetails.Id='" & dataId & "'"
                End If
                If type = "Companys" Then
                    thisQuery = "SELECT Name FROM Companys WHERE Id='" & dataId & "'"
                End If
                If type = "CustomerAddress" Then
                    thisQuery = "SELECT Customers.Name + ' | ' + CustomerAddress.Description FROM CustomerAddress LEFT JOIN Customers ON CustomerAddress.CustomerId=Customers.Id WHERE CustomerAddress.Id='" & dataId & "'"
                End If
                If type = "CustomerBusiness" Then
                    thisQuery = "SELECT Customers.Name + ' | ' + CustomerBusiness.ABNNumber FROM CustomerBusiness LEFT JOIN Customers ON CustomerBusiness.CustomerId=Customers.Id WHERE CustomerBusiness.Id='" & dataId & "'"
                End If
                If type = "CustomerContacts" Then
                    thisQuery = "SELECT Customers.Name + ' | ' + CustomerContacts.Name FROM CustomerContacts LEFT JOIN Customers ON CustomerContacts.CustomerId=Customers.Id WHERE CustomerContacts.Id='" & dataId & "'"
                End If
                If type = "CustomerDiscounts" Then
                    thisQuery = String.Empty
                    'thisQuery = "SELECT Customers.Name + ' | ' + CustomerDiscounts.Type FROM CustomerAddress LEFT JOIN Customers ON CustomerAddress.CustomerId=Customers.Id WHERE CustomerAddress.Id='" & dataId & "'"
                End If
                If type = "CustomerProductAccess" Then
                    thisQuery = "SELECT Customers.Name FROM CustomerProductAccess LEFT JOIN Customers ON CustomerProductAccess.Id=Customers.Id WHERE CustomerProductAccess.Id='" & dataId & "'"
                End If
                If type = "CustomerPromos" Then
                    thisQuery = String.Empty
                    'thisQuery = "SELECT Customers.Name FROM CustomerProductAccess LEFT JOIN Customers ON CustomerProductAccess.Id=Customers.Id WHERE CustomerProductAccess.Id='" & dataId & "'"
                End If
                If type = "CustomerQuotes" Then
                    thisQuery = String.Empty
                    'thisQuery = "SELECT Customers.Name FROM CustomerProductAccess LEFT JOIN Customers ON CustomerProductAccess.Id=Customers.Id WHERE CustomerProductAccess.Id='" & dataId & "'"
                End If
                If type = "Customers" Then
                    thisQuery = "SELECT Name FROM Customers WHERE Id='" & dataId & "'"
                End If
                If type = "Designs" Then
                    thisQuery = "SELECT Name FROM Designs WHERE Id='" & dataId & "'"
                End If
                If type = "FabricAlias" Then
                    thisQuery = String.Empty
                End If
                If type = "FabricColours" Then
                    thisQuery = String.Empty
                End If
                If type = "FabricGroupLocals" Then
                    thisQuery = String.Empty
                End If




                If type = "Logins" Then
                    thisQuery = String.Format("SELECT UserName FROM {0} WHERE Id={1}", type, dataId)
                End If
                If type = "OrderHeaders" Then
                    thisQuery = String.Format("SELECT OrderId FROM {0} WHERE Id={1}", type, dataId)
                End If
                If type = "OrderDetails" Then
                    thisQuery = String.Format("SELECT Id FROM {0} WHERE Id={1}", type, dataId)
                End If

                If String.IsNullOrEmpty(thisQuery) Then
                    Return String.Empty
                End If

                Dim dataName As String = settingClass.GetItemData(thisQuery)
                Return dataName
            End If
            Return String.Empty
        Catch ex As Exception
            Return "Error"
        End Try
        Return String.Empty
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
