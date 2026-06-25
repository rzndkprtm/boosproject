Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Bottom_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            txtSearch.Text = Session("SearchBottom")

            MessageError(False, String.Empty)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("SearchBottom") = txtSearch.Text
        Response.Redirect("~/setting/specification/bottom/add", False)
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
        MessageError(False, String.Empty)
        gvList.PageIndex = e.NewPageIndex
        BindData(txtSearch.Text)
    End Sub

    Protected Sub btnChangeStatus_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdStatus.Text
            Dim newStatus As String = ddlNewStatus.SelectedValue
            Dim oldStatus As String = txtOldStatus.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Bottoms SET Status=@Status WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Status", newStatus)
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim changeDesc As String = String.Format("Change Status Bottom Type : {0}", newStatus)

            dataLog = {"Bottoms", thisId, Session("LoginId").ToString(), changeDesc}
            settingClass.Logs(dataLog)

            Dim detailData As DataTable = settingClass.GetDataTable("SELECT * FROM BottomColours WHERE BottomId='" & thisId & "' AND Status='" & oldStatus & "'")
            If Not detailData.Rows.Count = 0 Then
                For i As Integer = 0 To detailData.Rows.Count - 1
                    Dim detailId As String = detailData.Rows(i)("Id").ToString()

                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As SqlCommand = New SqlCommand("UPDATE BottomColours SET Status=@Status WHERE Id=@Id", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", detailId)
                            thisCmd.Parameters.AddWithValue("@Status", newStatus)
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    changeDesc = String.Format("Change Status Bottom Colour : {0}", newStatus)

                    dataLog = {"BottomColours", detailId, Session("LoginId").ToString(), changeDesc}
                    settingClass.Logs(dataLog)
                Next
            End If

            Session("SearchBottom") = txtSearch.Text
            Response.Redirect("~/setting/specification/bottom", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String)
        Try
            Dim stringSearch As String = String.Empty
            If Not searchText = "" Then
                stringSearch = "WHERE Id LIKE '%" & searchText & "%' OR Name LIKE '%" & searchText & "%' OR Description LIKE '%" & searchText & "%' OR Status LIKE '%" & searchText & "%'"
            End If
            Dim thisString As String = String.Format("SELECT * FROM Bottoms {0} ORDER BY Name ASC", stringSearch)

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

    Protected Function BindDesign(bottomId As String) As String
        Try
            If Not String.IsNullOrEmpty(bottomId) Then
                Dim myData As DataTable = settingClass.GetDataTable("SELECT Designs.Name AS DesignName FROM Bottoms CROSS APPLY STRING_SPLIT(Bottoms.DesignId, ',') AS splitArray LEFT JOIN Designs ON splitArray.VALUE=Designs.Id WHERE Bottoms.Id='" & bottomId & "' ORDER BY Designs.Id ASC")
                Dim hasil As String = String.Empty
                If Not myData.Rows.Count = 0 Then
                    For i As Integer = 0 To myData.Rows.Count - 1
                        Dim designName As String = myData.Rows(i)("DesignName").ToString()
                        hasil += designName & ", "
                    Next
                End If

                Return hasil.Remove(hasil.Length - 2).ToString()
            End If
            Return String.Empty
        Catch ex As Exception
            Return "Error"
        End Try
    End Function

    Protected Function BindCompany(bottomId As String) As String
        Try
            If Not String.IsNullOrEmpty(bottomId) Then
                Dim myData As DataTable = settingClass.GetDataTable("SELECT CompanyDetails.Name AS CompanyName FROM Bottoms CROSS APPLY STRING_SPLIT(Bottoms.CompanyDetailId, ',') AS splitArray LEFT JOIN CompanyDetails ON splitArray.VALUE=CompanyDetails.Id WHERE Bottoms.Id='" & bottomId & "' ORDER BY CompanyDetails.Id ASC")
                Dim hasil As String = String.Empty
                If myData.Rows.Count > 0 Then
                    For i As Integer = 0 To myData.Rows.Count - 1
                        Dim designName As String = myData.Rows(i)("CompanyName").ToString()
                        hasil += designName & ","
                    Next
                    Return hasil.Remove(hasil.Length - 1).ToString()
                Else
                    Return String.Empty
                End If
            End If
            Return String.Empty
        Catch ex As Exception
            Return "Error"
        End Try
    End Function

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
