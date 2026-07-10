Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Fabric_Default
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
            MessageError(False, String.Empty)
            txtSearch.Text = Session("SearchFabric")
            BindCompanyDetail()
            ddlCompanyDetail.SelectedValue = Session("CompanyFabric")
            BindData(txtSearch.Text, ddlCompanyDetail.SelectedValue)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("SearchFabric") = txtSearch.Text
        Session("CompanyFabric") = ddlCompanyDetail.SelectedValue
        Response.Redirect("~/setting/specification/fabric/add", False)
    End Sub

    Protected Sub btnListColour_Click(sender As Object, e As EventArgs)
        Session("SearchFabric") = txtSearch.Text
        Session("CompanyFabric") = ddlCompanyDetail.SelectedValue
        Response.Redirect("~/setting/specification/fabric/colour", False)
    End Sub

    Protected Sub btnAlias_Click(sender As Object, e As EventArgs)
        Session("SearchFabric") = txtSearch.Text
        Session("CompanyFabric") = ddlCompanyDetail.SelectedValue
        Response.Redirect("~/setting/specification/fabric/alias", False)
    End Sub

    Protected Sub btnGroupLocal_Click(sender As Object, e As EventArgs)
        Session("SearchFabric") = txtSearch.Text
        Session("CompanyFabric") = ddlCompanyDetail.SelectedValue
        Response.Redirect("~/setting/specification/fabric/jakarta", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        gvList.PageIndex = 0
        MessageError(False, String.Empty)
        BindData(txtSearch.Text, ddlCompanyDetail.SelectedValue)
        Session("SearchFabric") = txtSearch.Text
        Session("CompanyFabric") = ddlCompanyDetail.SelectedValue
    End Sub

    Protected Sub ddlCompanyDetail_SelectedIndexChanged(sender As Object, e As EventArgs)
        gvList.PageIndex = 0

        MessageError(False, String.Empty)
        BindData(txtSearch.Text, ddlCompanyDetail.SelectedValue)
    End Sub

    Protected Sub rptPager_ItemCommand(sender As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Page" Then
            gvList.PageIndex = Convert.ToInt32(e.CommandArgument)
            BindData(txtSearch.Text, ddlCompanyDetail.SelectedValue)
        End If
    End Sub

    Protected Sub gvList_DataBound(sender As Object, e As EventArgs)
        BuildPager()
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        gvList.PageIndex = e.NewPageIndex

        MessageError(False, String.Empty)
        BindData(txtSearch.Text, ddlCompanyDetail.SelectedValue)
    End Sub

    Protected Sub btnChangeStatus_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdStatus.Text
            Dim newStatus As String = ddlNewStatus.SelectedValue
            Dim oldStatus As String = txtOldStatus.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE Fabrics SET Status=@Status WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisCmd.Parameters.AddWithValue("@Status", newStatus)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim changeDesc As String = String.Format("Change Status Fabric Type : {0}", newStatus)
            dataLog = {"Fabrics", thisId, Session("LoginId").ToString(), changeDesc}
            settingClass.Logs(dataLog)

            Dim detailData As DataTable = settingClass.GetDataTable("SELECT * FROM FabricColours WHERE FabricId='" & thisId & "' AND Status='" & oldStatus & "'")
            If Not detailData.Rows.Count = 0 Then
                For i As Integer = 0 To detailData.Rows.Count - 1
                    Dim detailId As String = detailData.Rows(i)("Id").ToString()

                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As SqlCommand = New SqlCommand("UPDATE FabricColours SET Status=@Status WHERE Id=@Id", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", detailId)
                            thisCmd.Parameters.AddWithValue("@Status", newStatus)
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    changeDesc = String.Format("Change Status Fabric Colour : {0}", newStatus)

                    dataLog = {"FabricColours", detailId, Session("LoginId").ToString(), changeDesc}
                    settingClass.Logs(dataLog)
                Next
            End If

            Dim aliasData As DataRow = settingClass.GetDataRow("SELECT SecondId FROM FabricAlias WHERE Type='Fabrics' AND FirstId='" & thisId & "'")
            If aliasData IsNot Nothing Then
                Dim aliasId As String = aliasData(0).ToString()

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE Fabrics SET Status=@Status WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", aliasId)
                        thisCmd.Parameters.AddWithValue("@Status", newStatus)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                changeDesc = String.Format("Change Status Fabric Type : {0}", newStatus)
                dataLog = {"Fabrics", aliasId, Session("LoginId").ToString(), changeDesc}
                settingClass.Logs(dataLog)

                Dim detailAliasData As DataTable = settingClass.GetDataTable("SELECT * FROM FabricColours WHERE FabricId='" & aliasId & "' AND Status='" & oldStatus & "'")
                If Not detailAliasData.Rows.Count = 0 Then
                    For i As Integer = 0 To detailAliasData.Rows.Count - 1
                        Dim detailId As String = detailAliasData.Rows(i)("Id").ToString()

                        Using thisConn As New SqlConnection(myConn)
                            Using thisCmd As SqlCommand = New SqlCommand("UPDATE FabricColours SET Status=@Status WHERE Id=@Id", thisConn)
                                thisCmd.Parameters.AddWithValue("@Id", detailId)
                                thisCmd.Parameters.AddWithValue("@Status", newStatus)
                                thisConn.Open()
                                thisCmd.ExecuteNonQuery()
                            End Using
                        End Using

                        changeDesc = String.Format("Change Status Fabric Colour : {0}", newStatus)

                        dataLog = {"FabricColours", detailId, Session("LoginId").ToString(), changeDesc}
                        settingClass.Logs(dataLog)
                    Next
                End If
            End If

            Session("SearchFabric") = txtSearch.Text
            Session("CompanyFabric") = ddlCompanyDetail.SelectedValue
            Response.Redirect("~/setting/specification/fabric", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String, companyText As String)
        Session("SearchFabric") = String.Empty : Session("CompanyFabric") = String.Empty
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", If(String.IsNullOrWhiteSpace(searchText), CType(DBNull.Value, Object), searchText)),
                New SqlParameter("@CompanyDetailId", If(String.IsNullOrWhiteSpace(companyText), CType(DBNull.Value, Object), companyText))
            }
            gvList.DataSource = settingClass.GetDataTableSP("sp_Fabrics_List", params)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID")
            gvList.Columns(5).Visible = LoginAccess("Visible Company Detail")

            btnAdd.Visible = LoginAccess("Add")
            btnListColour.Visible = LoginAccess("Visible Fabric + Colour")
            btnAlias.Visible = LoginAccess("Visible Fabric Alias")
            btnGroupLocal.Visible = LoginAccess("Visible Group Local")

            divCompanyDetail.Visible = LoginAccess("Visible Sort Company Detail")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompanyDetail()
        ddlCompanyDetail.Items.Clear()
        Try
            Dim thisString As String = "SELECT * FROM CompanyDetails ORDER BY Id ASC"
            If Not Session("CompanyDetailId") = "" Then
                thisString = "SELECT * FROM CompanyDetails WHERE CompanyId='" & Session("CompanyId") & "' ORDER BY Id ASC"
            End If
            ddlCompanyDetail.DataSource = settingClass.GetDataTable(thisString)
            ddlCompanyDetail.DataTextField = "Name"
            ddlCompanyDetail.DataValueField = "Id"
            ddlCompanyDetail.DataBind()

            If ddlCompanyDetail.Items.Count > 0 Then
                ddlCompanyDetail.Items.Insert(0, New ListItem("", ""))
            End If
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

    Protected Function BindCompanyDetail(fabricId As String) As String
        If Not String.IsNullOrEmpty(fabricId) Then
            Dim myData As DataTable = settingClass.GetDataTable("SELECT CompanyDetails.Name AS CompanyName FROM Fabrics CROSS APPLY STRING_SPLIT(Fabrics.CompanyDetailId, ',') AS splitArray LEFT JOIN CompanyDetails ON splitArray.VALUE=CompanyDetails.Id WHERE Fabrics.Id='" & fabricId & "' ORDER BY CompanyDetails.Id ASC")
            Dim hasil As String = String.Empty
            If myData.Rows.Count > 0 Then
                For i As Integer = 0 To myData.Rows.Count - 1
                    Dim companyDetailName As String = myData.Rows(i)("CompanyName").ToString()
                    hasil += companyDetailName & ", "
                Next
                Return hasil.Remove(hasil.Length - 2).ToString()
            Else
                Return String.Empty
            End If
        End If
        Return "Error"
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
