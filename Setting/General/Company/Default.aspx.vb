Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_General_Company_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/general", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchCompany")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/general/company/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)

        Session("SearchCompany") = txtSearch.Text
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        BuildPager()
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Page" Then
            gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim companyId As String = txtDeleteId.Text

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM Companys WHERE Id=@Id; DELETE FROM Logs WHERE Type='Companys' AND DataId=@Id; UPDATE Mailings SET CompanyId=NULL WHERE CompanyId=@Id; UPDATE Newsletters SET CompanyId=NULL WHERE CompanyId=@Id; UPDATE Notifications SET CompanyId=NULL WHERE CompanyId=@Id; UPDATE PriceGroups SET CompanyId=NULL WHERE CompanyId=@Id; UPDATE Sales SET CompanyId=NULL WHERE CompanyId=@Id; UPDATE Tutorials SET CompanyId=NULL WHERE CompanyId=@Id;", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", companyId)
                    thisCmd.ExecuteNonQuery()
                End Using

                ' TABLE DESIGNS
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE D SET CompanyId = STUFF((SELECT ',' + S.value FROM STRING_SPLIT(D.CompanyId, ',') S WHERE TRY_CAST(S.value AS INT) <> " & companyId & " FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'),1,1,'') FROM Designs D WHERE EXISTS (SELECT 1 FROM STRING_SPLIT(D.CompanyId, ',') S WHERE TRY_CAST(S.value AS INT) = " & companyId & ");", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", companyId)
                    thisCmd.ExecuteNonQuery()
                End Using

                thisConn.Close()
            End Using

            Dim companyDetail As DataTable = settingClass.GetDataTable("SELECT Id FROM Companys WHERE CompanyId='" & companyId & "'")
            If companyDetail.Rows.Count > 0 Then
                For i As Integer = 0 To companyDetail.Rows.Count - 1
                    Dim companyDetailId As String = companyDetail.Rows(i)("Id").ToString()

                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerPromos WHERE Id=@Id; DELETE FROM Logs WHERE Type='CustomerPromos' AND DataId=@Id;", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", companyDetailId)
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using
                Next
            End If

            Session("SearchCompany") = txtSearch.Text
            Response.Redirect("~/setting/general/company", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Try
            Dim search As String = String.Empty
            If Not String.IsNullOrEmpty(searchText) Then
                search = "WHERE Id LIKE '%" & searchText & "%' OR Name LIKE '%" & searchText & "%' OR Alias LIKE '%" & searchText & "%' OR Description LIKE '%" & searchText & "%'"
            End If

            Dim thisString As String = String.Format("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Companys {0} ORDER BY Id ASC", search)

            gvList.DataSource = settingClass.GetDataTable(thisString)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID")

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
