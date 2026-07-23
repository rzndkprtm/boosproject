Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Promo_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchPromo")
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("SearchPromo") = txtSearch.Text
        Response.Redirect("~/setting/price/promo/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex

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

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim promoId As String = txtDeleteId.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM Promos WHERE Id=@Id; DELETE FROM Logs WHERE Type='Promos' AND DataId=@Id;", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", promoId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim customerPromos As DataTable = settingClass.GetDataTable("SELECT Id FROM CustomerPromos WHERE PromoId='" & promoId & "'")
            If customerPromos.Rows.Count > 0 Then
                For i As Integer = 0 To customerPromos.Rows.Count - 1
                    Dim thisId As String = customerPromos.Rows(i)("Id").ToString()

                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerPromos WHERE Id=@Id; DELETE FROM Logs WHERE Type='CustomerPromos' AND DataId=@Id;", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", thisId)
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using
                Next
            End If

            Dim promoDetail As DataTable = settingClass.GetDataTable("SELECT Id FROM PromoDetails WHERE PromoId='" & promoId & "'")
            If promoDetail.Rows.Count > 0 Then
                For i As Integer = 0 To promoDetail.Rows.Count - 1
                    Dim thisId As String = promoDetail.Rows(i)("Id").ToString()

                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM PromoDetails WHERE Id=@Id; DELETE FROM Logs WHERE Type='PromoDetails' AND DataId=@Id;", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", thisId)
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using
                Next
            End If

            Session("SearchPromo") = txtSearch.Text
            Response.Redirect("~/setting/price/promo", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrWhiteSpace(searchText), CType(DBNull.Value, Object), searchText))
            }
            gvList.DataSource = settingClass.GetDataTableSP("sp_Promos_List", params)
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
